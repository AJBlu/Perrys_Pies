using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class PointOfInterest : MonoBehaviour
{
    public bool willBeSearched = false;
    private PatrolManager _patrolManager;
    private void Awake()
    {
        _patrolManager = GameObject.Find("PatrolManager").GetComponent<PatrolManager>(); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Perry")
        {
            _patrolManager.SearchNodes.Remove(this.transform);
            Destroy(this.gameObject);
            
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 1f);
    }
}
