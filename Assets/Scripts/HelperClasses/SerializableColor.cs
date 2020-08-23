using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableColor
{

    public float[] colorStore = new float[4] { 1F, 1F, 1F, 1F };

    public SerializableColor()
    {
        colorStore = new float[4]
        {
             1F, 1F, 1F, 1F
        };
    }
    private Color Color
    {
        get { return new Color(colorStore[0], colorStore[1], colorStore[2], colorStore[3]); }
        set { colorStore = new float[4] { value.r, value.g, value.b, value.a }; }
    }

    public Color GetColour()
    {
        return Color;
    }

    public void SetColour(Color colour)
    {
        Color = colour;
    }

    //makes this class usable as Color, Color normalColor = mySerializableColor;
    public static implicit operator Color(SerializableColor instance)
    {
        return instance.Color;
    }

    //makes this class assignable by Color, SerializableColor myColor = Color.white;
    public static implicit operator SerializableColor(Color color)
    {
        return new SerializableColor { Color = color };
    }


}


//[System.Serializable]
//public class SerializableColor
//{
//    public float _r, _g, _b, _a;

//    public Color GetColor() => new Color(_r, _g, _b, _a);
//    public void SetColor(Color color)
//    {
//        _r = color.r;
//        _g = color.g;
//        _b = color.b;
//        _a = color.a;
//    }

//    public SerializableColor() { _r = _g = _b = _a = 1f; }

//    public SerializableColor(Color color) : this(color.r, color.g, color.b, color.a) { }

//    //makes this class usable as Color, Color normalColor = mySerializableColor;
//    public static implicit operator Color(SerializableColor instance)
//    {
//        return instance.Color;
//    }

//    public SerializableColor(float r, float g, float b, float a = 0f)
//    {
//        _r = r;
//        _g = g;
//        _b = b;
//        _a = a;
//    }
//}