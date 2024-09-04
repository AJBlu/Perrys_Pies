using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationNode : MonoBehaviour
{
    Transform _coordinates;
    bool _runTo;
    int _priority;
    float _radius;

    public DestinationNode(Transform coordinates, bool shouldRun, int priority, float radius)
    {
        _coordinates = coordinates;
        _runTo = shouldRun;
        _priority = priority;
        _radius = radius;
    }

    //when created, alert perry
    private void Awake()
    {
        Collider[] colliders = Physics.OverlapSphere(_coordinates.position, _radius);
        foreach (var collider in colliders)
        {
            //if perry is in listen radius
            if (collider.gameObject.tag == "Perry")
            {
                //trigger event
            }

        }
    }


}
