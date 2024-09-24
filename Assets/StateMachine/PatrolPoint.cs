using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    public Transform nextPoint;

    private void OnTriggerEnter(Collider other)
    {
        /*
        if (other.tag == "Perry")
        {
            if (nextPoint != null)
            {
                other.gameObject.GetComponent<PerryNav>().NavMeshAgent.SetDestination(nextPoint.position);
            }
            else
            {
                other.gameObject.GetComponent<PerryNav>().GetClosestPatrolNode();
            }
        }
        */
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 1f);
    }
}
