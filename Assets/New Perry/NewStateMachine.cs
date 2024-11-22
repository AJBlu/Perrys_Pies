using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class NewStateMachine : MonoBehaviour
{
    public UnityEvent<States> StateChange;
    public States currentState;

    public States GetState() { return currentState; }

    public void ChangeState(States newState)
    {
        if (currentState != States.TRAPPED)
        {
            currentState = newState;
            StateChange.Invoke(currentState);
        }
    }

    public void UntrapPerry(States untrapState)
    {
        currentState = untrapState;
    }
}
