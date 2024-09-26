using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Door : MonoBehaviour
{
    public bool _unlocked = false;
    public GameObject TM;
    private Collider _collider;

    private void Awake()
    {
        TM.SetActive(false);
        _collider = gameObject.GetComponentInChildren<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Player" && _unlocked)
        {
            GameObject.Find("Perry").SetActive(false);
            TM.SetActive(true);
            gameObject.SetActive(false);
        }
    }
    public void onKeyObtained()
    {
        _unlocked = true;
    }
}
