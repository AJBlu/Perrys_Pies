using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;

public class DemoPlayerController : MonoBehaviour
{
    public LayerMask pickableLayerMask;
    public Transform playerCameraTransform;

    [Header("Movement Settings")]
    [Tooltip("Velocity of player while moving (default 10)")]
    [Range(1,20)]
    public float originalSpeed;
    [Tooltip("Changes how fast the player moves while sprinting (Sprint Factor 2 = Twice as fast as walking speed).")]
    [Range(1,5)]
    public float sprintFactor;
    [Tooltip("Changes how fast the player moves while crouching (Crouch Factor .75 = 3/4ths as fast as walking speed.")]
    public float crawlFactor;
    [Tooltip("Amount of time player can sprint in seconds.")]
    public float sprintMax;
    [Tooltip("Player height while not crouching.")]
    public float normalHeight;
    [Tooltip("Player height while crouching.")]
    public float crouchHeight;

    public UnityEvent<Vector3> PlayerJump;
    public UnityEvent<Vector3> PlayerSprint;

    public bool isCrouched;
    public bool sprinting;


    private bool canMove = true;
    private Rigidbody rigid;
    private RaycastHit hit;
    private float _sprint;
    //public Transform pickupParent;

    //private Transform inHandItem;

    private Vector3 movement;
    private float xInput;
    private float zInput;
    public float currentSpeed;
    public float jumpSpeed;

    [Min(1)]
    public float hitRange = 3f;

    public InputActionReference interactionInput;

    public static GameObject playerInstance;

    private void Awake()
    {
        Debug.Log("WARNING: This is a version of the Player prefab without advanced functionality, like interaction with outside objects. Use only for checking scene blocking and Perry navigation.");
        rigid = GetComponent<Rigidbody>();
        originalSpeed = currentSpeed;
        isCrouched = false;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeMoveMent();
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
            if (_sprint > 0)
            {
                _sprint -= 0.02f;
            }
        }
        else
        {
            if (_sprint < sprintMax )
            {
                _sprint += 0.02f;
            }
        }

        if (_sprint <= 0)
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
        PlayerJump.Invoke(transform.position);

    }

    public void sprint()
    {
        if (_sprint > 0 && rigid.velocity != Vector3.zero)
        {
            sprinting = true;
            if (currentSpeed == originalSpeed) currentSpeed *= sprintFactor;
            PlayerSprint.Invoke(transform.position);
        }
    }

    public void crouch()
    {
        isCrouched = true;
        playerCameraTransform.transform.localPosition = new Vector3(0f, crouchHeight, 0f);
        if (currentSpeed == originalSpeed) currentSpeed *= crawlFactor;
    }

    public void resetMovement()
    {
        isCrouched = false;
        sprinting = false;
        if (currentSpeed != originalSpeed) currentSpeed = originalSpeed;
        playerCameraTransform.transform.localPosition = new Vector3(0f, normalHeight, 0f);
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
}
