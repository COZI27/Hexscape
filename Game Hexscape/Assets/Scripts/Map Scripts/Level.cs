using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
        string json = JsonUtility.ToJson(this);

        levelName = json;

        File.WriteAllText(GetLevelPath(), json);
    }

    [ContextMenu("Load Json")]
    public void LoadJson()
    {
        string json = File.ReadAllText(GetLevelPath());

        Level loadedLevel = JsonUtility.FromJson<Level>(json);

        levelName = json;
        Debug.Log(loadedLevel.levelName);
    }
}



[System.Serializable]
public class MapElement
{
    public Vector2Int gridPos;
    public HexTypeEnum hexType;
   // protected Hex hex;

    public Hex GetHex ()
    {
        return HexBank.instance.GetHexFromType(hexType); ;
    }

    public MapElement(HexTypeEnum hexType, Vector2Int gridPos)
    {
        this.hexType = hexType;
        this.gridPos = gridPos;
    }
}

[System.Serializable]
public class HexButtonElement : MapElement
{

    GameManager.Command commandToCall;

    public HexButtonElement(HexTypeEnum hexType, Vector2Int gridPos, GameManager.Command commandToCall) : base (hexType, gridPos)
    {
        this.commandToCall = commandToCall;
    }

}
