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
    public bool exitUnlocked;

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

    private GameObject tempHolder;

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
            StartCoroutine(destroyByTag("EventSystem"));
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
            StartCoroutine(destroyByTag("UI"));
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

    public IEnumerator deactivateByName(string name)
    {
        yield return new WaitForSeconds(0.001f);
        tempHolder = GameObject.Find(name);
        if (tempHolder == null) StopAllCoroutines();
        tempHolder.SetActive(false);
        yield return new WaitForSeconds(0.001f);
    }

    public IEnumerator destroyByTag(string tag)
    {
        yield return new WaitForSeconds(0.001f);
        tempHolder = GameObject.FindWithTag(tag);
        if (tempHolder == null) StopAllCoroutines();
        Destroy(tempHolder);
        yield return new WaitForSeconds(0.001f);
    }

    public IEnumerator deactivateByTag(string tag)
    {
        yield return new WaitForSeconds(0.001f);
        tempHolder = GameObject.FindWithTag(tag);
        if (tempHolder == null) StopAllCoroutines();
        tempHolder.SetActive(false);
        yield return new WaitForSeconds(0.001f);
    }
    
    public void resetProgress()
    {
        StartCoroutine(respawnItemByTag("Lock"));
        StartCoroutine(respawnItemByTag("PieTin"));
        StartCoroutine(respawnItemByTag("KeyDeter"));
        for (int i = 0; i < player.GetComponent<PlayerController>().keysGrabbed.Count; i++)
        {
            StartCoroutine(respawnItemByName("Key" + (i + 1)));
        }
    }

    private IEnumerator respawnItemByTag(string tag)
    {
        yield return new WaitForSeconds(0.001f);
        tempHolder = GameObject.FindWithTag(tag);
        Debug.Log(tempHolder);
        tempHolder.SetActive(true);
        yield return new WaitForSeconds(0.001f);
    }

    private IEnumerator respawnItemByName(string name)
    {
        yield return new WaitForSeconds(0.001f);
        tempHolder = GameObject.Find(name);
        Debug.Log(tempHolder);
        tempHolder.SetActive(true);
        yield return new WaitForSeconds(0.001f);
    }


    public IEnumerator destroyProgressObjects()
    {
        if (player.GetComponent<PlayerController>().keyDeterGrabbed) StartCoroutine(deactivateByTag("KeyDeter"));
        if (player.GetComponent<PlayerController>().hasPieTin) StartCoroutine(deactivateByTag("PieTin"));

        for (int i = 0; i < player.GetComponent<PlayerController>().keysGrabbed.Count; i++)
        {
            if (player.GetComponent<PlayerController>().keysGrabbed[i]) StartCoroutine(deactivateByName("Key" + (i + 1)));
        }

        if (exitUnlocked) StartCoroutine(deactivateByTag("Lock"));

        yield return null;
    }
}
