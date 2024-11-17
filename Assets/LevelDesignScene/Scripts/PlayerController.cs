using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
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


    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (playerInstance == null)
        {
            playerInstance = this.gameObject;
        }
        else
        {
            Destroy(gameObject);
        }

        ogPos = this.gameObject.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
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

        uiManager = UIManager.UImanager;
    }

    
    public void findUI()
    {
        if (pickupUI == null)
        {
            pickupUI = GameObject.Find("HintSource");
            pickupUI.SetActive(false);
        }
    }
    

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

    public void storeLogic()
    {
        bool isStored = false;
        if (canInteract && (pickupUI.activeInHierarchy))
        {
            if (hit.collider.tag == "PieTin")
            {
                if (!keyDeterGrabbed)
                {
                    Debug.Log("I feel like I need something else...");
                    return;
                }
                else
                {
                    hit.collider.gameObject.SetActive(false);
                    hasPieTin = true;
                    pieTin = tinReference;
                    Debug.Log("Better start running!");
                    GameManager gameManager = FindObjectOfType<GameManager>();
                    gameManager.GetComponent<GameManager>().addHiddenItem(hit.collider.gameObject);
                    UIManager.UImanager.crossOff("PieTinInfo");
                    return;
                }
            }
            if (hit.collider.gameObject.tag == "EleDoor")
            {
                UIManager.UImanager.panelUp();
                return;
            }
            if (hit.collider.tag == "RoomDoor")
            {
                hit.collider.gameObject.GetComponent<RoomDoor>().isOpen = !hit.collider.gameObject.GetComponent<RoomDoor>().isOpen;
                return;
            }
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
                        UIManager.UImanager.keyColor(i, true);
                        keysGrabbed[i] = true;
                        GameManager gameManager = FindObjectOfType<GameManager>();
                        gameManager.GetComponent<GameManager>().addHiddenItem(hit.collider.gameObject);
                    }
                }
                keyCount++;
                UIManager.UImanager.keyTextUpdate();
                return;
            }

            if (hit.collider.tag == "Skeleton")
            {
                UIManager.UImanager.skeletonHint(hit.collider.gameObject);
                return;
            }
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
                        UIManager.UImanager.slotUpdate(i, hit.collider.tag);
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
            if (!isStored)
            {
                UIManager.UImanager.fadingText.GetComponent<TMPro.TextMeshProUGUI>().text = "There's no more room\nin my pockets for this.";
                UIManager.UImanager.fadingText.GetComponent<TMPro.TextMeshProUGUI>().color = Color.white;
                UIManager.UImanager.fadingText.GetComponent<FadeText>().fadeTime = 2;
                Debug.Log("Inventory is full.");
            }
        }
    }

    public void verifyInventory()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] != null)
            {
                UIManager.UImanager.slotUpdate(i, inventory[i].gameObject.tag);
            }
        }
    }

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
                    UIManager.UImanager.keyColor(i - 1, false);
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
                UIManager.UImanager.fadingText.GetComponent<TMPro.TextMeshProUGUI>().text = "I don't have a key to unlock this lock.";
                UIManager.UImanager.fadingText.GetComponent<FadeText>().fadeTime = 2;
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
        if (!UIManager.UImanager.menuOpen)
        {
            ChangeMoveMent();

            if (hit.collider != null)
            {
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
            if (!canInteract)
            {
                pickupUI.SetActive(false);
            }

            if (canInteract && (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange, pickableLayerMask)))
            {
                if (hit.collider.tag == "Skeleton") hit.collider.GetComponent<Outline>().OutlineWidth = 5;
                else if ((hit.collider.tag == "BallDeter") || (hit.collider.tag == "KeyDeter") || 
                    (hit.collider.tag == "BagDeter") || (hit.collider.tag == "CanDeter") ||
                    (hit.collider.tag == "PieDeterA") || (hit.collider.tag == "PieDeterB") ||
                    (hit.collider.tag == "PieDeterC"))
                    hit.collider.GetComponent<Outline>().OutlineWidth = 5;
                else if ((hit.collider.tag == "BellAttract") || (hit.collider.tag == "SprayAttract") ||
                    (hit.collider.tag == "CandleAttract"))
                    hit.collider.GetComponent<Outline>().OutlineWidth = 5;
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

    public void scrollCheck(InputAction.CallbackContext obj)
    {
        if (canScroll)
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0) StartCoroutine(slotByOne(true));
            else StartCoroutine(slotByOne(false));
        }
    }

    public void pauseToggle(InputAction.CallbackContext obj)
    {
        if (!UIManager.UImanager.gamePaused) UIManager.UImanager.pauseGame();
        else UIManager.UImanager.panelDown();
    }

    public void leftMouse(InputAction.CallbackContext obj)
    {
        if (UIManager.UImanager.givingHint)
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
                UIManager.UImanager.slotUpdate(i, null);
            }
        }
    }

    private void FixedUpdate()
    {
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
        }
        else
        {
            if (sprintLimit < 1)
            {
                sprintLimit += 0.01f;
            }
        }
        
        if (sprintLimit <= 0)
        {
            resetMovement();
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

    public void resetMovement()
    {
        isCrouched = false;
        sprinting = false;
        if (currentSpeed != originalSpeed) currentSpeed = originalSpeed;
        playerCameraTransform.transform.position = normalHeight.transform.position;
    }

    private bool IsGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1.5f))
        {
            return true;
        }
        return false;
    }

    private void ResetSpeed()
    {
        currentSpeed = originalSpeed;
    }
}
