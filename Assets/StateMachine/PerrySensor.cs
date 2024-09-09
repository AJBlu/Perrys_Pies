using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;


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

    public GameObject _player;
    public GameObject POI;

    private State_Machine _statemachine;
    private PerryNav _perryNav;

    //Events
    public UnityEvent PlayerSeen_Distant;
    public UnityEvent PlayerSeen_Close;
    public UnityEvent AudioCueHeard;
    public UnityEvent LineOfSightBroken;
    public UnityEvent SearchCompleted;

    private void Awake()
    {
        _perryNav = gameObject.GetComponent<PerryNav>();
        _statemachine = gameObject.GetComponent<State_Machine>();   
    }

    private void FixedUpdate()
    {
        CheckLineOfSight(CloseSightRadius, "Player");
        CheckLineOfSight(DistantSightRadius, "Player");
        if (gameObject.GetComponent<Patrol>().isDeaf)
        {
            
        }
        else
        {
            CheckHearing(HearingRadius);
        }

    }


    public void OnNewState()
    {
    }

    public void OnExitState()
    {

    }

    private void CheckHearing(float radius)
    {
        List<GameObject> POIs = CheckNearbyAudioSources(HearingRadius);

            if (POIs.Count != 0)
            {
                foreach (GameObject POI in POIs)
                {
                    Debug.Log("POI added to searchThese");          
                    _perryNav.searchThese.Add(POI);
                }
                AudioCueHeard.Invoke();
            }

    }

    private List<GameObject> CheckNearbyAudioSources(float radius)
    {
        //Debug.Log("Checking audio sources");
        Collider[] hit = Physics.OverlapSphere(transform.position, radius);
        List<GameObject> POIs = new List<GameObject>(); 
        foreach(Collider col in hit)
        {
            if (col.tag == "POI")
            {
                if (!col.GetComponent<PointOfInterest>().willBeSearched)
                {     
                    Debug.Log("Adding to POIs");
                    col.gameObject.GetComponent<PointOfInterest>().willBeSearched = true;
                    POIs.Add(col.gameObject);
                    Debug.LogFormat($"{POIs.Count}");
                }
            }
        }

        return POIs;
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
        _player = CheckDistance(radius, tagname);
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
                if (Physics.Raycast(transform.position, _player.transform.position * radius, out hit, radius))
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
                        InstantiatePOI();
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

    private void InstantiatePOI()
    {
        var _distantPlayer = GameObject.Find("Player");
        var lastKnownPosition = Instantiate(POI, _distantPlayer.transform.position, _distantPlayer.transform.rotation);
        _perryNav.searchThese.Add(lastKnownPosition);
        AudioCueHeard.Invoke();
    }

    private IEnumerator DelayedRaycast()
    {
        Debug.Log("Running delayed raycast.");
        runningDelayedRaycast = true;
        RaycastHit hit;
            if (Physics.Raycast(transform.position, _player.transform.position, out hit, CloseSightRadius))
            {
                Debug.Log("Player Seen up Close.");
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * CloseSightRadius, Color.red);
                PlayerSeen_Close.Invoke();
            }
            else if (Physics.Raycast(transform.position, _player.transform.position, out hit, DistantSightRadius))
            {
                Debug.Log("Player Seen at Distance.");
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * CloseSightRadius, Color.yellow);
                PlayerSeen_Distant.Invoke();
            }
            else
            {
                Debug.Log("Line of Sight officially broken.");
                playerSeen = false;
                LineOfSightBroken.Invoke();
            }

        yield return new WaitForSeconds(3f);
        _player = CheckDistance(CloseSightRadius, "Player");

        if (_player == null)
        {
            Debug.Log("Player is no longer in vision radius.");
            playerSeen = false;
            LineOfSightBroken.Invoke();
        }
        runningDelayedRaycast = false;

    }


    public void OnPursuitExit()
    {
        InstantiatePOI();
    }

    private void OnDrawGizmos()
    {
        Vector3 distantExtends = transform.position + new Vector3(0, 0, DistantSightRadius);
        Vector3 closeExtends = transform.position + new Vector3(0, 0, CloseSightRadius);
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, HearingRadius);
        UnityEditor.Handles.DrawLine(transform.position, distantExtends);
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawLine(transform.position, closeExtends);

    }


}
