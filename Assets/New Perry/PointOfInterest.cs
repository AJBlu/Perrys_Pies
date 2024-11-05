using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPointOfInterest : MonoBehaviour
{
    public Priority priority;

    public Transform point_transform;

    public Navigation Navigation;

    private void Awake()
    {
        point_transform = transform;
        Navigation = GameObject.FindGameObjectWithTag("Perry").GetComponent<Navigation>();
        StartCoroutine(RelevanceCountdown());
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
            Debug.Log("Collided with Perry. Deleting node.");
            RemoveNode();
        }
    }

}
