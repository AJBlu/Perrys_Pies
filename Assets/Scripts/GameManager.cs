using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject UI;
    public GameObject UIManager;
    public GameObject EventSystem;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        UI = GameObject.Find("Canvas");
        UIManager = GameObject.Find("UIManager");
        EventSystem = GameObject.Find("EventSystem");
        DontDestroyOnLoad(UI);
        DontDestroyOnLoad(UIManager);
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(EventSystem);
    }

    public void moveToFloor(int pressedButton)
    {
        UIManager.GetComponent<UIManager>().panelDown();
        SceneManager.LoadScene(pressedButton);
        player.GetComponent<PlayerController>().currentFloor = pressedButton;
    }
}
