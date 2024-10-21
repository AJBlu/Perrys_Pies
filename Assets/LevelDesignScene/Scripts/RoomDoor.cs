using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDoor : MonoBehaviour
{

    public bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen) transform.eulerAngles = new Vector3(0, 90, 0);
        else transform.eulerAngles = new Vector3(0, 0, 0);
    }
}
