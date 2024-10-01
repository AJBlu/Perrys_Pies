using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PatrolManager : MonoBehaviour
{
    public GameObject Perry;
    public List<Transform> PatrolNodes = new List<Transform>();
    public List<Transform> SearchNodes = new List<Transform>();

    private NavMeshAgent _perryAgent;
    private Patrol _patrol;
    private Search _search;

    private bool _isPathfinding;
    private int _nextNode;
    private void Awake()
    {
        if (Perry == null)
            GameObject.Find("Perry");

        _perryAgent = Perry.GetComponent<NavMeshAgent>();
        _patrol = Perry.GetComponent<Patrol>();
        _search = Perry.GetComponent<Search>();
        _nextNode = GetClosestPatrolNode(PatrolNodes);
        Perry.GetComponent<PerrySensor>().AudioCueHeard.AddListener(OnAudioCueHeard);
    }

    public void Update()
    {
        if (!_isPathfinding && _search.isActive && SearchNodes.Count > 0)
            StartCoroutine("SearchRoute");
        if (!_isPathfinding && _patrol.isActive && PatrolNodes.Count > 0)
            StartCoroutine("PatrolRoute");



    }
    public int GetClosestPatrolNode(List<Transform> nodeList)
    {
        int targetNode = 0;
        float distance = float.MaxValue;
        for(int i = 0; i < nodeList.Count; i++)
        {
            float _perryDistance = Vector3.Distance(Perry.transform.position, nodeList[i].position);
            if (distance > _perryDistance)
            {
                distance = _perryDistance;
            }
            targetNode = i;
        }

        return targetNode;
    }



    public IEnumerator PatrolRoute()
    {
        _isPathfinding = true;
        if (_patrol.isActive)
        {
            if (!_perryAgent.hasPath)
            {

                Debug.Log("Setting Destination");
                _perryAgent.SetDestination(PatrolNodes[_nextNode].position);
                _nextNode++;
            }

            if (_nextNode == PatrolNodes.Count)
                _nextNode = 0;

        }
        _isPathfinding = false;
        yield return null;
    }

    public IEnumerator SearchRoute()
    {
        _isPathfinding = true;

        //this could probably fuck things up
        _nextNode = GetClosestPatrolNode(SearchNodes);
        if (_search.isActive)
        {
            if (!_perryAgent.hasPath)
            {

                Debug.Log("Setting Destination");
                _perryAgent.SetDestination(PatrolNodes[_nextNode].position);
                _nextNode++;
            }

            if (_nextNode == PatrolNodes.Count)
                _nextNode = 0;

        }
        _isPathfinding = false;
        yield return null;
    }

    public void OnAudioCueHeard()
    {

    }

}
