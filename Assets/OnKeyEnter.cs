using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ProBuilder.Shapes;

public class OnKeyEnter : MonoBehaviour
{
    public GameObject door;

    private void Awake()
    {
        door.GetComponent<Door>()._unlocked = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            door.GetComponent<Door>()._unlocked = true;
            gameObject.SetActive(false);
        }
    }
}
