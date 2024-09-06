using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Search : State
{
    private Patrol _patrol;
    private Pursuit _pursuit;
    public Search(PerryNav perry, State_Machine statemachine, PerrySensor perrySensor) : base(perry, statemachine, perrySensor)
    {
        base._perry = perry;
        base._statemachine = statemachine;
        base._perrySensor = perrySensor;
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

    public void OnSearchCompleted()
    {
        _patrol = new Patrol(_perry, _statemachine, _perrySensor);
        _statemachine.ChangeState(_patrol);
    }
    public void OnClosePlayerSeen()
    {
        //change state to pursuit
        _pursuit = new Pursuit(_perry, _statemachine, _perrySensor);
        _statemachine.ChangeState(_pursuit);
    }
}
