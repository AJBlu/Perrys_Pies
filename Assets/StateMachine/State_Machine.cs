using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Machine : MonoBehaviour
{
    State _activeState;

    public void ChangeState(State state)
    {
        _activeState.ExitState();
        _activeState = state;
        _activeState.InitializeState();
    }


}
