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

    [Header("Node")]
    public PatrolManager PatrolManager;

    [Header("Perry AI Components")]
    public PerrySensor PerrySensor;
    public State_Machine StateMachine;
    public NavMeshAgent NavMeshAgent;


    [Header("States")]
    public State _Patrol;
    public State _Pursuit;
    public State _Search;

    [Header("Perry Movement")]
    [Range(1f, 10f)]
    [Tooltip("Adjusts speed of Perry in the patrol state.")]
    public float PerryPatrolSpeed;
    [Range(1f, 10f)]
    [Tooltip("Adjusts speed of Perry in the search state.")]
    public float PerrySearchSpeed;
    [Range(1f, 10f)]
    [Tooltip("Adjusts speed of Perry in the pursuit state.")]
    public float PerryPursuitSpeed;

    public UnityEvent allDestinationsSearched;


    private void Awake()
    {
        if(PatrolManager == null)
        {
            print("Warning [PerryNav.cs]: PatrolManager not attached. Attempting to find in scene.");
            PatrolManager = GameObject.Find("PatrolManager").GetComponent<PatrolManager>();
        }
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

        StateMachine.ChangeState(_Search);
    }

    private void FixedUpdate()
    {
        if (StateMachine._activeState == gameObject.GetComponent<Pursuit>())
        {
            NavMeshAgent.SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position);
        }
        else
        {

            //if there are still nodes to be searched and nav agent has no path
            if (PatrolManager.SearchNodes.Count != 0 && !NavMeshAgent.hasPath)
            {
                OnAudioCueHeard();

            }
            //if there are no more nodes to search and nav agent has reached last destination
            if (PatrolManager.SearchNodes.Count == 0 && !NavMeshAgent.hasPath)
            {
                allDestinationsSearched.Invoke();
            }
        }

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
        NavMeshAgent.speed = PerryPursuitSpeed;
        var player = GameObject.FindWithTag("Player");
        NavMeshAgent.SetDestination(player.transform.position);
    }

    public void OnPursuitExit()
    {

    }

    public void OnAudioCueHeard()
    {
        if (!NavMeshAgent.hasPath)
        {
            float shortest = float.MaxValue;
            int index = 0;
            //search 
            for (int i = 0; i < PatrolManager.SearchNodes.Count; i++)
            {
                if (Vector3.Distance(transform.position, PatrolManager.SearchNodes[i].transform.position) < shortest)
                {
                    shortest = Vector3.Distance(transform.position, PatrolManager.SearchNodes[i].transform.position);
                    index = i;
                }
            }
            NavMeshAgent.SetDestination(PatrolManager.SearchNodes[index].transform.position);
            PatrolManager.SearchNodes.Remove(PatrolManager.SearchNodes[index]);

        }
    }

    //events

    public void OnPatrol()
    {
        NavMeshAgent.speed = PerryPatrolSpeed;
        if(DebugEnabled)
            Debug.Log("Patrolling now.");
    }
        

    public void OnPatrolExit()
    {

    }

    public void OnSearch()
    {
        NavMeshAgent.speed = PerrySearchSpeed;
        allDestinationsSearched.AddListener(gameObject.GetComponent<Search>().OnSearchCompleted);

    }

    public void OnSearchExit()
    {
        allDestinationsSearched.RemoveListener(gameObject.GetComponent<Search>().OnSearchCompleted);
    }

    public void OnDeterrentItemHit(float duration)
    {
        StartCoroutine(PauseMovement(duration));
    }

    private IEnumerator PauseMovement(float duration)
    {
        NavMeshAgent.isStopped = true;
        yield return new WaitForSeconds(duration);
        NavMeshAgent.isStopped = false;

    }

    public void GetClosestPatrolNode()
    {


    }

}