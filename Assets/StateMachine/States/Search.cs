using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Search : State
{
    private Patrol _patrol;
    private Pursuit _pursuit;
    private void Awake()
    {
        _perrySensor = GetComponent<PerrySensor>();
        _statemachine = GetComponent<State_Machine>();
    }
    public override void InitializeState()
    {
        EnteredState.AddListener(_perrySensor.OnNewState);
        ExitedState.AddListener(_perrySensor.OnExitState);
        _perrySensor.AudioCueHeard.AddListener(OnAudioCueHeard);
        _perrySensor.SearchCompleted.AddListener(OnSearchCompleted);
        _perrySensor.PlayerSeen_Close.AddListener(OnClosePlayerSeen);

    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        
    }

    public void OnAudioCueHeard()
    {

    }

    public void OnLineOfSightBroken()
    {
        _statemachine.ChangeState(_patrol);
    }

    public void OnSearchCompleted()
    {
        _patrol = GetComponent<Patrol>();
        _statemachine.ChangeState(_patrol);
    }
    public void OnClosePlayerSeen()
    {
        //change state to pursuit
        _pursuit = GetComponent<Pursuit>();
        _statemachine.ChangeState(_pursuit);
    }
}
