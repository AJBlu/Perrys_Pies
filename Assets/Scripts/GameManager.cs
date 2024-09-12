using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject UI;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        UI = GameObject.Find("Canvas");
        DontDestroyOnLoad(player);
        DontDestroyOnLoad(UI);
        DontDestroyOnLoad(this);
    }

    public void moveToFloor(int pressedButton)
    {
        SceneManager.LoadScene(pressedButton);
    }
}
