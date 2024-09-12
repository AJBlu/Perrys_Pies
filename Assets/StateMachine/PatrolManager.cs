using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PatrolManager : MonoBehaviour
{
    public GameObject Perry;
    public Transform[] nodes;

    public NavMeshAgent _perryAgent;
    public Patrol _patrol;

    private bool _isPathfinding;
    private int _nodePlayerIsApproaching;
    private void Awake()
    {
        if (Perry == null)
            GameObject.Find("Perry");

        _perryAgent = Perry.GetComponent<NavMeshAgent>();
        _patrol = Perry.GetComponent<Patrol>();
        _nodePlayerIsApproaching = 0;
    }

    public void Update()
    {
        if(!_isPathfinding && _patrol.isActive)
            StartCoroutine("PatrolRoute");

    }
    public void GetClosestPatrolNode()
    {

    }

    public IEnumerator PatrolRoute()
    {
        _isPathfinding = true;
        if (_patrol.isActive)
        {
            if (!_perryAgent.hasPath)
            {

                Debug.Log("Setting Destination");
                _perryAgent.SetDestination(nodes[_nodePlayerIsApproaching].position);
                _nodePlayerIsApproaching++;
            }

            if (_nodePlayerIsApproaching == nodes.Length)
                _nodePlayerIsApproaching = 0;

        }
        _isPathfinding = false;
        yield return null;
    }

}
