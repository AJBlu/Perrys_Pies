using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


//fires off unity events to change states
public class PerrySensor : MonoBehaviour
{
    public UnityEvent PlayerSeen_Distant;
    public UnityEvent PlayerSeen_Close;
    public UnityEvent AudioCueHeard;
    public UnityEvent LineOfSightBroken;
    public UnityEvent SearchCompleted;


    public void OnNewState()
    {
    }

    public void OnExitState()
    {

    }


}
