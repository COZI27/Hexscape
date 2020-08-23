using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;





// This is for my Level Database, if you right click  in the project tap and press create you can create a new level...
//Not sure if we will use a level database for endless but at the moment we are... 
// we might also want to seperate endless and challenge levels into diffrent children of the Level class

public static class HexTypes
{
    public static bool IsMenuHexType(HexTypeEnum type)
    {
        if (new[] {
            HexTypeEnum.HexTile_Back,
            HexTypeEnum.HexTile_Info,
            HexTypeEnum.HexTile_Login,
            HexTypeEnum.HexTile_MenuOption,
            HexTypeEnum.HexTile_MenuOptionEdit,
            HexTypeEnum.HexTile_NewUser,
            HexTypeEnum.HexTile_Null,
            HexTypeEnum.HexTile_Settings,
            HexTypeEnum.HexTile_Skip
        }.Contains(type))
        {
            return true;
        }
        else return false;          
    }

    public static bool IsDigitType(HexTypeEnum type)
    {
        if (new[] {
            HexTypeEnum.HexTile_Digit0,
            HexTypeEnum.HexTile_Digit1,
            HexTypeEnum.HexTile_Digit2,
            HexTypeEnum.HexTile_Digit3,
            HexTypeEnum.HexTile_Digit4,
            HexTypeEnum.HexTile_Digit5,
            HexTypeEnum.HexTile_Digit6,
            HexTypeEnum.HexTile_Digit7,
            HexTypeEnum.HexTile_Digit8,
            HexTypeEnum.HexTile_Digit9
        }.Contains(type))
        {
            return true;
        }
        else return false;
    }

    public static bool IsPlayType(HexTypeEnum type)
    {
        if (new[] {
            HexTypeEnum.HexTile_ClickDestroy,
            HexTypeEnum.HexTile_ClickIndestructible,
            HexTypeEnum.HexTile_ExitDestroy,
            HexTypeEnum.HexTile_Indestructible,
        }.Contains(type))
        {
            return true;
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

#if (UNITY_EDITOR) 
    [ReadOnly] [HideInInspector] public string displayName = null;
#endif


#if (UNITY_EDITOR)
    public void UpdateDisplayName ()
    {
        string dName = hexType.ToString() + ": " + gridPos.ToString();
        hexAttributeInfo = "";

        if (hexAttributes == null) hexAttributes = new List<ElementAttribute>();

        foreach (var hexAttribute in hexAttributes)
        {
            if (hexAttribute != null)
            {

                dName = dName.Insert(0,"(A) ");
                if (hexAttribute.GetType() == typeof(MenuButtonElementAttribute))
                {
                    MenuButtonElementAttribute yeet = hexAttribute as MenuButtonElementAttribute;
                    hexAttributeInfo.Insert(0, yeet.GetType() + ": " + yeet.commandToCall + ", ");
                }
                else
                {
                    hexAttributeInfo.Insert(0, hexAttribute.GetType() + ", ");   
                }



                // Debug.Log(hexAttribute);
            }
            else
            {
              //  dName = dName.Insert(0, "♥ ");
              //  dName = dName.Replace("HexTile_", "♥ ");
               // hexAttributeInfo = null;
            }

            displayName = dName;
        }

       
       
    }
#endif




    public Vector2Int gridPos;
    public HexTypeEnum hexType;

    [JsonProperty(PropertyName = "HexAttributes")]
    public List <ElementAttribute> hexAttributes = new List<ElementAttribute>();

    public Hex GetHex ()
    {
        return HexBank.Instance.GetHexFromType(hexType); ;
    }

    public MapElement(HexTypeEnum hexType, Vector2Int gridPos, List<ElementAttribute> hexAttributes = null)
    {
        this.hexType = hexType;
        this.gridPos = gridPos;

        if (hexAttributes == null) hexAttributes = new List<ElementAttribute>();

        this.hexAttributes = hexAttributes;
    }


#if (UNITY_EDITOR)
    [ReadOnly] public string hexAttributeInfo = null;
#endif

}



[System.Serializable]
public class DigitElementAttribute : ElementAttribute
{
    public DigitElementAttribute(int leadingZeroCount, int numberToDisplay = 0)
    {
        this.leadingZeroCount = leadingZeroCount;
    }

    [SerializeField]
    public int leadingZeroCount;

    [SerializeField]
    public int numberToDisplay;


    public override void AddAttributeToHex(Hex hexInstance)
    {
        Debug.Log("DigitElementAttribute::AddAttributeToHex");

        
        hexInstance.AddAttribute(this);
     
        

        // hexInstance.hexAttribute = this;

        // Add component?
        DigitComponent component = hexInstance.gameObject.AddComponent<DigitComponent>();
        component.leadingZeroCount = this.leadingZeroCount;
        component.numberToDisplay = this.numberToDisplay;


        // NOTE: We may want some kind of delegate/ listener system for the number to automatically update its display upon vakue change
        // https://answers.unity.com/questions/1206632/trigger-event-on-variable-change.html
        //https://answers.unity.com/questions/1021048/unity-editor-inspector-delegate-function-pointer.html
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
        hexInstance.AddAttribute(this);
        // hexInstance.hexAttribute = this;

        hexInstance.clickedEvent.AddListener(() =>
        {
            GameManager.instance.ProcessCommand(commandToCall);
            hexInstance.DigHex(false); // Temp
            
        });
    }
}

public class CollectableHolderElementAttribute : ElementAttribute
{
    
    public override void AddAttributeToHex(Hex hexInstance)
    {
        hexInstance.AddAttribute(this);
    }

    [SerializeField]
    public CollectableType collectableToSpawn;
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

    public enum CollectableType
{ 
    key,
    coin
}
