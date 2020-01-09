using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LWRP;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine;

public class PostProcessingManager : MonoBehaviour
{

    PostProcessVolume volume;
    PostProcessProfile profile;

    ColorGrading colourGradingLayer;

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
        volume = GetComponent<PostProcessVolume>();
        profile = volume.profile;

        volume.profile.TryGetSettings<ColorGrading>(out colourGradingLayer);

        defaultSaturation = colourGradingLayer.saturation.value;
        defaultBrightness = colourGradingLayer.brightness.value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ModifySaturation(float newSaturation)
    {
        if (colourGradingLayer)
            colourGradingLayer.saturation.value = newSaturation;
    }

    public void ModifyColourGrading(float newSaturation, float newbrightness)
    {
        if (colourGradingLayer)
        {
            colourGradingLayer.saturation.value = newSaturation;
            colourGradingLayer.brightness.value = newbrightness;
        }
    }

    public void ResetPostProcessor()
    {
        if (colourGradingLayer)
        {
            colourGradingLayer.saturation.value = defaultSaturation;
            colourGradingLayer.brightness.value = defaultBrightness;
        }
    }
}
