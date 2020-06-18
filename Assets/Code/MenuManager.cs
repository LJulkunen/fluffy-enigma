using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject flashText;
    private Touch theTouch;

    public SaveSerial save;

    void Start()
    {
        InvokeRepeating("FlashTheText", 0f, 0.5f);
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
        if (!save.isIntroOver)
        {
            SceneManager.LoadScene("Intro");
        } else
        {
            SceneManager.LoadScene("Game");
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
