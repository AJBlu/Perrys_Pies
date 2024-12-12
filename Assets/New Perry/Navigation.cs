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

    public float MinNodeDistance;

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
    public AudioSource dragging;
    public AudioSource dragging_chase;

    public Animator ASM;


    // Audio Source Maximum Distance Variable
    public float MaxAudioDist = 3f;
    public void Awake()
    {
        NewStateMachine = GetComponent<NewStateMachine>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Player = GameObject.FindGameObjectWithTag("Player");
        NewStateMachine.StateChange.AddListener(OnStateChange);
        gameOverCollider = GameObject.Find("GameOverCollider").GetComponent<GameOverCollider>();
        gameOverCollider.GameOver.AddListener(OnGameOver);
        caughtPlayer = false;

        // Setting the maximum distance value on Awake
        dragging.maxDistance = MaxAudioDist;
        dragging_chase.maxDistance = MaxAudioDist;
    }

    public void FixedUpdate()
    {

        if (NavMeshAgent.speed > 0)
        {
            ASM.SetBool("IsMoving", true);
            if (NewStateMachine.currentState != States.PURSUIT)
            {
                ASM.SetBool("IsPursuing", false);
                dragging_chase.enabled = false;
                dragging.enabled = true;
            }
            else
            {
                ASM.SetBool("IsPursuing", true);
                dragging.enabled = false;
                dragging_chase.enabled = true;
            }

            if (caughtPlayer)
            {
                Debug.Log("Lose screen activated");
                var trash = GameObject.Find("UIManager").GetComponent<UIManager>();
                trash.activateLossText();
            }

        }
        else
        {
            ASM.SetBool("IsMoving", false);
            dragging.enabled = false;
            dragging_chase.enabled = false;
        }
        if (!caughtPlayer || !stopMoving)
        {
            //if current state of things is...
            //patrol
            if (NewStateMachine.GetState() == States.PATROL)
            {
                if (!isPatrolling)
                {
                    Debug.Log("Starting Patrol Routine");
                    //run patrol coroutine
                    //
                    //StartCoroutine(PatrolRoute());
                    NavMeshAgent.SetDestination(TargetNode.gameObject.transform.position);
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
                gameObject.transform.LookAt(HearingNode.transform.position);
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
                //isPatrolling = false;
                isSearching = false;
                if (HearingNode == null && NewStateMachine.GetState() != States.PATROL)
                {   
                    if(NewStateMachine.currentState == States.TRAPPED) { NewStateMachine.UntrapPerry(States.PATROL); }
                    //NavMeshAgent.ResetPath();
                    NavMeshAgent.speed = PatrolSpeed;
                    Debug.Log("No more hearing nodes. Going to Patrol state.");
                    NewStateMachine.ChangeState(States.PATROL);
                    isPatrolling = false;
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
        Debug.Log("Patrol route starting.");
        NavMeshAgent.ResetPath();
        if (TargetNode != null)
        {
            Debug.Log("Setting destination here.");
            NavMeshAgent.SetDestination(TargetNode.transform.position);

        }

        if (NavMeshAgent.hasPath)
        {
            isPatrolling = true;
        }

        yield return null;

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
            GameObject newNode = CreateHearingNode(sourceTransform, priority);
            NewPointOfInterest newNodePOI = newNode.GetComponent<NewPointOfInterest>();
            NewPointOfInterest oldNode = HearingNode;
            if(newNode != null)
            {
                HearingNode = newNodePOI;
            }
            oldNode.RemoveNode();
        }

    }

    private GameObject CreateHearingNode(Transform noisePosition, Priority priority)
    {
        GameObject newNode = null;
        if (HearingNode == null)
        {
            newNode = Instantiate(PointOfInterest, noisePosition.position, transform.rotation);
            newNode.GetComponent<NewPointOfInterest>().priority = priority;
            newNode.GetComponent<NewPointOfInterest>().Navigation = this;

        }
        else if(Vector3.Distance(noisePosition.position, HearingNode.transform.position) < MinNodeDistance)
        {
            Debug.Log("Node not found an appropriate distance away.");

        }
        else if(priority == Priority.ATTRACTANT)
        {
            newNode = Instantiate(PointOfInterest, noisePosition.position, transform.rotation);
            newNode.GetComponent<NewPointOfInterest>().priority = priority;
            newNode.GetComponent<NewPointOfInterest>().Navigation = this;
            newNode.GetComponent<NewPointOfInterest>().isTrappedNode = true;
            newNode.GetComponent<NewPointOfInterest>().TrapDuration = 5f;
        }

        return newNode;
    }

    public void OnStateChange(States state)
    {
        if(state == States.PATROL)
        {
            ASM.SetBool("IsPursuing", false);
            isPatrolling = false;
            NavMeshAgent.speed = PatrolSpeed;
        }
        if(state == States.SEARCH) {
            ASM.SetBool("IsPursuing", false);

            NavMeshAgent.speed = SearchSpeed;
        }
        if(state == States.PURSUIT)
        {
            ASM.SetBool("IsPursuing", true);
            NavMeshAgent.speed = PursuitSpeed;
        }
        if(state == States.TRAPPED)
        {
            ASM.SetBool("IsPursuing", true);
            NavMeshAgent.speed = PursuitSpeed;
        }
    }

    private void OnGameOver()
    {
        NewStateMachine.ChangeState(States.PATROL);
    }
}
