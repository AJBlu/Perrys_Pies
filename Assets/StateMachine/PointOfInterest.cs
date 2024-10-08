using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class PointOfInterest : MonoBehaviour
{
    private PatrolManager _patrolManager;
    public bool willBeSearched = false;
    public Priority priority;

    private void Awake()
    {
        _patrolManager = GameObject.Find("PatrolManager").GetComponent<PatrolManager>(); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Perry")
        {
            _patrolManager.SearchNodes.Remove(this.transform);
            StartCoroutine("DestroyThis");
            
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 1f);
    }

    private IEnumerator DestroyThis()
    {
        yield return new WaitForSeconds(5.0f);
        gameObject.SetActive(false);
    }
}
