using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Search : State
{
    private Patrol _patrol;
    private Pursuit _pursuit;

    public float SearchToPatrolDelay;
    bool stillSearching;
    private void Awake()
    {
        _perry = GetComponent<PerryNav>();
        _perrySensor = GetComponent<PerrySensor>();
        _statemachine = GetComponent<State_Machine>();
        _patrol = GetComponent<Patrol>();
        _pursuit = GetComponent<Pursuit>();
        isActive = false;
        stillSearching = false;
    }
    public override void InitializeState()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.yellow;
        EnteredState.AddListener(gameObject.GetComponent<PerrySensor>().OnSearch);
        ExitedState.AddListener(gameObject.GetComponent<PerrySensor>().OnSearchExit);
        EnteredState.AddListener(gameObject.GetComponent<PerryNav>().OnSearch);
        ExitedState.AddListener(gameObject.GetComponent<PerryNav>().OnSearchExit);
        isActive = true;
        EnteredState.Invoke();

    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        isActive = false;


    }

    public void OnAudioCueHeard()
    {
        stillSearching = true;
    }

    public void OnDistantPlayerSeen()
    {

    }

    public void OnLineOfSightBroken()
    {
        if (isActive)
        {
            //Debug.Log("No longer seeing the player at distance");
            //continue searching for player
            //code for checking destinations will be here, for now returns to patrol
           
        }
    }

    public void OnSearchCompleted()
    {
        if (isActive)
        {
            Debug.Log("All destinations reached.");
            _statemachine.ChangeState(_patrol);
        }
    }
    public void OnClosePlayerSeen()
    {
        if (isActive)
        {
            //change state to pursuit
            _statemachine.ChangeState(_pursuit);
        }
    }
    
    private IEnumerator CheckPlayerRadius()
    {
        yield return new WaitForSeconds(SearchToPatrolDelay);


    }

    private void ComponentAssignment()
    {
        if (gameObject.GetComponent<Pursuit>())
        {
            _pursuit = gameObject.GetComponent<Pursuit>();
        }
        else
        {
            Debug.LogFormat($"{gameObject.name} [Search.cs:ComponentAssignment()] Pursuit component not attached to {gameObject.name}, instancing now.");
            _pursuit = gameObject.AddComponent(typeof(Pursuit)) as Pursuit;
        }

        if (gameObject.GetComponent<Patrol>())
        {
            _patrol = gameObject.GetComponent<Patrol>();
        }
        else
        {
            Debug.LogFormat($"{gameObject.name} [Search.cs:ComponentAssignment()] Patrol component not attached to {gameObject.name}, instancing now.");
            _patrol = gameObject.AddComponent(typeof(Patrol)) as Patrol;

        }


        if (gameObject.GetComponent<State_Machine>())
        {
            _statemachine = gameObject.GetComponent<State_Machine>();
        }
        else
        {
            Debug.LogFormat($"WARNING! {gameObject.name} [Search.cs:ComponentAssignment()] StateMachine component not " +
                $"attached to {gameObject.name} PerryNav.cs should have instanced it.");
        }


        if (gameObject.GetComponent<PerryNav>())
        {
            _perry = gameObject.GetComponent<PerryNav>();
        }
        else
        {
            Debug.LogFormat($"WARNING! {gameObject.name} [Search.cs:ComponentAssignment()] PerryNav component not " +
                $"attached to {gameObject.name}.");
        }


        if (gameObject.GetComponent<PerrySensor>())
        {
            _perrySensor = gameObject.GetComponent<PerrySensor>();
        }
        else
        {
            Debug.LogFormat($"WARNING! {gameObject.name} [Search.cs:ComponentAssignment()] PerrySensor component not " +
                $"attached to {gameObject.name} PerryNav.cs should have instanced it.");
        }

    }
}
