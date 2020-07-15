using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AloeVera : ObjectType
{

    SaveSerial save;
    public GameObject fridge;
    public GameObject aloe;
    public GameObject exitButton;
    public Sprite exitSprite;
    public Sprite fridgeSprite;
    public Sprite cornerSprite;

    public GameObject wateringCan;

    public SpriteRenderer aloeSprite;
    public Sprite healedAloe;
    public Sprite growingAloe;
    public Sprite grownAloe;

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

    #region dialogueBubbles
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
    #endregion

    public float randX;
    public Vector3 newPos;

    private void Start()
    {
        save = FindObjectOfType<SaveSerial>();

        // Needs to be set as something at the start to avoid errors. 
        dialogue = fridge.GetComponent<Dialogue>();
        dialogueManager = GetComponent<DialogueManager>();

        color = bubbleSpriteRenderer.color;

        bubbleText = bubbleTextObject.GetComponent<TextMeshProUGUI>();

        Debug.Log("Aloe watered? " + save.aloeWatered + " Aloe level? :" + save.aloeLevel);

        switch (save.aloeLevel) {

            case 1:
                aloeSprite.sprite = healedAloe;
                break;
            case 2:
                aloeSprite.sprite = growingAloe;
                break;
            case 3:
                aloeSprite.sprite = grownAloe;
                break;
        }
        #region randomPlacement(A joke, don't use in actual game)
        /*randX = Random.Range(xMin, xMax);
        newPos.x = randX;
        newPos.y = transform.position.y;
        transform.Translate(newPos);
        Debug.Log(newPos);*/
        #endregion
    }

    void Update()
    {
        BubbleCounter();

        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved
             || Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                DoTouch(Input.GetTouch(0).position);
                if (Input.GetTouch(0).phase == TouchPhase.Stationary)
                {
                    Debug.Log("TouchPhase: Stationary");
                }
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            DoTouch(Input.mousePosition);
        }

        #region movement
        /*Vector3 position = transform.position;
        if (direction == 0 || (direction == -1 && position.x < xMin))
        {
            direction = 1;
        }
        else if (direction == 0 || (direction == 1 && position.x > xMax))
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
        }*/
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
                wateringCan.SetActive(!wateringCan);
            }
        }
        else
        {
            color.a = 1f;
        }

        bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, color.a);
        bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, color.a);
    }

    public float exitCounter = 1.5f;

    void DoTouch(Vector2 point)
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(point), Vector2.down);
        Object hitType = hit.transform.GetComponent<ObjectType>().type;

        //Debug.Log(hitType);
        switch (hitType)
        {
            case Object.AloeVera:
                if (save.aloeLevel == 3)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        dialogue = aloe.GetComponent<Dialogue>();

                        // Bubble not active, gets activated. Doesn't do anything yet.
                        if (!bubbleObject.activeInHierarchy)
                        {
                            bubbleCounter = 0;
                            bubbleObject.SetActive(bubbleObject);
                            bubbleSpriteRenderer.sprite = cornerSprite;
                            bubbleTextObject.SetActive(bubbleTextObject);
                            dialogueManager.StartDialogue(dialogue);
                            color.a = 1;
                        }

                        //Bubble has been activated and is touched when opacity is full. Calls Feed method.
                        else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite == cornerSprite && dialogueManager.sentences.Count > 0)
                        {
                            dialogueManager.DisplayNextSentence();
                        }
                        else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite == cornerSprite && dialogueManager.sentences.Count == 0)
                        {
                            Debug.LogWarning("RESETTING FROM ALOE END!");
                            save.ResetSave();
                        }
                        // Generic bubble object active but sprite and dialogue wrong. They are changed here.
                        else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite != cornerSprite)
                        {
                            bubbleSpriteRenderer.sprite = cornerSprite;
                            dialogue = aloe.GetComponent<Dialogue>();
                            dialogueManager.StartDialogue(dialogue);
                        }
                        // Opacity is less than 1 but more than 0. Opacity is set back to 1 and counter to 0.
                        else if (bubbleSpriteRenderer.color.a < 1f && bubbleSpriteRenderer.color.a > 0 && bubbleSpriteRenderer.sprite == cornerSprite)
                        {
                            bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, 1f);
                            bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                            bubbleCounter = 0;
                        }
                        // Generic bubble object active but sprite and dialogue wrong. They are changed here. Opacity is less than 1 but more than 0. Opacity is set back to 1 and counter to 0.
                        else if (bubbleSpriteRenderer.color.a < 1f && bubbleSpriteRenderer.color.a > 0 && bubbleSpriteRenderer.sprite != cornerSprite)
                        {
                            bubbleSpriteRenderer.sprite = cornerSprite;
                            dialogue = aloe.GetComponent<Dialogue>();
                            dialogueManager.StartDialogue(dialogue);
                            bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, 1f);
                            bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                            bubbleCounter = 0;
                        }
                    }
                }
                break;
            case Object.Fridge:
                #region fridgeBubble
                if (Input.GetTouch(0).phase == TouchPhase.Began)
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
                    else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite == fridgeSprite)
                    {
                        wateringCan.SetActive(wateringCan);
                        save.WaterAloe();
                    }
                    // Generic bubble object active but sprite and dialogue wrong. They are changed here.
                    else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite != fridgeSprite)
                    {
                        bubbleSpriteRenderer.sprite = fridgeSprite;
                        dialogue = fridge.GetComponent<Dialogue>();
                        dialogueManager.StartDialogue(dialogue);
                    }
                    // Opacity is less than 1 but more than 0. Opacity is set back to 1 and counter to 0.
                    else if (bubbleSpriteRenderer.color.a < 1f && bubbleSpriteRenderer.color.a > 0 && bubbleSpriteRenderer.sprite == fridgeSprite)
                    {
                        bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, 1f);
                        bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                        bubbleCounter = 0;
                    }
                    // Generic bubble object active but sprite and dialogue wrong. They are changed here. Opacity is less than 1 but more than 0. Opacity is set back to 1 and counter to 0.
                    else if (bubbleSpriteRenderer.color.a < 1f && bubbleSpriteRenderer.color.a > 0 && bubbleSpriteRenderer.sprite != fridgeSprite)
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
                exitCounter -= Time.deltaTime;
                if (exitCounter <= 0)
                {
                    save.ResetSave();
                    exitCounter = 1.5f;
                }
                break;
            case Object.ExitButton:
                #region exitBubble
                if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Stationary)
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
                    else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite == exitSprite
                        && exitCounter > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary)
                    {
                        exitCounter -= Time.deltaTime;
                        bubbleCounter = 0;
                        Debug.Log("THIS SHOULD WORK?");
                    } 
                    // Exit bubble has been activated and is touched when opacity is full. Calls Exit method.
                    else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite == exitSprite
                        && exitCounter <= 0)
                    {
                        Debug.Log("AND NOW, EXIT!");
                        save.Exit();
                    }
                    // Generic bubble object active but sprite and dialogue wrong. They are changed here.
                    else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite != exitSprite)
                    {
                        bubbleSpriteRenderer.sprite = exitSprite;
                        dialogue = exitButton.GetComponent<Dialogue>();
                        dialogueManager.StartDialogue(dialogue);
                    }
                    // Opacity is less than 1 but more than 0. Opacity is set back to 1 and counter to 0.
                    else if (bubbleSpriteRenderer.color.a < 1f && bubbleSpriteRenderer.color.a > 0 && bubbleSpriteRenderer.sprite == exitSprite)
                    {
                        bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, 1f);
                        bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                        bubbleCounter = 0;
                    }
                    // Generic bubble object active but sprite and dialogue wrong. They are changed here. Opacity is less than 1 but more than 0. Opacity is set back to 1 and counter to 0.
                    else if (bubbleSpriteRenderer.color.a < 1f && bubbleSpriteRenderer.color.a > 0 && bubbleSpriteRenderer.sprite != exitSprite)
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
