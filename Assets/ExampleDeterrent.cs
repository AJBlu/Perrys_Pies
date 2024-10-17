using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExampleDeterrent : MonoBehaviour
{
    public UnityEvent DeterrentItemHit;
    private void Awake()
    {
        StartCoroutine("StartDeterrent");
        var Perry = GameObject.FindGameObjectWithTag("Perry");
        DeterrentItemHit.AddListener(Perry.GetComponent<PerryNav>().OnDeterentItemHit);
    }

    private  IEnumerator StartDeterrent()
    {
        yield return new WaitForSeconds(4f);
        DeterrentItemHit.Invoke();

    }
}
