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
        _perry = GetComponent<PerryNav>();
        _perrySensor = GetComponent<PerrySensor>();
        _statemachine = GetComponent<State_Machine>();
        _patrol = GetComponent<Patrol>();
        _search = GetComponent<Search>();


    }
    public override void InitializeState()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.red;

        EnteredState.Invoke();

        isActive = true;
    }

    public override void UpdateState()
    {
        chase.Invoke();
    }

    public override void ExitState()
    {
        Debug.Log("If this doesn't show up I'm killing my dog");
        ExitedState.Invoke();
        isActive = false;
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
