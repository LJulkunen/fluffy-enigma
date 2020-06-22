using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    SaveSerial save;

    //public GameObject textPlacementObject;

    public Dialogue dialogue;
    public DialogueManager dialogueManager;
    public GameObject dialogueBoxObject;
    public GameObject dialogueTextObject;
    public TextMeshProUGUI dialogueText;

    private void Start()
    {
        save = FindObjectOfType<SaveSerial>();

        dialogue = GetComponent<Dialogue>();
        dialogueManager = GetComponent<DialogueManager>();

        dialogueText = dialogueTextObject.GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (!dialogueBoxObject.activeInHierarchy)
                {
                    dialogueBoxObject.SetActive(dialogueBoxObject);
                    dialogueTextObject.SetActive(dialogueTextObject);
                    dialogueManager.StartDialogue(dialogue);
                }
            }
            else if (dialogueBoxObject.activeInHierarchy && dialogueManager.sentences.Count > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                dialogueManager.DisplayNextSentence();
            } else if (dialogueBoxObject.activeInHierarchy && dialogueManager.sentences.Count == 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                LoadGameScene();
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (!dialogueBoxObject.activeInHierarchy)
            {
                dialogueBoxObject.SetActive(dialogueBoxObject);
                dialogueTextObject.SetActive(dialogueTextObject);
                dialogueManager.StartDialogue(dialogue);
            }
            else if (dialogueBoxObject.activeInHierarchy && dialogueManager.sentences.Count > 0)
            {
                dialogueManager.DisplayNextSentence();
            }
            else if (dialogueBoxObject.activeInHierarchy && dialogueManager.sentences.Count == 0)
            {
                LoadGameScene();
            }
        }
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene("Shelter");
    }
}
