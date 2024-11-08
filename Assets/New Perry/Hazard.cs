using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    Sensor perry_Sensor;
    PlayerController playerController;
    
    
    public AudioClip[] soundsForHazard;
    public float nearRadius, medRadius, farRadius;

    public bool playerIsInArea;

    public bool triggeredByLoudAction, triggeredByWalk, triggeredByCrouch, resettableTrap;

    public bool soundTrapTriggered;
    public void FixedUpdate()
    {
        if (playerIsInArea && !soundTrapTriggered)
        {
            if (playerController.isCrouched)
            {
                CrouchMovementAction();
            }

            if(playerController.isWalking)
            {
                WalkMovementAction();
            }

            if (playerController.isLoudMovement)
            {
                LoudMovementAction();
            }

        }
    }

    public void Awake()
    {
        //will send nodes to Sensor... If it's in an appropriate radius to be heard
        perry_Sensor = GameObject.FindGameObjectWithTag("Perry").GetComponent<Sensor>();
        playerIsInArea = false;

    }

    public void OnTriggerEnter(Collider other)
    {
        //when player enters area sound trap affects
        if(other.gameObject.tag == "Player")
        {
            playerIsInArea = true;
            playerController = other.gameObject.GetComponent<PlayerController>();

        }
    }

    public void OnTriggerExit(Collider other)
    {
        //when player leaves area sound trap affects
        if(other.gameObject.tag == "Player")
        {
            playerIsInArea = false;
            if (resettableTrap)
            {
                soundTrapTriggered = false;
            }
        }
    }

    public void AlertPerry(float radius)
    {
        if (Vector3.Distance(gameObject.transform.position, perry_Sensor.gameObject.transform.position) < radius)
        {
            Debug.Log("Informing Perry");
            perry_Sensor.OnNoiseEvent(gameObject.transform, Priority.SOUNDTRAP);
        }
    }


    /// <summary>
    /// Fill out if Perry needs to be alerted on crouch. If soundtrap is done after being triggered, set soundTrapTriggered to true.
    /// </summary>
    public virtual void CrouchMovementAction()
    {
        soundTrapTriggered = true;
    }

    /// <summary>
    /// Fill out if Perry needs to be alerted on Walk. If soundtrap is done after being triggered, set soundTrapTriggered to true.
    /// </summary>
    public virtual void WalkMovementAction()
    {
        soundTrapTriggered = true;
    }
    /// <summary>
    /// Fill out if Perry needs to be alerted on loud movement. If soundtrap is done after being triggered, set soundTrapTriggered to true.
    /// </summary>
    public virtual void LoudMovementAction()
    {
        soundTrapTriggered = true;

    }

}
