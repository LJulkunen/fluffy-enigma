using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    public GameObject dialogueBox;
    public GameObject dialogue;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                dialogueBox.SetActive(dialogueBox);
                dialogue.SetActive(dialogue);
                //LoadGameScene();
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            dialogueBox.SetActive(dialogueBox);
            dialogue.SetActive(dialogue);
            //LoadGameScene();
        }
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene("Game");
    }
}
