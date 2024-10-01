using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class PointOfInterest : MonoBehaviour
{
    public bool willBeSearched = false;
    private PerryNav _pn;
    private float _hearingRadius;
    private void Awake()
    {
        _pn = GameObject.FindGameObjectWithTag("Perry").GetComponent<PerryNav>();
        _hearingRadius = GameObject.FindGameObjectWithTag("Perry").GetComponent<PerrySensor>().HearingRadius;
        if (Vector3.Distance(this.transform.position, _pn.gameObject.transform.position) < _hearingRadius)
            _pn.searchThese.Add(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Perry")
        {
            _pn.searchThese.Remove(this.gameObject);
            Destroy(this.gameObject);
            
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 1f);
    }
}
