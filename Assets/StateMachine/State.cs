using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class State : MonoBehaviour
{
    public PerryNav _perry;
    public State_Machine _statemachine;
    public PerrySensor _perrySensor;

    public UnityEvent EnteredState;
    public UnityEvent ExitedState;

    //What's run whenever the state is entered
    public abstract void InitializeState();


    //State code that runs in the update function
    public abstract void UpdateState();


    //What's run whenever the state has left
    public abstract void ExitState();

}
