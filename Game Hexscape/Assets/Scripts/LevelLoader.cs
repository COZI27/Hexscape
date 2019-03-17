using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;



public class LevelLoader : MonoBehaviour
{

    public static LevelLoader instance;

   [SerializeField]  public Level testLevel;

    private void Awake()
    {
        instance = this;
    }


    [ContextMenu("Force Add Child")]
    public void ForceAddChild()
    {
        testLevel.levelName = "scoreboardLevel";

        testLevel.hexs = new MapElement[3];

        testLevel.hexs[0] = new MapElement(HexTypeEnum.HexTile_Digit0, new Vector2Int(0, 2), new DigitElementAttribute(3) );
        testLevel.hexs[1] = new MapElement(HexTypeEnum.HexTile_Digit0, new Vector2Int(0, 0), new DigitElementAttribute(5));
        testLevel.hexs[2] = new MapElement(HexTypeEnum.HexTile_MenuOptionPlay, new Vector2Int(1, -2), new MenuButtonElementAttribute(GameManager.Command.NextMenu));
    }


    public string resourceLocation = "Levels/Json/";

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

    public Level LoadLevelAtPath(string path)
    {
        if (File.Exists(path))
        {

            string dataAsJson = File.ReadAllText(path);

            ///TextAsset loadedJson = (TextAsset)Resources.Load(path, typeof(TextAsset));
            if (dataAsJson != null)
            {

                Level returnLevel = JsonConvert.DeserializeObject<Level>(dataAsJson, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All } );
                if (returnLevel != null)
                    return returnLevel;

                else Debug.LogWarning("Failed to convert Json to Level at path: " + path);
            }
            else Debug.LogWarning("Failed to load Json from path: " + path);
        }
        else Debug.LogWarning("No file found at path: " + path);
        return null;
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

        File.WriteAllText(GetLevelPath() + level.levelName + ".json" , json);
    }
    

    [ContextMenu("Create Newtonsoft Json")]
    public void CreateLevel_Newtonsoft()
    {
        Level level = testLevel;

        string json = JsonConvert.SerializeObject(level, Formatting.Indented, new JsonSerializerSettings()  { TypeNameHandling = TypeNameHandling.All } );



        // level.levelName = json;

        File.WriteAllText(GetLevelPath() + level.levelName + ".json", json);
    }

    public void CreateLevel(Level level, bool autoName = false)
    {
        

        string json = JsonUtility.ToJson(level);

        // level.levelName = json;

        string lName = level.levelName;
        if (autoName) lName = level.levelName;

        File.WriteAllText(GetLevelPath() + lName + ".json", json);

        Debug.Log(json);
    }

    


    [ContextMenu("GetLevel")]
    public void GetLevel()
    {
        string json = File.ReadAllText(GetLevelPath() + "levelName" + ".json");

        testLevel = JsonUtility.FromJson<Level>(json);

    }





}
