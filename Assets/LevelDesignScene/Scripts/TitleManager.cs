using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        GameObject camera = GameObject.Find("Main Camera");

        var videoPlayer = camera.GetComponent<VideoPlayer>();

        videoPlayer.clip = videos[0];

        startButton.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        quitButton.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        logo.GetComponent<RawImage>().color = new Color(1, 1, 1, 0);

        StartCoroutine(FadeMenuToFullAlpha(24.5f));
    }

    void EndReached(VideoPlayer vp)
    {
        vp.clip = videos[1];
        vp.isLooping = true;
    }

    public IEnumerator FadeMenuToFullAlpha(float buffer)
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
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
