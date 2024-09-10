using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject elevatorPanelHolder;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        elevatorPanelHolder.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
