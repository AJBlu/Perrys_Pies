using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathfindingBasic : MonoBehaviour
{
    private GameObject _destination;
    private NavMeshAgent _agent;

    public float sightRadius;

    public float soundRadius;


 

    private void Start()
    {
        _destination = GameObject.FindGameObjectWithTag("destination");
        _agent = gameObject.GetComponent<NavMeshAgent>();
        _agent.SetDestination(_destination.transform.position);
    }
}
