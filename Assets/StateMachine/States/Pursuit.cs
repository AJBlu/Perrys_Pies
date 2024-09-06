using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursuit : State
{

    private Search _search;
    private Patrol _patrol;
    public Pursuit(PerryNav perry, State_Machine statemachine, PerrySensor perrySensor) : base(perry, statemachine, perrySensor)
    {
        base._perry = perry;
        base._statemachine = statemachine;
        base._perrySensor = perrySensor;
    }
    public override void InitializeState()
    {
        EnteredState.AddListener(_perrySensor.OnNewState);
        ExitedState.AddListener(_perrySensor.OnExitState);
        _perrySensor.LineOfSightBroken.AddListener(OnLineOfSightBroken);
        _perrySensor.AudioCueHeard.AddListener(OnAudioCueHeard);
    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();

    }

    public override void ExitState()
    {
        throw new System.NotImplementedException();
    }

    public void OnLineOfSightBroken()
    {

    }

    public void OnAudioCueHeard()
    {

    }

    public void OnDistractionItem()
    {
        //perry gets distracted

        //and then returns to patrolling
        _patrol = new Patrol(_perry, _statemachine, _perrySensor);
        _statemachine.ChangeState(_patrol);
    }
}
