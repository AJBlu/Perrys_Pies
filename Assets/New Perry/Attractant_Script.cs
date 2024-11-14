using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Attractant_Script : MonoBehaviour
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
    }

    public IEnumerator ThrowableCoroutine()
    {
        Debug.Log("Throwable Coroutine Starting");
        Sensor _p = GameObject.FindGameObjectWithTag("Perry").GetComponent<Sensor>();
        _p.OnNoiseEvent(gameObject.transform, Priority.ATTRACTANT);
        yield return new WaitForSeconds(Duration);
        StartCoroutine(_cleanup());
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
                            Debug.Log("Perry found and starting attractant action.");
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
