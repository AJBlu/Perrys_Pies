using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerryActivator : MonoBehaviour
{
    public PlayerController pc;
    public GameObject Perry;
    public bool isBakery;
    public AudioSource ding;
    private void Awake()
    {
        Perry.gameObject.SetActive(false);
        if (!isBakery)
            StartCoroutine("SpawnInTen");
    }

    public void FixedUpdate()
    {
        if (isBakery)
        {
            if (pc.keyDeterGrabbed)
            {
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
