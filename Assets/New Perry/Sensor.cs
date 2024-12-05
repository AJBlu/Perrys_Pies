using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public float visionAngle;
    public float closeRadius;
    public float distantRadius;
    public float _cd;
    public float _dr;
    public float walkingHearingRadius;
    public float farHearingRadius;
    public GameObject Player;
    public Navigation Navigation;
    public NewStateMachine NewStateMachine;
    public PlayerController PlayerController;
    public bool checkingEyeContact;
    public bool playerSeenByEyeContactCheck;

    public AudioSource alert;
    private void Awake()
    {
        _cd = closeRadius;
        _dr = distantRadius;
        Navigation = gameObject.GetComponent<Navigation>();
        NewStateMachine = gameObject.GetComponent<NewStateMachine>();
        Player = GameObject.FindGameObjectWithTag("Player");
        PlayerController = Player.GetComponent<PlayerController>();
        if(PlayerController != null)
        {
            PlayerController.playerJump.AddListener(OnNoiseEvent);
            PlayerController.playerRun.AddListener(OnNoiseEvent);
        }
    }

    private void FixedUpdate()
    {
        checkPlayerCrouched();
        //check if player can be seen at all
        if (isPlayerDistant(_dr, Player.transform))
        {
            //then check if player is in vision cone as long as perry isn't chasing them
            if (isPlayerInCone(visionAngle, Player.transform) && NewStateMachine.GetState() != States.PURSUIT)
            {
                //make decisions after checking if player is crouched

                //look at player
                if(NewStateMachine.currentState != States.TRAPPED)
                    gameObject.transform.LookAt(Player.transform.position);
                RaycastHit hit;
                if (isPlayerClose(closeRadius, Player.transform))
                {
                //Debug.Log("player is close");

                //draw raycast for close
                //if it hits, change state to pursuit
                if (Physics.Raycast(transform.position, transform.forward * _cd, out hit, _cd))
                    {
                        if (hit.collider.gameObject.tag == "Player")
                        {
                            if (NewStateMachine.GetState() != States.PURSUIT && NewStateMachine.GetState() != States.TRAPPED) 
                            {
                                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * _cd, Color.green);
                                Debug.Log("Player seen up close, changing to pursuit state.");
                                //Navigation.NavMeshAgent.ResetPath();
                                NewStateMachine.ChangeState(States.PURSUIT);
                            }
                            if (!checkingEyeContact && NewStateMachine.GetState() == States.PURSUIT)
                            {
                                Debug.Log("Starting eyecontact check.");
                                StartCoroutine(EyecontactCheck());
                            }
                            
                        }

                    }
                }
                else
                {
                    if(!checkingEyeContact && NewStateMachine.GetState() == States.PURSUIT)
                    {
                        Debug.Log("Player is out of close sight range but still being pursued.");
                        StartCoroutine(EyecontactCheck());
                    }
                    else //if not in pursuit, then run distant sight code
                    {
                        Debug.Log("Checking to see if player is within distant sight.");
                        if(Physics.Raycast(transform.position, transform.forward * _dr, out hit, _dr))
                        {
                            if(hit.collider.gameObject.tag == "Player")
                            {   //if perry is not actively chasing the player, create a search node on distant sight radius
                                if (NewStateMachine.GetState() != States.PURSUIT)
                                {
                                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * _dr, Color.yellow);
                                    Debug.Log("Player has been seen from afar. Creating search node.");
                                    OnNoiseEvent(Player.transform, Priority.SEEN);
                                }
                            }
                        }
                    }
                    //if that doesn't work, draw far raycast
                    //if that hits, change state to search and create hearing node on player location
                }
            }

            //if player is in walking radius and isn't being chased, make a search node
            if (isPlayerInWalkingRadius(walkingHearingRadius, Player.transform) && Player.gameObject.GetComponent<Rigidbody>().velocity.magnitude > 1)
            {
                Debug.Log("Player found in walking radius and is moving. Seeing if node can be made.");
                OnNoiseEvent(Player.transform, Priority.WALKING);
            }
        }
    }

    private void checkPlayerCrouched()
    {
        if (Player.GetComponent<PlayerController>().isCrouched)
        {
            _cd = (closeRadius * 2f) / 3f;
            _dr = (distantRadius * 2f) / 3f;
        }
        else
        {
            _cd = closeRadius;
            _dr = distantRadius;
        }
    }

    private IEnumerator EyecontactCheck()
    {
        checkingEyeContact = true;
        yield return new WaitForSeconds(5f);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, _dr))
        {
            if(hit.collider.gameObject.tag == "Player")
            {
                Debug.Log("Continuing to pursue player.");
            }
            else
            {
                Debug.Log("Player no longer found. Switching to search and adding node with last known position.");
                OnNoiseEvent(Player.transform, Priority.SEEN);
                NewStateMachine.ChangeState(States.SEARCH);

            }
        }
        else
        {
            Debug.Log("Empty raycast, player no longer found. Switching to search and adding node with last known position.");
            OnNoiseEvent(Player.transform, Priority.SEEN);
            NewStateMachine.ChangeState(States.SEARCH);
        }
        checkingEyeContact = false;

    }

    private bool isPlayerInCone(float visionAngle, Transform playerPosition)
    {
        Vector3 target = Player.transform.position - gameObject.transform.position;
        //Debug.Log(Vector3.Angle(target, transform.TransformDirection(Vector3.forward)));
        return Vector3.Angle(target, transform.TransformDirection(Vector3.forward)) < visionAngle;
    }
    private bool isPlayerClose(float closeRadius, Transform playerPosition)
    {

        return Vector3.Distance(playerPosition.position, gameObject.transform.position) < _cd;
    }

    private bool isPlayerDistant(float distantRadius, Transform playerPosition)
    {
        return Vector3.Distance(playerPosition.position, gameObject.transform.position) < _dr;
    }

    private bool isPlayerInWalkingRadius(float walkingHearingRadius, Transform playerPosition)
    {
        return Vector3.Distance(playerPosition.position, gameObject.transform.position) < walkingHearingRadius;
    }



    /// <summary>
    /// Check distance of hearing node according to priority. If it would be created outside of the priority's hearing radius, do not make it. Otherwise, have Navigation check the node vs. the current hearing node's priority.
    /// </summary>
    /// <param name="sourceTransform"></param>
    /// <param name="priority"></param>
    public void OnNoiseEvent(Transform sourceTransform, Priority priority)
    {
        if(priority == Priority.WALKING)
        {
            if(Vector3.Distance(sourceTransform.position, gameObject.transform.position) < walkingHearingRadius)
            {
                Debug.Log("Walking node in appropriate distance. Checking to see if it can be the new hearing node.");

                Navigation.CheckNewNodePriority(sourceTransform, priority);
            }
        }
        else if (priority == Priority.SOUNDTRAP || priority == Priority.ATTRACTANT)
        {
            Debug.Log("Soundtrap or throwable triggered.");
            Navigation.CheckNewNodePriority(sourceTransform, priority);
            
        }
        else
        {
            if (Vector3.Distance(sourceTransform.position, gameObject.transform.position) < farHearingRadius)
            {
                Debug.Log("Non-walking node in appropriate distance. Checking to see if it can be the new hearing node.");
                Navigation.CheckNewNodePriority(sourceTransform, priority);
            }
        }
    }
}
