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

    public int currentFloor;

    private Transform interactedItem;

    private bool canMove = true;

    private Rigidbody rigid;

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

    public List<Button> elevatorButtons;

    public GameObject pieTin;

    public InputActionReference interactionInput;

    [SerializeField]
    private int keyCount;

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
        pieTin = GameObject.FindGameObjectWithTag("PieTin");
        currentFloor = 1;
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
        if (hit.collider.gameObject == pieTin)
        {
            if (!keyDeterGrabbed)
            {
                Debug.Log("I feel like I need something else...");
                return;
            }
            else
            {
                hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
                hit.collider.enabled = false;
                hasPieTin = true;
                Debug.Log("Better start running!");
                return;
            }
        }
        if (hit.collider.gameObject.tag == "EleDoor")
        {
            //"elevator panel".setActive = true;
        }
        if (hit.collider.tag == "Key")
        {
            hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
            hit.collider.enabled = false;
            keySpace[keyCount] = hit.collider.gameObject;
            keyCount++;
            return;
        }
        if (hit.collider.tag == "KeyDeter")
        {
            keyDeterGrabbed = true;
        }
        for (int i = 0; i < inventory.Count; i++)
        {
            if (isStored) return;
            else
            {
                if (inventory[i] == null)
                {
                    inventory[i] = hit.collider.gameObject;
                    hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    hit.collider.enabled = false;
                    isStored = true;
                }
            }
        }
        if (!isStored) Debug.Log("Inventory is full.");
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
    }

    public void Drop(InputAction.CallbackContext obj)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ChangeMoveMent();

        //Debug.DrawRay(playerCameraTransform.position,
            //playerCameraTransform.forward * hitRange,
            //Color.red);
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
    }

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

        for (int i = 0; i < elevatorButtons.Count; i++)
        {
            if (i == currentFloor) elevatorButtons[i].interactable = false;
            else elevatorButtons[i].interactable = true;
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
