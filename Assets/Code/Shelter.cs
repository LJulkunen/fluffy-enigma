using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shelter : MonoBehaviour
{
    SaveSerial save;

    //public GameObject textPlacementObject;

    public Dialogue dialogue;
    public DialogueManager dialogueManager;
    public GameObject dialogueBoxObject;
    public GameObject dialogueTextObject;
    public TextMeshProUGUI dialogueText;

    public GameObject flashText;

    float timer = 0;

    private void Start()
    {
        save = FindObjectOfType<SaveSerial>();

        dialogue = GetComponent<Dialogue>();
        dialogueManager = GetComponent<DialogueManager>();

        dialogueText = dialogueTextObject.GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > 2.5)
        {
            if (!dialogueBoxObject.activeInHierarchy)
            {
                dialogueBoxObject.SetActive(dialogueBoxObject);
                dialogueTextObject.SetActive(dialogueTextObject);
                dialogueManager.StartDialogue(dialogue);
                flashText.SetActive(true);
                InvokeRepeating("FlashTheText", 0f, 0.5f);
            }
             else if (Input.touchCount > 0)
            {
                
                if (dialogueBoxObject.activeInHierarchy && dialogueManager.sentences.Count > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    dialogueManager.DisplayNextSentence();
                }
                else if (dialogueBoxObject.activeInHierarchy && dialogueManager.sentences.Count == 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
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
    }

    void LoadGameScene()
    {
        save.isIntroOver = 1;
        SceneManager.LoadScene("Game");
    }

    void FlashTheText()
    {
        if (flashText.activeInHierarchy)
        {
            flashText.SetActive(false);
        }
        else
        {
            flashText.SetActive(true);
        }
    }
}
