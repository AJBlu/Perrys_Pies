using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject UI;
    public static GameObject UIinstance;
    //public GameObject UIManager;
    public static UIManager uiManager;
    public GameObject EventSystem;
    public static GameObject EventSystemInstance;
    public static GameManager gmInstance;

    private void Awake()
    {
        if (gmInstance == null)
        {
            gmInstance = this;
            DontDestroyOnLoad(this);
        }
        else if (gmInstance != this) Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        /*
        player = GameObject.FindGameObjectWithTag("Player");
        UI = GameObject.Find("Canvas");
        UIManager = GameObject.Find("UIManager");
        EventSystem = GameObject.Find("EventSystem");
        checkForDupes();
        DontDestroyOnLoad(UIinstance);
        DontDestroyOnLoad(UIManager);
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(EventSystemInstance);
        */
        seekDestroyAndDont();
    }

    public void seekDestroyAndDont()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        UI = GameObject.Find("Canvas");
        uiManager = UIManager.UImanager;
        EventSystem = GameObject.Find("EventSystem");
        checkForDupes();
        DontDestroyOnLoad(UIinstance);
        DontDestroyOnLoad(uiManager);
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(EventSystemInstance);
    }

    public void destroyThis()
    {
        Destroy(this.gameObject);
    }

    public void moveToFloor(int pressedButton)
    {
        UIManager.UImanager.panelDown();
        SceneManager.LoadScene(pressedButton);
        player.GetComponent<PlayerController>().currentFloor = pressedButton;
        if (pressedButton == 1)
        {
            checkForDupes();
            StartCoroutine(destroyProgressObjects());
        }
    }

    public void checkForDupes()
    {
        if (EventSystemInstance == null && EventSystemInstance != EventSystem)
        {
            EventSystemInstance = EventSystem;
        }
        else if (EventSystemInstance == EventSystem)
        {
            Debug.Log("We're destroying the wrong Event System!!!");
        }
        else
        {
            Destroy(EventSystem.gameObject);
        }

        if (UIinstance == null && UIinstance != UI)
        {
            UIinstance = UI;
        }
        else if (UIinstance == UI)
        {
            Debug.Log("We're destroying the wrong UI!!!");
        }
        else
        {
            Destroy(UI.gameObject);
        }
        UIManager.UImanager.checkForMissingStuff();
        StartCoroutine(UIManager.UImanager.waitAndCheck());
        player.GetComponent<PlayerController>().findUI();
        player.GetComponent<PlayerController>().verifyInventory();
    }

    public IEnumerator waitFor(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    public IEnumerator destroyProgressObjects()
    {
        StartCoroutine(waitFor(0.5f));
        if (player.GetComponent<PlayerController>().keyDeterGrabbed) Destroy(GameObject.FindGameObjectWithTag("KeyDeter"));
        if (player.GetComponent<PlayerController>().hasPieTin) Destroy(GameObject.FindGameObjectWithTag("PieTin"));
        for (int i = 0; i < player.GetComponent<PlayerController>().keySpace.Count; i++)
        {
            if (player.GetComponent<PlayerController>().keySpace[i] != null)
            {
                Destroy(GameObject.FindGameObjectWithTag(player.GetComponent<PlayerController>().keySpace[i].tag));
            }
        }
        yield return null;
    }
}
