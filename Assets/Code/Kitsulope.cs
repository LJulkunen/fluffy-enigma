using System;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    private void Start()
    {
        save = GetComponent<SaveSerial>();
        dialogue = GetComponent<Dialogue>();
        dialogueManager = GetComponent<DialogueManager>();
    }

    private int clickCount;

    void Update()
    {
        animator.SetInteger("Satisfaction", save.satisfiedLvlToSave);
        animator.SetInteger("Direction", direction);

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

        if (save.satisfiedLvlToSave == 0 || save.satisfiedLvlToSave == 4)
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

    void DoTouch(Vector2 point)
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(point), Vector2.zero);
        Object hitType = hit.transform.GetComponent<ObjectType>().type;

        // Find out how to do this with touching anything BUT the fridge.
        if(hitType != Object.Fridge)
        {
            fridgeBubble.SetActive(!fridgeBubble);
        }

        Debug.Log(hitType);
        switch (hitType)
        {
            case Object.Kitsulope:
                save.Pet();
                break;
            case Object.Fridge:
                dialogueManager.StartDialogue(dialogue);
                if (fridgeBubble.activeInHierarchy)
                {
                        save.Feed();
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
