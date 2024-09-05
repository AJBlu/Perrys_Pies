using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Search : State
{

    Search(PerryNav perry, State_Machine statemachine) : base(perry, statemachine)
    {
        base._perry = perry;
        base._statemachine = statemachine;
    }
    public override void InitializeState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();

    }

    public override void ExitState()
    {
        throw new System.NotImplementedException();
    }
}
