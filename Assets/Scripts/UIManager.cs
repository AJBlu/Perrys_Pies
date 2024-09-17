using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject elevatorPanelHolder;
    public GameObject player;
    private static GameManager gameManager;

    public bool menuOpen;

    public List<Button> elevatorButtons;

    // Start is called before the first frame update
    void Start()
    {
        checkForMissingStuff();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void checkForMissingStuff()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        player = GameObject.Find("Player");
    }
}
