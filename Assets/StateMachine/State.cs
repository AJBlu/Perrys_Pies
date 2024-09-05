using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class State : MonoBehaviour
{
    protected PerryNav _perry;
    protected State_Machine _statemachine;

    public UnityEvent EnteredState;
    public UnityEvent ExitedState;

    public State (PerryNav perry, State_Machine statemachine)
    {
        this._perry = perry;
        this._statemachine = statemachine;
    }

    //What's run whenever the state is entered
    public abstract void InitializeState();


    //State code that runs in the update function
    public abstract void UpdateState();


    //What's run whenever the state has left
    public abstract void ExitState();

}
