using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class justMove : MonoBehaviour
{
    private Rigidbody _rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _rigidbody.AddForce(new Vector3(0, 0, 1));
    }
}
