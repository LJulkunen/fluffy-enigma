﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExitDialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public Queue<string> sentences;

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(ExitDialogue dialogue)
    {
        Debug.Log("Starting dialogue");

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
    }

    void EndDialogue()
    {
        Debug.Log("End of dialogue.");
    }
}
