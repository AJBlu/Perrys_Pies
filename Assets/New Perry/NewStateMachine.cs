using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class NewStateMachine : MonoBehaviour
{
    public UnityEvent<States> StateChange;
    States currentState;

    public States GetState() { return currentState; }

    public void ChangeState(States newState)
    {
        currentState = newState;
        StateChange.Invoke(currentState);
    }
}
