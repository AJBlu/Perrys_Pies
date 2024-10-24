using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

public class Deterrent_Events : MonoBehaviour
{

    public bool isSightBased;

    public float DeterrentDuration;

    public float DeterrentRadius;

    public float DeterrentDelay;

    private bool _coroutineRunning;



    public void FixedUpdate()
    {
        if (_isGrounded())
        {
            if(!_coroutineRunning)
                StartCoroutine(_radiusDeterrentActions());
        }
    }

    private bool _isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, gameObject.GetComponent<Collider>().bounds.extents.y + .1f);
    }

    private void _deterrentAction()
    {
        StartCoroutine(_stopPerry());
    }

    private IEnumerator _stopPerry()
    {
        var Perry = GameObject.Find("Perry");
        var PerryNMA = Perry.GetComponent<NavMeshAgent>();
        PerryNMA.isStopped = true;
        yield return new WaitForSeconds(DeterrentDuration);
        PerryNMA.isStopped = false;
    }

    private IEnumerator _radiusDeterrentActions()
    {
        _coroutineRunning = true;
        yield return new WaitForSeconds(DeterrentDelay);
        Collider[] found = Physics.OverlapSphere(transform.position, DeterrentRadius);

        foreach (Collider collider in found)
        {
            if (collider.tag == "Perry")
            {
                Debug.Log("Perry found in radius.");
                if (!isSightBased)
                {
                    _deterrentAction();
                }
                //if deterrent is based on LOS with perry, see if there is an unbroken line between it and perry
                else
                {
                    RaycastHit hit;
                    var Perry = GameObject.Find("Perry");
                    if (Physics.Raycast(gameObject.transform.position, (Perry.transform.position - gameObject.transform.position), out hit, DeterrentRadius))
                    {
                        if (hit.collider.gameObject.tag == "Perry")
                        {
                            Debug.Log("Perry found and starting deterrent action.");
                            _deterrentAction();
                        }
                    }
                }
            }
        }
        StartCoroutine(_cleanupCoroutine());
    }

    private IEnumerator _cleanupCoroutine()
    {
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);

    }

}
