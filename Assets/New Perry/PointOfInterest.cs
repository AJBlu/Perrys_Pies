using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPointOfInterest : MonoBehaviour
{

    public bool isPatrolNode;

    public Priority priority;

    public Transform point_transform;

    public Navigation Navigation;

    public int NodePosition;

    public GameObject nextNode;

    private void Awake()
    {
        point_transform = transform;
        Navigation = GameObject.FindGameObjectWithTag("Perry").GetComponent<Navigation>();
        if (!isPatrolNode)
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
            if (!isPatrolNode)
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

}
