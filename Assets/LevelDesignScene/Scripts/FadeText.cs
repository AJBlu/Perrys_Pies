using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FadeText : MonoBehaviour
{
    public float fadeTime;
    private TextMeshProUGUI fadeAwayText;

    // Start is called before the first frame update
    void Start()
    {
        fadeAwayText = GetComponent<TextMeshProUGUI>();
        fadeAwayText.color = new Color(fadeAwayText.color.r, fadeAwayText.color.g, fadeAwayText.color.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeTime > 0)
        {
            fadeTime -= Time.deltaTime;
            fadeAwayText.color = new Color(fadeAwayText.color.r, fadeAwayText.color.g, fadeAwayText.color.b, fadeTime);
        }
    }
}
