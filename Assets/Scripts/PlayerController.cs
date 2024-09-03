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
    private bool isLaunching;
    private bool isJumpingOutOfPlane;
    private Transform interactedItem;

    private bool canMove = true;

    private Rigidbody rigid;


    private Vector3 movement;
    private float xInput;
    private float zInput;
    private float currentSpeed;
    private float originalSpeed;
    private float jumpSpeed;


    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        originalSpeed = currentSpeed = 10;
        jumpSpeed = 10;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeMoveMent();

        if (isLaunching)
        {
            Vector3 targetPos=interactedItem.position;
            if (Vector3.Distance(transform.position,targetPos)<=1)
            {
                rigid.velocity = Vector3.zero;
                canMove = true;
                isLaunching = false;
            }
        }

        if (isJumpingOutOfPlane)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1.5f))
            {
                isJumpingOutOfPlane = false;
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
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

    public void HandleJump(InputAction.CallbackContext context)
    {
        if (context.performed&&IsGround())
        {
            Vector3 jumpVec = new Vector3(0, jumpSpeed, 0);
            rigid.AddRelativeForce(jumpVec,ForceMode.Impulse);
        }
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

    public void InteractWithItem(InteractingItem item,Transform secondItem=null,bool _canMove=false)
    {
        canMove = _canMove;
        switch (item)
        {
            case InteractingItem.Cannon:
                isLaunching = true;
                interactedItem = secondItem;
                break;
            case InteractingItem.Plane:
                if (_canMove == true)
                {
                    isJumpingOutOfPlane = true;
                }
                break;
            default:
                break;
        }
    }



    private void ResetSpeed()
    {
        currentSpeed = originalSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="DashPad")
        {
            currentSpeed *= 3;
            Invoke("ResetSpeed", 3);
        }
        if (other.tag=="JumpPad")
        {
            Vector3 jumpVec = new Vector3(20, jumpSpeed*7, 0);
            rigid.AddRelativeForce(jumpVec, ForceMode.Impulse);
        }
    }


}
