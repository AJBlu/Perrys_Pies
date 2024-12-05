using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public GameObject alt_restart;
    public LayerMask pickableLayerMask;
    public Transform playerCameraTransform;
    public GameObject pickupUI;
    [Min(1)]
    public float hitRange = 3f;
    public float sprintLimit;
    public float sprintFactor;
    public float crawlFactor;
    private RaycastHit hit;

    public GameObject normalHeight;
    public GameObject crouchHeight;

    public bool isCrouched;
    public bool sprinting;
    public bool hasPieTin;
    public bool keyDeterGrabbed;

    public UIManager uiManager;

    public int currentFloor;

    private Transform interactedItem;

    private bool canMove = true;

    private Rigidbody rigid;

    public bool canInteract;

    //public Transform pickupParent;

    //private Transform inHandItem;

    private Vector3 movement;
    private float xInput;
    private float zInput;
    public float currentSpeed;
    public float originalSpeed;
    public float jumpSpeed;

    public List<GameObject> inventory;
    public List<GameObject> keySpace;

    public GameObject pieTin;

    public InputActionReference interactionInput;

    public int keyCount;

    public static GameObject playerInstance;

    public GameObject ballDeter;
    public GameObject bagDeter;
    public GameObject bellAttract;
    public GameObject sprayAttract;
    public GameObject tinReference;

    public List<GameObject> keyReferences;

    public List<bool> keysGrabbed;

    public float ballThrowStrength;
    public float bagThrowStrength;
    public float bellThrowStrength;
    public float canThrowStrength;

    public GameObject target;

    public int selectedSlot;

    public Vector3 ogPos;

    public UnityEvent<Transform, Priority> playerJump;
    public UnityEvent<Transform, Priority> playerRun;

    public bool isLoudMovement, isWalking;

    public AudioSource steps;
    public AudioSource sprint_steps;
    private void Awake()
    {
        DontDestroyOnLoad(this);

        //Ensures that there is only one player game object
        if (playerInstance == null)
        {
            playerInstance = this.gameObject;
        }
        else
        {
            Destroy(gameObject);
        }

        //References this value when the restart button is pressed
        ogPos = this.gameObject.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        alt_restart.SetActive(false);
        interactionInput.action.performed += Interact;
        rigid = GetComponent<Rigidbody>();
        originalSpeed = currentSpeed = 5f;
        jumpSpeed = 1.65f;
        hasPieTin = false;
        isCrouched = false;
        keyDeterGrabbed = false;
        currentFloor = 1;
        findUI();
        canInteract = true;
        pickupUI.SetActive(false);
        selectedSlot = 1;
        canScroll = true;

        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    //Looks for the UI in case there was some corruption when transitioning between scenes
    public void findUI()
    {
        if (pickupUI == null)
        {
            pickupUI = GameObject.Find("HintSource");
            pickupUI.SetActive(false);
        }
    }
    
    //Without this, there will be times when the interact message will remain visible on screen.
    public IEnumerator interactBuffer()
    {
        canInteract = false;
        yield return new WaitForSeconds(.5f);
        canInteract = true;
        yield return null;
    }

    
    private void Interact(InputAction.CallbackContext obj)
    {
        //Debug.Log("Interacted with: " + hit.collider.name);
        //hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
        if (canInteract)
        {
            storeLogic();
            deactivatePickup();
        }
    }

    /*
    public void debugSlotAssignment(InputAction.CallbackContext obj)
    {
        if (Input.GetKey(KeyCode.Keypad0)) UIManager.UImanager.slotUpdate(0, null);
        if (Input.GetKey(KeyCode.Keypad1)) UIManager.UImanager.slotUpdate(0, "BallDeter");
        if (Input.GetKey(KeyCode.Keypad2)) UIManager.UImanager.slotUpdate(0, "BagDeter");
        if (Input.GetKey(KeyCode.Keypad3)) UIManager.UImanager.slotUpdate(0, "BellAttract");
        if (Input.GetKey(KeyCode.Keypad4)) UIManager.UImanager.slotUpdate(0, "SprayAttract");
        if (Input.GetKey(KeyCode.Keypad5)) UIManager.UImanager.slotUpdate(0, "CandleAttract");
        if (Input.GetKey(KeyCode.Keypad6)) UIManager.UImanager.slotUpdate(0, "CanDeter");
        if (Input.GetKey(KeyCode.Keypad7)) UIManager.UImanager.slotUpdate(0, "PieDeterA");
    }
    */

    //Handles the interaction for any interactable object
    public void storeLogic()
    {
        bool isStored = false;
        if (canInteract && (pickupUI.activeInHierarchy))
        {
            if (hit.collider.tag == "PieTin")
            {
                //Prevents sequence breaking; you need the "KeyDeter" before progressing with the story
                if (!keyDeterGrabbed)
                {
                    Debug.Log("I feel like I need something else...");
                    uiManager.fadingText.GetComponent<TMPro.TextMeshProUGUI>().text = "I feel like I'm\nforgetting something...";
                    uiManager.fadingText.GetComponent<TMPro.TextMeshProUGUI>().color = Color.white;
                    uiManager.fadingText.GetComponent<FadeText>().fadeTime = 2;
                    return;
                }
                //Kicks off the story properly
                else
                {
                    hit.collider.gameObject.SetActive(false);
                    hasPieTin = true;
                    pieTin = tinReference;
                    Debug.Log("Better start running!");
                    GameManager gameManager = FindObjectOfType<GameManager>();
                    gameManager.GetComponent<GameManager>().addHiddenItem(hit.collider.gameObject);
                    uiManager.crossOff("PieTinInfo");
                    return;
                }
            }
            //Brings up the elevator panel
            if (hit.collider.gameObject.tag == "EleDoor")
            {

                //uiManager.panelUp();
                if (keySpace[0] != null)
                {
                    uiManager.menuOpen = true;
                    Time.timeScale = 0;
                    uiManager.activateWinText();
                    alt_restart.SetActive(true);
                }
                return;
            }
            //Opens or closes the interacted door
            if (hit.collider.tag == "RoomDoor")
            {
                hit.collider.gameObject.GetComponent<RoomDoor>().isOpen = !hit.collider.gameObject.GetComponent<RoomDoor>().isOpen;
                return;
            }
            //Collects the proper key and lights up the right UI element
            if ((hit.collider.tag == "Key") && hit.collider.enabled)
            {
                hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
                hit.collider.enabled = false;
                for (int i = 0; i < 3; i++)
                {
                    string keyName = ("Key" + (i + 1));
                    Debug.Log(keyName);
                    //if i = 0, then it checks if the name is Key1, and so on
                    if (hit.collider.name == keyName)
                    {
                        Debug.Log("Adding a key, but it should be the only one added.");
                        keySpace[keyCount] = keyReferences[i];
                        uiManager.keyColor(i, true);
                        keysGrabbed[i] = true;
                        GameManager gameManager = FindObjectOfType<GameManager>();
                        gameManager.GetComponent<GameManager>().addHiddenItem(hit.collider.gameObject);
                    }
                }
                //These next three are intended to be edge cases if all else fails with the above statement
                if (hit.collider.name == "Eye_Key" || hit.collider.name == "Eye Key")
                {
                    keySpace[keyCount] = keyReferences[0];
                    uiManager.keyColor(0, true);
                    keysGrabbed[0] = true;
                    GameManager gameManager = FindObjectOfType<GameManager>();
                    gameManager.GetComponent<GameManager>().addHiddenItem(hit.collider.gameObject);
                    hit.collider.gameObject.SetActive(false);
                    //temporary; for beta build
                    uiManager.crossOff("PieTinInfo");
                }
                else if (hit.collider.name == "Finger_Key" || hit.collider.name == "Finger Key")
                {
                    keySpace[keyCount] = keyReferences[0];
                    uiManager.keyColor(0, true);
                    keysGrabbed[0] = true;
                    GameManager gameManager = FindObjectOfType<GameManager>();
                    gameManager.GetComponent<GameManager>().addHiddenItem(hit.collider.gameObject);
                }
                else if (hit.collider.name == "Pie_Key" || hit.collider.name == "Pie Key")
                {
                    keySpace[keyCount] = keyReferences[0];
                    uiManager.keyColor(0, true);
                    keysGrabbed[0] = true;
                    GameManager gameManager = FindObjectOfType<GameManager>();
                    gameManager.GetComponent<GameManager>().addHiddenItem(hit.collider.gameObject);
                }
                keyCount++;
                uiManager.keyTextUpdate();
                return;
            }

            //Start the dialog for the skeleton
            if (hit.collider.tag == "Skeleton")
            {
                uiManager.skeletonHint(hit.collider.gameObject);
                return;
            }
            //If the interacted object is a genuine distraction
            for (int i = 0; i < inventory.Count; i++)
            {
                if (isStored)
                {
                    return;
                }
                else
                {
                    if (inventory[i] == null)
                    {
                        deactivatePickup();
                        if (hit.collider.tag == "BallDeter" || hit.collider.tag == "KeyDeter") inventory[i] = ballDeter;
                        else if (hit.collider.tag == "BagDeter") inventory[i] = bagDeter;
                        else if (hit.collider.tag == "BellAttract") inventory[i] = bellAttract;
                        else if (hit.collider.tag == "SprayAttract") inventory[i] = sprayAttract;
                        uiManager.slotUpdate(i, hit.collider.tag);
                        if (hit.collider.tag == "KeyDeter")
                        {
                            keyDeterGrabbed = true;
                            GameManager gameManager = FindObjectOfType<GameManager>();
                            StartCoroutine(gameManager.GetComponent<GameManager>().deactivateByTag(hit.collider.tag));
                            gameManager.GetComponent<GameManager>().addHiddenItem(hit.collider.gameObject);
                        }
                        else Destroy(hit.collider.gameObject);
                        isStored = true;
                        
                    }
                }
            }
            //If you interacted with a distraction
            //but your inventory is full
            if (!isStored)
            {
                uiManager.fadingText.GetComponent<TMPro.TextMeshProUGUI>().text = "There's no more room\nin my pockets for this.";
                uiManager.fadingText.GetComponent<TMPro.TextMeshProUGUI>().color = Color.white;
                uiManager.fadingText.GetComponent<FadeText>().fadeTime = 2;
                Debug.Log("Inventory is full.");
            }
        }
    }

    //Referenced when the player switches scenes via the elevator
    public void verifyInventory()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] != null)
            {
                uiManager.slotUpdate(i, inventory[i].gameObject.tag);
            }
        }
    }

    //Another method for preventing the graphical bug
    public void deactivatePickup()
    {
        StartCoroutine(interactBuffer());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Lock")
        {
            if (keyCount == 3)
            {
                for (int i = keySpace.Count; i > 0; i--)
                {
                    uiManager.keyColor(i - 1, false);
                }
                other.gameObject.SetActive(false);
                GameManager gameManager = FindObjectOfType<GameManager>();
                gameManager.GetComponent<GameManager>().exitUnlocked = true;
                gameManager.GetComponent<GameManager>().addHiddenItem(other.gameObject);
            }
            else Debug.Log("Not enough keys.");
        }

        if (other.transform.tag == "ElevatorLock")
        {
            if (keyCount != 0)
            {
                keyCount--;
                Destroy(other.gameObject);
            }
            else
            {
                uiManager.fadingText.GetComponent<TMPro.TextMeshProUGUI>().text = "I don't have a key to unlock this lock.";
                uiManager.fadingText.GetComponent<FadeText>().fadeTime = 2;
                Debug.Log("I need a key");
            }
        }
    }

    private void OnEnable()
    {
        //interactions.Enable();
    }

    private void OnDisable()
    {
        //interactions.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!uiManager.menuOpen)
        {
            ChangeMoveMent();

            if (hit.collider != null)
            {
                //Removes graphical highlight when looking away from the object
                if (hit.collider.tag == "Skeleton") hit.collider.GetComponent<Outline>().OutlineWidth = 0;
                else if ((hit.collider.tag == "BallDeter") || (hit.collider.tag == "KeyDeter") ||
                    (hit.collider.tag == "BagDeter") || (hit.collider.tag == "CanDeter") ||
                    (hit.collider.tag == "PieDeterA") || (hit.collider.tag == "PieDeterB") ||
                    (hit.collider.tag == "PieDeterC"))
                    hit.collider.GetComponent<Outline>().OutlineWidth = 0;
                else if ((hit.collider.tag == "BellAttract") || (hit.collider.tag == "SprayAttract") ||
                    (hit.collider.tag == "CandleAttract"))
                    hit.collider.GetComponent<Outline>().OutlineWidth = 0;
                else hit.collider.GetComponent<Highlight>()?.ToggleHighlight(false);

                pickupUI.SetActive(false);
            }
            //prevents interact message from remaining visible
            if (!canInteract)
            {
                pickupUI.SetActive(false);
            }

            if (canInteract && (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange, pickableLayerMask)))
            {
                //Highlights the interactable object when looking at it
                if (hit.collider.tag == "Skeleton") hit.collider.GetComponent<Outline>().OutlineWidth = 5;
                else if ((hit.collider.tag == "BallDeter") || (hit.collider.tag == "KeyDeter") || 
                    (hit.collider.tag == "BagDeter") || (hit.collider.tag == "CanDeter") ||
                    (hit.collider.tag == "PieDeterA") || (hit.collider.tag == "PieDeterB") ||
                    (hit.collider.tag == "PieDeterC"))
                    hit.collider.GetComponent<Outline>().OutlineWidth = 5;
                else if ((hit.collider.tag == "BellAttract") || (hit.collider.tag == "SprayAttract") ||
                    (hit.collider.tag == "CandleAttract"))
                    hit.collider.GetComponent<Outline>().OutlineWidth = 5;
                else if (hit.collider.tag == "Elevator" && hasPieTin) 
                    hit.collider.GetComponent<Highlight>()?.ToggleHighlight(true);
                else hit.collider.GetComponent<Highlight>()?.ToggleHighlight(true);

                pickupUI.SetActive(true);
            }

            if (Input.GetKeyUp("left shift"))
            {
                resetMovement();
                isLoudMovement = false;
            }

            if(Input.GetKeyUp("left ctrl"))
            {
                resetMovement();
                isCrouched = false;
            }

            /*
            if (Input.GetKeyDown("u"))
            {
                throwDistraction(ballDeter);
            }
            if (Input.GetKeyDown("i"))
            {
                throwDistraction(bagDeter);
            }
            if (Input.GetKeyDown("o"))
            {
                throwDistraction(bellAttract);
            }
            if (Input.GetKeyDown("p"))
            {
                throwDistraction(sprayAttract);
            }
            */
        }
    }

    public void slot1(InputAction.CallbackContext obj)
    {
        selectedSlot = 1;
    }
    public void slot2(InputAction.CallbackContext obj)
    {
        selectedSlot = 2;
    }
    public void slot3(InputAction.CallbackContext obj)
    {
        selectedSlot = 3;
    }
    public void slot4(InputAction.CallbackContext obj)
    {
        selectedSlot = 4;
    }
    public void slot5(InputAction.CallbackContext obj)
    {
        selectedSlot = 5;
    }

    bool canScroll;

    //Checks which direction the scroll wheel is moving in and changes the selector accordingly
    public void scrollCheck(InputAction.CallbackContext obj)
    {
        if (canScroll)
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0) StartCoroutine(slotByOne(true));
            else StartCoroutine(slotByOne(false));
        }
    }

    //Pauses the game when it's not and vice versa
    public void pauseToggle(InputAction.CallbackContext obj)
    {
        if (!uiManager.gamePaused) UIManager.UImanager.pauseGame();
        else uiManager.panelDown();
    }

    //Throws the currently selected distraction when the left mouse button is pressed
    public void leftMouse(InputAction.CallbackContext obj)
    {
        if (uiManager.givingHint)
        {
            //UIManager.UImanager.givingHint = false;
        }
        else
        {
            if (inventory[selectedSlot - 1] != null)
            {
                throwDistraction(inventory[selectedSlot - 1]);
            }
            else return;
        }
    }

    //changes the location of the selector by one slot
    public IEnumerator slotByOne(bool isScrollUp)
    {
        canScroll = false;
        if (isScrollUp)
        {
            selectedSlot++;
        }
        else
        {
            selectedSlot--;
        }
        if (selectedSlot == 6)
        {
            selectedSlot = 1;
        }
        else if (selectedSlot == 0)
        {
            selectedSlot = 5;
        }
        yield return new WaitForSeconds(0.1f);
        canScroll = true;
    }

    float projectileSpeed;

    //Checks which type of distraction is selected
    //Throws the distraction with its appropriate strength
    public void throwDistraction(GameObject throwable)
    {
        
        target.transform.forward = this.transform.forward;
        if (throwable == ballDeter)
        {
            //target.transform.position = this.transform.position + (this.transform.forward * ballThrowStrength);
            projectileSpeed = ballThrowStrength;
            Debug.Log("Throwing Blueberry Ball");
        }
        if (throwable == bagDeter)
        {
            //target.transform.position = this.transform.position + (this.transform.forward * bagThrowStrength);
            projectileSpeed = bagThrowStrength;
            Debug.Log("Throwing Hardened Icing Bag");
        }
        if (throwable == bellAttract)
        {
            //target.transform.position = this.transform.position + (this.transform.forward * bellThrowStrength);
            projectileSpeed = bellThrowStrength;
            Debug.Log("Throwing Pie Bell");
        }
        if (throwable == sprayAttract)
        {
            //target.transform.position = this.transform.position + (this.transform.forward * canThrowStrength);
            projectileSpeed = canThrowStrength;
            Debug.Log("Throwing Pie Scent");
        }
        var position = transform.position + transform.forward;
        var rotation = transform.rotation;
        var projectile = Instantiate(throwable, position, rotation);
        projectile.GetComponent<Projectile>().Fire(projectileSpeed, transform.forward);

        for (int i = 0; i < inventory.Count; i++)
        {
            if (i + 1 == selectedSlot)
            {
                inventory[i] = null;
                uiManager.slotUpdate(i, null);
            }
        }
    }

    //Used for calculating movement and sprinting speed
    private void FixedUpdate()
    {
        Debug.Log(Time.timeScale);
        //Debug.Log(gameObject.GetComponent<Rigidbody>().velocity.magnitude);
        if (canMove)
        {
            rigid.velocity = transform.TransformDirection(movement);
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }

        if (sprinting)
        {
            if (sprintLimit > 0)
            {
                sprintLimit -= 0.01f;
            }
            steps.enabled = false;
            sprint_steps.enabled = true;
        }
        else
        {
            if (sprintLimit < 1)
            {
                sprintLimit += 0.01f;
            }
            sprint_steps.enabled = false;
        }
        
        if (sprintLimit <= 0)
        {
            resetMovement();
        }

        if (!sprint_steps.enabled)
        {
            if (rigid.velocity.magnitude > .1)
            {
                steps.enabled = true;
            }
            else
            {
                steps.enabled = false;
            }
        }

    }


    public void ChangeMoveMent()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        zInput = Input.GetAxisRaw("Vertical");
        movement = new Vector3(xInput * currentSpeed, rigid.velocity.y, zInput * currentSpeed);
    }

    public void HandleJump(InputAction.CallbackContext obj)
    {
        if (IsGround())
        {
            Vector3 jumpVec = new Vector3(0, jumpSpeed, 0);
            rigid.AddRelativeForce(jumpVec, ForceMode.Impulse);
            playerJump.Invoke(gameObject.transform, Priority.RUNNING);
            isLoudMovement = true;
        }
    }

    public void sprint(InputAction.CallbackContext obj)
    {
        if (sprintLimit > 0 && rigid.velocity != Vector3.zero)
        {
            sprinting = true;
            if (currentSpeed == originalSpeed) currentSpeed *= sprintFactor;
            playerRun.Invoke(gameObject.transform, Priority.RUNNING);
            isLoudMovement = true;
        }
        else
        {
        }
    }

    public void crouch(InputAction.CallbackContext obj)
    {
        isCrouched = true;
        playerCameraTransform.transform.position = crouchHeight.transform.position;
        if (currentSpeed == originalSpeed) currentSpeed *= crawlFactor;
    }

    //When the sprint or crouch button is released
    //The movement speed is reset
    public void resetMovement()
    {
        isCrouched = false;
        sprinting = false;
        if (currentSpeed != originalSpeed) currentSpeed = originalSpeed;
        playerCameraTransform.transform.position = normalHeight.transform.position;
    }

    //checks if the player is on the ground
    private bool IsGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1.5f))
        {
            return true;
        }
        return false;
    }
}
