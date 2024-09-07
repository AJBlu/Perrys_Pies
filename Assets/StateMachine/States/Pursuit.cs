using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursuit : State
{

    private Patrol _patrol;
    private void Awake()
    {
        _perrySensor = GetComponent<PerrySensor>();
        _statemachine = GetComponent<State_Machine>();
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

    }

    public override void ExitState()
    {
        ExitedState.Invoke();
    }

    public void OnLineOfSightBroken()
    {
        _patrol = GetComponent<Patrol>();
        _statemachine.ChangeState(_patrol);
    }

    public void OnAudioCueHeard()
    {

    }

    public void OnDistractionItem()
    {
        //perry gets distracted

        //and then returns to patrolling
        _patrol = GetComponent<Patrol>();
        _statemachine.ChangeState(_patrol);
    }
}
