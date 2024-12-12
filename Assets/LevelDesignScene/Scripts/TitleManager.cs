using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TitleManager : MonoBehaviour
{
    public GameObject menuScreen;
    public List<VideoClip> videos;

    public GameObject startButton;
    public GameObject quitButton;
    public GameObject logo;
    public GameObject knownIssuesButton;
    public GameObject knownIssuesText;

    public AudioSource menuMusic;

    // Start is called before the first frame update
    void Start()
    {
        GameObject camera = GameObject.Find("Main Camera");

        var videoPlayer = camera.GetComponent<VideoPlayer>();

        videoPlayer.clip = videos[0];

        startButton.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        quitButton.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        logo.GetComponent<RawImage>().color = new Color(1, 1, 1, 0);
        
        
        // Known Issues Button
        knownIssuesButton.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        knownIssuesText.SetActive(false);

        startButton.GetComponent<Button>().interactable = false;
        quitButton.GetComponent<Button>().interactable = false;

        StartCoroutine(FadeMenuToFullAlpha(24.5f, videoPlayer));
    }

    void EndReached(VideoPlayer vp)
    {
        vp.clip = videos[1];
        vp.isLooping = true;

        menuMusic.Play();
    }

    public IEnumerator FadeMenuToFullAlpha(float buffer, VideoPlayer vp)
    {
        yield return new WaitForSeconds(Mathf.Abs(buffer));

        while (startButton.GetComponent<Image>().color.a < 1.0f)
        {
            startButton.GetComponent<Image>().color = new Color
                (1, 1, 1, startButton.GetComponent<Image>().color.a + (Time.deltaTime / Mathf.Abs(2)));
            quitButton.GetComponent<Image>().color = new Color
                (1, 1, 1, quitButton.GetComponent<Image>().color.a + (Time.deltaTime / Mathf.Abs(2)));
            logo.GetComponent<RawImage>().color = new Color
                (1, 1, 1, logo.GetComponent<RawImage>().color.a + (Time.deltaTime / Mathf.Abs(2)));

            // Known Issues Button
            knownIssuesButton.GetComponent<Image>().color = new Color
                (1, 1, 1, knownIssuesButton.GetComponent<Image>().color.a + (Time.deltaTime / Mathf.Abs(2)));
            knownIssuesText.SetActive(true);


            yield return null;
        }

        startButton.GetComponent<Button>().interactable = true;
        quitButton.GetComponent<Button>().interactable = true;

        yield return new WaitForSeconds(2f);

        EndReached(vp);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
