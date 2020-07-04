﻿using UnityEngine;
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

public class Kitsulope : ObjectType
{

    SaveSerial save;
    public GameObject fridge;
    public GameObject window;
    public GameObject shelf;
    public GameObject exitButton;
    public GameObject bike;
    public GameObject rusakko;

    public Sprite exitBubble;
    public Sprite windowBubble;
    public Sprite downBubble;
    public Sprite cornerBubble;

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

    [SerializeField]
    public float yawningAnimationLength = 2.5f;
    public float maxYawningAnimationLength = 2.5f;

    public bool isChilling;
    public bool isPetNoticingYou;
    public bool isYawning;

    public float rand;

    public SpriteRenderer fridgeRenderer;
    public Sprite fridgeClosed;
    public Sprite fridgeOpen;
    #endregion

    public Vector3 newPos;

    private void Start()
    {
        save = FindObjectOfType<SaveSerial>();

    [SerializeField]
    public float feedingAnimationLength = 2.5f;
    public float maxFeedingAnimationLength = 2.5f;

        maxPettingAnimationLength = pettingAnimationLength;
        maxFeedingAnimationLength = feedingAnimationLength;
        maxChillingAnimationLength = chillingAnimationLength;
        color = bubbleSpriteRenderer.color;

        bubbleText = bubbleTextObject.GetComponent<TextMeshProUGUI>();

        if (SceneManager.GetActiveScene().name == "Field")
        {
            newPos.x = xMin;
            transform.Translate(newPos);
        }
        else if (SceneManager.GetActiveScene().name == "Game")
        {
            newPos.x = xMax;
            transform.Translate(newPos);
        }
    }

    public bool isChilling;
    public bool isPetNoticingYou;

        if (!save.isFeeding || !save.isPetting)
        {
            rand = Random.Range(0.0f, 100.0f);

            if (rand > 99.6f)
            {
                isChilling = true;
                if (rand > 99.9)
                {
                    isYawning = true;
                    chillingAnimationLength = maxYawningAnimationLength;
                }
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
        animator.SetBool("IsYawning", isYawning);
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

        if (isYawning)
        {
            chillingAnimationLength -= Time.deltaTime;
            if (chillingAnimationLength <= 0)
            {
                isChilling = false;
                isYawning = false;
                chillingAnimationLength = maxChillingAnimationLength;
            }
        } else if (isChilling)
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
                fridgeRenderer.sprite = fridgeClosed;
            }
        } else
        {
            color.a = 1f;
        }

        bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, color.a);
        bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, color.a);
    }

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

    void DoTouch(Vector2 point)
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(point), Vector2.down);
        Object hitType = hit.transform.GetComponent<ObjectType>().type;

        //Debug.Log(hitType);
        switch (hitType)
        {
            case Object.Kitsulope:
                #region kitsulope
                // These might be useful later if I have time & it would be an improvement to add more scenes/animations.
                /*if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Debug.Log("You tapped.");
                    isPetNoticingYou = true;
                    animator.SetBool("IsPetting", true);
                }

                if ((Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch(0).phase == TouchPhase.Began) && isPetNoticingYou == true)*/
                if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary)
                {
                    Debug.Log("Touch phase is Began, Moved or Stationary.");
                    animator.SetBool("IsPetting", true);
                } else if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    save.Pet();
                    //isPetNoticingYou = false;
                }
                #endregion
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
                        bubbleSpriteRenderer.sprite = downBubble;
                        bubbleTextObject.SetActive(bubbleTextObject);
                        dialogueManager.StartDialogue(dialogue);
                        color.a = 1;
                    }
                    // Fridge bubble has been activated and is touched when opacity is full. Calls Feed method.
                    else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite == downBubble)
                    {
                        fridgeRenderer.sprite = fridgeOpen;
                        save.Feed();
                    }
                    // Generic bubble object active but sprite and dialogue wrong. They are changed here.
                    else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite != downBubble)
                    {
                        bubbleSpriteRenderer.sprite = downBubble;
                        dialogue = fridge.GetComponent<Dialogue>();
                        dialogueManager.StartDialogue(dialogue);
                    }
                    // Opacity is less than 1 but more than 0. Opacity is set back to 1 and counter to 0.
                    else if (bubbleSpriteRenderer.color.a < 1f && bubbleSpriteRenderer.color.a > 0 && bubbleSpriteRenderer.sprite == downBubble)
                    {
                        bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, 1f);
                        bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                        bubbleCounter = 0;
                    }
                    // Generic bubble object active but sprite and dialogue wrong. They are changed here. Opacity is less than 1 but more than 0. Opacity is set back to 1 and counter to 0.
                    else if (bubbleSpriteRenderer.color.a < 1f && bubbleSpriteRenderer.color.a > 0 && bubbleSpriteRenderer.sprite != downBubble)
                    {
                        bubbleSpriteRenderer.sprite = downBubble;
                        dialogue = fridge.GetComponent<Dialogue>();
                        dialogueManager.StartDialogue(dialogue);
                        bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, 1f);
                        bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                        bubbleCounter = 0;
                    }
                }
                #endregion
                break;
            case Object.Shelf:
                #region shelfBubble
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    dialogue = shelf.GetComponent<Dialogue>();

                    // Fridge bubble not active, gets activated. Doesn't do anything yet.
                    if (!bubbleObject.activeInHierarchy)
                    {
                        bubbleCounter = 0;
                        bubbleObject.SetActive(bubbleObject);
                        bubbleSpriteRenderer.sprite = cornerBubble;
                        bubbleTextObject.SetActive(bubbleTextObject);
                        dialogueManager.StartDialogue(dialogue);
                        color.a = 1;
                    }
                    // Reading.
                    else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite == cornerBubble)
                    {
                        dialogueManager.DisplayNextSentence();
                        save.Read();
                        Debug.LogWarning("READING!");
                    }
                    // Generic bubble object active but sprite and dialogue wrong. They are changed here.
                    else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite != cornerBubble)
                    {
                        bubbleSpriteRenderer.sprite = cornerBubble;
                        dialogue = shelf.GetComponent<Dialogue>();
                        dialogueManager.StartDialogue(dialogue);
                    }
                    // Opacity is less than 1 but more than 0. Opacity is set back to 1 and counter to 0.
                    else if (bubbleSpriteRenderer.color.a < 1f && bubbleSpriteRenderer.color.a > 0 && bubbleSpriteRenderer.sprite == cornerBubble)
                    {
                        bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, 1f);
                        bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                        bubbleCounter = 0;
                    }
                    // Generic bubble object active but sprite and dialogue wrong. They are changed here. Opacity is less than 1 but more than 0. Opacity is set back to 1 and counter to 0.
                    else if (bubbleSpriteRenderer.color.a < 1f && bubbleSpriteRenderer.color.a > 0 && bubbleSpriteRenderer.sprite != cornerBubble)
                    {
                        bubbleSpriteRenderer.sprite = cornerBubble;
                        dialogue = shelf.GetComponent<Dialogue>();
                        dialogueManager.StartDialogue(dialogue);
                        bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, 1f);
                        bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                        bubbleCounter = 0;
                    }
                }
                #endregion
                break;
            case Object.Window:
                #region toTheField?
                if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Stationary)
                {
                    dialogue = window.GetComponent<Dialogue>();

                    // Exit bubble not active, gets activated. Doesn't do anything yet.
                    if (!bubbleObject.activeInHierarchy)
                    {
                        bubbleCounter = 0;
                        exitCounter = 2.0f;
                        bubbleObject.SetActive(bubbleObject);
                        bubbleSpriteRenderer.sprite = windowBubble;
                        bubbleTextObject.SetActive(bubbleTextObject);
                        dialogueManager.StartDialogue(dialogue);
                        color.a = 1;
                    }
                    // Exit bubble active. Shouldn't start fading when touch stationary.
                    else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite == windowBubble
                        && exitCounter > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary)
                    {
                        exitCounter -= Time.deltaTime;
                        bubbleCounter = 0;
                        Debug.Log("THIS SHOULD WORK?");
                    }
                    // Exit bubble has been activated and is touched when opacity is full. Calls Exit method.
                    else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite == windowBubble
                        && exitCounter <= 0)
                    {
                        dialogue = window.GetComponent<Dialogue>();
                        dialogueManager.StartDialogue(dialogue);
                        Debug.Log("To the field!");
                        SceneManager.LoadScene("Field");
                    }
                    // Generic bubble object active but sprite and dialogue wrong. They are changed here.
                    else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite != windowBubble)
                    {
                        bubbleSpriteRenderer.sprite = windowBubble;
                        dialogue = window.GetComponent<Dialogue>();
                        dialogueManager.StartDialogue(dialogue);
                        bubbleCounter = 0;
                    }
                    // Opacity is less than 1 but more than 0. Opacity is set back to 1 and counter to 0.
                    else if (bubbleSpriteRenderer.color.a < 1f && bubbleSpriteRenderer.color.a > 0 && bubbleSpriteRenderer.sprite == windowBubble)
                    {
                        bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, 1f);
                        bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                        bubbleCounter = 0;
                    }
                    // Generic bubble object active but sprite and dialogue wrong. They are changed here. Opacity is less than 1 but more than 0. Opacity is set back to 1 and counter to 0.
                    else if (bubbleSpriteRenderer.color.a < 1f && bubbleSpriteRenderer.color.a > 0 && bubbleSpriteRenderer.sprite != windowBubble)
                    {
                        bubbleSpriteRenderer.sprite = windowBubble;
                        dialogue = window.GetComponent<Dialogue>();
                        dialogueManager.StartDialogue(dialogue);
                        bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, 1f);
                        bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                        bubbleCounter = 0;
                    }
                }
                #endregion
                break;
            case Object.WildRabbit:
                #region resetRusakko
                if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Stationary)
                {
                    dialogue = rusakko.GetComponent<Dialogue>();

                    // Exit bubble not active, gets activated. Doesn't do anything yet.
                    if (!bubbleObject.activeInHierarchy)
                    {
                        bubbleCounter = 0;
                        exitCounter = 2.0f;
                        bubbleObject.SetActive(bubbleObject);
                        bubbleSpriteRenderer.sprite = downBubble;
                        bubbleTextObject.SetActive(bubbleTextObject);
                        dialogueManager.StartDialogue(dialogue);
                        color.a = 1;
                    }
                    // Exit bubble active. Shouldn't start fading when touch stationary.
                    else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite == downBubble
                        && exitCounter > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary)
                    {
                        exitCounter -= Time.deltaTime;
                        bubbleCounter = 0;
                    }
                    // Exit bubble has been activated and is touched when opacity is full. Calls Exit method.
                    else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite == downBubble
                        && exitCounter <= 0)
                    {
                        dialogue = rusakko.GetComponent<Dialogue>();
                        dialogueManager.StartDialogue(dialogue);
                        save.ResetSave();
                        Debug.Log("ResetRusakko!");
                    }
                    // Generic bubble object active but sprite and dialogue wrong. They are changed here.
                    else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite != downBubble)
                    {
                        bubbleSpriteRenderer.sprite = downBubble;
                        dialogue = rusakko.GetComponent<Dialogue>();
                        dialogueManager.StartDialogue(dialogue);
                        bubbleCounter = 0;
                    }
                    // Opacity is less than 1 but more than 0. Opacity is set back to 1 and counter to 0.
                    else if (bubbleSpriteRenderer.color.a < 1f && bubbleSpriteRenderer.color.a > 0 && bubbleSpriteRenderer.sprite == downBubble)
                    {
                        bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, 1f);
                        bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                        bubbleCounter = 0;
                    }
                    // Generic bubble object active but sprite and dialogue wrong. They are changed here. Opacity is less than 1 but more than 0. Opacity is set back to 1 and counter to 0.
                    else if (bubbleSpriteRenderer.color.a < 1f && bubbleSpriteRenderer.color.a > 0 && bubbleSpriteRenderer.sprite != downBubble)
                    {
                        bubbleSpriteRenderer.sprite = downBubble;
                        dialogue = rusakko.GetComponent<Dialogue>();
                        dialogueManager.StartDialogue(dialogue);
                        bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, 1f);
                        bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                        bubbleCounter = 0;
                    }
                }
                #endregion
                break;
            case Object.Bike:
                #region backInside?
                if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Stationary)
                {
                    dialogue = bike.GetComponent<Dialogue>();

                    // Exit bubble not active, gets activated. Doesn't do anything yet.
                    if (!bubbleObject.activeInHierarchy)
                    {
                        bubbleCounter = 0;
                        exitCounter = 2.0f;
                        bubbleObject.SetActive(bubbleObject);
                        bubbleSpriteRenderer.sprite = cornerBubble;
                        bubbleTextObject.SetActive(bubbleTextObject);
                        dialogueManager.StartDialogue(dialogue);
                        color.a = 1;
                    }
                    // Exit bubble active. Shouldn't start fading when touch stationary.
                    else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite == cornerBubble
                        && exitCounter > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary)
                    {
                        exitCounter -= Time.deltaTime;
                        bubbleCounter = 0;
                    }
                    // Exit bubble has been activated and is touched when opacity is full. Calls Exit method.
                    else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite == cornerBubble
                        && exitCounter <= 0)
                    {
                        dialogue = bike.GetComponent<Dialogue>();
                        dialogueManager.StartDialogue(dialogue);
                        Debug.Log("Back inside!");
                        SceneManager.LoadScene("Game");
                    }
                    // Generic bubble object active but sprite and dialogue wrong. They are changed here.
                    else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite != cornerBubble)
                    {
                        bubbleSpriteRenderer.sprite = cornerBubble;
                        dialogue = bike.GetComponent<Dialogue>();
                        dialogueManager.StartDialogue(dialogue);
                        bubbleCounter = 0;
                    }
                    // Opacity is less than 1 but more than 0. Opacity is set back to 1 and counter to 0.
                    else if (bubbleSpriteRenderer.color.a < 1f && bubbleSpriteRenderer.color.a > 0 && bubbleSpriteRenderer.sprite == cornerBubble)
                    {
                        bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, 1f);
                        bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                        bubbleCounter = 0;
                    }
                    // Generic bubble object active but sprite and dialogue wrong. They are changed here. Opacity is less than 1 but more than 0. Opacity is set back to 1 and counter to 0.
                    else if (bubbleSpriteRenderer.color.a < 1f && bubbleSpriteRenderer.color.a > 0 && bubbleSpriteRenderer.sprite != cornerBubble)
                    {
                        bubbleSpriteRenderer.sprite = cornerBubble;
                        dialogue = bike.GetComponent<Dialogue>();
                        dialogueManager.StartDialogue(dialogue);
                        bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, 1f);
                        bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                        bubbleCounter = 0;
                    }
                }
                #endregion
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
                        bubbleSpriteRenderer.sprite = exitBubble;
                        bubbleTextObject.SetActive(bubbleTextObject);
                        dialogueManager.StartDialogue(dialogue);
                        color.a = 1;
                    }
                    // Exit bubble active. Shouldn't start fading when touch stationary.
                    else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite == exitBubble
                        && exitCounter > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary)
                    {
                        exitCounter -= Time.deltaTime;
                        bubbleCounter = 0;
                        Debug.Log("THIS SHOULD WORK?");
                    }
                    // Exit bubble has been activated and is touched when opacity is full. Calls Exit method.
                    else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite == exitBubble
                        && exitCounter <= 0)
                    {
                        Debug.Log("AND NOW, EXIT!");
                        save.Exit();
                    }
                    // Generic bubble object active but sprite and dialogue wrong. They are changed here.
                    else if (bubbleObject.activeInHierarchy && bubbleSpriteRenderer.color.a == 1f && bubbleSpriteRenderer.sprite != exitBubble)
                    {
                        bubbleSpriteRenderer.sprite = exitBubble;
                        dialogue = exitButton.GetComponent<Dialogue>();
                        dialogueManager.StartDialogue(dialogue);
                        bubbleCounter = 0;
                    }
                    // Opacity is less than 1 but more than 0. Opacity is set back to 1 and counter to 0.
                    else if (bubbleSpriteRenderer.color.a < 1f && bubbleSpriteRenderer.color.a > 0 && bubbleSpriteRenderer.sprite == exitBubble)
                    {
                        bubbleSpriteRenderer.color = new Color(bubbleSpriteRenderer.color.r, bubbleSpriteRenderer.color.g, bubbleSpriteRenderer.color.b, 1f);
                        bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                        bubbleCounter = 0;
                    }
                    // Generic bubble object active but sprite and dialogue wrong. They are changed here. Opacity is less than 1 but more than 0. Opacity is set back to 1 and counter to 0.
                    else if (bubbleSpriteRenderer.color.a < 1f && bubbleSpriteRenderer.color.a > 0 && bubbleSpriteRenderer.sprite != exitBubble)
                    {
                        bubbleSpriteRenderer.sprite = exitBubble;
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
