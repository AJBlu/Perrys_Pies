using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InteractingItem
{
    Cannon,
    Plane,
}

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
    }

    private void Interact(InputAction.CallbackContext obj)
    {
        Debug.Log("Interacted with: " + hit.collider.name);
        //hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
        if (hit.collider.tag != "NotItem") storeLogic();
    }

    public void storeLogic()
    {
        bool isStored = false;
        if (hit.collider.tag == "PieTin")
        {
            hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
            hit.collider.enabled = false;
            Debug.Log("Better start running!");
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
                    inventory[i] = hit.collider.gameObject;
                    itemCheck();
                    isStored = true;
                }
            }
        }
        Debug.Log("Inventory is full.");
    }

    public void itemCheck()
    {
        
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
    }

    private void ChangeMoveMent()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        zInput = Input.GetAxisRaw("Vertical");
        movement = new Vector3(xInput * currentSpeed, rigid.velocity.y, zInput * currentSpeed);
    }

    public void HandleJump()
    {
        Debug.Log("Should be jumping.");
        Vector3 jumpVec = new Vector3(0, jumpSpeed, 0);
        rigid.AddRelativeForce(jumpVec, ForceMode.Impulse);
    }

    public void sprint()
    {
        Debug.Log("Checking the sprint limit and velocity...");
        if (sprintLimit > 0 && rigid.velocity != Vector3.zero)
        {
            Debug.Log("Sprint Success");
            if (currentSpeed == originalSpeed) currentSpeed *= sprintFactor;
        }
        else
        {
            Debug.Log("Sprint Failure");
        }
    }

    public void crouch()
    {
        Debug.Log("Getting lower to the ground");
        playerCameraTransform.transform.position = crouchHeight.transform.position;
        if (currentSpeed == originalSpeed) currentSpeed *= crawlFactor;
    }

    public void resetMovement()
    {
        Debug.Log("Going back to normal.");
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
