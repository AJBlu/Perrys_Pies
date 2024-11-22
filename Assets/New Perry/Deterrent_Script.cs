using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Deterrent_ : MonoBehaviour
{
    public float Duration;

    public float Radius;

    public float Delay;

    public bool IsSightBased = false;

    private bool _coroutineRunning = false;

    NewStateMachine nsm;

    private bool isPerryTrapped = false;

    public void FixedUpdate()
    {
        if (_isGrounded())
        {
            if (!_coroutineRunning)
                StartCoroutine(_radiusAction());
        }
        if (isPerryTrapped)
        {

            if (nsm.currentState != States.TRAPPED)
            {
                isPerryTrapped = false;
                StartCoroutine(_cleanup());
            }
        }
    }

    public IEnumerator ThrowableCoroutine()
    {
        Debug.Log("Throwable Coroutine Starting");
        nsm = GameObject.FindGameObjectWithTag("Perry").GetComponent<NewStateMachine>();
        Navigation nav = GameObject.FindGameObjectWithTag("Perry").GetComponent<Navigation>();
        //_p.OnNoiseEvent(gameObject.transform, Priority.DETERRENT);
        //nsm.ChangeState(States.TRAPPED);
        GameObject.FindGameObjectWithTag("Perry").GetComponent<NavMeshAgent>().speed = 0;
        yield return new WaitForSeconds(Duration);
        GameObject.FindGameObjectWithTag("Perry").GetComponent<NavMeshAgent>().speed = nav.PatrolSpeed;
        nsm.UntrapPerry(States.PATROL);
        nav.isPatrolling = false;
    }
    private IEnumerator _radiusAction()
    {
        var P = GameObject.FindGameObjectWithTag("Perry");
        _coroutineRunning = true;
        yield return new WaitForSeconds(Delay);
        if (Vector3.Distance(gameObject.transform.position, P.transform.position) < Radius)
        {
            Debug.Log("Perry found in radius.");
            if (!IsSightBased)
            {
                StartCoroutine(ThrowableCoroutine());
            }
            //if deterrent is based on LOS with perry, see if there is an unbroken line between it and perry
            else
            {
                RaycastHit hit;
                var Perry = GameObject.Find("Perry");
                if (Physics.Raycast(gameObject.transform.position, (Perry.transform.position - gameObject.transform.position), out hit, Radius))
                {
                    if (hit.collider.gameObject.tag == "Perry")
                    {
                        Debug.Log("Perry found and starting deterrent action.");
                        ThrowableCoroutine();
                    }
                }
            }
        }
        else
        {
            StartCoroutine(_cleanup());
        }
    }

    private IEnumerator _cleanup()
    {
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);

    }
    private bool _isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, gameObject.GetComponent<Collider>().bounds.extents.y + .1f);
    }

}
