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

        ComponentAssignment();

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
    private void ComponentAssignment()
    {
        if (gameObject.GetComponent<Search>())
        {
            _search = gameObject.GetComponent<Search>();
        }
        else
        {
            Debug.LogFormat($"{gameObject.name} [Pursuit.cs:ComponentAssignment()] Search component not attached to {gameObject.name}, instancing now.");
            _search = gameObject.AddComponent(typeof(Search)) as Search;
        }

        if (gameObject.GetComponent<Patrol>())
        {
            _patrol = gameObject.GetComponent<Patrol>();
        }
        else
        {
            Debug.LogFormat($"{gameObject.name} [Pursuit.cs:ComponentAssignment()] Patrol component not attached to {gameObject.name}, instancing now.");
            _patrol = gameObject.AddComponent(typeof(Patrol)) as Patrol;

        }


        if (gameObject.GetComponent<State_Machine>())
        {
            _statemachine = gameObject.GetComponent<State_Machine>();
        }
        else
        {
            Debug.LogFormat($"WARNING! {gameObject.name} [Pursuit.cs:ComponentAssignment()] StateMachine component not " +
                $"attached to {gameObject.name} PerryNav.cs should have instanced it.");
        }


        if (gameObject.GetComponent<PerryNav>())
        {
            _perry = gameObject.GetComponent<PerryNav>();
        }
        else
        {
            Debug.LogFormat($"WARNING! {gameObject.name} [Pursuit.cs:ComponentAssignment()] PerryNav component not " +
                $"attached to {gameObject.name}.");
        }


        if (gameObject.GetComponent<PerrySensor>())
        {
            _perrySensor = gameObject.GetComponent<PerrySensor>();
        }
        else
        {
            Debug.LogFormat($"WARNING! {gameObject.name} [Pursuit.cs:ComponentAssignment()] PerrySensor component not " +
                $"attached to {gameObject.name} PerryNav.cs should have instanced it.");
        }

    }
}
