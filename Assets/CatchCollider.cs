using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class CatchCollider : MonoBehaviour
{
    GameObject GameOver_Template;
    public UnityEvent OnPlayerGrabbed = new UnityEvent();

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            OnPlayerGrabbed.Invoke();

        }
    }
}
