using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject flashText;
    private Touch theTouch;

    private SaveSerial save;

    void Start()
    {
        InvokeRepeating("FlashTheText", 0f, 0.5f);

        save = SaveSerial.SAVE;
    }
    
    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                LoadGameScene();
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            LoadGameScene();
        }
    }

    void LoadGameScene()
    {
        if (save.hungerLvlToSave == 0)
        {
            SceneManager.LoadScene("AloeCareGame");
        }
        else if (save.isIntroOver == 1)
        {
            SceneManager.LoadScene("Game");
        }
        else
        {
            SceneManager.LoadScene("Intro");
        }
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
