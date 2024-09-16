using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class State_Machine : MonoBehaviour
{
    public State _activeState;

    public void FixedUpdate()
    {
        RunActiveState();
    }

    public void ChangeState(State state)
    {      
        if(_activeState)
            _activeState.ExitState();
        _activeState = state;
        _activeState.InitializeState();
    }

    public void RunActiveState()
    {
        _activeState.UpdateState();
    }

}
