using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerryActivator : MonoBehaviour
{
    public PlayerController pc;
    public GameObject Perry;
    public bool isBakery;
    public AudioSource ding;
    bool perryActive;
    private void OnEnable()
    {
        perryActive = false;
        Perry.SetActive(false);
        Debug.Log("Restarting Coroutine");
        if (!isBakery)
            StartCoroutine("SpawnInTen");

    }

    public void FixedUpdate()
    {
        if (isBakery)
        {
            if (pc.keyDeterGrabbed && !perryActive)
            {
                perryActive=true;
                Perry.gameObject.SetActive(true);
                ding.Play();
            }
        }
    }

    public IEnumerator SpawnInTen()
    {
        yield return new WaitForSeconds(10f);
        Perry.gameObject.SetActive(true);
        ding.Play();
    }
}
