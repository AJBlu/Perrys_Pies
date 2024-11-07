using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Patrol : State
{
    public bool isDeaf = false;
    public float TimeDeafened = 10f;
    private Search _search;
    private Pursuit _pursuit;


    private void Awake()
    {
        //wrapper for assignment
        ComponentAssignment();

        //added in awake so entered and exited state can be actually invoked

    }
    public override void InitializeState()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.green;
        EnteredState.AddListener(gameObject.GetComponent<PerryNav>().OnPatrol);
        EnteredState.AddListener(gameObject.GetComponent<PerrySensor>().OnPatrol);
        ExitedState.AddListener(gameObject.GetComponent<PerryNav>().OnPatrolExit);
        ExitedState.AddListener(gameObject.GetComponent<PerrySensor>().OnPatrolExit);
        //implement deafness
        StartCoroutine("deafenPerry");
        //add entered and exited states to Nav and Sensor

        EnteredState.Invoke();
        isActive = true;


    }

    public override void UpdateState()
    {
        if (!isDeaf)
        {
            //audio cue code goes here
        }

        //idle pathing code goes here
            //go to closest random unexplored node
        

        //eyesight code goes here
            //if distant eyesight range sees player, change state to pursue
            
            //if close eyesight range sees player, change state to pursue

    }

    public override void ExitState()
    {
        ExitedState.Invoke();
        EnteredState.RemoveListener(gameObject.GetComponent<PerryNav>().OnPatrol);
        EnteredState.RemoveListener(gameObject.GetComponent<PerrySensor>().OnPatrol);
        ExitedState.RemoveListener(gameObject.GetComponent<PerryNav>().OnPatrolExit);
        ExitedState.RemoveListener(gameObject.GetComponent<PerrySensor>().OnPatrolExit);
        isActive = false;

    }

    private IEnumerator deafenPerry()
    {
        isDeaf = true;
        yield return new WaitForSeconds(TimeDeafened);
        isDeaf = false;
    }

    public void OnDistantPlayerSeen()
    {
        if (isActive)
        {
            if (_perry.DebugEnabled)
                Debug.LogFormat($"{gameObject.name} [Patrol.cs:OnDistantPlayerSeen()] Seeing player from afar in patrol state.");
            //change state to search
            _statemachine.ChangeState(_search);
        }

    }

    public void OnClosePlayerSeen()
    {
        if (isActive)
        {
            if(_perry.DebugEnabled)
                Debug.LogFormat($"{gameObject.name} [Patrol.cs:OnClosePlayerSeen()] Seeing player from patrol state.");
            //change state to pursuit
            _statemachine.ChangeState(_pursuit);
        }
    }
    public void OnAudioCueHeard()
    {
        if (isActive)
        {
            if(!isDeaf)
            {
                if (_perry.DebugEnabled)
                    Debug.LogFormat($"{gameObject.name} [Patrol.cs:OnAudioCueHeard()] Hearing audio from patrol state.");
                _statemachine.ChangeState(_search);
            }
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
            Debug.LogFormat($"{gameObject.name} [Patrol.cs:ComponentAssignment()] Search component not attached to {gameObject.name}, instancing now.");
            _search = gameObject.AddComponent(typeof(Search)) as Search;
        }

        if (gameObject.GetComponent<Pursuit>())
        {
            _pursuit = gameObject.GetComponent<Pursuit>();
        }
        else
        {
            Debug.LogFormat($"{gameObject.name} [Patrol.cs:ComponentAssignment()] Pursuit component not attached to {gameObject.name}, instancing now.");
            _pursuit = gameObject.AddComponent(typeof(Pursuit)) as Pursuit;

        }


        if (gameObject.GetComponent<State_Machine>())
        {
            _statemachine = gameObject.GetComponent<State_Machine>();
        }
        else
        {
            Debug.LogFormat($"WARNING! {gameObject.name} [Patrol.cs:ComponentAssignment()] StateMachine component not " +
                $"attached to {gameObject.name} PerryNav.cs should have instanced it.");
        }


        if (gameObject.GetComponent<PerryNav>())
        {
            _perry = gameObject.GetComponent<PerryNav>();
        }
        else
        {
            Debug.LogFormat($"WARNING! {gameObject.name} [Patrol.cs:ComponentAssignment()] PerryNav component not " +
                $"attached to {gameObject.name}.");
        }


        if (gameObject.GetComponent<PerrySensor>())
        {
            _perrySensor = gameObject.GetComponent<PerrySensor>();
        }
        else
        {
            Debug.LogFormat($"WARNING! {gameObject.name} [Patrol.cs:ComponentAssignment()] PerrySensor component not " +
                $"attached to {gameObject.name} PerryNav.cs should have instanced it.");
        }

    }

}
