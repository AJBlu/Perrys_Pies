using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject elevatorPanelHolder;
    public GameObject pauseHolder;
    public GameObject inventoryHolder;
    public GameObject player;
    public static GameManager gameManager;

    public static UIManager UImanager;

    public bool menuOpen;
    public bool gamePaused;

    public List<Button> elevatorButtons;
    public List<GameObject> inventorySlots;

    public GameObject slotIdentifyer;

    public GameObject fadingText;

    public GameObject keyImageHolder;
    public List<Image> keyImages;

    public GameObject skeletonHintHolder;
    public GameObject skeletonHintText;

    public bool givingHint;

   

    private void Awake()
    {
        if (UImanager == null)
        {
            UImanager = this;
            DontDestroyOnLoad(this);
        }
        else if (UImanager != this) Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        checkForMissingStuff();
        for (int i = 0; i < keyImages.Count; i++)
        {
            keyColor(i, false);
        }
        panelDown();
        skeletonHintHolder.SetActive(false);
        givingHint = false;

        gameManager = GameManager.gmInstance;
    }

    public void keyColor(int keySlot, bool isObtained)
    {
        if (isObtained) keyImages[keySlot].color = Color.white;
        else keyImages[keySlot].color = Color.black;
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

        if ((givingHint && !menuOpen))
        {
            Time.timeScale = 0;
            skeletonHintHolder.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (menuOpen)
        {
            Time.timeScale = 0;
            skeletonHintHolder.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1;
            skeletonHintHolder.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void slotUpdate(int slotNumber, string slotName)
    {
        if (slotName == "BallDeter" || slotName == "KeyDeter") inventorySlots[slotNumber].GetComponent<Image>().color = Color.blue;
        else if (slotName == "BagDeter") inventorySlots[slotNumber].GetComponent<Image>().color = new Color32(238, 229, 190, 255);
        else if (slotName == "BellAttract") inventorySlots[slotNumber].GetComponent<Image>().color = new Color32(210, 180, 140, 255);
        else if (slotName == "CanAttract") inventorySlots[slotNumber].GetComponent<Image>().color = Color.red;
        else if (slotName == null) inventorySlots[slotNumber].GetComponent<Image>().color = Color.white;
    }

    public void checkForMissingStuff()
    {
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
        if (fadingText == null) fadingText = GameObject.Find("KeyNeeded");
        if (keyImageHolder == null) keyImageHolder = GameObject.Find("KeysHolder");
        for (int i = 0; i < 3; i++)
        {
            if (keyImages[i] == null)
            {
                keyImages[i] = GameObject.FindGameObjectWithTag("Key" + (i + 1) + "Image").GetComponent<Image>();
            }
        }
        if (skeletonHintHolder == null)
        {
            GameObject.Find("HintHolder");
        }
        if (pauseHolder == null)
        {
            GameObject.Find("PauseMenuHolder");
        }
        if (skeletonHintText == null)
        {
            GameObject.Find("Hint Text");
        }
    }

    public int GetLargestBuildIndex()
    {
        int largestIndex = -1; // Initialize with a value that will be overwritten

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            largestIndex = Mathf.Max(largestIndex, i);
        }
        return largestIndex;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < elevatorButtons.Count; i++)
        {
            if ((i == player.GetComponent<PlayerController>().currentFloor) || (i + 1 > GetLargestBuildIndex())) elevatorButtons[i].interactable = false;
            else elevatorButtons[i].interactable = true;
        }
    }

    public void skeletonHint(GameObject skeleton)
    {
        activateDialogSwitch(false);
        givingHint = true;
        string skeletonHint;
        //this is where the logic for determining the string would go
        skeletonHint = "This is where the hint goes.";

        skeleton.GetComponent<SkeletonDialog>().setHintText(skeletonHintText.GetComponent<TMPro.TextMeshProUGUI>(), skeletonHint);
    }

    public void skeletonLore(GameObject skeleton)
    {
        activateDialogSwitch(true);
        givingHint = true;
        string skeletonLore;
        //this is where the logic for determining the string would go
        skeletonLore = "Look out, Tom Robinson.";

        skeleton.GetComponent<SkeletonDialog>().setHintText(skeletonHintText.GetComponent<TMPro.TextMeshProUGUI>(), skeletonLore);
    }

    public void stopSkeletonStuff()
    {
        givingHint = false;
    }

    public void activateDialogSwitch(bool isLore)
    {
        if (!isLore)
        {
            skeletonHintHolder.transform.Find("LoreDrop").GetComponent<Button>().interactable = true;
            skeletonHintHolder.transform.Find("BasicHint").GetComponent<Button>().interactable = false;
        }
        else
        {
            skeletonHintHolder.transform.Find("LoreDrop").GetComponent<Button>().interactable = false;
            skeletonHintHolder.transform.Find("BasicHint").GetComponent<Button>().interactable = true;
        }
    }

    public void panelUp()
    {
        elevatorPanelHolder.SetActive(true);
        inventoryHolder.SetActive(false);
        slotIdentifyer.SetActive(false);
        keyImageHolder.SetActive(false);
        pauseHolder.SetActive(false);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menuOpen = true;

        
    }

    public void panelDown()
    {
        elevatorPanelHolder.SetActive(false);
        pauseHolder.SetActive(false);
        inventoryHolder.SetActive(true);
        slotIdentifyer.SetActive(true);
        keyImageHolder.SetActive(true);
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuOpen = false;
        gamePaused = false;
    }

    public void pauseGame()
    {
        elevatorPanelHolder.SetActive(false);
        inventoryHolder.SetActive(false);
        slotIdentifyer.SetActive(false);
        keyImageHolder.SetActive(false);
        pauseHolder.SetActive(true);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menuOpen = true;
        gamePaused = true;
    }
    public void quitGame()
    {
        Debug.Log("You'll be back... and I'll be waiting.");
        Application.Quit();
    }

    public void mainMenu()
    {
        GameObject eventSystem = GameObject.Find("EventSystem");
        SceneManager.LoadScene("MainMenu");
        GameManager.gmInstance.destroyThis();
        Destroy(player);
        Destroy(eventSystem);
        Destroy(pauseHolder.transform.parent.gameObject);
        Destroy(this.gameObject);
    }

    public void restartGame()
    {
        GameObject playerCam = GameObject.FindGameObjectWithTag("MainCamera");

        GameManager.gmInstance.moveToFloor(2, true);
        //player.GetComponent<PlayerController>().transform.position = player.GetComponent<PlayerController>().ogPos;
        Debug.Log("PlayerCam should reset rotation.");
        playerCam.gameObject.GetComponent<FirstPersonCamera>().resetRotation();
        player.GetComponent<PlayerController>().keyDeterGrabbed = false;
        player.GetComponent<PlayerController>().hasPieTin = false;
        for (int i = 0; i < player.GetComponent<PlayerController>().keysGrabbed.Count; i++)
        {
            player.GetComponent<PlayerController>().keySpace[i] = null;
            player.GetComponent<PlayerController>().keysGrabbed[i] = false;
            keyColor(i, false);
        }
        for (int i = 0; i < player.GetComponent<PlayerController>().inventory.Count; i++)
        {
            if (player.GetComponent<PlayerController>().inventory[i] != null)
            {
                player.GetComponent<PlayerController>().inventory[i] = null;
                slotUpdate(i, null);
            }
        }
        player.GetComponent<PlayerController>().keyCount = 0;

        GameManager.gmInstance.resetProgress();
    }

    public void moveToBasement()
    {
        GameManager.gmInstance.moveToFloor(1, false);
    }

    public void moveToGround()
    {
        GameManager.gmInstance.moveToFloor(2, false);
    }

    public void moveToFloor2()
    {
        GameManager.gmInstance.moveToFloor(3, false);
    }

    public void moveToFloor3()
    {
        GameManager.gmInstance.moveToFloor(4, false);
    }
    public IEnumerator waitAndCheck()
    {
        yield return new WaitForSeconds(1f);
        checkForMissingStuff();
        yield return null;
    }
}
