using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject flashText;
    private Touch theTouch;

    void Start()
    {
        InvokeRepeating("flashTheText", 0f, 0.5f);
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            theTouch = Input.GetTouch(0);
            if (theTouch.phase == TouchPhase.Ended)
            {
                SceneManager.LoadScene("Game");
            }
        }
    }

    void flashTheText()
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
