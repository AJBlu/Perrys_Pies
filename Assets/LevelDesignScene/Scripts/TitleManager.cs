using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TitleManager : MonoBehaviour
{
    public GameObject menuScreen;
    public List<VideoClip> videos;

    

    // Start is called before the first frame update
    void Start()
    {
        GameObject camera = GameObject.Find("Main Camera");

        var videoPlayer = camera.GetComponent<VideoPlayer>();

        videoPlayer.clip = videos[0];
    }

    void EndReached(VideoPlayer vp)
    {
        vp.clip = videos[1];
        vp.isLooping = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
