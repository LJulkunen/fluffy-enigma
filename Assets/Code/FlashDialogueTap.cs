using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashDialogueTap : MonoBehaviour
{
    public GameObject flashText;

    void Start()
    {
        InvokeRepeating("FlashTheText", 0f, 0.5f);
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
