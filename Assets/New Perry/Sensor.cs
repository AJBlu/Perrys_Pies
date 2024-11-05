using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public float dotProduct;
    public float closeRadius;
    public float distantRadius;
    public float walkingHearingRadius;
    public float farHearingRadius;
    public GameObject Player;
    public Navigation Navigation;
    public NewStateMachine NewStateMachine;

    public bool checkingEyeContact;
    public bool playerSeenByEyeContactCheck;


    private void Awake()
    {
        Navigation = gameObject.GetComponent<Navigation>();
        NewStateMachine = gameObject.GetComponent<NewStateMachine>();
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        //check if player can be seen at all
        if (isPlayerDistant(distantRadius, Player.transform))
        {
            //then check if player is in vision cone
            //if (isPlayerInCone(dotProduct, Player.transform))
            //{

                //look at player
                gameObject.transform.LookAt(Player.transform.position);
                RaycastHit hit;
                if (isPlayerClose(closeRadius, Player.transform))
                {
                //Debug.Log("player is close");

                //draw raycast for close
                //if it hits, change state to pursuit
                if (Physics.Raycast(transform.position, transform.forward * closeRadius, out hit, closeRadius))
                    {
                        if (hit.collider.gameObject.tag == "Player")
                        {
                            if (NewStateMachine.GetState() != States.PURSUIT)
                            {
                                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward), Color.green);
                                Debug.Log("Player seen up close, changing to pursuit state.");
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
                    //if that doesn't work, draw far raycast
                    //if that hits, change state to search and create hearing node on player location
                }
                //}
        }
    }

    private IEnumerator EyecontactCheck()
    {
        checkingEyeContact = true;
        yield return new WaitForSeconds(5f);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, distantRadius))
        {
            if(hit.collider.gameObject.tag == "Player")
            {
                Debug.Log("Continuing to pursue player.");
            }
            else
            {
                Debug.Log("Player no longer found. Switching to search and adding node with last known position.");
                OnNoiseEvent(Player.transform, Priority.RUNNING);
                NewStateMachine.ChangeState(States.SEARCH);

            }
        }
        else
        {
            Debug.Log("Empty raycast, player no longer found. Switching to search and adding node with last known position.");
            OnNoiseEvent(Player.transform, Priority.RUNNING);
            NewStateMachine.ChangeState(States.SEARCH);
        }
        checkingEyeContact = false;

    }

    private bool isPlayerInCone(float dotProduct, Transform playerPosition)
    {

        return false;
    }
    private bool isPlayerClose(float closeRadius, Transform playerPosition)
    {

        return Vector3.Distance(playerPosition.position, gameObject.transform.position) < closeRadius;
    }

    private bool isPlayerDistant(float distantRadius, Transform playerPosition)
    {
        return Vector3.Distance(playerPosition.position, gameObject.transform.position) < distantRadius;
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
