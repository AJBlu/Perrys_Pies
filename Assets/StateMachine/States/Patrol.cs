using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Patrol : State
{
    private bool _isDeaf = false;
    public float TimeDeafened = 5f;
    private Search _search;
    private Pursuit _pursuit;

    public Patrol(PerryNav perry, State_Machine statemachine, PerrySensor perrySensor) : base(perry, statemachine, perrySensor) {
        base._perry = perry;
        base._statemachine = statemachine;
        base._perrySensor = perrySensor;
    }

    public override void InitializeState()
    {
        //implement deafness
        StartCoroutine("deafenPerry");
        //create room nodes
        EnteredState.AddListener(_perrySensor.OnNewState);
        ExitedState.AddListener(_perrySensor.OnExitState);
        _perrySensor.PlayerSeen_Distant.AddListener(OnDistantPlayerSeen);
        _perrySensor.PlayerSeen_Close.AddListener(OnClosePlayerSeen);
        EnteredState.Invoke();


    }

    public override void UpdateState()
    {
        if (!_isDeaf)
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
        //clear explored room nodes
        

    }

    private IEnumerator deafenPerry()
    {
        _isDeaf = true;
        yield return new WaitForSeconds(TimeDeafened);
        _isDeaf = false;
    }

    public void OnDistantPlayerSeen()
    {
        //change state to search
        _search = new Search(_perry, _statemachine, _perrySensor);
        _statemachine.ChangeState(_search);
    }

    public void OnClosePlayerSeen()
    {
        //change state to pursuit
        _pursuit = new Pursuit(_perry, _statemachine, _perrySensor);
        _statemachine.ChangeState(_pursuit);
    }
}
