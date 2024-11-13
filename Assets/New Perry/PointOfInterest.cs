using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NewPointOfInterest : MonoBehaviour
{

    public bool isPatrolNode;

    public bool isTrappedNode;

    public Priority priority;

    public Transform point_transform;

    public Navigation Navigation;

    public int NodePosition;

    public GameObject nextNode;

    public float TrapDuration;

    private NavMeshAgent NMA;

    private void Awake()
    {
        point_transform = transform;
        Navigation = GameObject.FindGameObjectWithTag("Perry").GetComponent<Navigation>();
        if (!isPatrolNode || !isTrappedNode)
        {
            StartCoroutine(RelevanceCountdown());
            NodePosition = 99;
        }
    }
    public void RemoveNode()
    {
        if(this == Navigation.HearingNode)
            Navigation.HearingNode = null;
        StartCoroutine(DeleteThis());

    }

    private IEnumerator DeleteThis()
    {
        yield return new WaitForSeconds(3f);
        Destroy(this.gameObject);
    }

    private IEnumerator RelevanceCountdown()
    {
        yield return new WaitForSeconds(10f);
        Destroy(this.gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.tag == "Perry")
        {
            if (isTrappedNode)
            {
                StartCoroutine(Trapped(TrapDuration));
            }
            else if (!isPatrolNode || !isTrappedNode)
            {
                Debug.Log("Collided with Perry. Deleting node.");
                RemoveNode();
            }
            else 
            {

                    Debug.Log("Collided with Perry. Going to next node in list.");
                    Navigation.TargetNode = nextNode;
                    Navigation.reachedNode = true;
                
            }
        }
    }

    public IEnumerator Trapped(float TrapDuration)
    {
        NewStateMachine nsm = GameObject.FindGameObjectWithTag("Perry").GetComponent<NewStateMachine>();
        nsm.ChangeState(States.TRAPPED);
        yield return new WaitForSeconds(TrapDuration);
        nsm.UntrapPerry(States.PATROL);
        RemoveNode();
    }

}
