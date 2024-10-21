using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnProximityWithPlayer : MonoBehaviour
{
    public UnityEvent OnPlayerGet = new UnityEvent();
    public GameObject GameOver;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Here!");
        if(other.tag == "Player")
        {
            OnPlayerGet.Invoke();
            GameOver.SetActive(true);
            other.gameObject.GetComponent<PlayerController>().currentSpeed = 0;
            other.gameObject.GetComponent<PlayerController>().originalSpeed = 0;

        }
    }
}
