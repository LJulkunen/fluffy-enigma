using System;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(SaveSerial))]
public class Kitsulope : MonoBehaviour
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

    public Animator animator;

    private void Start()
    {
        save = GetComponent<SaveSerial>();
    }

    private int clickCount;

    void Update()
    {
        animator.SetInteger("Satisfaction", save.satisfiedLvlToSave);
        animator.SetInteger("Direction", direction);

        if (Input.touchCount > 0)
        {
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            if (GetComponent<Collider2D>().OverlapPoint(touchPos) && Input.GetTouch(0).phase == TouchPhase.Began) {
                save.Pet();
            }
        }

        #region forPCbuild
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Affection level: " + save.affectionLvlToSave);
            Vector2 v = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(v), Vector2.zero);

            if (hit)
            {
                Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.gameObject.tag == "Kitsulope")
                {
                    clickCount++;

                    save.Pet();
                    if (clickCount >= 1)
                    {
                        
                    }
                }
            }
        } 
        #endregion

        #region movement
        Vector3 position = transform.position;

        /*if (direction == 1 && position.x > xMax)
        {
            direction = -1;
            //animator.SetInteger("Direction", direction);
        }
        else if (direction == -1 && position.x < xMin)
        {
            direction = 1;
            //animator.SetInteger("Direction", direction);
        }*/

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
}


/* // Updated affection not saved, fix that.
// Find out how to save a changed value.
public class Kitsulope : UnitBase
{
    [SerializeField]
    private float xMin = -0.5F;

    [SerializeField]
    private float xMax = 0.5F;

    int direction = 1;
    float counter = 0;

    [SerializeField]
    public int hunger;
    [SerializeField]
    public int affection;
    [SerializeField]
    public String name;

    private bool serverTime;

    public int Hunger
    {
        get { return hunger; }
        set { hunger = value; }
    }

    public int Affection
    {
        get { return affection; }
        set { affection = value; }
    }

    public String Name
    {
        get { return name; }
        set { name = value; }
    }

    private void Start()
    {
        // v This should be when the game is first started.
        //PlayerPrefs.SetString("then", "01/08/2020 07:05:00");
        UpdateStatus();
        if (!PlayerPrefs.HasKey("name"))
        {
            PlayerPrefs.SetString("name", "Default");
        }
        name = PlayerPrefs.GetString("name");
    }

    void UpdateStatus()
    {
        
        if (!PlayerPrefs.HasKey("hunger"))
        {
            hunger = 100;
            PlayerPrefs.SetInt("hunger", hunger);
        }
        else
        {
            hunger = PlayerPrefs.GetInt("hunger");
        }

        if (!PlayerPrefs.HasKey("affection"))
        {
            affection = 100;
            PlayerPrefs.SetInt("affection", affection);
        }
        else
        {
            affection = PlayerPrefs.GetInt("affection");
        }
        TimeSpan ts = GetTimeSpan();
        //affection = 100;
        //PlayerPrefs.SetInt("affection", affection);
        //hunger = 100;
        //PlayerPrefs.SetInt("hunger", hunger);
        /*if (ts.TotalHours < 1)
        {
            hunger = 100;
            affection = 100;
        }
//hunger = 100;
Debug.Log("HUNGER AT START IS: " + hunger);
        //Debug.LogWarning("TOTAL HOURS: " + ts.TotalHours);
        hunger -= (int) (ts.TotalHours* 2);
        if (hunger< 0)
        {
            hunger = 0;
        }
        Debug.Log("HUNGER IS REDUCED TO: " + hunger);

        Debug.Log("AFFECTION AT START IS: " + affection);
        Debug.Log("TOTAL HOURS: " + ts.TotalHours);
        affection -= (int) ((100 - hunger) * (ts.TotalHours / 5));
        //Debug.Log(ts.TotalHours);
        //Debug.Log(100 - hunger + " " + ts.TotalHours / 5);
        if (affection< 0)
        {
            affection = 0;
        }
        Debug.Log("AFFECTION IS REDUCED TO: " + affection);
        if (!PlayerPrefs.HasKey("then"))
        {
            PlayerPrefs.SetString("then", GetStringTime());
        }

        if (serverTime)
        {
            UpdateServer();
        } else
        {
            //UpdateDevice();
            InvokeRepeating("UpdateDevice", 0f, 1f);
        }
    }

    void UpdateServer() { }
void UpdateDevice()
{
    PlayerPrefs.GetString("then", GetStringTime());
}

private int clickCount;

protected override void Update()
{

    //UpdateStatus();

    if (Input.GetMouseButtonUp(0))
    {
        Debug.Log("Clicked");
        Vector2 v = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(v), Vector2.zero);

        if (hit)
        {
            Debug.Log(hit.transform.gameObject.name);
            if (hit.transform.gameObject.tag == "Kitsulope")
            {
                clickCount++;
                if (clickCount >= 1)
                {
                    //clickCount = 0;
                    UpdateAffection(1);
                }
            }
        }
    }

    //Debug.Log("A & H = " + affection + "," + hunger);
    Vector3 position = transform.position;

    if (direction == 1 && position.x > xMax)
    {

        direction = -1;
    }
    else if (direction == -1 && position.x < xMin)
    {

        direction = 1;
    }

    Vector3 movement = Vector3.right * direction * Time.deltaTime;

    if (counter < 1)
    {
        counter += Time.deltaTime;
    }
    else
    {
        Mover.Move(movement);
        counter = 0;
    }

    //Debug.Log("YOUR POSITION IS " + position + " ,COUNTER IS " + counter);
}

public void UpdateAffection(int i)
{
    affection += i;
    if (affection > 100)
    {
        affection = 100;
    }
    PlayerPrefs.SetInt("affection", affection);
}

public void FeedKitsulope()
{
    hunger++;
    if (hunger > 100)
    {
        hunger = 100;
    }
    PlayerPrefs.SetInt("hunger", hunger);
}

public void SaveKitsulope()
{
    if (!serverTime)
    {
        UpdateDevice();
    }
    PlayerPrefs.SetInt("hunger", hunger);
    PlayerPrefs.SetInt("affection", affection);
}

TimeSpan GetTimeSpan()
{
    if (serverTime)
    {
        return new TimeSpan();
    }
    else
    {
        return DateTime.Now - Convert.ToDateTime(PlayerPrefs.GetString("then"));
    }
}

String GetStringTime()
{
    DateTime now = DateTime.Now;
    return now.Month + "/" + now.Day + "/"
    + now.Year + ":" + now.Hour + ":"
    + now.Minute + ":" + now.Second;
}
 */
