using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameOverCollider : MonoBehaviour
{
    public UnityEvent GameOver;
    public GameObject restartGameButton;
    public GameObject Text;
    public GameObject Perry, Player;
    public Vector3 PerryStart, PlayerStart;
    public UIManager UIM;
    private void Awake()
    {
        Perry = GameObject.FindGameObjectWithTag("Perry");
        Player = GameObject.FindGameObjectWithTag("Player");
        UIM = GameObject.Find("UIManager").GetComponent<UIManager>();
        UIM.gameOverTriggered = false;
        PerryStart = Perry.transform.position;
        PlayerStart = Player.transform.position;
        Text.SetActive(false);
        restartGameButton.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("Player is here.");
            UIM.gameOverTriggered = true;
            Perry.GetComponent<Navigation>().caughtPlayer = true;
            //Cursor.lockState = CursorLockMode.None;
            Text.SetActive(true);
            restartGameButton.SetActive(true);
        }
    }

    public void RestartGame()
    {
        Player.transform.position = PlayerStart;
        Perry.transform.position = PerryStart;
        Text.SetActive(false);
        restartGameButton.SetActive(false);
        UIM.gameOverTriggered = false;
        //Cursor.lockState = CursorLockMode.Locked;
        GameOver.Invoke();
    }
}
