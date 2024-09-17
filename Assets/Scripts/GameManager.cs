using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject UI;
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
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(EventSystemInstance);
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
    }
}
