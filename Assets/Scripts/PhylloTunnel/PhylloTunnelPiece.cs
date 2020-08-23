using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhylloTunnelPiece : ObserverPattern.Observer
{

    [Header("Gradient Palette")]
    public Gradient trailGradient1;
    public Gradient trailGradient2;
    public Gradient trailGradient3;

    private TrailRenderer trailRenderer;
    private Material trailMat;

    public float targetYPos { get; set; }

    private bool bIsGradientLerping = false;
    private float gradientLerpTimer = 0;
    private float gradientLerpDivider;
    private int colourIndexToLerpTo;

    [Header("Phyllotaxis Settings")]
    public float degree, scale;
    public int numberStart;
    public int stepSize;
    public int maxIt;

    private int currentIt;
    private int currentNumber;


    [Header("Wobble Settings")]
    public bool useWobble;
    public float wobbleRange = 10;
    public float wobbleMultiplier = 5;

    private bool invertWobble;
    private float targetWobblePos;

    [Header("Trail Lerp Settings")]
    public bool useLerp;
    public int lerpPosBand;
    public Vector2 lerpPosSpeedMinMax;
    private Vector3 startPos, endPos;
    private float lerpPosTimer, lerpPosSpeed;
    public bool repeat, invert;


    public override void OnNotify()
    {
        if (true /*test whether colour change is appropriate*/)
        {
            InitiateLerpToGradient(ColourManager.instance.GetGradientFromPalette() );
        }
    }

    // TODO: Lerp colour gradient and apply

    //private Vector2 CalculatePhyllotaxis(float degree, float scale, int num) {
    //    float angle = num * (degree * Mathf.Deg2Rad);

    //    float r = scale * Mathf.Sqrt(num);

    //    float x = r * (float)System.Math.Cos(angle);
    //    float y = r * (float)System.Math.Sin(angle);


    //    Vector2 returnVec = new Vector2(x, y);
    //    return returnVec;
    //}

    private Vector2 CalculatePhyllotaxis(float degree, float scale, int num)
    {
        float angle = num * (degree * Mathf.Deg2Rad);
        float r = scale * Mathf.Sqrt(num);

        //DEBUG
        this.angle = angle;
        this.r = r;


        float x = r * Mathf.Cos(angle);
        float y = r * Mathf.Sin(angle);

        if (x > calculatedXPos) calculatedXPos = x;
        if (y > calculatedYPos) calculatedYPos = y;

        Vector2 returnVec = new Vector2(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
        return returnVec;
    }


    //private Vector2 CalculatePhyllotaxis(float degree, float scale, int num)
    //{
    //    double angle = num * (degree * Mathf.Deg2Rad);

    //    double r = scale * System.Math.Sqrt(num);

    //    double x = r * System.Math.Cos(angle);
    //    double y = r * System.Math.Sin(angle);


    //    Vector2 returnVec = new Vector2((float)x, (float)y);
    //    return returnVec;
    //}

    void SetLerpPosition() {
        phyllotaxisPos = CalculatePhyllotaxis(degree, scale, currentNumber);
        startPos = this.transform.localPosition;
        endPos = new Vector3(phyllotaxisPos.x, targetYPos + targetWobblePos, phyllotaxisPos.y);
    }

    private Vector2 phyllotaxisPos;

    void Awake() {
        trailRenderer = GetComponent<TrailRenderer>();
        trailMat = new Material(trailRenderer.material);
        trailRenderer.material = trailMat;

        //trailMat.SetColor("_TintColor", trailColour);
        trailRenderer.colorGradient = trailGradient1;

        trailRenderer.enabled = false;

       currentNumber = numberStart;
        transform.localPosition = CalculatePhyllotaxis(degree, scale, currentNumber);
        if (useLerp) {
            SetLerpPosition();
        }
        

    }

    private void Start()
    {
        ColourManager.instance.AddObserver(this);
        StartCoroutine(DelayEnableTral());
    }

    private IEnumerator DelayEnableTral()
    {
        yield return new WaitForSeconds(0.1f);
        trailRenderer.enabled = true;
    }

    private Gradient targetGradient; // TODO: Replace with array

    private void InitiateLerpToGradient(Gradient newGradient)
    {
        //Debug.Log("newGradient = " + newGradient.colorKeys[0].color + ", " + newGradient.colorKeys[0].color  +", "+ newGradient.colorKeys[2].color );


        targetGradient = newGradient;
        gradientLerpDivider = 3;
        gradientLerpTimer = 0;
        bIsGradientLerping = true;
    }

    // Triggers a gradient Lerp
    void LerpToColourIndex (int index, float divider) {
        switch (index) { /*TODO: Use index in array*/
            case 1:
                targetGradient = trailGradient1;
                break;
            case 2:
                targetGradient = trailGradient2;
                break;
            case 3:
                targetGradient = trailGradient3;
                break;
            default:
                return; // Lerp request failed
        }

        gradientLerpDivider = divider;
        gradientLerpTimer = 0;
        colourIndexToLerpTo = index;
        bIsGradientLerping = true;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Keypad1)) LerpToColourIndex(1, 3);
        if (Input.GetKeyDown(KeyCode.Keypad2)) LerpToColourIndex(2, 3);
        if (Input.GetKeyDown(KeyCode.Keypad3)) LerpToColourIndex(3, 3);

        if (bIsGradientLerping)
        {
            trailRenderer.colorGradient = Util.Gradient.Lerp(trailRenderer.colorGradient, targetGradient/*TODO: Use array index*/, Time.deltaTime / gradientLerpDivider);
            gradientLerpTimer += (Time.deltaTime / gradientLerpDivider);
            if (Gradient.Equals(trailRenderer.colorGradient, targetGradient)) bIsGradientLerping = false;
        }


        if (useWobble)
        {
            if ((targetWobblePos + targetYPos) > (targetYPos + wobbleRange) || (targetYPos + targetWobblePos) < (targetYPos - wobbleRange)) invertWobble = !invertWobble;
            if (!invertWobble) targetWobblePos += (Time.deltaTime * wobbleMultiplier);
            else targetWobblePos -= (Time.deltaTime * wobbleMultiplier);
        }


        phyllotaxisPos = CalculatePhyllotaxis(degree, scale, currentNumber);
        transform.localPosition = new Vector3(phyllotaxisPos.x, Mathf.Lerp(this.transform.position.y, Mathf.Lerp(transform.position.y, targetYPos + targetWobblePos, Time.deltaTime), Time.deltaTime * 5), phyllotaxisPos.y);
        currentNumber += stepSize;
        currentIt++;

        if (currentNumber > 950 * Mathf.Abs(stepSize) && stepSize > 0)
        {
            stepSize = -stepSize;
        }
        else if (currentNumber < 900 * Mathf.Abs(stepSize) && stepSize < 0)
        {
            stepSize = -stepSize;
        }


        //if (!useLerp)
        //{
        //    if (forward)
        //    {
        //        currentNumber += stepSize;
        //        currentIt++;
        //    }
        //    else
        //    {
        //        currentNumber -= stepSize;
        //        currentIt--;
        //    }
        //    if (currentIt > 0 && currentIt < maxIt)
        //    {
        //        phyllotaxisPos = CalculatePhyllotaxis(degree, scale, currentNumber);
        //        transform.localPosition = new Vector3(phyllotaxisPos.x, Mathf.Lerp(this.transform.position.y, Mathf.Lerp(transform.position.y, targetYPos + targetWobblePos, Time.deltaTime), Time.deltaTime * 5), phyllotaxisPos.y);
        //    }
        //    else
        //    {  // current iteration has hit 0 or maxiteration
        //       //if (repeat)
        //       //{
        //        if (invert)
        //        {
        //            forward = !forward;
        //            phyllotaxisPos = CalculatePhyllotaxis(degree, scale, currentNumber);
        //            transform.localPosition = new Vector3(phyllotaxisPos.x, Mathf.Lerp(this.transform.position.y, Mathf.Lerp(transform.position.y, targetYPos + targetWobblePos, Time.deltaTime), Time.deltaTime * 5), phyllotaxisPos.y);
        //        }
        //        else
        //        {
        //            currentNumber = numberStart;
        //            currentIt = 0;
        //            phyllotaxisPos = CalculatePhyllotaxis(degree, scale, currentNumber);
        //            transform.localPosition = new Vector3(phyllotaxisPos.x, Mathf.Lerp(this.transform.position.y, Mathf.Lerp(transform.position.y, targetYPos + targetWobblePos, Time.deltaTime), Time.deltaTime * 5), phyllotaxisPos.y);
        //        }
        //        // }
        //    }
        //}
    }

    float calculatedXPos, calculatedYPos, angle, r;

    //void OnGUI()
    //{
    //    if (this.transform.GetSiblingIndex() == 0)
    //    {
    //        GUI.Label(new Rect(0, 0, 200, 200), "Calculated x pos = :");
    //        GUI.Label(new Rect(0, 10, 200, 200), calculatedXPos.ToString());

    //        GUI.Label(new Rect(100, 0, 200, 200), "Calculated Y Pos = ");
    //        GUI.Label(new Rect(100, 10, 200, 200), calculatedYPos.ToString());

    //        GUI.Label(new Rect(0, 30, 200, 200), "CurrentNumber = ");
    //        GUI.Label(new Rect(0, 40, 200, 200), currentNumber.ToString());

    //        GUI.Label(new Rect(0, 60, 200, 200), "Angle = ");
    //        GUI.Label(new Rect(0, 70, 200, 200), angle.ToString());

    //        GUI.Label(new Rect(0, 90, 200, 200), "rads = ");
    //        GUI.Label(new Rect(0, 100, 200, 200), r.ToString());





    //    }
    //}

}
