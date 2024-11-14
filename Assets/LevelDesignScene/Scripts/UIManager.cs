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

    public GameObject objectiveList;
    public GameObject pieTinText;
    public GameObject escapeText;
    public GameObject keyCountText;
    public GameObject returnKeyText;

    public bool givingHint;

    public bool gameOverTriggered;

    public List<Sprite> distractionSprites;

    GameObject pauseMenu;
    GameObject controls;
    GameObject controlsButton;

    Color middleGray = new Color32(128, 128, 128, 255);



    private void Awake()
    {
        if (UImanager == null)
        {
            UImanager = this;
            DontDestroyOnLoad(this);
        }
        else if (UImanager != this) Destroy(gameObject);
    }

    GameObject tempSkeleton;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        controls = GameObject.Find("Controls");
        controlsButton = controls.transform.Find("GoBack").gameObject;

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
        panelDown();
        skeletonHintHolder.SetActive(false);
        givingHint = false;

        gameManager = GameManager.gmInstance;

        tempSkeleton = GameObject.FindGameObjectWithTag("Skeleton");

        objectiveList = GameObject.FindGameObjectWithTag("Objectives");
        pieTinText = objectiveList.transform.Find("PieTinInfo").gameObject;
        escapeText = objectiveList.transform.Find("EscapeInfo").gameObject;
        keyCountText = objectiveList.transform.Find("KeyCountInfo").gameObject;
        returnKeyText = objectiveList.transform.Find("ReturnKeyInfo").gameObject;

        squareOne();
        keyTextUpdate();
    }
    
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

    public void showControls()
    {
        pauseMenu.transform.localPosition = new Vector3(0, -600, 0);
        controls.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        controlsButton.GetComponent<Image>().color = new Color(1, 0, 0, 1);
        controlsButton.GetComponent<Button>().interactable = true;
        controlsButton.transform.Find("Text (TMP)").gameObject.GetComponent<TMPro.TextMeshProUGUI>().color =
            new Color32(50, 50, 50, 255);
    }

    public void hideControls()
    {
        pauseMenu.transform.localPosition = new Vector3(0, -7.5f, 0);
        controls.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        controlsButton.GetComponent<Image>().color = new Color(1, 0, 0, 0);
        controlsButton.GetComponent<Button>().interactable = false;
        controlsButton.transform.Find("Text (TMP)").gameObject.GetComponent<TMPro.TextMeshProUGUI>().color =
            new Color32(50, 50, 50, 0);
    }

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

    public void keyTextUpdate()
    {
        keyCountText.GetComponent<TMPro.TextMeshProUGUI>().text = "Objective: Find the Keys ("
            + player.GetComponent<PlayerController>().keyCount + "/3)";
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
    }

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

    string publicSkeletonHint;

    public void skeletonHint(GameObject skeleton)
    {
        activateDialogSwitch(false);
        givingHint = true;
        //this is where the logic for determining the string would go
        publicSkeletonHint = "This is where the hint goes.";

        skeleton.GetComponent<SkeletonDialog>().setHintText(skeletonHintText.GetComponent<TMPro.TextMeshProUGUI>(), publicSkeletonHint);
    }

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
