using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExampleDeterrent : MonoBehaviour
{
    public float StopDuration;

    public float DeterrentDelay;

    public float DeterrentRadius;

    public UnityEvent<float> DeterrentItemHit;
    private void Awake()
    {
        StartCoroutine("StartDeterrent");
    }

    private  IEnumerator StartDeterrent()
    {
        var Perry = GameObject.FindGameObjectWithTag("Perry");
        DeterrentItemHit.AddListener(Perry.GetComponent<PerryNav>().OnDeterrentItemHit);
        yield return new WaitForSeconds(DeterrentDelay);
        DeterrentItemHit.Invoke(StopDuration);
        DeterrentItemHit.RemoveListener(Perry.GetComponent<PerryNav>().OnDeterrentItemHit);

    }
}
