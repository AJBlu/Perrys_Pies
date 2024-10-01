using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class PerryNav : MonoBehaviour
{
    [Header("Enable Debug Messages")]
    public bool DebugEnabled;

    public List<GameObject> searchThese;

    private GameObject player;

    //Critical Components
    public PerrySensor PerrySensor;
    public State_Machine StateMachine;
    public NavMeshAgent NavMeshAgent;


    //States
    public State _Patrol;
    public State _Pursuit;
    public State _Search;


    //pursuit and speed
    [Range(0f, 10f)]
    public float _speed;

    [Range(0f, 20f)]
    public float _chaseSpeed;

    private bool _isChasing = false;

    private bool _isTrackingPlayerPosition = false;

    public UnityEvent allDestinationsSearched;

    private void Awake()
    {
        searchThese = new List<GameObject>();
        if (!PerrySensor)
        {
            print("Warning [PerryNav.cs]: PerrySensor not attached. Instancing it instead.");
            PerrySensor = gameObject.AddComponent(typeof(PerrySensor)) as PerrySensor;
        }

        if (!StateMachine)
        {
            print("Warning [PerryNav.cs]: StateMachine not attached. Instancing it instead.");
            StateMachine = gameObject.AddComponent(typeof(State_Machine)) as State_Machine;
        }

        AddStates();

        StateMachine.ChangeState(_Patrol);
    }

    private void FixedUpdate()
    {

        //if there are still nodes to be searched and nav agent has no path
        if (searchThese.Count != 0 && !NavMeshAgent.hasPath)
        {
            //OnAudioCueHeard();

        }
        //if there are no more nodes to search and nav agent has reached last destination
        if (searchThese.Count == 0 && !NavMeshAgent.hasPath)
        {
            allDestinationsSearched.Invoke();
        }

        if (_isChasing)
        {
            if (!_isTrackingPlayerPosition)
                StartCoroutine("TrackPlayerPosition");

        }

    }        
    
    private IEnumerator TrackPlayerPosition()
    {
        _isTrackingPlayerPosition = true;
        NavMeshAgent.SetDestination(player.transform.position);

        yield return new WaitForSeconds(2.5f);
        _isTrackingPlayerPosition = false;
    }

    private void AddStates()
    {
        _Patrol = gameObject.GetComponent<Patrol>();
        if(_Patrol == null){
            Debug.Log("Warning [PerryNav.cs]: Patrol not attached. Instancing it instead.");
           _Patrol = gameObject.AddComponent(typeof (Patrol)) as Patrol;
        }
        _Pursuit = gameObject.GetComponent<Pursuit>();
        if (_Pursuit == null)
        {
            Debug.Log("Warning [PerryNav.cs]: Pursuit not attached. Instancing it instead.");
            _Pursuit = gameObject.AddComponent(typeof(Pursuit)) as Pursuit;
        }
        _Search = gameObject.GetComponent<Search>();
        if (_Search == null)
        {
            Debug.Log("Warning [PerryNav.cs]: Patrol not attached. Instancing it instead.");
            _Search = gameObject.AddComponent(typeof(Search)) as Search;
        }
    }

    public void OnPursuit()
    {
        player = GameObject.FindWithTag("Player");
        NavMeshAgent.SetDestination(player.transform.position);
        NavMeshAgent.speed = _chaseSpeed;
    }

    public void OnPursuitExit()
    {
        NavMeshAgent.speed = _speed;
    }

    public void OnAudioCueHeard()
    {
        if (!NavMeshAgent.hasPath)
        {
            float shortest = float.MaxValue;
            int index = 0;
            //search 
            for (int i = 0; i < searchThese.Count; i++)
            {
                if (Vector3.Distance(transform.position, searchThese[i].transform.position) < shortest)
                {
                    shortest = Vector3.Distance(transform.position, searchThese[i].transform.position);
                    index = i;
                }
            }
            NavMeshAgent.SetDestination(searchThese[index].transform.position);
            searchThese.Remove(searchThese[index]);

        }
    }

    //events

    public void OnPatrol()
    {
        if(DebugEnabled)
            Debug.Log("Patrolling now.");
        NavMeshAgent.speed = _speed;

    }


    public void OnPatrolExit()
    {

    }

    public void OnSearch()
    {
        allDestinationsSearched.AddListener(gameObject.GetComponent<Search>().OnSearchCompleted);
        NavMeshAgent.speed = _speed;


    }

    public void OnSearchExit()
    {
        allDestinationsSearched.RemoveListener(gameObject.GetComponent<Search>().OnSearchCompleted);
    }

    public void GetClosestPatrolNode()
    {


    }

}