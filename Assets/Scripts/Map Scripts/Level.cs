using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;





// This is for my Level Database, if you right click  in the project tap and press create you can create a new level...
//Not sure if we will use a level database for endless but at the moment we are... 
// we might also want to seperate endless and challenge levels into diffrent children of the Level class

//public static class HexTypes
//{
//    public static bool IsMenuHexType(HexTypeEnum type)
//    {
//        if (new[] {
//            HexTypeEnum.HexTile_Back,
//            HexTypeEnum.HexTile_Info,
//            HexTypeEnum.HexTile_Login,
//            HexTypeEnum.HexTile_MenuOption,
//            HexTypeEnum.HexTile_MenuOptionEdit,
//            HexTypeEnum.HexTile_NewUser,
//            HexTypeEnum.HexTile_Null,
//            HexTypeEnum.HexTile_Settings,
//            HexTypeEnum.HexTile_Skip
//        }.Contains(type))
//        {
//            return true;
//        }
//        else return false;          
//    }

//    public static bool IsDigitType(HexTypeEnum type)
//    {
//        if (new[] {
//            HexTypeEnum.HexTile_Digit0,
//            HexTypeEnum.HexTile_Digit1,
//            HexTypeEnum.HexTile_Digit2,
//            HexTypeEnum.HexTile_Digit3,
//            HexTypeEnum.HexTile_Digit4,
//            HexTypeEnum.HexTile_Digit5,
//            HexTypeEnum.HexTile_Digit6,
//            HexTypeEnum.HexTile_Digit7,
//            HexTypeEnum.HexTile_Digit8,
//            HexTypeEnum.HexTile_Digit9
//        }.Contains(type))
//        {
//            return true;
//        }
//        else return false;
//    }

//    public static bool IsPlayType(HexTypeEnum type)
//    {
//        if (new[] {
//            HexTypeEnum.HexTile_ClickDestroy,
//            HexTypeEnum.HexTile_ExitDestroy,
//            HexTypeEnum.HexTile_Indestructible,
//        }.Contains(type))
//        {
//            return true;
//        }
//        else return false;
//    }

//    //public static string[] GetCompatibleAttrributes(this HexTypeEnum hexType, out ElementAttribute defaultAttributeValues)
//    //{

//    //    switch (hexType)
//    //    {
//    //        case HexTypeEnum.HexTile_Digit0:
//    //            defaultAttributeValues = new DigitElementAttribute(0, 0);
//    //            return new string[] { typeof(DigitElementAttribute).ToString() };
//    //        case HexTypeEnum.HexTile_Digit1:
//    //            defaultAttributeValues = new DigitElementAttribute(0, 1);
//    //            return new string[] { typeof(DigitElementAttribute).ToString() };
//    //        case HexTypeEnum.HexTile_Digit2:
//    //            defaultAttributeValues = new DigitElementAttribute(0, 2);
//    //            return new string[] { typeof(DigitElementAttribute).ToString() };
//    //        case HexTypeEnum.HexTile_Digit3:
//    //            defaultAttributeValues = new DigitElementAttribute(0, 3);
//    //            return new string[] { typeof(DigitElementAttribute).ToString() };
//    //        case HexTypeEnum.HexTile_Digit4:
//    //            defaultAttributeValues = new DigitElementAttribute(0, 4);
//    //            return new string[] { typeof(DigitElementAttribute).ToString() };
//    //        case HexTypeEnum.HexTile_Digit5:
//    //            defaultAttributeValues = new DigitElementAttribute(0, 5);
//    //            return new string[] { typeof(DigitElementAttribute).ToString() };
//    //        case HexTypeEnum.HexTile_Digit6:
//    //            defaultAttributeValues = new DigitElementAttribute(0, 6);
//    //            return new string[] { typeof(DigitElementAttribute).ToString() };
//    //        case HexTypeEnum.HexTile_Digit7:
//    //            defaultAttributeValues = new DigitElementAttribute(0, 7);
//    //            return new string[] { typeof(DigitElementAttribute).ToString() };
//    //        case HexTypeEnum.HexTile_Digit8:
//    //            defaultAttributeValues = new DigitElementAttribute(0, 8);
//    //            return new string[] { typeof(DigitElementAttribute).ToString() };
//    //        case HexTypeEnum.HexTile_Digit9:
//    //            defaultAttributeValues = new DigitElementAttribute(0, 9);
//    //            return new string[]  { typeof(DigitElementAttribute).ToString() };



//    //        case HexTypeEnum.HexTile_ClickDestroy:
//    //            defaultAttributeValues = null;
//    //            return allhexAttributes;
//    //        case HexTypeEnum.HexTile_ClickIndestructible:
//    //            defaultAttributeValues = null;
//    //            return allhexAttributes;
//    //        case HexTypeEnum.HexTile_ExitDestroy:
//    //            defaultAttributeValues = null;
//    //            return allhexAttributes;
//    //        case HexTypeEnum.HexTile_Indestructible:
//    //            defaultAttributeValues = null;
//    //            return allhexAttributes;



//    //        case HexTypeEnum.HexTile_Back:
//    //            defaultAttributeValues = new MenuButtonElementAttribute(Command.BackMenu);
//    //            return new string[] { typeof(MenuButtonElementAttribute).ToString() };
//    //        case HexTypeEnum.HexTile_Info:
//    //            defaultAttributeValues = new MenuButtonElementAttribute(Command.Info);
//    //            return new string[] { typeof(MenuButtonElementAttribute).ToString() };
//    //        case HexTypeEnum.HexTile_Login:
//    //            defaultAttributeValues = new MenuButtonElementAttribute(Command.Login);
//    //            return new string[] { typeof(MenuButtonElementAttribute).ToString() };
//    //        case HexTypeEnum.HexTile_MenuOption:
//    //            defaultAttributeValues = new MenuButtonElementAttribute(Command.NextMenu);
//    //            return new string[] { typeof(MenuButtonElementAttribute).ToString() };
//    //        case HexTypeEnum.HexTile_MenuOptionEdit:
//    //            defaultAttributeValues = new MenuButtonElementAttribute(Command.Edit);
//    //            return new string[] { typeof(MenuButtonElementAttribute).ToString() };
//    //        case HexTypeEnum.HexTile_NewUser:
//    //            defaultAttributeValues = new MenuButtonElementAttribute(Command.NewUser);
//    //            return new string[] { typeof(MenuButtonElementAttribute).ToString() };
//    //        case HexTypeEnum.HexTile_Null:
//    //            defaultAttributeValues = null;
//    //            return allhexAttributes;
//    //        case HexTypeEnum.HexTile_Settings:
//    //            defaultAttributeValues = new MenuButtonElementAttribute(Command.Options);
//    //            return new string[] { typeof(MenuButtonElementAttribute).ToString() };
//    //        case HexTypeEnum.HexTile_Skip:
//    //            defaultAttributeValues = new MenuButtonElementAttribute(Command.Skip);
//    //            return new string[] { typeof(MenuButtonElementAttribute).ToString() };
//    //        default:
//    //            defaultAttributeValues = null;
//    //            return allhexAttributes;
//    //    }
//    //}

//    //ElementAttribute tempAt = System.Type.GetType("type");


////public static string[] GetCompatibleAttrributes(this HexTypeEnum hexType)
////{

////    switch (hexType)
////    {
////        case HexTypeEnum.HexTile_Digit0:
////        case HexTypeEnum.HexTile_Digit1:
////        case HexTypeEnum.HexTile_Digit2:
////        case HexTypeEnum.HexTile_Digit3:
////        case HexTypeEnum.HexTile_Digit4:
////        case HexTypeEnum.HexTile_Digit5:
////        case HexTypeEnum.HexTile_Digit6:
////        case HexTypeEnum.HexTile_Digit7:
////        case HexTypeEnum.HexTile_Digit8:
////            //default
////            return new string[]  {
////                       typeof(DigitElementAttribute).ToString(),
////                };

////        case HexTypeEnum.HexTile_MenuOptionEdit:
////            return new string[]  {
////                       typeof(MenuButtonElementAttribute).ToString()
////                };
////        default:
////            return allhexAttributes;
////    }
////}

//    //ElementAttribute tempAt = System.Type.GetType("type");
//}




[System.Serializable]
public class Level
{
    #region Serializable Data
    public string levelName;

    public MapElement[][] hexs;// Contains the information for each layer containing each of the elements to spawn

    public int gridRadius; // Used to set the dimensions of the grid

    public int startLayer; // the layer on which the player will spawn

    public Vector2Int playerStartIndex;

    #endregion

    public Level(string name = "defaultLevelName", MapElement[][] hexs = null, int gridRadius = 8, int startLayer = 0)
    {
        levelName = name;
        this.hexs = hexs;
        this.gridRadius = gridRadius;
        this.startLayer = startLayer;
    }

    private readonly string saveLocation = "/Resources/Levels/Json/";
    private readonly string jsonFileName = "TestLevel";

    private string GetLevelPath()
    {
        return Application.dataPath + saveLocation + jsonFileName + ".json";
    }
}

// This class's data is serialised in order to store player progress - this is used to display
[System.Serializable]
public class LevelProgressData {
    int score; // score for the level  -also used to display score
    public int[] collectedKeyIds; // used to check whether a key has been collected when spawning pickups. Also used to display level info



    // path = binaryFilePath + "/" + levelName
    //keyID = name.int.Parse(other.gameObject.name + gridLoc.ToString + floor/tier.ToString);
}