using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhylloTunnelPiece : ObserverPattern.Observer {

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
    private bool isLerping;
    private Vector3 startPos, endPos;
    private float lerpPosTimer, lerpPosSpeed;
    private bool forward;
    public bool repeat, invert;


    public override void OnNotify()
    {
        if (true /*test whether colour change is appropriate*/)
        {
            InitiateLerpToGradient(ColourManager.instance.GetGradientFromPalette() );
        }
    }

    // TODO: Lerp colour gradient and apply

    private Vector2 CalculatePhyllotaxis(float degree, float scale, int num) {
        double angle = num * (degree * Mathf.Deg2Rad);

        float r = scale * Mathf.Sqrt(num);

        float x = r * (float)System.Math.Cos(angle);
        float y = r * (float)System.Math.Sin(angle);

        Vector2 returnVec = new Vector2(x, y);
        return returnVec;
    }

    void SetLerpPosition() {
        phyllotaxisPos = CalculatePhyllotaxis(degree, scale, currentNumber);
        startPos = this.transform.localPosition;
        endPos = new Vector3(phyllotaxisPos.x, targetYPos + targetWobblePos, phyllotaxisPos.y);
    }

    private Vector2 phyllotaxisPos;

    void Awake() {

        forward = true;
        trailRenderer = GetComponent<TrailRenderer>();
        trailMat = new Material(trailRenderer.material);
        trailRenderer.material = trailMat;

        //trailMat.SetColor("_TintColor", trailColour);
        trailRenderer.colorGradient = trailGradient1;



        currentNumber = numberStart;
        transform.localPosition = CalculatePhyllotaxis(degree, scale, currentNumber);
        if (useLerp) {
            isLerping = true;
            SetLerpPosition();
        }

    }

    private void Start()
    {
        ColourManager.instance.AddObserver(this);
    }

    private Gradient targetGradient; // TODO: Replace with array

    private void InitiateLerpToGradient(Gradient newGradient)
    {
        Debug.Log("newGradient = " + newGradient.colorKeys[0].color + ", " + newGradient.colorKeys[0].color  +", "+ newGradient.colorKeys[2].color );


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

    void Update() {

        if (Input.GetKeyDown(KeyCode.Keypad1)) LerpToColourIndex(1, 3);
        if (Input.GetKeyDown(KeyCode.Keypad2)) LerpToColourIndex(2, 3);
        if (Input.GetKeyDown(KeyCode.Keypad3)) LerpToColourIndex(3, 3);

        if (bIsGradientLerping) {
            trailRenderer.colorGradient = Util.Gradient.Lerp(trailRenderer.colorGradient, targetGradient/*TODO: Use array index*/, Time.deltaTime / gradientLerpDivider);        
            gradientLerpTimer += (Time.deltaTime / gradientLerpDivider);
          //  Debug.Log("bIsGradientLerping...");
            if (Gradient.Equals(trailRenderer.colorGradient, targetGradient)) {
                bIsGradientLerping = false;
                    Debug.Log("Timer Up!");
            }
            //if (gradientLerpTimer >= 1) { bIsGradientLerping = false;
            //    Debug.Log("Timer Up!");
            //}
        }


        if (useWobble) {
            if ((targetWobblePos + targetYPos) > (targetYPos + wobbleRange) || (targetYPos + targetWobblePos) < (targetYPos - wobbleRange)) invertWobble = !invertWobble;
            if (!invertWobble) targetWobblePos += (Time.deltaTime * wobbleMultiplier);
            else  targetWobblePos -= (Time.deltaTime * wobbleMultiplier);
        }


        if (useLerp) {
            if (isLerping) {
                lerpPosSpeed = Mathf.Lerp(lerpPosSpeedMinMax.x, lerpPosSpeedMinMax.y, 0.5f /*0 - 1*/);
                lerpPosTimer += Time.deltaTime * lerpPosSpeed;
                transform.localPosition = Vector3.Lerp(startPos, endPos, Mathf.Clamp01(lerpPosTimer));
                if (lerpPosTimer >= 1) {
                    lerpPosTimer -= 1;
                    if (forward) {
                        currentNumber += stepSize;
                        currentIt++;
                    }
                    else {
                        currentNumber -= stepSize;
                        currentIt--;
                    }
                    if (currentIt > 0 && currentIt < maxIt) {
                        SetLerpPosition();
                    }
                    else {  // current iteration has hit 0 or maxiteration
                        if (repeat) {
                            if (invert) {
                                forward = !forward;
                                SetLerpPosition();
                            }
                            else {
                                currentNumber = numberStart;
                                currentIt = 0;
                                SetLerpPosition();
                            }
                        }
                        else { isLerping = false; }
                    }
                }
            }
        }
        if (!useLerp) {
   
            phyllotaxisPos = CalculatePhyllotaxis(degree, scale, currentNumber);
            transform.localPosition = new Vector3(phyllotaxisPos.x, Mathf.Lerp(this.transform.position.y, Mathf.Lerp(transform.position.y, targetYPos + targetWobblePos, Time.deltaTime), Time.deltaTime * 5), phyllotaxisPos.y);
            currentNumber += stepSize;
            currentIt++;
        }
    }
}
