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
    public GameManager gameManager;

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

    public GameObject objectiveList;
    public GameObject pieTinText;
    public GameObject escapeText;
    public GameObject keyCountText;
    public GameObject returnKeyText;

    public GameObject Activator;
    public bool givingHint;

    public bool gameOverTriggered;

    public List<Sprite> distractionSprites;

    public GameObject lossHolder;
    public GameObject winHolder;

    GameObject pauseMenu;
    GameObject controls;
    GameObject controlsButton;
    GameObject altRestart;

    public GameObject perry;

    Color middleGray = new Color32(128, 128, 128, 255);

    /*
    public void activateWinText()
    {
        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.None;
        winText.SetActive(true);
        //elevatorPanelHolder.SetActive(false);
        Time.timeScale = 0;
    }
    */

    public void activateLossText()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        lossHolder.SetActive(true);
        Time.timeScale = 0;
        menuOpen = true;
        gameManager.GetComponent<AudioSource>().Stop();
        perry.GetComponent<AudioSource>().Stop();
        player.GetComponent<AudioSource>().Stop();
    }

    public void activateWinText()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        winHolder.SetActive(true);
        Time.timeScale = 0;
        menuOpen = true;
        gameManager.GetComponent<AudioSource>().Stop();
        perry.GetComponent<AudioSource>().Stop();
        player.GetComponent<AudioSource>().Stop();
    }

    private void Awake()
    {
        if (UImanager == null)
        {
            UImanager = this;
        }
        else if (UImanager != this) Destroy(gameObject);

    }



    GameObject tempSkeleton;

    // Start is called before the first frame update
    void Start()
    {
        //perry = GameObject.Find("Perry");
        winHolder = GameObject.Find("WinHolder");
        winHolder.SetActive(false);
        lossHolder = GameObject.Find("LoseHolder");
        lossHolder.SetActive(false);
        assignButtons();

        controls.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        controlsButton.GetComponent<Image>().color = new Color(1, 1, 1, 0);

        controlsButton.GetComponent<Button>().interactable = false;
        controlsButton.transform.Find("Text (TMP)").gameObject.GetComponent<TMPro.TextMeshProUGUI>().color =
            new Color32(50, 50, 50, 0);

        checkForMissingStuff();
        for (int i = 0; i < keyImages.Count; i++)
        {
            keyColor(i, false);
        }
        skeletonHintHolder.SetActive(false);
        givingHint = false;

        gameManager = FindAnyObjectByType<GameManager>();

        tempSkeleton = GameObject.FindGameObjectWithTag("Skeleton");

        objectiveList = GameObject.FindGameObjectWithTag("Objectives");
        pieTinText = objectiveList.transform.Find("PieTinInfo").gameObject;
        escapeText = objectiveList.transform.Find("EscapeInfo").gameObject;
        keyCountText = objectiveList.transform.Find("KeyCountInfo").gameObject;
        returnKeyText = objectiveList.transform.Find("ReturnKeyInfo").gameObject;
        panelDown();
        squareOne();
        keyTextUpdate();
    }

    //Resets the objectives list to the beginning when the game restarts
    public void squareOne()
    {
        pieTinText.GetComponent<TMPro.TextMeshProUGUI>().color = Color.white;
        escapeText.transform.localPosition = new Vector2(escapeText.transform.localPosition.x, -82.52f);
        escapeText.GetComponent<TMPro.TextMeshProUGUI>().alpha = 0f;
        keyCountText.transform.localPosition = new Vector2(escapeText.transform.localPosition.x, -134.52f);
        keyCountText.GetComponent<TMPro.TextMeshProUGUI>().alpha = 0f;
        returnKeyText.transform.localPosition = new Vector2(escapeText.transform.localPosition.x, -186.52f);
        returnKeyText.GetComponent<TMPro.TextMeshProUGUI>().alpha = 0f;
    }

    GameObject tempText;
    string tempTextContents;

    //Crosses off the objective that was cleared, fades it away,
    //and generates the new objectives in its place
    public void crossOff(string clearedObjective)
    {
        tempText = objectiveList.transform.Find(clearedObjective).gameObject;
        tempTextContents = tempText.GetComponent<TMPro.TextMeshProUGUI>().text;

        tempText.GetComponent<TMPro.TextMeshProUGUI>().text = "<s>" + tempTextContents;
        tempText.GetComponent<TMPro.TextMeshProUGUI>().color = middleGray;
        tempText.GetComponent<TMPro.TextMeshProUGUI>().alpha = 2;

        if (clearedObjective == "PieTinInfo")
        {
            escapeText.GetComponent<TMPro.TextMeshProUGUI>().alpha = 1f;
            keyCountText.GetComponent<TMPro.TextMeshProUGUI>().alpha = 1f;
        }

        StartCoroutine(FadeTextToZeroAlpha(2f, tempText, tempText.name));

        tempText = null;
        tempTextContents = null;
    }

    //Shows the controls on the pause menu
    public void showControls()
    {
        pauseMenu.transform.localPosition = new Vector3(0, -600, 0);
        controls.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        controlsButton.GetComponent<Image>().color = new Color(1, 0, 0, 1);
        controlsButton.GetComponent<Button>().interactable = true;
        controlsButton.transform.Find("Text (TMP)").gameObject.GetComponent<TMPro.TextMeshProUGUI>().color =
            new Color32(50, 50, 50, 255);
    }

    //Brings back up the pause menu
    public void hideControls()
    {
        pauseMenu.transform.localPosition = new Vector3(0, -7.5f, 0);
        controls.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        controlsButton.GetComponent<Image>().color = new Color(1, 0, 0, 0);
        controlsButton.GetComponent<Button>().interactable = false;
        controlsButton.transform.Find("Text (TMP)").gameObject.GetComponent<TMPro.TextMeshProUGUI>().color =
            new Color32(50, 50, 50, 0);
    }

    //Responible for fading the text
    public IEnumerator FadeTextToZeroAlpha(float start, GameObject FadeText, string textName)
    {
        while (FadeText.GetComponent<TMPro.TextMeshProUGUI>().alpha > 0.0f)
        {
            FadeText.GetComponent<TMPro.TextMeshProUGUI>().color = new Color
                (FadeText.GetComponent<TMPro.TextMeshProUGUI>().color.r,
                FadeText.GetComponent<TMPro.TextMeshProUGUI>().color.g,
                FadeText.GetComponent<TMPro.TextMeshProUGUI>().color.b,
                FadeText.GetComponent<TMPro.TextMeshProUGUI>().color.a - (Time.deltaTime / start));
            yield return null;
        }

        if (textName == "PieTinInfo")
        {
            escapeText.transform.localPosition = new Vector2(escapeText.transform.localPosition.x, -4.522247f);
            keyCountText.transform.localPosition = new Vector2(escapeText.transform.localPosition.x, -56.54f);
        }
    }

    //Updates the key count objective when a new key is collected
    public void keyTextUpdate()
    {
        keyCountText.GetComponent<TMPro.TextMeshProUGUI>().text = "Objective: Find the Keys ("
            + player.GetComponent<PlayerController>().keyCount + "/3)";
    }

    //Changes the visibility of the specified key sprite
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
                slotIdentifyer.transform.localPosition = new Vector3((-165 + (85 * i)), 4, 0);
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
        else if (gameOverTriggered)
        {
            Time.timeScale = 0;
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

        if (reservedPlayingClip && pauseMenuActive)
        {
            gameManager.GetComponent<AudioSource>().Pause();
        }
        else
        {
            gameManager.GetComponent<AudioSource>().UnPause();
        }
    }

    //Responsible for updating the designated inventory slot with the right image
    public void slotUpdate(int slotNumber, string slotName)
    {
        if (slotName == null)
        {
            inventorySlots[slotNumber].GetComponent<Image>().rectTransform.sizeDelta = new Vector3(75f, 75f, 1);
            inventorySlots[slotNumber].GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
        else
        {
            inventorySlots[slotNumber].GetComponent<Image>().color = new Color(1, 1, 1, 1);

            Debug.Log("Recieved Item Name: " + slotName);

            if (slotName == "BallDeter" || slotName == "KeyDeter")
            {
                inventorySlots[slotNumber].GetComponent<Image>().rectTransform.sizeDelta = new Vector3(75f, 70.89f, 1);
                inventorySlots[slotNumber].GetComponent<Image>().sprite = distractionSprites[0];
            }
            else if (slotName == "BagDeter")
            {
                inventorySlots[slotNumber].GetComponent<Image>().rectTransform.sizeDelta = new Vector3(75f, 24.14f, 1);
                inventorySlots[slotNumber].GetComponent<Image>().sprite = distractionSprites[1];
            }
            else if (slotName == "BellAttract")
            {
                inventorySlots[slotNumber].GetComponent<Image>().rectTransform.sizeDelta = new Vector3(75f, 49.81f, 1);
                inventorySlots[slotNumber].GetComponent<Image>().sprite = distractionSprites[2];
            }
            else if (slotName == "SprayAttract")
            {
                inventorySlots[slotNumber].GetComponent<Image>().rectTransform.sizeDelta = new Vector3(20.27f, 75f, 1);
                inventorySlots[slotNumber].GetComponent<Image>().sprite = distractionSprites[3];
            }
            else if (slotName == "CandleAttract")
            {
                inventorySlots[slotNumber].GetComponent<Image>().rectTransform.sizeDelta = new Vector3(71.41f, 75f, 1);
                inventorySlots[slotNumber].GetComponent<Image>().sprite = distractionSprites[4];
            }
            else if (slotName == "CanDeter")
            {
                inventorySlots[slotNumber].GetComponent<Image>().rectTransform.sizeDelta = new Vector3(50.92f, 75f, 1);
                inventorySlots[slotNumber].GetComponent<Image>().sprite = distractionSprites[5];
            }
            else if (slotName == "PieDeterA")
            {
                inventorySlots[slotNumber].GetComponent<Image>().rectTransform.sizeDelta = new Vector3(75f, 56.29f, 1);
                inventorySlots[slotNumber].GetComponent<Image>().sprite = distractionSprites[6];
            }
            else if (slotName == "PieDeterB")
            {
                inventorySlots[slotNumber].GetComponent<Image>().rectTransform.sizeDelta = new Vector3(75f, 56.29f, 1);
                inventorySlots[slotNumber].GetComponent<Image>().sprite = distractionSprites[6];
            }
            else if (slotName == "PieDeterC")
            {
                inventorySlots[slotNumber].GetComponent<Image>().rectTransform.sizeDelta = new Vector3(75f, 56.29f, 1);
                inventorySlots[slotNumber].GetComponent<Image>().sprite = distractionSprites[6];
            }
        }
    }

    //Edge case if anything became missing references upon switching scenes
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

    //Responsible for finding the highest scene number in build settings
    //Used for development purposes
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
            if (i == player.GetComponent<PlayerController>().currentFloor) elevatorButtons[i].interactable = false;
            else elevatorButtons[i].interactable = true;
        }

        //If you have a certain key, then the floor will be inaccessible
        for (int i = 0; i < player.GetComponent<PlayerController>().keysGrabbed.Count; i++)
        {
            if (player.GetComponent<PlayerController>().keysGrabbed[i])
            {
                if ((i == 0) && (elevatorButtons[0].interactable)) elevatorButtons[i].interactable = false;
                else if ((i == 1) && (elevatorButtons[2].interactable)) elevatorButtons[2].interactable = false;
                else if ((i == 2) && (elevatorButtons[3].interactable)) elevatorButtons[3].interactable = false;
            }
            else
            {
                if ((i == 0) && (player.GetComponent<PlayerController>().currentFloor != 0))
                    elevatorButtons[i].interactable = true;
                else if ((i == 1) && (player.GetComponent<PlayerController>().currentFloor != 3))
                    elevatorButtons[3].interactable = true;
                else if ((i == 1) && (player.GetComponent<PlayerController>().currentFloor != 2))
                    elevatorButtons[2].interactable = true;
            }
        }
    }

    string publicSkeletonHint;

    //Brings up the hint given by the skeleton
    public void skeletonHint(GameObject skeleton)
    {
        activateDialogSwitch(false);
        givingHint = true;
        //this is where the logic for determining the string would go
        publicSkeletonHint = "This is where the hint goes.";

        skeleton.GetComponent<SkeletonDialog>().setHintText(skeletonHintText.GetComponent<TMPro.TextMeshProUGUI>(), publicSkeletonHint);
    }

    //Brings up the corresponding lore given by the skeleton
    public void skeletonLore()
    {
        tempSkeleton = GameObject.FindGameObjectWithTag("Skeleton");
        activateDialogSwitch(true);
        givingHint = true;
        string skeletonLore;
        //this is where the logic for determining the string would go
        skeletonLore = "Look out, Tom Robinson.";

        skeletonHintText.GetComponent<TMPro.TextMeshProUGUI>().text = skeletonLore;
    }

    //Brings up the previously given hint given by the skeleton
    public void skeletonHintReturn()
    {
        tempSkeleton = GameObject.FindGameObjectWithTag("Skeleton");
        activateDialogSwitch(false);
        givingHint = true;
        string skeletonHint;
        //this is where the logic for determining the string would go
        skeletonHint = publicSkeletonHint;

        skeletonHintText.GetComponent<TMPro.TextMeshProUGUI>().text = skeletonHint;
    }

    //Brings down everything relating to the skeleton hint system
    public void stopSkeletonStuff()
    {
        givingHint = false;
    }

    //
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

    //Brings up the elevator panel
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

    bool reservedPlayingClip;
    bool pauseMenuActive;

    //Puts down the elevator panel
    public void panelDown()
    {
        elevatorPanelHolder.SetActive(false);
        pauseHolder.SetActive(false);
        inventoryHolder.SetActive(true);
        slotIdentifyer.SetActive(true);
        keyImageHolder.SetActive(true);
        pauseMenuActive = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuOpen = false;
        gamePaused = false;

        if (reservedPlayingClip)
        {
            reservedPlayingClip = false;
        }
    }

    public void pauseGame()
    {
        elevatorPanelHolder.SetActive(false);
        inventoryHolder.SetActive(false);
        slotIdentifyer.SetActive(false);
        keyImageHolder.SetActive(false);
        pauseHolder.SetActive(true);
        pauseMenuActive = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menuOpen = true;
        gamePaused = true;

        if (!reservedPlayingClip) 
        {
            reservedPlayingClip = true;
        }
    }
    public void quitGame()
    {
        Debug.Log("You'll be back... and I'll be waiting.");
        Application.Quit();
    }

    //Returns to the title screen
    public void mainMenu()
    {
        GameObject eventSystem = GameObject.Find("EventSystem");
        SceneManager.LoadScene(2);
        gameManager.destroyThis();
        Destroy(player);
        Destroy(eventSystem);
        Destroy(pauseHolder.transform.parent.gameObject);
        Destroy(this.gameObject);
    }

    //Restarts the game
    public void restartGame()
    {
        gameOverTriggered = false;
        GameObject playerCam = GameObject.FindGameObjectWithTag("MainCamera");

        gameManager.moveToFloor(2, true);
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
        altRestart.SetActive(false);
        winHolder.SetActive(false);
        lossHolder.SetActive(false);
        if(GameObject.FindGameObjectWithTag("Perry") != null)
            GameObject.FindGameObjectWithTag("Perry").SetActive(false);
        Activator.SetActive(false);
        Activator.SetActive(true);
        GameManager.gmInstance.resetProgress();
        squareOne();
    }

    //Switches scenes to the basement
    public void moveToBasement()
    {
        gameManager.moveToFloor(1, false);
    }

    //Switches scenes to the ground floor
    public void moveToGround()
    {
        gameManager.moveToFloor(2, false);
    }


    //Switches scenes to the second floor
    public void moveToFloor2()
    {
        gameManager.moveToFloor(3, false);
    }

    //Switches scenes to the third floor
    public void moveToFloor3()
    {
        gameManager.moveToFloor(4, false);
    }

    //Waits before finding the possibly missing stuff
    //Accounts for the time in between loading scenes
    public IEnumerator waitAndCheck()
    {
        yield return new WaitForSeconds(1f);
        checkForMissingStuff();
        yield return null;
    }

    private void assignButtons()
    {

        elevatorPanelHolder = GameObject.Find("ElevatorScreenParent");
        pauseHolder = GameObject.Find("PauseMenuHolder");
        inventoryHolder = GameObject.Find("InventoryParent");
        player = GameObject.FindGameObjectWithTag("Player");

        elevatorButtons[0] = GameObject.Find("Basement").GetComponent<Button>();
        elevatorButtons[1] = GameObject.Find("Ground").GetComponent<Button>();
        elevatorButtons[2] = GameObject.Find("Floor2").GetComponent<Button>();
        elevatorButtons[3] = GameObject.Find("Floor3").GetComponent<Button>();
        elevatorPanelHolder.SetActive(false);

        inventorySlots[0] = GameObject.Find("Slot1");
        inventorySlots[1] = GameObject.Find("Slot2");
        inventorySlots[2] = GameObject.Find("Slot3");
        inventorySlots[3] = GameObject.Find("Slot4");
        inventorySlots[4] = GameObject.Find("Slot5");

        slotIdentifyer = GameObject.Find("SelectorHolder");

        fadingText = GameObject.Find("FadingText");

        keyImageHolder = GameObject.Find("KeysHolder");
        keyImages[0] = GameObject.Find("Key1Image").GetComponent<Image>();
        keyImages[1] = GameObject.Find("Key2Image").GetComponent<Image>();
        keyImages[2] = GameObject.Find("Key3Image").GetComponent<Image>();

        skeletonHintHolder = GameObject.Find("HintHolder");
        skeletonHintHolder.SetActive(false);
        skeletonHintText = GameObject.Find("Hint Text");

        objectiveList = GameObject.Find("ObjectiveListHolder");
        pieTinText = GameObject.Find("pieTinText");
        escapeText = GameObject.Find("EscapeInfo");
        keyCountText = GameObject.Find("KeyCountInfo");
        returnKeyText = GameObject.Find("ReturnKeyInfo");
        pauseMenu = GameObject.Find("PauseMenu");
        controls = GameObject.Find("Controls");
        controlsButton = GameObject.Find("GoBack");

        //winText = GameObject.Find("WinText");
        //winText.SetActive(false);
    }
}
