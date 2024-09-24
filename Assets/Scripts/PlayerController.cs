using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    public GameObject UIManager;

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
    private float originalSpeed;
    public float jumpSpeed;

    public List<GameObject> inventory;
    public List<GameObject> keySpace;

    public GameObject pieTin;

    public InputActionReference interactionInput;

    [SerializeField]
    private int keyCount;

    public static GameObject playerInstance;

    public GameObject ballDeter;
    public GameObject bagDeter;
    public GameObject bellAttract;
    public GameObject canAttract;
    public GameObject tinReference;

    public float ballThrowStrength;
    public float bagThrowStrength;
    public float bellThrowStrength;
    public float canThrowStrength;

    public GameObject target;

    public int selectedSlot;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        interactionInput.action.performed += Interact;
        rigid = GetComponent<Rigidbody>();
        originalSpeed = currentSpeed = 10;
        jumpSpeed = 5;
        hasPieTin = false;
        isCrouched = false;
        keyDeterGrabbed = false;
        currentFloor = 1;
        findUI();
        canInteract = true;
        pickupUI.SetActive(false);
        selectedSlot = 1;
    }

    public void findUI()
    {
        if (UIManager == null)
        {
            UIManager = GameObject.Find("UIManager");
        }
        if (pickupUI == null)
        {
            pickupUI = GameObject.Find("HintSource");
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
        storeLogic();
    }

    public void storeLogic()
    {
        bool isStored = false;
        if (canInteract)
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
                    return;
                }
            }
            if (hit.collider.gameObject.tag == "EleDoor")
            {
                UIManager.GetComponent<UIManager>().panelUp();
                return;
            }
            if (hit.collider.tag == "RoomDoor")
            {
                hit.collider.gameObject.GetComponent<RoomDoor>().isOpen = !hit.collider.gameObject.GetComponent<RoomDoor>().isOpen;
                return;
            }
            if (hit.collider.tag == "Key")
            {
                hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
                hit.collider.enabled = false;
                keySpace[keyCount] = hit.collider.gameObject;
                keyCount++;
                return;
            }
            for (int i = 0; i < inventory.Count; i++)
            {
                if (isStored) return;
                else
                {
                    if (inventory[i] == null)
                    {
                        //inventory[i] = hit.collider.gameObject;
                        if (hit.collider.tag == "BallDeter" || hit.collider.tag == "KeyDeter") inventory[i] = ballDeter;
                        else if (hit.collider.tag == "BagDeter") inventory[i] = bagDeter;
                        else if (hit.collider.tag == "BellAttract") inventory[i] = bellAttract;
                        else if (hit.collider.tag == "CanAttract") inventory[i] = canAttract;
                        UIManager.GetComponent<UIManager>().slotUpdate(i, hit.collider.tag);
                        Destroy(hit.collider.gameObject);
                        isStored = true;
                        if (hit.collider.tag == "KeyDeter")
                        {
                            keyDeterGrabbed = true;
                        }
                        pickupUI.SetActive(false);
                    }
                }
            }
            if (!isStored) Debug.Log("Inventory is full.");
        }
    }

    public void verifyInventory()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] != null)
            {
                UIManager.GetComponent<UIManager>().slotUpdate(i, inventory[i].gameObject.tag);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Lock")
        {
            if (keyCount == 3)
            {
                for (int i = 0; i < keySpace.Count; i++)
                {
                    if (keySpace[i].tag == "Key")
                    {
                        keySpace[i] = null;
                        keyCount--;
                    }
                    if (keyCount == 0)
                    {
                        i = 10;
                    }
                }
                other.gameObject.SetActive(false);
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
                UIManager.GetComponent<UIManager>().fadingText.GetComponent<FadeText>().fadeTime = 2;
                Debug.Log("I need a key");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!UIManager.GetComponent<UIManager>().menuOpen)
        {
            ChangeMoveMent();

            if (hit.collider != null)
            {
                hit.collider.GetComponent<Highlight>()?.ToggleHighlight(false);
                pickupUI.SetActive(false);
            }

            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange, pickableLayerMask))
            {
                hit.collider.GetComponent<Highlight>()?.ToggleHighlight(true);
                pickupUI.SetActive(true);
            }

            if (Input.GetKeyDown("space"))
            {
                if (IsGround()) HandleJump();
            }
            if (Input.GetKeyDown("left shift"))
            {
                sprint();
            }
            if (Input.GetKeyDown("left ctrl"))
            {
                crouch();
            }
            if (Input.GetKeyUp("left shift") || Input.GetKeyUp("left ctrl"))
            {
                resetMovement();
            }
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
                throwDistraction(canAttract);
            }
            if (Input.GetKeyDown("1"))
            {
                selectedSlot = 1;
            }
            if (Input.GetKeyDown("2"))
            {
                selectedSlot = 2;
            }
            if (Input.GetKeyDown("3"))
            {
                selectedSlot = 3;
            }
            if (Input.GetKeyDown("4"))
            {
                selectedSlot = 4;
            }
            if (Input.GetKeyDown("5"))
            {
                selectedSlot = 5;
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                slotByOne(false);
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                slotByOne(true);
            }
            if (Input.GetMouseButtonDown(0))
            {
                for (int i = 0; i < 5; i++)
                {
                    if (i + 1 == selectedSlot)
                    {
                        throwDistraction(inventory[i]);
                        return;
                    }
                }
            }
        }
    }

    public void slotByOne(bool isScrollUp)
    {
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
        if (throwable == canAttract)
        {
            //target.transform.position = this.transform.position + (this.transform.forward * canThrowStrength);
            projectileSpeed = canThrowStrength;
            Debug.Log("Throwing Pie Scent");
        }
        var position = transform.position + transform.forward;
        var rotation = transform.rotation;
        var projectile = Instantiate(throwable, position, rotation);
        projectile.GetComponent<Projectile>().Fire(projectileSpeed, transform.forward);
    }

    public float lerpFloat = 0f;

    //THIS CODE IS COMPLETE AND UTTER GARBAGE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    /*
    public IEnumerator throwObject(GameObject distraction)
    {
        Instantiate(distraction, this.transform.position, Quaternion.identity);

        Vector3 initPlayerPos = this.transform.position;
        Vector3 initTargetPos = target.transform.position;

        float target_Distance = Vector3.Distance(distraction.transform.position, initTargetPos);

        Vector3 interpolatedPosition = Vector3.Lerp(initPlayerPos, initTargetPos, target_Distance);

        lerpFloat += .01f;

        while (!Mathf.Approximately(target_Distance, lerpFloat))
        {
            Debug.Log("Moving forward a bit.");
            distraction.transform.position += (distraction.transform.forward);
            lerpFloat += .01f;
        }
        yield return null;
    }
    */

    private void FixedUpdate()
    {
        if (canMove)
        {
            rigid.velocity = transform.TransformDirection(movement);
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

    private void ChangeMoveMent()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        zInput = Input.GetAxisRaw("Vertical");
        movement = new Vector3(xInput * currentSpeed, rigid.velocity.y, zInput * currentSpeed);
    }

    public void HandleJump()
    {
        Vector3 jumpVec = new Vector3(0, jumpSpeed, 0);
        rigid.AddRelativeForce(jumpVec, ForceMode.Impulse);
    }

    public void sprint()
    {
        if (sprintLimit > 0 && rigid.velocity != Vector3.zero)
        {
            sprinting = true;
            if (currentSpeed == originalSpeed) currentSpeed *= sprintFactor;
        }
        else
        {
        }
    }

    public void crouch()
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
