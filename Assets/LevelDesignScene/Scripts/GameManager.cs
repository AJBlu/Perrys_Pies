using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject perry;
    public GameObject player;
    private Vector3 originalPlayerPosition, originalPerryPosition;
    public GameObject UI;
    public static GameObject UIinstance;
    //public GameObject UIManager;
    public static UIManager uiManager;
    public GameObject EventSystem;
    public static GameObject EventSystemInstance;
    public static GameManager gmInstance;
    public bool exitUnlocked;

    public bool introDialoguePlayed;

    public List<GameObject> hiddenObjects;

    public List<GameObject> floors;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        perry = GameObject.FindGameObjectWithTag("Perry");
        if (perry != null)
        {
            originalPerryPosition = perry.transform.position;
            originalPlayerPosition = player.transform.position;
        }
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
        */
        seekDestroyAndDont();

        if (!introDialoguePlayed)
        {
            this.gameObject.GetComponent<AudioSource>().Play();
            introDialoguePlayed = true;
        }
    }

    public void seekDestroyAndDont()
    {
        findCrucialStuff();
        checkForDupes();
    }

    //Gets rid of this object when returning to the main menu
    public void destroyThis()
    {
        Destroy(this.gameObject);
    }

    //Transports the player to the designated floor
    //Also Accounts for restarting the game
    public void moveToFloor(int pressedButton, bool restarting)
    {
        UIManager.UImanager.panelDown();
        //SceneManager.LoadScene(pressedButton);
        for (int i = 0; i < floors.Count; i++)
        {
            if (i + 1 == pressedButton)
            {
                floors[i].SetActive(true);
            }
            else floors[i].SetActive(false);
        }
        player.GetComponent<PlayerController>().currentFloor = pressedButton - 1;
        StartCoroutine(player.GetComponent<PlayerController>().interactBuffer());
        if (pressedButton == 2)
        {
            StartCoroutine(waitFor(1f));
            findCrucialStuff();
            checkForDupes();
            if (!restarting)
            {
                StartCoroutine(destroyProgressObjects());

            }
        }
    }

    //Finds some items in case they go missing in between scenes
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

    //Looks for duplicate objects and gets rid of any if applicable
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

    //Waits for a certain amount of time
    public IEnumerator waitFor(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    //Deactivates a certain object given its name
    public IEnumerator deactivateByName(string name)
    {
        yield return new WaitForSeconds(0.001f);
        tempHolder = GameObject.Find(name);
        if (tempHolder == null) StopAllCoroutines();
        tempHolder.SetActive(false);
        yield return new WaitForSeconds(0.001f);
    }

    //Deactivates a certain object given its tag
    public IEnumerator deactivateByTag(string tag)
    {
        yield return new WaitForSeconds(0.001f);
        tempHolder = GameObject.FindWithTag(tag);
        if (tempHolder == null) StopAllCoroutines();
        tempHolder.SetActive(false);
        yield return new WaitForSeconds(0.001f);
    }

    //Destroys a certain object given its tag
    public IEnumerator destroyByTag(string tag)
    {
        yield return new WaitForSeconds(0.001f);
        tempHolder = GameObject.FindWithTag(tag);
        if (tempHolder == null) StopAllCoroutines();
        Destroy(tempHolder);
        yield return new WaitForSeconds(0.001f);
    }

    //References an object that was collected that pertains to the game's progression
    public void addHiddenItem(GameObject currentItem)
    {
        hiddenObjects.Add(currentItem);
        Debug.Log("Added: " + currentItem);
    }

    //Empties the list of progression objects upon restart
    public void resetProgress()
    {
        Time.timeScale = 1;
        /*
        StartCoroutine(respawnItemByTag("Lock"));
        StartCoroutine(respawnItemByTag("PieTin"));
        StartCoroutine(respawnItemByTag("KeyDeter"));
        for (int i = 0; i < player.GetComponent<PlayerController>().keysGrabbed.Count; i++)
        {
            StartCoroutine(respawnItemByName("Key" + (i + 1)));
        }
        */

        for (int i = 0; i < hiddenObjects.Count; i++)
        {
            hiddenObjects[i].SetActive(true);
            Debug.Log(hiddenObjects[i] + " should be active.");
        }
        hiddenObjects = new List<GameObject>(0);
    }

    /*
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
    */

    //Intended to all progression objects when going back into a certain scene
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
