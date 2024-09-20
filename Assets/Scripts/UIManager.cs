using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject elevatorPanelHolder;
    public GameObject inventoryHolder;
    public GameObject player;
    private static GameManager gameManager;

    public bool menuOpen;

    public List<Button> elevatorButtons;
    public List<GameObject> inventorySlots;

    public GameObject slotIdentifyer;

    // Start is called before the first frame update
    void Start()
    {
        panelDown();
        checkForMissingStuff();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i + 1 == player.GetComponent<PlayerController>().selectedSlot)
            {
                slotIdentifyer.transform.localPosition = new Vector3((-450 + (85 * i)), -256, 0);
            }
        }
    }

    public void slotUpdate(int slotNumber, string slotName)
    {
        if (slotName == "BallDeter" || slotName == "KeyDeter") inventorySlots[slotNumber].GetComponent<Image>().color = Color.blue;
        else if (slotName == "BagDeter") inventorySlots[slotNumber].GetComponent<Image>().color = new Color32(238, 229, 190, 255);
        else if (slotName == "BellAttract") inventorySlots[slotNumber].GetComponent<Image>().color = new Color32(210, 180, 140, 255);
        else if (slotName == "CanAttract") inventorySlots[slotNumber].GetComponent<Image>().color = Color.red;
    }

    public void checkForMissingStuff()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        player = GameObject.Find("Player");
        if (elevatorPanelHolder == null)
        {
            elevatorPanelHolder = GameObject.Find("ElevatorScreenParent");
        }
        if (inventoryHolder == null)
        {
            inventoryHolder = GameObject.Find("InventoryParent");
        }
        if (elevatorButtons[0] == null) elevatorButtons[0] = GameObject.Find("Basement").GetComponent<Button>();
        if (elevatorButtons[1] == null) elevatorButtons[1] = GameObject.Find("Ground").GetComponent<Button>();
        if (elevatorButtons[2] == null) elevatorButtons[2] = GameObject.Find("Floor 2").GetComponent<Button>();
        if (elevatorButtons[3] == null) elevatorButtons[3] = GameObject.Find("Floor 3").GetComponent<Button>();
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
        elevatorPanelHolder.SetActive(true);
        inventoryHolder.SetActive(false);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menuOpen = true;
    }

    public void panelDown()
    {
        elevatorPanelHolder.SetActive(false);
        inventoryHolder.SetActive(true);
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuOpen = false;
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
    public IEnumerator waitAndCheck()
    {
        yield return new WaitForSeconds(1f);
        checkForMissingStuff();
        yield return null;
    }
}
