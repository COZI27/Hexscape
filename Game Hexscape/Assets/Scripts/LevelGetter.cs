using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class LevelGetter : MonoBehaviour
{

    public static LevelGetter instance;

   [SerializeField]  public Level testLevel;

    private void Awake()
    {
        instance = this;
    }


    [ContextMenu("Force Add Child")]
    public void ForceAddChild()
    {
        testLevel.hexs[0] = new MapElement(HexTypeEnum.HexTile_Digit0, new Vector2Int(0, 0), new DigitElementAttribute(3) );
    }


    public string resourceLocation = "Levels/Json/";
    public string jsonFileName = "TestLevel";


    public  Level[] GetAllLevels()
    {
        return GetLevelsFrom(resourceLocation);
    }

    public Level[] GetAllLevels(string resourceLocation)
    {
        return GetLevelsFrom(resourceLocation);
    } 

    public Level[] GetLevelsFrom (string path)
    {
        Debug.Log("Looking Found");
        Object[] loadedJsonFiles = Resources.LoadAll(path, typeof(TextAsset));
        Debug.Log("Files Found: " + loadedJsonFiles.Length);

        Level[] levels = new Level[loadedJsonFiles.Length];

   

        for (int i = 0; i < levels.Length; i++)
        {
            levels[i] = JsonUtility.FromJson<Level>(loadedJsonFiles[i].ToString());
        }

        return levels;
    }


    private string GetLevelPath()
    {
        return  Application.dataPath + "/Resources/" + resourceLocation;
    }

    [ContextMenu("Create")]
    public void CreateLevel()
    {
        Level level = testLevel;

        string json = JsonUtility.ToJson(level);

       // level.levelName = json;

        File.WriteAllText(GetLevelPath() + jsonFileName + ".json" , json);
    }
    

    [ContextMenu("Create Newtonsoft Json")]
    public void CreateLevel_Newtonsoft()
    {
        Level level = testLevel;

        string json = JsonConvert.SerializeObject(level);



        // level.levelName = json;

        File.WriteAllText(GetLevelPath() + jsonFileName + ".json", json);
    }

    public void CreateLevel(Level level, bool autoName = false)
    {
        

        string json = JsonUtility.ToJson(level);

        // level.levelName = json;

        string lName = jsonFileName;
        if (autoName) lName = level.levelName;

        File.WriteAllText(GetLevelPath() + lName + ".json", json);

        Debug.Log(json);
    }

    


    [ContextMenu("GetLevel")]
    public void GetLevel()
    {
        string json = File.ReadAllText(GetLevelPath() + jsonFileName + ".json");

        testLevel = JsonUtility.FromJson<Level>(json);

    }





}
