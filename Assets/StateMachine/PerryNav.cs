using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class PerryNav : MonoBehaviour
{
    //Critical Components
    public PerrySensor PerrySensor;
    public State_Machine StateMachine;
    public NavMeshAgent NavMeshAgent;


    //States
    public State _Patrol;
    public State _Pursuit;
    public State _Search;

    private void Awake()
    {
        NavMeshAgent = gameObject.AddComponent(typeof (NavMeshAgent) ) as NavMeshAgent;
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
    void Update()
    {
       StateMachine.RunActiveState();
    }

    private void AddEvents()
    {
        _Patrol = gameObject.GetComponent<Patrol>();
        _Pursuit = gameObject.GetComponent<Pursuit>();
        _Search = gameObject.GetComponent<Search>();
    }
}
