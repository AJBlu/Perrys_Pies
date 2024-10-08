using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody _rb;
    public float decelleration;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Fire(float speed, Vector3 direction)
    {
        _rb.velocity = direction * speed;
    }

    public void Update()
    {
        if (Mathf.Approximately(_rb.velocity.y, 0f))
        {
            _rb.AddRelativeForce(-_rb.velocity * decelleration);
        }
    }
}
