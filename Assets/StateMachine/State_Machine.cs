using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class State_Machine : MonoBehaviour
{
    public State _activeState;

    private void Awake()
    {

    }

    public void ChangeState(State state)
    {
        if(_activeState)
            _activeState.ExitState();
        _activeState = state;
        _activeState.InitializeState();
    }


}
