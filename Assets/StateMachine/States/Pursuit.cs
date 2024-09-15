using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pursuit : State
{

    private Patrol _patrol;
    private Search _search;
    public UnityEvent chase;
    private void Awake()
    {
        _statemachine = GetComponent<State_Machine>();
        _patrol = GetComponent<Patrol>();
        _search = GetComponent<Search>();

        //events
        EnteredState.AddListener(gameObject.GetComponent<PerryNav>().OnPursuit);
        ExitedState.AddListener(gameObject.GetComponent<PerryNav>().OnPursuitExit);
        EnteredState.AddListener(gameObject.GetComponent<PerrySensor>().OnPursuit);
        ExitedState.AddListener(gameObject.GetComponent<PerrySensor>().OnPursuitExit);



    }
    public override void InitializeState()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.red;

        isActive = true;
        EnteredState.Invoke();
    }

    public override void UpdateState()
    {
        //chase.Invoke();
    }

    public override void ExitState()
    {

        isActive = false;
        ExitedState.Invoke();
 
    }

    public void OnLineOfSightBroken()
    {
        if (isActive)
        {
            _statemachine.ChangeState(_search);

        }
    }

    public void OnAudioCueHeard()
    {

    }

    public void OnDistractionItem()
    {
        if (isActive)
        {
            //perry gets distracted

            //and then returns to patrolling
            _statemachine.ChangeState(_patrol);
        }
    }
}
