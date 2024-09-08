using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class PerryNav : MonoBehaviour
{
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

        AddEvents();

        StateMachine.ChangeState(_Patrol);
    }

    private void FixedUpdate()
    { }        
    

    private void AddEvents()
    {
        _Patrol = gameObject.GetComponent<Patrol>();
        _Pursuit = gameObject.GetComponent<Pursuit>();
        _Search = gameObject.GetComponent<Search>();
    }

    public void OnPursuit()
    {
        player = GameObject.FindWithTag("Player");
        NavMeshAgent.SetDestination(player.transform.position);
    }

    public void OnAudioCueHeard()
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
        if(searchThese.Count == 0)
        {
            allDestinationsSearched.Invoke();
        }
    }


}
