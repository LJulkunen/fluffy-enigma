using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;

public class Kitsulope : ObjectType
{
    SaveSerial save;
    public GameObject fridge;
    public GameObject exitButton;
    public Sprite exitSprite;
    public Sprite fridgeSprite;
    public float exitCounter = 4.0f;

    #region MovementVariables
    [SerializeField]
    private float xMin = -0.9F;
    [SerializeField]
    private float xMax = 0.9F;
    [SerializeField]
    float speed = 3F;
    int direction = 1;
    float counter = 0;
    #endregion

    public Dialogue dialogue;
    DialogueManager dialogueManager;
    public GameObject bubbleObject;
    public GameObject bubbleTextObject;
    public Color color;
    public TextMeshProUGUI bubbleText;
    public SpriteRenderer bubbleSpriteRenderer;
    [SerializeField]
    public float maxBubbleCounter = 10f;
    public float bubbleCounter = 0f;

    #region Animation
    public Animator animator;
    [SerializeField]
    public float pettingAnimationLength = 2.5f;
    public float maxPettingAnimationLength = 2.5f;

    [SerializeField]
    public float feedingAnimationLength = 2.5f;
    public float maxFeedingAnimationLength = 2.5f;

    [SerializeField]
    public float chillingAnimationLength = 2.5f;
    public float maxChillingAnimationLength = 2.5f;

    public bool isChilling;
    public bool isPetNoticingYou;

    public float rand;
    #endregion

    bool _touchDown, _touchHold, _touchUp;

    private void Start()
    {
        save = FindObjectOfType<SaveSerial>();

        // Needs to be set as something at the start to avoid errors. 
        dialogue = fridge.GetComponent<Dialogue>();
        dialogueManager = GetComponent<DialogueManager>();

        maxPettingAnimationLength = pettingAnimationLength;
        maxFeedingAnimationLength = feedingAnimationLength;
        maxChillingAnimationLength = chillingAnimationLength;
        color = bubbleSpriteRenderer.color;

        bubbleText = bubbleTextObject.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        BubbleCounter();

        if (!save.isFeeding || !save.isPetting)
        {
            rand = Random.Range(0.0f, 100.0f);

            if (rand > 99.6f)
            {
                isChilling = true;
            }
        } else
        {
            isChilling = false;
        }
        
        animator.SetInteger("Satisfaction", (int)SaveLoad.SaveData[(int)SaveLoad.Line.Satisfaction]);
        animator.SetBool("IsPetting", save.isPetting);
        animator.SetBool("IsFeeding", save.isFeeding);
        animator.SetInteger("Direction", direction);
        animator.SetBool("IsChilling", isChilling);
        animator.SetFloat("Rand", rand);

        if (save.isPetting)
        {
            pettingAnimationLength -= Time.deltaTime;
            if (pettingAnimationLength <= 0)
            {
                save.isPetting = false;
                pettingAnimationLength = maxPettingAnimationLength;
            }
        }

        if (isChilling)
        {
            chillingAnimationLength -= Time.deltaTime;
            if (chillingAnimationLength <= 0)
            {
                isChilling = false;
                chillingAnimationLength = maxChillingAnimationLength;
            }
        }

        if (save.isFeeding)
        {
            feedingAnimationLength -= Time.deltaTime;
            if (feedingAnimationLength <= 0)
            {
                save.isFeeding = false;
                feedingAnimationLength = maxFeedingAnimationLength;
            }
        }

        // && operations happen before || operations
        // in math terms:
        // && is * (multiply), || is + (add)
        _touchDown = Input.GetMouseButtonDown(0) ||
            Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
        _touchHold = Input.GetMouseButton(0) || Input.touchCount > 0 &&
            (Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch(0).phase == TouchPhase.Moved);
        _touchUp = Input.GetMouseButtonUp(0) || Input.touchCount > 0 &&
            (Input.GetTouch(0).phase == TouchPhase.Canceled || Input.GetTouch(0).phase == TouchPhase.Ended);

        if (_touchDown || _touchHold || _touchUp)
        {
            if (Input.touchCount > 0)
                DoTouch(Input.GetTouch(0).position);
            else
                DoTouch(Input.mousePosition);
        }

        #region movement
        Vector3 position = transform.position;

        if (isChilling || save.isPetting || save.isFeeding || SaveLoad.SaveData[(int)SaveLoad.Line.Satisfaction] == 0)
        {
            direction = 0;
        } else if (direction == 0 || (direction == -1 && position.x < xMin))
        {
            direction = 1;
        } else if (direction == 0 || (direction == 1 && position.x > xMax))
        {
            direction = -1;
        }

        Vector3 movement = Vector3.right * direction * Time.deltaTime;

        if (counter < 0.1)
        {
            counter += Time.deltaTime;
        }
        else
        {
            transform.Translate(movement * speed);
            counter = 0;
        }
        #endregion
    } 

    void BubbleCounter()
    {
        if (bubbleObject.activeInHierarchy)
        {
            bubbleCounter += Time.deltaTime;
        }

        if (bubbleCounter >= maxBubbleCounter)
        {
            bubbleCounter = maxBubbleCounter;
            color.a = color.a - 0.01f;
            if (color.a <= 0)
            {
                color.a = 0;
                bubbleCounter = 0;
                bubbleObject.SetActive(!bubbleObject);
                bubbleTextObject.SetActive(!bubbleTextObject);
            }
        } else
        {
            color.a = 1f;
        }

        bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, color.a);
        bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, color.a);
    }

    void DoTouch(Vector2 point)
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(point), Camera.main.transform.forward);
        if (!hit) return;
        Object hitType = hit.transform.GetComponent<ObjectType>().type;

        Debug.Log(hitType);
        switch (hitType)
        {
            case Object.Kitsulope:
                #region Kitsulope
                if (_touchDown)
                {
                    Debug.Log("You tapped.");
                    isPetNoticingYou = true;
                }

                if (_touchHold && isPetNoticingYou == true)
                {
                    Debug.Log("Touch phase is Moved or Stationary.");
                    animator.SetBool("IsPetting", true);
                }

                if (_touchUp)
                {
                    save.Pet();
                    isPetNoticingYou = false;
                }
                #endregion
                break;
            case Object.Fridge:
                #region fridgeBubble
                if (_touchDown)
                {
                    dialogue = fridge.GetComponent<Dialogue>();

                    // Fridge bubble not active, gets activated. Doesn't do anything yet.
                    if (!bubbleObject.activeInHierarchy)
                    {
                        bubbleCounter = 0;
                        bubbleObject.SetActive(bubbleObject);
                        bubbleSpriteRenderer.sprite = fridgeSprite;
                        bubbleTextObject.SetActive(bubbleTextObject);
                        dialogueManager.StartDialogue(dialogue);
                        color.a = 1;
                    }
                    // Fridge bubble has been activated and is touched when opacity is full. Calls Feed method.
                    else if (bubbleObject.activeInHierarchy
                        && bubbleSpriteRenderer.color.a == 1f
                        && bubbleSpriteRenderer.sprite == fridgeSprite)
                    {
                        save.Feed();
                    }
                    // Generic bubble object active but sprite and dialogue wrong. They are changed here.
                    else if (bubbleObject.activeInHierarchy
                        && bubbleSpriteRenderer.color.a == 1f
                        && bubbleSpriteRenderer.sprite != fridgeSprite)
                    {
                        bubbleSpriteRenderer.sprite = fridgeSprite;
                        dialogue = fridge.GetComponent<Dialogue>();
                        dialogueManager.StartDialogue(dialogue);
                    }
                    // Opacity is less than 1 but more than 0. Opacity is set back to 1 and counter to 0.
                    else if (bubbleSpriteRenderer.color.a < 1f
                        && bubbleSpriteRenderer.color.a > 0
                        && bubbleSpriteRenderer.sprite == fridgeSprite)
                    {
                        bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, 1f);
                        bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                        bubbleCounter = 0;
                    }
                    // Generic bubble object active but sprite and dialogue wrong. They are changed here. Opacity is less than 1 but more than 0. Opacity is set back to 1 and counter to 0.
                    else if (bubbleSpriteRenderer.color.a < 1f
                        && bubbleSpriteRenderer.color.a > 0
                        && bubbleSpriteRenderer.sprite != fridgeSprite)
                    {
                        bubbleSpriteRenderer.sprite = fridgeSprite;
                        dialogue = fridge.GetComponent<Dialogue>();
                        dialogueManager.StartDialogue(dialogue);
                        bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, 1f);
                        bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                        bubbleCounter = 0;
                    }
                }
                #endregion
                break;
            case Object.Window:
                //save.ResetSave();
                break;
            case Object.ExitButton:
                #region exitBubble
                if (_touchHold)
                {
                    dialogue = exitButton.GetComponent<Dialogue>();

                    // Exit bubble not active, gets activated. Doesn't do anything yet.
                    if (!bubbleObject.activeInHierarchy)
                    {
                        bubbleCounter = 0;
                        exitCounter = 4.0f;
                        bubbleObject.SetActive(bubbleObject);
                        bubbleSpriteRenderer.sprite = exitSprite;
                        bubbleTextObject.SetActive(bubbleTextObject);
                        dialogueManager.StartDialogue(dialogue);
                        color.a = 1;
                    }
                    // Exit bubble active. Shouldn't start fading when touch stationary.
                    else if (bubbleObject.activeInHierarchy
                        && bubbleSpriteRenderer.color.a == 1f
                        && bubbleSpriteRenderer.sprite == exitSprite
                        && exitCounter > 0)
                    {
                        exitCounter -= Time.deltaTime;
                        bubbleCounter = 0;
                        Debug.Log("THIS SHOULD WORK?");
                    }
                    // Exit bubble has been activated and is touched when opacity is full. Calls Exit method.
                    else if (bubbleObject.activeInHierarchy
                        && bubbleSpriteRenderer.color.a == 1f
                        && bubbleSpriteRenderer.sprite == exitSprite
                        && exitCounter <= 0)
                    {
                        Debug.Log("AND NOW, EXIT!");
                        save.Exit();
                    }
                    // Generic bubble object active but sprite and dialogue wrong. They are changed here.
                    else if (bubbleObject.activeInHierarchy
                        && bubbleSpriteRenderer.color.a == 1f
                        && bubbleSpriteRenderer.sprite != exitSprite)
                    {
                        bubbleSpriteRenderer.sprite = exitSprite;
                        dialogue = exitButton.GetComponent<Dialogue>();
                        dialogueManager.StartDialogue(dialogue);
                    }
                    // Opacity is less than 1 but more than 0. Opacity is set back to 1 and counter to 0.
                    else if (bubbleSpriteRenderer.color.a < 1f
                        && bubbleSpriteRenderer.color.a > 0
                        && bubbleSpriteRenderer.sprite == exitSprite)
                    {
                        bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, 1f);
                        bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                        bubbleCounter = 0;
                    }
                    // Generic bubble object active but sprite and dialogue wrong. They are changed here. Opacity is less than 1 but more than 0. Opacity is set back to 1 and counter to 0.
                    else if (bubbleSpriteRenderer.color.a < 1f
                        && bubbleSpriteRenderer.color.a > 0
                        && bubbleSpriteRenderer.sprite != exitSprite)
                    {
                        bubbleSpriteRenderer.sprite = exitSprite;
                        dialogue = exitButton.GetComponent<Dialogue>();
                        dialogueManager.StartDialogue(dialogue);
                        bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, 1f);
                        bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                        bubbleCounter = 0;
                    }
                }
                #endregion
                break;
            default:
                Debug.LogWarning("Blep");
                break;
        }
    }
}
