using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManagerClone : MonoBehaviour
{
    GameObject mainMenu;
    GameObject controls;
    GameObject controlsButton;

    // Start is called before the first frame update
    void Start()
    {
        mainMenu = GameObject.FindGameObjectWithTag("MainMenu");
        controls = GameObject.Find("Controls");
        controlsButton = controls.transform.Find("StartGame").gameObject;

        controls.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        controlsButton.GetComponent<Image>().color = new Color(1, 1, 1, 0);

        controlsButton.GetComponent<Button>().interactable = false;
        controlsButton.transform.Find("Text (TMP)").gameObject.GetComponent<TMPro.TextMeshProUGUI>().color =
            new Color32(50, 50, 50, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void quitGame()
    {
        Debug.Log("You'll be back... and I'll be waiting.");
        Application.Quit();
    }

    public void showControls()
    {
        mainMenu.SetActive(false);
        controls.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        controlsButton.GetComponent<Image>().color = new Color(1, 0, 0, 1);
        controlsButton.GetComponent<Button>().interactable = true;
        controlsButton.transform.Find("Text (TMP)").gameObject.GetComponent<TMPro.TextMeshProUGUI>().color =
            new Color32(50, 50, 50, 255);
    }

    public void startGame()
    {
        SceneManager.LoadScene(1);
    }
}
