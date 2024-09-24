using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject UI;
    public static GameObject UIinstance;
    public GameObject UIManager;
    public static GameObject InterManageInstance;
    public GameObject EventSystem;
    public static GameObject EventSystemInstance;
    public static GameObject gmInstance;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        UI = GameObject.Find("Canvas");
        UIManager = GameObject.Find("UIManager");
        EventSystem = GameObject.Find("EventSystem");
        checkForDupes();
        DontDestroyOnLoad(UIinstance);
        DontDestroyOnLoad(UIManager);
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(EventSystemInstance);
    }

    public void moveToFloor(int pressedButton)
    {
        UIManager.GetComponent<UIManager>().panelDown();
        SceneManager.LoadScene(pressedButton);
        player.GetComponent<PlayerController>().currentFloor = pressedButton;
        if (pressedButton == 1)
        {
            checkForDupes();
            //destroySingleObjects();
        }
    }

    public void checkForDupes()
    {
        if (EventSystemInstance == null)
        {
            EventSystemInstance = EventSystem;
        }
        else
        {
            Destroy(EventSystem.gameObject);
        }

        if (UIinstance == null)
        {
            UIinstance = UI;
        }
        else
        {
            Destroy(UI.gameObject);
        }

        if (gmInstance == null)
        {
            gmInstance = this.gameObject;
        }
        else
        {
            Destroy(gameObject);
        }

        if (InterManageInstance == null)
        {
            InterManageInstance = UIManager;
        }
        else
        {
            Destroy(UIManager.gameObject);
        }
        UIManager.GetComponent<UIManager>().checkForMissingStuff();
        StartCoroutine(UIManager.GetComponent<UIManager>().waitAndCheck());
        player.GetComponent<PlayerController>().findUI();
        player.GetComponent<PlayerController>().verifyInventory();
    }

    public IEnumerator destroySingleObjects()
    {
        yield return new WaitForSeconds(0.5f);
        if (player.GetComponent<PlayerController>().keyDeterGrabbed) Destroy(GameObject.FindGameObjectWithTag("KeyDeter"));
        if (player.GetComponent<PlayerController>().hasPieTin) Destroy(GameObject.FindGameObjectWithTag("PieTin"));
        yield return null;
    }
}
