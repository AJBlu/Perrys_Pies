using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class PointOfInterest : MonoBehaviour
{
    private PatrolManager _patrolManager;
    public bool willBeSearched = false;
    public Priority priority;

    private void Awake()
    {
        _patrolManager = GameObject.Find("PatrolManager").GetComponent<PatrolManager>(); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Perry")
        {
            if(this.gameObject == _patrolManager.HearingNode)
                _patrolManager.HearingNode = null;
            StartCoroutine("DestroyThis");
            
        }
    }

    public void FixedUpdate()
    {
        if (_patrolManager.HearingNode != this.gameObject)
        {
            DestroyNode();
        }
        if (!_isGrounded())
            DestroyNode();
    }

    private bool _isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, gameObject.GetComponent<Collider>().bounds.extents.y + .1f);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 1f);
    }

    public void DestroyNode()
    {
        StartCoroutine(DestroyThis());
    }

    private IEnumerator DestroyThis()
    {
        yield return new WaitForSeconds(2.0f);
        if (this.gameObject == _patrolManager.HearingNode)
            _patrolManager.HearingNode = null;
        gameObject.SetActive(false);
    }

    public void SetPriority(Priority p)
    {
        this.priority = p;
    }
}
