using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HexColourLerp : ObserverPattern.Observer {

   
    [SerializeField] private Material[] materialsToColourLerp = null;

    public float lerpSpeed = 10f;
    public Color[] colourBank;

    private bool isDisabled;
    public Color disabledColour;

    public int colourTargetIndex;

    private void Start()
    {
        ColourManager.instance.AddObserver(this);
    }


    public override void OnNotify()
    {
        foreach (Material mat in materialsToColourLerp)
        {
            StartCoroutine(CycleMaterial(
                mat.GetColor("_EmissionColor"),
                ColourManager.instance.GetColour(true),
                2.0f,
                mat
            ));
        }
    }

    IEnumerator CycleMaterial(Color32 startColor, Color32 endColor, float cycleTime, Material mat)
    {
        float currentTime = 0;
        while (currentTime < cycleTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / cycleTime;
            Color32 currentColor = Color32.Lerp(startColor, endColor, t);
            //mat.color = currentColor;

            mat.SetColor("_EmissionColor", currentColor);
            yield return null;
        }
    }

    
}
