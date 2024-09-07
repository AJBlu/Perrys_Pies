using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


//fires off unity events to change states
public class PerrySensor : MonoBehaviour
{
    //Detection Ranges
    public float CloseSightRadius;

    public float DistantSightRadius;

    public float HearingRadius;

    const float VISIONANGLE = .707f;

    public bool playerSeen;
    private bool runningDelayedRaycast;

    //Events
    public UnityEvent PlayerSeen_Distant;
    public UnityEvent PlayerSeen_Close;
    public UnityEvent AudioCueHeard;
    public UnityEvent LineOfSightBroken;
    public UnityEvent SearchCompleted;

    private void FixedUpdate()
    {
        CheckLineOfSight(CloseSightRadius, "Player");
        


    }


    public void OnNewState()
    {
    }

    public void OnExitState()
    {

    }

    private GameObject CheckDistance(float radius, string tagname)
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider col in hit)
        {
            if (col.tag == tagname)
            {
                return col.gameObject;

            }
        }
        return null;
    
    }


    private void CheckLineOfSight(float radius, string tagname)
    {
        GameObject _player = CheckDistance(radius, tagname);
        RaycastHit hit;
        if(_player != null)
        {
            //if player is confirmed to exist in space, check if player is within vision cone
            // if(Vector3.Dot(gameObject.transform.position, _player.transform.position) < VISIONANGLE)
            // {
            transform.LookAt(_player.transform);
            if (!playerSeen)
            {
                //then send raycast
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, radius))
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * radius, Color.green);
                    //invoke events depending on radius used
                    if (radius == CloseSightRadius)
                    {
                        PlayerSeen_Close.Invoke();
                        playerSeen = true;
                    }
                    else if (radius == DistantSightRadius)
                    {
                        PlayerSeen_Distant.Invoke();
                        playerSeen = true;
                    }
                }
            }
            //if the player has been seen, check at a slower rate
            else
            {
                if(!runningDelayedRaycast)
                    StartCoroutine("DelayedRaycast");
            }

                    
            // }
        }
    }

    private IEnumerator DelayedRaycast()
    {
        Debug.Log("Running delayed raycast.");
        runningDelayedRaycast = true;
        yield return new WaitForSeconds(3f);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, CloseSightRadius))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * CloseSightRadius, Color.green);
        }
        else
        {
            playerSeen = false;
            LineOfSightBroken.Invoke();
        }
        runningDelayedRaycast = false;

    }


    private void checkSoundCueHeard()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 distantExtends = transform.position + new Vector3(0, 0, DistantSightRadius);
        Vector3 closeExtends = transform.position + new Vector3(0, 0, CloseSightRadius);
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, HearingRadius);
        UnityEditor.Handles.DrawLine(transform.position, distantExtends);
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawLine(transform.position, closeExtends);

    }


}
