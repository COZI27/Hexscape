using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;





// This is for my Level Database, if you right click  in the project tap and press create you can create a new level...
//Not sure if we will use a level database for endless but at the moment we are... 
// we might also want to seperate endless and challenge levels into diffrent children of the Level class

public static class HexTypes
{
    // In order to access the attribute in the Unity Editor Hex Attribute Window - attributes must be added here 
    // NOTE: This is intended to be replaced by the AttributeFinder
    static string[] allhexAttributes = {
        typeof(DigitElementAttribute).ToString(),
        typeof(MenuButtonElementAttribute).ToString(),
        typeof(CollectableHolderElementAttribute).ToString()
    };

    public static string[] GetCompatibleAttrributes(this HexTypeEnum hexType, out ElementAttribute defaultAttributeValues)
    {

        switch (hexType)
        {
            case HexTypeEnum.HexTile_Digit0:
                defaultAttributeValues = new DigitElementAttribute(0, 0);
                return new string[] { typeof(DigitElementAttribute).ToString() };
            case HexTypeEnum.HexTile_Digit1:
                defaultAttributeValues = new DigitElementAttribute(0, 1);
                return new string[] { typeof(DigitElementAttribute).ToString() };
            case HexTypeEnum.HexTile_Digit2:
                defaultAttributeValues = new DigitElementAttribute(0, 2);
                return new string[] { typeof(DigitElementAttribute).ToString() };
            case HexTypeEnum.HexTile_Digit3:
                defaultAttributeValues = new DigitElementAttribute(0, 3);
                return new string[] { typeof(DigitElementAttribute).ToString() };
            case HexTypeEnum.HexTile_Digit4:
                defaultAttributeValues = new DigitElementAttribute(0, 4);
                return new string[] { typeof(DigitElementAttribute).ToString() };
            case HexTypeEnum.HexTile_Digit5:
                defaultAttributeValues = new DigitElementAttribute(0, 5);
                return new string[] { typeof(DigitElementAttribute).ToString() };
            case HexTypeEnum.HexTile_Digit6:
                defaultAttributeValues = new DigitElementAttribute(0, 6);
                return new string[] { typeof(DigitElementAttribute).ToString() };
            case HexTypeEnum.HexTile_Digit7:
                defaultAttributeValues = new DigitElementAttribute(0, 7);
                return new string[] { typeof(DigitElementAttribute).ToString() };
            case HexTypeEnum.HexTile_Digit8:
                defaultAttributeValues = new DigitElementAttribute(0, 8);
                return new string[] { typeof(DigitElementAttribute).ToString() };
            case HexTypeEnum.HexTile_Digit9:
                defaultAttributeValues = new DigitElementAttribute(0, 9);
                return new string[]  { typeof(DigitElementAttribute).ToString() };



            case HexTypeEnum.HexTile_ClickDestroy:
                defaultAttributeValues = null;
                return allhexAttributes;
            case HexTypeEnum.HexTile_ClickIndestructible:
                defaultAttributeValues = null;
                return allhexAttributes;
            case HexTypeEnum.HexTile_ExitDestroy:
                defaultAttributeValues = null;
                return allhexAttributes;
            case HexTypeEnum.HexTile_Indestructible:
                defaultAttributeValues = null;
                return allhexAttributes;



            case HexTypeEnum.HexTile_Back:
                defaultAttributeValues = new MenuButtonElementAttribute(Command.BackMenu);
                return new string[] { typeof(MenuButtonElementAttribute).ToString() };
            case HexTypeEnum.HexTile_Info:
                defaultAttributeValues = new MenuButtonElementAttribute(Command.Info);
                return new string[] { typeof(MenuButtonElementAttribute).ToString() };
            case HexTypeEnum.HexTile_Login:
                defaultAttributeValues = new MenuButtonElementAttribute(Command.Login);
                return new string[] { typeof(MenuButtonElementAttribute).ToString() };
            case HexTypeEnum.HexTile_MenuOption:
                defaultAttributeValues = new MenuButtonElementAttribute(Command.NextMenu);
                return new string[] { typeof(MenuButtonElementAttribute).ToString() };
            case HexTypeEnum.HexTile_MenuOptionEdit:
                defaultAttributeValues = new MenuButtonElementAttribute(Command.Edit);
                return new string[] { typeof(MenuButtonElementAttribute).ToString() };
            case HexTypeEnum.HexTile_NewUser:
                defaultAttributeValues = new MenuButtonElementAttribute(Command.NewUser);
                return new string[] { typeof(MenuButtonElementAttribute).ToString() };
            case HexTypeEnum.HexTile_Null:
                defaultAttributeValues = null;
                return allhexAttributes;
            case HexTypeEnum.HexTile_Settings:
                defaultAttributeValues = new MenuButtonElementAttribute(Command.Options);
                return new string[] { typeof(MenuButtonElementAttribute).ToString() };
            case HexTypeEnum.HexTile_Skip:
                defaultAttributeValues = new MenuButtonElementAttribute(Command.Skip);
                return new string[] { typeof(MenuButtonElementAttribute).ToString() };
            default:
                defaultAttributeValues = null;
                return allhexAttributes;
        }
    }

    //ElementAttribute tempAt = System.Type.GetType("type");


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
            //default
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

    public string levelName;

    public MapElement[] hexs;

    //public int passAmount;
    //public int bronzeAmount;
    //public int silverAmount;
    //public int goldAmount;

    public Level(string name = "defaultLevelName", MapElement[] hexs = null)
    {
        levelName = name;
        this.hexs = hexs;
    }

    private readonly string saveLocation = "/Resources/Levels/Json/";
    private readonly string jsonFileName = "TestLevel";

    private string GetLevelPath()
    {
        return Application.dataPath + saveLocation + jsonFileName + ".json";
    }

    [ContextMenu("Save Json")]
    public void GetJson()
    {
        Debug.LogError("Method disabled - Json functionality/include removed from class");
        //string json = JsonConvert.SerializeObject(this); //JsonUtility.ToJson(this);

        //levelName = json;

        //File.WriteAllText(GetLevelPath(), json);
    }

    [ContextMenu("Load Json")]
    public void LoadJson()
    {
        Debug.LogError("Method disabled - Json functionality/include removed from class");

        //string json = File.ReadAllText(GetLevelPath());

        //Level loadedLevel = JsonConvert.DeserializeObject<Level>(json); // JsonUtility.FromJson<Level>(json);

        //levelName = json;
        //Debug.Log(loadedLevel.levelName);
    }
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

    public enum CollectableType
{ 
    key,
    coin
}
