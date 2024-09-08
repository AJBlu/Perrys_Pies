using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Search : State
{
    private Patrol _patrol;
    private Pursuit _pursuit;

    public float SearchToPatrolDelay;
    bool stillSearching;
    bool playerEyeContact;
    private void Awake()
    {
        _perry = GetComponent<PerryNav>();
        _perrySensor = GetComponent<PerrySensor>();
        _statemachine = GetComponent<State_Machine>();
        _patrol = GetComponent<Patrol>();
        _pursuit = GetComponent<Pursuit>();
        isActive = false;
        stillSearching = false;
        playerEyeContact = false;
    }
    public override void InitializeState()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.yellow;

        EnteredState.AddListener(_perrySensor.OnNewState);
        ExitedState.AddListener(_perrySensor.OnExitState);
        isActive = true;


    }

    public override void UpdateState()
    {
        if(!stillSearching && !playerEyeContact)
        {
            _statemachine.ChangeState(_patrol);
        }
    }

    public override void ExitState()
    {
        isActive = false;
    }

    public void OnAudioCueHeard()
    {
        stillSearching = true;
    }

    public void OnLineOfSight()
    {
        playerEyeContact = true;
    }

    public void OnLineOfSightBroken()
    {
        if (isActive)
        {
            Debug.Log("No longer seeing the player at distance");
            playerEyeContact = false;
            //continue searching for player
            //code for checking destinations will be here, for now returns to patrol
        }
    }

    public void OnSearchCompleted()
    {
        if (isActive)
        {
            Debug.Log("All destinations reached.");
            //StartCoroutine("CheckPlayerRadius");
            stillSearching = false;
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

}
