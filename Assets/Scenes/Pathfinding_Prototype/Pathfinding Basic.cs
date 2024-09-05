using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PathfindingBasic : MonoBehaviour
{
    private float _withinSightCone = .707f;
    private GameObject _destination;
    private NavMeshAgent _agent;
    private List<DestinationNode> _destinations;

    public float sightRadius_Suspicious;
    public float sightRadius_Chase;


    bool isRunning = false;


    private void Awake()
    {
        _destinations = new List<DestinationNode>();
        //sample code for finding single destination
        _destination = GameObject.FindGameObjectWithTag("destination");
        _agent = gameObject.GetComponent<NavMeshAgent>();
        _agent.SetDestination(_destination.transform.position);
        StartCoroutine("refreshDestination");
    }

    private void FixedUpdate()
    {
        if (!isRunning)
        {
            StartCoroutine("refreshDestination");
        }
    }


    private IEnumerator refreshDestination()
    {
        isRunning = true;
        _agent.SetDestination(_destination.transform.position);
        yield return new WaitForSeconds(1);
        _agent.SetDestination(_destination.transform.position);
        isRunning = false;

    }
    

    //returns whether or not object is in sphere determined by detection radius
    private bool _isEntityDetectable(float radius, GameObject entity)
    {
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, radius);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].tag == entity.tag)
            {
                return true;
            }
        }

        return false;
    }

    //return whether or not object is hit by raycast determined by detection radius
    private bool _isEntityInSight(float radius, GameObject entityTag)
    {
        //is object within 90 degrees
        if(Vector3.Dot(gameObject.transform.position, entityTag.transform.position) > _withinSightCone)
        {
            RaycastHit hit;
            //send raycast out to player
            if(Physics.Raycast(transform.position, entityTag.transform.position, out hit, radius))
            {
                //successful hit, depending on radius update alert level and create a destination node or start pursuit
                if(radius == sightRadius_Suspicious)
                {
                    //update alert level to investigate
                    //create node on last seen player position
                }
                else if(radius == sightRadius_Chase)
                {
                    //update alert level to chasing
                    //create pursuit function
                }
                else
                {
                    Debug.Log("Something weird happened; sightRadius not used for line of sight!");
                }
            }
        }
        return false;
    }


    //add node to destinations list, then see if there's a higher priority node
    private void _addNode(DestinationNode node)
    {
        _destinations.Append(node);
        _getPriorityDestination();

    }

    //pull node with highest priority as part of the destinations list
    private void _getPriorityDestination()
    {
        foreach(DestinationNode node in _destinations)
        {

        }
    }

    
    
}
