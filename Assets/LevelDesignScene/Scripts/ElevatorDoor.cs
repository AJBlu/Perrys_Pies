using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDoor : MonoBehaviour
{
    public GameObject elevatorLock;

    // Start is called before the first frame update
    void Start()
    {
        elevatorLock = GameObject.FindGameObjectWithTag("ElevatorLock");
    }

    // Update is called once per frame
    void Update()
    {
        if (elevatorLock == null)
        {
            this.gameObject.layer = LayerMask.NameToLayer("Pickable");
        }
        else
        {
            this.gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }
}
