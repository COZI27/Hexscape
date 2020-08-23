using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Rendering.LWRP;
using UnityEngine.Rendering.Universal;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessingManager : MonoBehaviour
{

    Volume volume;
    VolumeProfile profile;

    ColorAdjustments colourAjustments;

    float defaultSaturation, defaultBrightness;



    public static PostProcessingManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        volume = GetComponent<Volume>();
        profile = volume.profile;

        volume.profile.TryGet<ColorAdjustments>(out colourAjustments);

        //volume.profile.TryGetSettings<ColorGrading>(out colourGradingLayer);

        defaultSaturation = colourAjustments.saturation.value;
       defaultBrightness = colourAjustments.postExposure.value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ModifySaturation(float newSaturation)
    {
        if (colourAjustments)
            colourAjustments.saturation.value = newSaturation;
    }

    public void ModifyColourGrading(float newSaturation, float newbrightness)
    {
        if (colourAjustments)
        {
            colourAjustments.saturation.value = newSaturation;
            colourAjustments.postExposure.value = newbrightness;
        }
    }

    public void ResetPostProcessor()
    {
        if (colourAjustments)
        {
            colourAjustments.saturation.value = defaultSaturation;
            colourAjustments.postExposure.value = defaultBrightness;
        }
    }
}
