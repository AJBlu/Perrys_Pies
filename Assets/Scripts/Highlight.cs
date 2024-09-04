using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    public List<Renderer> renderers;
    public Color color = Color.white;
    private List<Material> materials;

    private void Awake()
    {
        materials = new List<Material>();
        foreach (var renderer in renderers)
        {
            materials.AddRange(new List<Material>(renderer.materials));
        }
    }

    public void ToggleHighlight(bool val)
    {
        if (val)
        {
            foreach(var material in materials)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color);
            }
        }
        else
        {
            foreach (var material in materials)
            {
                material.DisableKeyword("_EMISSION"); 
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
