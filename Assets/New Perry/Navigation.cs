using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation : MonoBehaviour
{
    public float PatrolSpeed;
    public float SearchSpeed;
    public float PursuitSpeed;

    public NewStateMachine NewStateMachine;
    public GameObject PointOfInterest;
    public NewPointOfInterest HearingNode;
    public NavMeshAgent NavMeshAgent;

    public List<GameObject> patrolRoute = new List<GameObject>();

    public void Awake()
    {
        NewStateMachine = GetComponent<NewStateMachine>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        NewStateMachine.StateChange.AddListener(OnStateChange);
    }

    public void FixedUpdate()
    {
        //if current state of things is...
        //patrol
        if(NewStateMachine.GetState() == States.PATROL)
        {
            //run patrol coroutine
        }
        //search
        else if(NewStateMachine.GetState() == States.SEARCH)
        {
            //go to search node
        }
        //pursuit
        else{
            //chase player until eye contact has been broken for more than five seconds
        }
        
        if(HearingNode == null){
            NewStateMachine.ChangeState(States.PATROL);
        }
        else
        {
            NewStateMachine.ChangeState(States.SEARCH);
        }

    }

    public void CheckNewNodePriority(Transform sourceTransform, Priority priority)
    {
        if(priority < HearingNode.priority || HearingNode == null)
        {
            NewPointOfInterest oldNode = HearingNode;
            HearingNode = CreateHearingNode(sourceTransform, priority).GetComponent<NewPointOfInterest>();
            oldNode.RemoveNode();
        }
    }

    private GameObject CreateHearingNode(Transform noisePosition, Priority priority)
    {
        var newNode = Instantiate(PointOfInterest, noisePosition.position, transform.rotation, this.gameObject.transform);
        newNode.GetComponent<NewPointOfInterest>().priority = priority;
        newNode.GetComponent<NewPointOfInterest>().Navigation = this;
        return newNode;
    }

    public void OnStateChange(States state)
    {
        if(state == States.PATROL)
        {
            NavMeshAgent.speed = PatrolSpeed;
        }
        if(state == States.SEARCH) {
            NavMeshAgent.speed = SearchSpeed;
        }
        if(state == States.PURSUIT)
        {
            NavMeshAgent.speed = PursuitSpeed;
        }
    }
}
