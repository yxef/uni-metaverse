using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    [SerializeField] private List<Renderer> renderers;
    [SerializeField] private Color highlightColor = Color.white;
    private List<Material> materials;
    void Awake()
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
            foreach (var material in materials)
            {
                // We enable emission on the materials
                material.EnableKeyword("_EMISSION");
                // And set its color to the highlight color
                material.SetColor("_EmissionColor", highlightColor);
            }
        }
        else
        {
            foreach (var material in materials)
            {
                // We disable it completely on the whole object when we are done
                material.DisableKeyword("_EMISSION");
            }
        }
    }
}
