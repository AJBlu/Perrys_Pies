using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class PointOfInterest : MonoBehaviour
{
    public bool willBeSearched = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Perry")
        {

            //Destroy(this.gameObject);
            
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 1f);
    }
}
