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

    public Transform pickupParent;

    private Transform inHandItem;

    private Vector3 movement;
    private float xInput;
    private float zInput;
    public float currentSpeed;
    private float originalSpeed;
    public float jumpSpeed;

    public InputActionReference interactionInput;

    // Start is called before the first frame update
    void Start()
    {
        interactionInput.action.performed += Interact;
        rigid = GetComponent<Rigidbody>();
        originalSpeed = currentSpeed = 10;
        jumpSpeed = 10;
    }

    private void Interact(InputAction.CallbackContext obj)
    {
        Debug.Log("Should try to hold on.");
        if (hit.collider != null)
        {
            Debug.Log(hit.collider.name);
            if (hit.collider.GetComponent<Holdable>())
            {
                inHandItem = hit.collider.gameObject.transform;
                inHandItem.transform.SetParent(pickupParent.transform, false);
                return;
            }
        }
        else
        {
            Debug.Log("WHY?!?!?!?!?!?!?!?!?!?!?!?!?!");
        }
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

        if (inHandItem != null)
        {
            return;
        }

        if (Physics.Raycast(playerCameraTransform.position,
            playerCameraTransform.forward,
            out hit, hitRange, pickableLayerMask))
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

    

    public void Drop(InputAction.CallbackContext obj)
    {

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

    private void OnTriggerEnter(Collider other)
    {
        
    }


}
