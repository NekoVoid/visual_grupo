using UnityEngine;
using UnityEngine.UI;

public class MaterialController : MonoBehaviour
{
    public Material targetMaterial;
    public Slider normalSlider;
    public Slider smoothnessSlider;

    void Start()
    {
        normalSlider.onValueChanged.AddListener(UpdateMaterial);
        smoothnessSlider.onValueChanged.AddListener(UpdateMaterial);
        UpdateMaterial(0);
    }

    void UpdateMaterial(float value)
    {
        if (targetMaterial != null)
        {
            // va de 0 a 100
            float bumpStrength =  normalSlider.value;
            targetMaterial.SetFloat("_BumpScale", bumpStrength);

            targetMaterial.SetFloat("_Smoothness", smoothnessSlider.value);
        }
    }
}


