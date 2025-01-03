using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PatrolManager : MonoBehaviour
{
    public GameObject HearingNode;

    [Tooltip("Hearing radius for running, sprinting, and other audio cues.")]
    public float PerryHearingRadius;

    [Tooltip("Hearing radius for walking.")]
    public float PerryWalkHearingRadius;

    public GameObject Perry;
    public GameObject Player;
    public List<Transform> PatrolNodes = new List<Transform>();

    private NavMeshAgent _perryAgent;
    private Patrol _patrol;
    private Search _search;

    private bool _isPathfinding;
    private int _nextNode;
    private void Awake()
    {
        GameObject.FindGameObjectWithTag("Player");
        GameObject.FindGameObjectWithTag("Perry");
        _perryAgent = Perry.GetComponent<NavMeshAgent>();
        _patrol = Perry.GetComponent<Patrol>();
        _search = Perry.GetComponent<Search>();
        _nextNode = GetClosestPatrolNode(PatrolNodes);
        Perry.GetComponent<PerrySensor>().AudioCueHeard.AddListener(OnAudioCueHeard);
        Player.GetComponent<DemoPlayerController>().PlayerJump.AddListener(OnPlayerJump);
        Player.GetComponent<DemoPlayerController>().PlayerSprint.AddListener(OnPlayerSprint);


    }

    public void Update()
    {
        if (!_isPathfinding && _search.isActive)
            StartCoroutine("SearchRoute");
        if (!_isPathfinding && _patrol.isActive)
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
        if (_search.isActive)
        {
            if (!_perryAgent.hasPath)
            {
                if (HearingNode != null)
                {
                    Debug.Log("Setting Destination");
                    _perryAgent.SetDestination(HearingNode.transform.position);
                }
                else
                {
                    Debug.Log("No available hearing node. Returning to patrol.");
                }
            }
        }
        _isPathfinding = false;
        yield return null;
    }



    public void OnAudioCueHeard()
    {
        _perryAgent.SetDestination(HearingNode.transform.position);
    }

    public void OnPlayerJump()
    {

    }

    public void OnPlayerSprint()
    {

    }
}
