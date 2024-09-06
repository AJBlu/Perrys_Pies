using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PerryNav : MonoBehaviour
{
    public PerrySensor _perrySensor;
    public State_Machine _stateMachine;
    public NavMeshAgent _navMeshAgent;

    private void Awake()
    {
        Patrol _patrol = new Patrol(this, _stateMachine, _perrySensor);
        _stateMachine.ChangeState(_patrol);
        _navMeshAgent = gameObject.AddComponent(typeof (NavMeshAgent) ) as NavMeshAgent;
        if (!_perrySensor)
            print("Warning [PerryNav.cs]: PerrySensor not attached. Instancing it instead.");
        else
            _perrySensor = gameObject.AddComponent(typeof (PerrySensor)) as PerrySensor;
    }
    void Update()
    {
        
    }
}
