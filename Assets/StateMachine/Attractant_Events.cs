using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractant_Events : MonoBehaviour
{
    public bool isSightBased;

    public float AttractionDuration;

    public float AttractionRadius;

    public float AttractionDelay;

    private bool _coroutineRunning;


    public void FixedUpdate()
    {
        if (_isGrounded())
        {
            if (!_coroutineRunning)
                StartCoroutine(_radiusAttractantActions());
        }
    }

    private void _attractionAction()
    {
        var Perry = GameObject.Find("Perry");
        var PerryNMA = Perry.GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    private bool _isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, gameObject.GetComponent<Collider>().bounds.extents.y + .1f);
    }

    private IEnumerator _radiusAttractantActions()
    {
        _coroutineRunning = true;
        yield return new WaitForSeconds(AttractionDelay);
        Collider[] found = Physics.OverlapSphere(transform.position, AttractionRadius);

        foreach (Collider collider in found)
        {
            if (collider.tag == "Perry")
            {
                Debug.Log("Perry found in radius.");
                if (!isSightBased)
                {
                    _attractionAction();
                }
                //if deterrent is based on LOS with perry, see if there is an unbroken line between it and perry
                else
                {
                    RaycastHit hit;
                    var Perry = GameObject.Find("Perry");
                    if (Physics.Raycast(gameObject.transform.position, (Perry.transform.position - gameObject.transform.position), out hit, AttractionRadius))
                    {
                        if (hit.collider.gameObject.tag == "Perry")
                        {
                            Debug.Log("Perry found and starting deterrent action.");
                            _attractionAction();
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
