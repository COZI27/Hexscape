using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Temp clas for testing out a colour lerp
public class HexColourLerp : ObserverPattern.Observer {

   
    [SerializeField] private Material[] materialsToColourLerp;
    [SerializeField] private bool autoLerp = false;


    public float lerpSpeed = 10f;
    public Color[] colourBank;

    private bool isDisabled;
    public Color disabledColour;

    public int colourTargetIndex;

    private void Start()
    {
        ColourManager.instance.AddObserver(this);
    }

    private void Update()
    {


        //Color targetColour = (isDisabled) ? disabledColour : colourBank[colourTargetIndex]; //selects the target colour... if disabled gets the disabled colour
        //foreach (Material mat in materialsToColourLerp)
        //{
           
        //    Color currentColour = mat.GetColor("_EmissionColor");

        //    if (currentColour == targetColour)
        //    {
        //        if (isDisabled == false && autoLerp == true)
        //        {
        //            NextColour();
        //        }
               
        //        return;
        //    }

        //    Color lerpColour = Color.Lerp(currentColour, targetColour, Time.deltaTime * lerpSpeed);

        //    mat.SetColor("_EmissionColor", lerpColour);
        //}
    }

    //public void NextColour()
    //{
    //    //isDisabled = false;
    //    //colourTargetIndex++;
    //    //colourTargetIndex %= colourBank.Length ;
    //}

    //public void DisabledColour ()
    //{
    //    isDisabled = true;
    //}

    public override void OnNotify()
    {
        foreach (Material mat in materialsToColourLerp)
        {
            StartCoroutine(CycleMaterial(
                mat.GetColor("_EmissionColor"),
                ColourManager.instance.GetColour(),
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
