using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;


// This is for my Level Database, if you right click  in the project tap and press create you can create a new level...
//Not sure if we will use a level database for endless but at the moment we are... 
// we might also want to seperate endless and challenge levels into diffrent children of the Level class

public static class HexTypes
{
    // In order to access the attribute in the Unity Editor Hex Attribute Window - attributes must be added here 
    static string[] allhexAttributes = {
        typeof(DigitElementAttribute).ToString(),
        typeof(MenuButtonElementAttribute).ToString()
    };

    public static string[] GetCompatibleAttrributes(this HexTypeEnum hexType)
    {

        switch (hexType)
        {
            case HexTypeEnum.HexTile_Digit0:
            case HexTypeEnum.HexTile_Digit1:
            case HexTypeEnum.HexTile_Digit2:
            case HexTypeEnum.HexTile_Digit3:
            case HexTypeEnum.HexTile_Digit4:
            case HexTypeEnum.HexTile_Digit5:
            case HexTypeEnum.HexTile_Digit6:
            case HexTypeEnum.HexTile_Digit7:
            case HexTypeEnum.HexTile_Digit8:
                return new string[]  {
                       typeof(DigitElementAttribute).ToString(),
                };

            case HexTypeEnum.HexTile_MenuOptionEdit:
                return new string[]  {
                       typeof(MenuButtonElementAttribute).ToString()
                };
            default:
                return allhexAttributes;
        }
    }

    //ElementAttribute tempAt = System.Type.GetType("type");
}

[System.Serializable]
public class Level
{
   [ReadOnly] public string levelName;
    public MapElement[] hexs;

    //public int passAmount;
    //public int bronzeAmount;
    //public int silverAmount;
    //public int goldAmount;

    private readonly string saveLocation = "/Resources/Levels/Json/";
    private readonly string jsonFileName = "TestLevel";

    private string GetLevelPath()
    {
        return Application.dataPath + saveLocation + jsonFileName + ".json";
    }

    [ContextMenu("Save Json")]
    public void GetJson()
    {
        string json = JsonConvert.SerializeObject(this); //JsonUtility.ToJson(this);

        levelName = json;

        File.WriteAllText(GetLevelPath(), json);
    }

    [ContextMenu("Load Json")]
    public void LoadJson()
    {
        string json = File.ReadAllText(GetLevelPath());

        Level loadedLevel = JsonConvert.DeserializeObject<Level>(json); // JsonUtility.FromJson<Level>(json);

        levelName = json;
        Debug.Log(loadedLevel.levelName);
    }
}



[System.Serializable]
public class MapElement
{
    [ReadOnly] [HideInInspector] public string displayName = null;
   

    public void UpdateDisplayName ()
    {
        string dName = hexType.ToString() + ": " + gridPos.ToString();
        
       

        if (hexAttribute != null)
        {

            dName = dName.Replace("HexTile_", "☰ ");
            if (hexAttribute.GetType() == typeof( MenuButtonElementAttribute))
            {
                MenuButtonElementAttribute yeet = hexAttribute as MenuButtonElementAttribute ;
                hexAttributeInfo = yeet.GetType() + ": " + yeet.commandToCall;
            } else
            {

                hexAttributeInfo = hexAttribute.GetType().ToString();
            }

            

            // Debug.Log(hexAttribute);
        } else
        {
            dName = dName.Replace("HexTile_", "♥ ");
            hexAttributeInfo = null;
        }

        displayName = dName;
       
    }


    

    public Vector2Int gridPos;
    public HexTypeEnum hexType;

    [JsonProperty(PropertyName = "HexAttribute")]
    public ElementAttribute hexAttribute;

    public Hex GetHex ()
    {
        return HexBank.instance.GetHexFromType(hexType); ;
    }

    public MapElement(HexTypeEnum hexType, Vector2Int gridPos, ElementAttribute hexAttributes = null)
    {
        this.hexType = hexType;
        this.gridPos = gridPos;
        this.hexAttribute = hexAttributes;
    }


    [ReadOnly] public string hexAttributeInfo = null;

}



[System.Serializable]
public class DigitElementAttribute : ElementAttribute
{
    public DigitElementAttribute(int leadingZeroCount)
    {
        this.leadingZeroCount = leadingZeroCount;
    }

    [SerializeField]
    public int leadingZeroCount;

    public override void AddAttributeToHex(Hex hexInstance)
    {

    }
}


[System.Serializable]
public class MenuButtonElementAttribute : ElementAttribute
{
    public MenuButtonElementAttribute(Command commandToCall)
    {
        this.commandToCall = commandToCall;
    }

    [SerializeField]
    public Command commandToCall;

    public override void AddAttributeToHex(Hex hexInstance)
    {
        hexInstance.clickedEvent.AddListener(() =>
        {
            //HandleRegisterClick();
            GameManager.instance.ProcessCommand(commandToCall);
            hexInstance.DigHex(); // Temp
            
        });
    }
}

[System.Serializable]
public abstract class ElementAttribute {
    public abstract void AddAttributeToHex(Hex hexInstance);
}

//[System.Serializable]
//public class HexButtonElement : MapElement
//{

//    GameManager.Command commandToCall;

//    public HexButtonElement(HexTypeEnum hexType, Vector2Int gridPos, GameManager.Command commandToCall) : base (hexType, gridPos)
//    {
//        this.commandToCall = commandToCall;
//    }

//}

//[System.Serializable]
//public class DigitElement : MapElement
//{

//    int leadingZeroCount;

//    public DigitElement(HexTypeEnum hexType, Vector2Int gridPos, int leadingZeroCount) : base(hexType, gridPos)
//    {
//        this.leadingZeroCount = leadingZeroCount;
//    }

//}

