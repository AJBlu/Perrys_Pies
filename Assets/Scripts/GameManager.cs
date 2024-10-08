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
        findCrucialStuff();
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
            StartCoroutine(waitFor(1f));
            findCrucialStuff();
            checkForDupes();
            StartCoroutine(destroyProgressObjects());
        }
    }

    public void findCrucialStuff()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        EventSystem = GameObject.FindGameObjectWithTag("EventSystem");
        UI = GameObject.FindGameObjectWithTag("UI");
        uiManager = UIManager.UImanager;
    }

    public GameObject tempHolder;

    /*
    public void destroyProblem(string tag)
    {
        StartCoroutine(waitFor(1f));
        Debug.Log("Searching for a GameObject with the tag: " + tag);
        tempHolder = GameObject.FindWithTag(tag);
        if (tempHolder == null)
        {
            Debug.Log("For some reason, the game is stupid and didn't want to find it.");
            return;
        }
        else Debug.Log("Found the object in question. It's called: " + tempHolder);
        Destroy(tempHolder);
        if (tempHolder == null) Debug.Log("Successfully deleted.");
        else Debug.Log("The game is being stupid once again!");
    }
    */

    public void checkForDupes()
    {
        if (EventSystemInstance == null)
        {
            EventSystemInstance = EventSystem;
            EventSystemInstance.tag = "TrueEventSystem";
            EventSystem = null;
        }
        else
        {
            //Debug.Log("Problematic Event System should be destroyed.");
            StartCoroutine(destroyProblem("EventSystem"));
            //Destroy(GameObject.FindGameObjectWithTag("EventSystem"));
        }

        if (UIinstance == null)
        {
            UIinstance = UI;
            UIinstance.tag = "TrUI";
            UI = null;
        }
        else
        {
            //Debug.Log("Problematic UI should be destroyed.");
            //Destroy(GameObject.FindGameObjectWithTag("UI"));
            StartCoroutine(destroyProblem("UI"));
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

    public IEnumerator destroyProblem(string tag)
    {
        yield return new WaitForSeconds(0.001f);
        Debug.Log("Searching for a GameObject with the tag: " + tag);
        tempHolder = GameObject.FindWithTag(tag);
        if (tempHolder == null)
        {
            Debug.Log("For some reason, the game is stupid and didn't want to find it.");
            StopAllCoroutines();
        }
        else Debug.Log("Found the object in question. It's called: " + tempHolder);
        Destroy(tempHolder);
        yield return new WaitForSeconds(0.001f);
        if (tempHolder == null) Debug.Log("Successfully deleted.");
        else Debug.Log("The game is being stupid once again!");
        yield return null;
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
