using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Throwable : MonoBehaviour
{

    public float Duration;

    public float Radius;

    public float Delay;

    public bool IsSightBased;

    private bool _coroutineRunning;

    public void FixedUpdate()
    {
        if (_isGrounded())
        {
            if (!_coroutineRunning)
                StartCoroutine(_radiusAction());
        }
    }

    public virtual IEnumerator ThrowableCoroutine()
    {
        var Perry = GameObject.Find("Perry");
        var PerryNMA = Perry.GetComponent<NavMeshAgent>();
        yield return new WaitForSeconds(5f);

    }

    private IEnumerator _radiusAction()
    {
        _coroutineRunning = true;
        yield return new WaitForSeconds(Delay);
        Collider[] found = Physics.OverlapSphere(transform.position, Radius);

        foreach (Collider collider in found)
        {
            if (collider.tag == "Perry")
            {
                Debug.Log("Perry found in radius.");
                if (!IsSightBased)
                {
                    ThrowableCoroutine();
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
        }
        StartCoroutine(_cleanup());

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
