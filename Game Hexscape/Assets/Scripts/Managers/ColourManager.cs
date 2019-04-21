using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ColourManager : ObserverPattern.Subject
{
    public static ColourManager instance;

    private void MakeSingleton()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }



    struct Palette
    {
        public enum PaletteColour
        {
            ForegroundA = 0,
            ForegroundB = 1,
            ForegroundC = 2
        }


        public Palette(Color32 A, Color32 B, Color32 C)
        {
            Colours = new Color32[3]
            {
                A,
                B,
                C
            };
        }

        Color32[] Colours;
        // public Color BackgroundColour


        public Color32 GetColour(PaletteColour colourType)
        {
            return Colours[(int)colourType];
        }

        public  Color32[] GetForegroundColours()
        {
            return new Color32[] { Colours[(int)PaletteColour.ForegroundA],
                                 Colours[(int)PaletteColour.ForegroundB],
                                 Colours[(int)PaletteColour.ForegroundC]
                };
        }
    }


    List<Palette> colourPalettes = new List<Palette>
    {
        { new Palette(new Color32(255,108,17, 255),new Color32(255,56,100, 255),new Color32(45,226,230, 255) )}, // Orange, Pink, Teal
        { new Palette(new Color32(2,55,255, 255),new Color32(101,13,137, 255),new Color32(146,0,117, 255) )}, // Blue, Purple, Magenta
        { new Palette(new Color32(249,200,14, 255),new Color32(255,67,101, 255),new Color32(84,13,110, 255) )}, // Yellow, Pink, Purple
    };

    [SerializeField]
    private int currentPaletteIndex = 0;
    [SerializeField]
    private Gradient GeneratedGradient;


    //private static List<Action> paletteActions;

    private void Awake()
    {
        MakeSingleton();
    }

    public Color32 GetColour()
    {
        return colourPalettes[currentPaletteIndex].GetColour(Palette.PaletteColour.ForegroundA);
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangePalette();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangePalette();
        }
    }


    public Gradient GetGradientFromPalette( )
    {

        Gradient returnGradient = GeneratedGradient;
        return returnGradient;
    }

    public void ChangePalette()
    {
        currentPaletteIndex = Random.Range(0, colourPalettes.Count);
        GeneratedGradient = GenerateGradientFromPalette(colourPalettes[currentPaletteIndex]);
        Notify(); // Notifies observers of colour change
    }

    public void ChangePalette(int index)
    {
        if (colourPalettes.Count > index && index > -1)
        {
            currentPaletteIndex = index;
            Notify(); // Notifies observers of colour change
        }
    }



    private Gradient GenerateGradientFromPalette(Palette palette)
    {


        Gradient returnGradient = new Gradient();
        returnGradient.mode = GradientMode.Blend;

        GradientColorKey[] colorKey = new GradientColorKey[3];
        GradientAlphaKey[] alphaKey = new GradientAlphaKey[3];

        Color32[] colourArr = palette.GetForegroundColours();

        for (int t = 0; t < colourArr.Length; t++)
        {
            Color32 tmp = colourArr[t];
            int r = Random.Range(t, colourArr.Length);
            colourArr[t] = colourArr[r];
            colourArr[r] = tmp;
        }

        alphaKey[0].alpha = 0.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 0.5f;
        alphaKey[2].alpha = 0.0f;
        alphaKey[2].time = 1.0f;

        colorKey[0].color = colourArr[0];
        colorKey[0].time = 0.0f;
        colorKey[1].color = colourArr[1];
        colorKey[1].time = 0.5f;
        colorKey[2].color = colourArr[2]; // TODO: Ensure index is valid
        colorKey[2].time = 1.0f;

        returnGradient.SetKeys(colorKey, alphaKey);

        return returnGradient;
    }
}
