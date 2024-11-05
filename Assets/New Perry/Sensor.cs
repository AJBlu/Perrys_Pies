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
            if (isPlayerInCone(dotProduct, Player.transform))
            {

                //look at player
                gameObject.transform.LookAt(Player.transform.position);

                if (isPlayerClose(closeRadius, Player.transform))
                {


                    //draw raycast for close
                    //if it hits, change state to pursuit

                }
                else
                {

                    //if that doesn't work, draw far raycast
                    //if that hits, change state to search and create hearing node on player location
                }
            }
        }
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
            if(Vector3.Distance(sourceTransform.position, gameObject.transform.position) < closeRadius)
            {
                Navigation.CheckNewNodePriority(sourceTransform, priority);
            }
        }
        else
        {
            if (Vector3.Distance(sourceTransform.position, gameObject.transform.position) < distantRadius)
            {
                Navigation.CheckNewNodePriority(sourceTransform, priority);
            }
        }
    }
}
