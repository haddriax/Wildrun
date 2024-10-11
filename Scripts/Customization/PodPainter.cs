using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

public class PodPainter : MonoBehaviour
{
    [System.Serializable]
    public class ColorSlider
    {
        public Slider sliderRed;
        public Slider sliderGreen;
        public Slider sliderBlue;
    }

    [SerializeField] List<ColorSlider> colorSliders;
    public PodOnCustom podCustom => customization.pod;
    
    List<Material> materials = null;
    private CustomizationBeta customization;

    private void Start()
    {
        customization = FindObjectOfType<CustomizationBeta>();
        if (podCustom)
        {
            GetMaterials();
        }
    }

    private void Update()
    {
        if (podCustom)
        {
            GetMaterials();
        }
        if (materials != null)
        {
            foreach (Material mat in materials)
            {
                try
                {
                    mat.SetColor("_MainColor", new Color(colorSliders[0].sliderRed.value, colorSliders[0].sliderGreen.value, colorSliders[0].sliderBlue.value));
                    mat.SetColor("_SecondaryColor", new Color(colorSliders[1].sliderRed.value, colorSliders[1].sliderGreen.value, colorSliders[1].sliderBlue.value));
                    mat.SetColor("_DetailsColor", new Color(colorSliders[2].sliderRed.value, colorSliders[2].sliderGreen.value, colorSliders[2].sliderBlue.value));
                }
                catch { }
            }
        }
    }

    private void SetSliders()
    {

    }

    private void GetMaterials()
    {
        if (podCustom)
        {
            materials = new List<Material>();
            podCustom.GetComponentsInChildren<SkinnedMeshRenderer>().ToList().ForEach(x => x.materials.ToList().ForEach(y => materials.Add(y)));
            podCustom.GetComponentsInChildren<MeshRenderer>().ToList().ForEach(x => x.materials.ToList().ForEach(y => materials.Add(y)));
        }
        Debug.Log(materials.Count);
    }
}
