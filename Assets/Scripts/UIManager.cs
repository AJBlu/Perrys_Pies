using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject elevatorPanelHolder;
    public GameObject player;
    private static GameManager gameManager;

    public bool menuOpen;

    public List<Button> elevatorButtons;

    // Start is called before the first frame update
    void Start()
    {
        panelDown();
        checkForMissingStuff();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void checkForMissingStuff()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < elevatorButtons.Count; i++)
        {
            if (i == player.GetComponent<PlayerController>().currentFloor) elevatorButtons[i].interactable = false;
            else elevatorButtons[i].interactable = true;
        }
    }

    public void panelUp()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menuOpen = true;
        elevatorPanelHolder.SetActive(true);
    }

    public void panelDown()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuOpen = false;
        elevatorPanelHolder.SetActive(false);
    }

    public void moveToBasement()
    {
        gameManager.GetComponent<GameManager>().moveToFloor(0);
    }

    public void moveToGround()
    {
        gameManager.GetComponent<GameManager>().moveToFloor(1);
    }

    public void moveToFloor2()
    {
        gameManager.GetComponent<GameManager>().moveToFloor(2);
    }

    public void moveToFloor3()
    {
        gameManager.GetComponent<GameManager>().moveToFloor(3);
    }
}
