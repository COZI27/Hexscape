using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

// This is for my Level Database, if you right click  in the project tap and press create you can create a new level...
//Not sure if we will use a level database for endless but at the moment we are... 
// we might also want to seperate endless and challenge levels into diffrent children of the Level class

[System.Serializable]
public class Level
{
    public string levelName;
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
    public Vector2Int gridPos;
    public HexTypeEnum hexType;

    [SerializeField]
    public int class1Field = 1;

    [SerializeField]
    public ElementAttrubute hexAttribute = null;

    public Hex GetHex ()
    {
        return HexBank.instance.GetHexFromType(hexType); ;
    }

    public MapElement(HexTypeEnum hexType, Vector2Int gridPos, ElementAttrubute hexAttributes = null)
    {
        this.hexType = hexType;
        this.gridPos = gridPos;
        this.hexAttribute = hexAttributes;
    }
}



[System.Serializable]
public class DigitElementAttribute : ElementAttrubute
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
public class MenuButtonElementAttribute : ElementAttrubute
{
    public MenuButtonElementAttribute(GameManager.Command commandToCall)
    {
        this.commandToCall = commandToCall;
    }

    [SerializeField]
    public GameManager.Command commandToCall;

    public override void AddAttributeToHex(Hex hexInstance)
    {
        hexInstance.clickedEvent.AddListener(() =>
        {
            //HandleRegisterClick();
            GameManager.instance.ProcessCommand(commandToCall);
            hexInstance.DestroyHex();
        });
    }
}

[System.Serializable]
public abstract class ElementAttrubute {
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

