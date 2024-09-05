using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Patrol : State
{
    private bool _isDeaf = false;

    public float TimeDeafened = 5f;

    Patrol(PerryNav perry, State_Machine statemachine) : base(perry, statemachine) {
        base._perry = perry;
        base._statemachine = statemachine;
    }

    public override void InitializeState()
    {
        //implement deafness
        StartCoroutine("deafenPerry");
        //create room nodes


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

}
