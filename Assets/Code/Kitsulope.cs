using System;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Random = UnityEngine.Random;
using TMPro;

public class Kitsulope : ObjectType
{

    SaveSerial save;
    public GameObject fridge;
    public GameObject exitButton;
    public Sprite exitSprite;
    public Sprite fridgeSprite;

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

    // TODO: When object touched Get component from object (atm fridge & exitButton).
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
    #endregion

    private int clickCount;
    public float rand;

    private void Start()
    {
        save = FindObjectOfType<SaveSerial>();

        // Probably just needs to be set as something at the start to avoid errors. 
        // TODO: Set dialogue to be the dialogue of another object when needed later.
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
        
        animator.SetInteger("Satisfaction", save.satisfiedLvlToSave);
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

        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                DoTouch(Input.GetTouch(0).position);
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            DoTouch(Input.mousePosition);
        }

        #region movement
        Vector3 position = transform.position;

        if (isChilling || save.isPetting || save.isFeeding || save.satisfiedLvlToSave == 0)
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
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(point), Vector2.zero);
        Object hitType = hit.transform.GetComponent<ObjectType>().type;

        /*if (hitType == Object.Fridge)
        {
            bubbleSpriteRenderer.sprite = fridgeSprite;
            dialogue = fridge.GetComponent<Dialogue>();
        } else if (hitType == Object.ExitButton)
        {
            bubbleSpriteRenderer.sprite = exitSprite;
            dialogue = exitButton.GetComponent<Dialogue>();
        }*/

        //TODO: Figure out if there should be object types for bubbles.

        Debug.Log(hitType);
        switch (hitType)
        {
            case Object.Kitsulope:
                save.Pet();
                break;
            case Object.Fridge:
                dialogue = fridge.GetComponent<Dialogue>();

                if (!bubbleObject.activeInHierarchy)
                {
                    bubbleCounter = 0;
                    bubbleObject.SetActive(bubbleObject);
                    bubbleSpriteRenderer.sprite = fridgeSprite;
                    bubbleTextObject.SetActive(bubbleTextObject);
                    dialogueManager.StartDialogue(dialogue);
                    color.a = 1;
                } else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite == fridgeSprite)
                {
                    save.Feed();
                }
                else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite != fridgeSprite)
                {
                    bubbleSpriteRenderer.sprite = fridgeSprite;
                    dialogue = fridge.GetComponent<Dialogue>();
                    dialogueManager.StartDialogue(dialogue);
                }
                else if(bubbleSpriteRenderer.color.a < 1f && bubbleSpriteRenderer.color.a > 0 && bubbleSpriteRenderer.sprite == fridgeSprite)
                {
                    bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, 1f);
                    bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                    bubbleCounter = 0;
                } else if (bubbleSpriteRenderer.color.a < 1f && bubbleSpriteRenderer.color.a > 0 && bubbleSpriteRenderer.sprite != fridgeSprite)
                {
                    bubbleSpriteRenderer.sprite = fridgeSprite;
                    dialogue = fridge.GetComponent<Dialogue>();
                    dialogueManager.StartDialogue(dialogue);
                    bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, 1f);
                    bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                    bubbleCounter = 0;
                }
                break;
            case Object.Window:
                save.ResetSave();
                break;
            case Object.ExitButton:
                dialogue = exitButton.GetComponent<Dialogue>();

                if (!bubbleObject.activeInHierarchy)
                {
                    bubbleCounter = 0;
                    bubbleObject.SetActive(bubbleObject);
                    bubbleSpriteRenderer.sprite = exitSprite;
                    bubbleTextObject.SetActive(bubbleTextObject);
                    dialogueManager.StartDialogue(dialogue);
                    color.a = 1;
                }
                else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite == exitSprite)
                {
                    save.Exit();
                }
                else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite != exitSprite)
                {
                    bubbleSpriteRenderer.sprite = exitSprite;
                    dialogue = exitButton.GetComponent<Dialogue>();
                    dialogueManager.StartDialogue(dialogue);
                }
                else if (bubbleSpriteRenderer.color.a < 1f && bubbleSpriteRenderer.color.a > 0 && bubbleSpriteRenderer.sprite == exitSprite)
                {
                    bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, 1f);
                    bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                    bubbleCounter = 0;
                } else if (bubbleSpriteRenderer.color.a < 1f && bubbleSpriteRenderer.color.a > 0 && bubbleSpriteRenderer.sprite != exitSprite)
                {
                    bubbleSpriteRenderer.sprite = exitSprite;
                    dialogue = exitButton.GetComponent<Dialogue>();
                    dialogueManager.StartDialogue(dialogue);
                    bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, 1f);
                    bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                    bubbleCounter = 0;
                }
                break;
            default:
                Debug.LogWarning("Blep");
                break;
        }
    }
}
