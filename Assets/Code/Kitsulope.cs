using System;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Random = UnityEngine.Random;
using TMPro;

[RequireComponent(typeof(SaveSerial))]
public class Kitsulope : ObjectType
{
    [SerializeField]
    private float xMin = -0.9F;

    [SerializeField]
    private float xMax = 0.9F;

    [SerializeField]
    float speed = 3F;

    int direction = 1;
    float counter = 0;

    SaveSerial save;

    Dialogue dialogue;
    DialogueManager dialogueManager;

    public Animator animator;

    public GameObject fridgeBubble;
    public GameObject fridgeBubbleText;

    public TextMeshProUGUI bubbleText;

    public SpriteRenderer fridgeSprite;

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

    public Color color;
    
    [SerializeField]
    public float maxBubbleCounter = 10f;
    public float bubbleCounter = 0f;

    private void Start()
    {
        save = GetComponent<SaveSerial>();
        dialogue = GetComponent<Dialogue>();
        dialogueManager = GetComponent<DialogueManager>();
        maxPettingAnimationLength = pettingAnimationLength;
        maxFeedingAnimationLength = feedingAnimationLength;
        maxChillingAnimationLength = chillingAnimationLength;
        color = fridgeSprite.color;
        bubbleText = fridgeBubbleText.GetComponent<TextMeshProUGUI>();
    }

    private int clickCount;


    public float rand;

    void Update()
    {
        FridgeBubbleCounter();
        rand = Random.Range(0.0f, 100.0f);

        if (rand > 99.9f)
        {
            isChilling = true;
        }
        
        animator.SetInteger("Satisfaction", save.satisfiedLvlToSave);
        animator.SetBool("IsPetting", save.isPetting);
        animator.SetBool("IsFeeding", save.isFeeding);
        animator.SetInteger("Direction", direction);
        animator.SetBool("IsChilling", isChilling);

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

    void FridgeBubbleCounter()
    {
        if (fridgeBubble.activeInHierarchy)
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
                fridgeBubble.SetActive(!fridgeBubble);
                fridgeBubbleText.SetActive(!fridgeBubbleText);
            }
        } else
        {
            color.a = 1f;
        }

        fridgeSprite.color = new Color(fridgeSprite.color.r, fridgeSprite.color.g, fridgeSprite.color.b, color.a);
        bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, color.a);
    }
    void DoTouch(Vector2 point)
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(point), Vector2.zero);
        Object hitType = hit.transform.GetComponent<ObjectType>().type;

        // Find out how to do this with touching anything BUT the fridge.
        /*if(hitType != Object.Fridge)
        {
            fridgeBubble.SetActive(!fridgeBubble);
            fridgeBubbleText.SetActive(!fridgeBubbleText);
        }*/

        Debug.Log(hitType);
        switch (hitType)
        {
            case Object.Kitsulope:
                save.Pet();
                break;
            case Object.Fridge:
                if (!fridgeBubble.activeInHierarchy)
                {
                    fridgeBubble.SetActive(fridgeBubble);
                    fridgeBubbleText.SetActive(fridgeBubbleText);
                    dialogueManager.StartDialogue(dialogue);
                    color.a = 1;
                } else if (fridgeBubble.activeInHierarchy && fridgeSprite.color.a == 1f)
                {
                        save.Feed();
                } else if(fridgeSprite.color.a < 1f && fridgeSprite.color.a > 0)
                {
                    fridgeSprite.color = new Color(fridgeSprite.color.r, fridgeSprite.color.g, fridgeSprite.color.b, 1f);
                    bubbleText.color = new Color(bubbleText.color.r, bubbleText.color.g, bubbleText.color.b, 1f);
                    bubbleCounter = 0;
                }
                break;
            case Object.Window:
                break;
            case Object.ExitButton:
                if (Input.touchCount == 1)
                {
                    save.Exit();
                }
                break;
            default:
                Debug.LogWarning("Blep");
                break;
        }
    }
}
