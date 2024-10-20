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
        //remove this when deterrent is attached to actual deterrent
        OnDeterrentThrown();
    }

    //to be triggered when deterrent is thrown 
    public void OnDeterrentThrown()
    {
        StartCoroutine(activateDeterrent());
    }

    private IEnumerator activateDeterrent()
    {
        yield return new WaitForSeconds(DeterrentDelay);
        Collider[] objectsInRadius = Physics.OverlapSphere(this.transform.position, DeterrentRadius);

        foreach (Collider collider in objectsInRadius)
        {
            if (collider.tag == "Perry")
            {

                StartDeterrent();
                //changes color on deterrent
                collider.gameObject.GetComponent<Renderer>().material.color = Color.magenta;

            }
        }
    }

    private  void StartDeterrent()
    {
        var Perry = GameObject.FindGameObjectWithTag("Perry");
        DeterrentItemHit.AddListener(Perry.GetComponent<PerryNav>().OnDeterrentItemHit);
        DeterrentItemHit.Invoke(StopDuration);
        DeterrentItemHit.RemoveListener(Perry.GetComponent<PerryNav>().OnDeterrentItemHit);

    }
}
