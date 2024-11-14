using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Navigation : MonoBehaviour
{
    public bool stopMoving;
    public bool caughtPlayer;
    public bool isPatrolling;
    public bool isSearching;
    public bool reachedNode;
    public GameObject TargetNode;

    public float PatrolSpeed;
    public float SearchSpeed;
    public float PursuitSpeed;

    public NewStateMachine NewStateMachine;
    public GameObject PointOfInterest;
    public GameObject Player;
    public NewPointOfInterest HearingNode;
    public NavMeshAgent NavMeshAgent;

    public List<GameObject> patrolNodes = new List<GameObject>();

    private GameOverCollider gameOverCollider;

    public void Awake()
    {
        NewStateMachine = GetComponent<NewStateMachine>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Player = GameObject.FindGameObjectWithTag("Player");
        NewStateMachine.StateChange.AddListener(OnStateChange);
        gameOverCollider = GameObject.Find("GameOverCollider").GetComponent<GameOverCollider>();
        gameOverCollider.GameOver.AddListener(OnGameOver);
        caughtPlayer = false;
    }

    public void FixedUpdate()
    {
        if (!caughtPlayer || !stopMoving)
        {
            //if current state of things is...
            //patrol
            if (NewStateMachine.GetState() == States.PATROL)
            {
                if (!isPatrolling)
                {
                    //run patrol coroutine
                    StartCoroutine(PatrolRoute());
                }
            }
            //search
            else if (NewStateMachine.GetState() == States.SEARCH)
            {
                //stopMoving = false;
                //go to search node
                if (!NavMeshAgent.hasPath)
                {
                    NavMeshAgent.SetDestination(HearingNode.gameObject.transform.position);
                }

            }
            else if (NewStateMachine.GetState() == States.TRAPPED)
            {
                Debug.Log("Going to attractant node.");
                if (!NavMeshAgent.hasPath)
                {
                    NavMeshAgent.SetDestination(HearingNode.gameObject.transform.position);
                }
            }
            //pursuit
            else
            {
                //stopMoving = true;
                //chase player until eye contact has been broken for more than five seconds
                NavMeshAgent.SetDestination(Player.transform.position);
            }

            //pursuit has to be broken out of by its own coroutine. otherwise, patrol until there's a hearing node, then search until there isn't a hearing node
            if (NewStateMachine.GetState() != States.PURSUIT)
            {
                isPatrolling = false;
                isSearching = false;
                if (HearingNode == null && NewStateMachine.GetState() != States.PATROL)
                {   
                    if(NewStateMachine.currentState == States.TRAPPED) { NewStateMachine.UntrapPerry(States.PATROL); }
                    NavMeshAgent.ResetPath();
                    Debug.Log("No more hearing nodes. Going to Patrol state.");
                    NewStateMachine.ChangeState(States.PATROL);
                    isPatrolling = true;
                }
                else if(HearingNode != null && NewStateMachine.GetState() != States.SEARCH && NewStateMachine.currentState != States.TRAPPED)
                {
                    NavMeshAgent.ResetPath();
                    Debug.Log("Hearing node found. Going to Search state.");
                    NewStateMachine.ChangeState(States.SEARCH);
                    //nodes that require Perry to sprint to location instead of usual search speed
                }
            }
        }
    }

    public IEnumerator PatrolRoute()
    {
        isPatrolling = true;
        if (!NavMeshAgent.hasPath && TargetNode != null)
            NavMeshAgent.SetDestination(TargetNode.transform.position
                );
        yield return null;
        isPatrolling = false;

    }

    public void CheckNewNodePriority(Transform sourceTransform, Priority priority)
    {
        if(HearingNode == null)
        {
            Debug.Log("Creating hearing node, no other node found.");
            HearingNode = CreateHearingNode(sourceTransform, priority).GetComponent<NewPointOfInterest>();
        }
        else if(priority <= HearingNode.priority)
        {
            Debug.Log("Higher priority node or newer node of same priority, replacing old node.");
            NewPointOfInterest oldNode = HearingNode;
            HearingNode = CreateHearingNode(sourceTransform, priority).GetComponent<NewPointOfInterest>();
            oldNode.RemoveNode();
        }

    }

    private GameObject CreateHearingNode(Transform noisePosition, Priority priority)
    {
        var newNode = Instantiate(PointOfInterest, noisePosition.position, transform.rotation);
        newNode.GetComponent<NewPointOfInterest>().priority = priority;
        newNode.GetComponent<NewPointOfInterest>().Navigation = this;
        if(priority == Priority.ATTRACTANT)
        {
            newNode.GetComponent<NewPointOfInterest>().isTrappedNode = true;
            newNode.GetComponent<NewPointOfInterest>().TrapDuration = 5f;
<<<<<<< Updated upstream
=======
            NavMeshAgent.ResetPath();
            NewStateMachine.ChangeState(States.TRAPPED);

>>>>>>> Stashed changes
        }
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
        if(state == States.TRAPPED)
        {
            NavMeshAgent.speed = PursuitSpeed;
<<<<<<< Updated upstream
=======
            //Empty state designed to make Perry run to whatever
>>>>>>> Stashed changes
        }
    }

    private void OnGameOver()
    {
        NewStateMachine.ChangeState(States.PATROL);
    }
}
