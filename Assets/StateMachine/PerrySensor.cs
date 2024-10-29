using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;


/// <summary>
/// PerrySensor keeps track of what Perry has seen and heard, changing states and adding points of interest to PerryNav.
/// </summary>
public class PerrySensor : MonoBehaviour
{
    //Detection Ranges
    [Header("Detection Ranges")]

    [Range(1f,50f)]
    public float CloseSightRadius = 5f;

    [Range(5f, 50f)]
    public float DistantSightRadius = 10f;

    [Range(5f, 50f)]
    public float HearingRadius = 10f;

    const float VISIONANGLE = .707f;

    public bool playerSeen;
    private bool runningDelayedRaycast;

    public GameObject _player;
    public GameObject POI;

    private State_Machine _statemachine;
    private PerryNav _perryNav;

    //Events
    [Header("Events")]
    public UnityEvent PlayerSeen_Distant = new UnityEvent();
    public UnityEvent PlayerSeen_Close = new UnityEvent();
    public UnityEvent AudioCueHeard = new UnityEvent();
    public UnityEvent LineOfSightBroken = new UnityEvent();
    public UnityEvent SearchCompleted = new UnityEvent();

    private void Awake()
    {
        if (gameObject.GetComponent<PerryNav>())
        {
            _perryNav = gameObject.GetComponent<PerryNav>();
        } else {

            if (_perryNav.DebugEnabled)
                Debug.LogFormat($"[{gameObject.name}] in PerrySensor.cs: No PerryNav Component in {gameObject.name}");

        }

        if (gameObject.GetComponent<State_Machine>())
        {
            _statemachine = gameObject.GetComponent<State_Machine>();
        } else {
            if (_perryNav.DebugEnabled)
                Debug.LogFormat($"[{gameObject.name}] in PerrySensor.cs: No State_Machine Component in {gameObject.name}");
        }


        if (GameObject.FindWithTag("Player"))
        {

        } else
        {
            _player = GameObject.FindWithTag("Player");
            if (_perryNav.DebugEnabled)
                Debug.LogFormat($"[{gameObject.name}] in PerrySensor.cs: No Player GameObject in Scene");
        }
        //add listeners to events
        AudioCueHeard.AddListener(_perryNav.OnAudioCueHeard);
    }

    private void FixedUpdate()
    {
        CheckLineOfSight(CloseSightRadius, "Player");
        CheckLineOfSight(DistantSightRadius, "Player");
        if (!gameObject.GetComponent<Patrol>().isDeaf)
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
        PatrolManager _patrolManager = GameObject.Find("PatrolManager").GetComponent<PatrolManager>();
        if (_patrolManager == null)
        {
            Debug.LogFormat($"{gameObject.name} [PerrySensor.cs:CheckNearbyAudioSources()] WARNING!" +
                $"  Patrol and Search Node manager is not attached, gameobject is not in scene, or unable to be found.");
        }
        if (POIs.Count != 0)
        {
           foreach (GameObject POI in POIs)
           {
               if (_perryNav.DebugEnabled)
                   Debug.LogFormat($"POI at location {POI.transform.position} added to Search Nodes in Patrol Manager.");
               _patrolManager.SearchNodes.Add(POI.transform);
           }
           AudioCueHeard.Invoke();
        }


    }

    private List<GameObject> CheckNearbyAudioSources(float radius)
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, radius);
        List<GameObject> POIs = new List<GameObject>();
            foreach (Collider col in hit)
            {
                if (col.tag == "POI")
                {
                    if (!col.GetComponent<PointOfInterest>().willBeSearched)
                    {
                        col.gameObject.GetComponent<PointOfInterest>().willBeSearched = true;
                        POIs.Add(col.gameObject);
                        //Debug.LogFormat($"{POIs.Count}");
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
            if(Vector3.Dot(gameObject.transform.position, transform.TransformDirection(Vector3.forward) * radius) < VISIONANGLE){
                transform.LookAt(_player.transform);
                if (!playerSeen)
                {
                    //then send raycast
                    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward) * radius, out hit, radius))
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

                    
            }
        }
    }

    private void InstantiatePOI()
    {
        var _distantPlayer = GameObject.FindGameObjectWithTag("Player");
        var _patrolmanager = GameObject.Find("PatrolManager");
        var lastKnownPosition = Instantiate(POI, _distantPlayer.transform.position, _distantPlayer.transform.rotation);
        lastKnownPosition.transform.SetParent(_patrolmanager.transform);
        _patrolmanager.GetComponent<PatrolManager>().SearchNodes.Add(lastKnownPosition.transform);
        
        AudioCueHeard.Invoke();
    }

    private IEnumerator DelayedRaycast()
    {
        runningDelayedRaycast = true;
        RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward) * CloseSightRadius, out hit, CloseSightRadius))
            {
                Debug.LogFormat($"{gameObject.name} [Perrysensor.cs:DelayedRaycast()] Player has been seen up close.");
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * CloseSightRadius, Color.red);
                PlayerSeen_Close.Invoke();
            }
            else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward) * DistantSightRadius, out hit, DistantSightRadius))
            {
                Debug.LogFormat($"{gameObject.name} [PerrySensor.cs:DelayedRaycast()] Player has been seen from afar.");
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * CloseSightRadius, Color.yellow);
                PlayerSeen_Distant.Invoke();
            }
            else
            {
                Debug.LogFormat($"{gameObject.name} [PerrySensor.cs:DelayedRaycast()] Line of Sight officially broken.");
                playerSeen = false;
                LineOfSightBroken.Invoke();
            }

        yield return new WaitForSeconds(3f);
        _player = CheckDistance(CloseSightRadius, "Player");

        if (_player == null)
        {
            if(_perryNav.DebugEnabled)
                Debug.LogFormat($"{gameObject.name} [PerrySensor.cs:DelayedRaycast()] Player is no longer in vision radius.");
            playerSeen = false;
            LineOfSightBroken.Invoke();
        }
        runningDelayedRaycast = false;

    }


    public void OnPatrol()
    {
        PlayerSeen_Close.AddListener(gameObject.GetComponent<Patrol>().OnClosePlayerSeen);
        PlayerSeen_Distant.AddListener(gameObject.GetComponent<Patrol>().OnDistantPlayerSeen);
        AudioCueHeard.AddListener(gameObject.GetComponent<Patrol>().OnAudioCueHeard);
    }

    public void OnPatrolExit()
    {
        if (_perryNav.DebugEnabled)
            Debug.LogFormat($"{gameObject.name} [PerrySensor.cs:OnPatrolExit()] Removing PlayerSeen_Close, PlayerSeen_Distant, and AudioCueHeard from Patrol State.");
        PlayerSeen_Close.RemoveListener(gameObject.GetComponent<Patrol>().OnClosePlayerSeen);
        PlayerSeen_Distant.RemoveListener(gameObject.GetComponent<Patrol>().OnDistantPlayerSeen);
        AudioCueHeard.RemoveListener(gameObject.GetComponent<Patrol>().OnAudioCueHeard);
    }

    public void OnSearch()
    {
        if (_perryNav.DebugEnabled)
            Debug.LogFormat($"{gameObject.name} [PerrySensor.cs:OnSearch()] Adding PlayerSeen_Close, PlayerSeen_Distant, AudioCueHeard, and LineOfSightBroken to Search State.");

        AudioCueHeard.AddListener(gameObject.GetComponent<Search>().OnAudioCueHeard);
        PlayerSeen_Close.AddListener(gameObject.GetComponent<Search>().OnClosePlayerSeen);
        PlayerSeen_Distant.AddListener(gameObject.GetComponent<Search>().OnDistantPlayerSeen);
        LineOfSightBroken.AddListener(gameObject.GetComponent<Search>().OnLineOfSightBroken);
        

    }

    public void OnSearchExit()
    {
        if (_perryNav.DebugEnabled)
            Debug.LogFormat($"{gameObject.name} [PerrySensor.cs:OnSearchExit()] Removing PlayerSeen_Close, PlayerSeen_Distant, LineOfSightBroken and AudioCueHeard from Search State.");
        AudioCueHeard.RemoveListener(gameObject.GetComponent<Search>().OnAudioCueHeard);
        PlayerSeen_Close.RemoveListener(gameObject.GetComponent<Search>().OnClosePlayerSeen);
        PlayerSeen_Distant.RemoveListener(gameObject.GetComponent<Search>().OnDistantPlayerSeen);
        LineOfSightBroken.RemoveListener(gameObject.GetComponent<Search>().OnLineOfSightBroken);


    }

    public void OnPursuit()
    {
        if (_perryNav.DebugEnabled)
            Debug.LogFormat($"{gameObject.name} [PerrySensor.cs:OnPursuit()] Adding LineOfSightBroken and AudioCueHeard to Patrol State.");
        LineOfSightBroken.AddListener(gameObject.GetComponent<Pursuit>().OnLineOfSightBroken);
        AudioCueHeard.AddListener(gameObject.GetComponent<Pursuit>().OnAudioCueHeard);
    }

    public void OnPursuitExit()
    {
        if (_perryNav.DebugEnabled)
            Debug.LogFormat($"{gameObject.name} [PerrySensor.cs:OnPursuitExit()] Removing LineOfSightBroken and AudioCueHeard from Patrol State.");
        LineOfSightBroken.RemoveListener(gameObject.GetComponent<Pursuit>().OnLineOfSightBroken);
        AudioCueHeard.RemoveListener(gameObject.GetComponent<Pursuit>().OnAudioCueHeard);
        InstantiatePOI();
    }
    
    public void OnPlayerJump(Transform playerTransform)
    {
        Debug.Log("Jump detected! Time to determine if it's listenable.");
        if (isListenable(playerTransform))
        {
            InstantiatePOI();
        }
    }

    public void OnPlayerSprint(Transform playerTransform)
    {
        Debug.Log("Sprint detected! Time to determine if it's listenable.");
        if (isListenable(playerTransform))
            InstantiatePOI();
    }

    private bool isListenable(Transform playerTransform)
    {
        if (Vector3.Distance(playerTransform.position, transform.position) < HearingRadius)
            return true;

        return false;
    }
    /*
    private void OnDrawGizmosSelected()
    {

            Vector3 distantExtends = transform.position + new Vector3(0, 0, DistantSightRadius);
            Vector3 closeExtends = transform.position + new Vector3(0, 0, CloseSightRadius);
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, HearingRadius);
            UnityEditor.Handles.DrawLine(transform.position, distantExtends);
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawLine(transform.position, closeExtends);
    }
    */

}
