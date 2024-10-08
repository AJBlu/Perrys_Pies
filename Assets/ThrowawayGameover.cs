using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowawayGameover : MonoBehaviour
{
    public GameObject textGameOver;
    public void Awake()
    {
        textGameOver.SetActive(false);
    }
    public void OnCatchCollider()
    {
        textGameOver.SetActive(true);
    }   

}
