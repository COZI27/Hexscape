using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;

public class LevelLoader : MonoBehaviour
{

    public static LevelLoader instance;

    [SerializeField] public Level levelBeingEdited;
    private string lastResourceLocation = "/Resources/Levels/";

    private void Awake()
    {
        instance = this;
    }


    [ContextMenu("Force Add Child")]
    public void ForceAddChild()
    {
       // levelBeingEdited.levelName = "LoadProfileMenuLevel"; // what is the renaming for? 

        // testLevel.hexs = new MapElement[1];
        levelBeingEdited.hexs[0] = new MapElement(HexTypeEnum.HexTile_MenuOptionPlay, new Vector2Int(-1, 0), new MenuButtonElementAttribute(Command.Begin));
        levelBeingEdited.hexs[1] = new MapElement(HexTypeEnum.HexTile_MenuOptionEdit, new Vector2Int(1, 0), new MenuButtonElementAttribute(Command.Edit));


        UpdateInspectorUI();
    }


    

    public Level[] GetAllLevels()
    {
        return GetLevelsFrom("/Resources/Levels/");
    }

    public Level[] GetAllLevels(string resourceLocation)
    {
        return GetLevelsFrom(resourceLocation);
    }

    public Level[] GetLevelsFrom(string path)
    {
        Debug.Log("Looking for level files...");
        Object[] loadedJsonFiles = Resources.LoadAll(path, typeof(TextAsset));
        Debug.Log("Files Found: " + loadedJsonFiles.Length);

        Level[] levels = new Level[loadedJsonFiles.Length];

        
        for (int i = 0; i < levels.Length; i++)
        {
           
           levels[i] = DeserialisLevelFromJsonFile(loadedJsonFiles[i]);
        }

        return levels;
    }

    public Level DeserialisLevelFromJsonFile(Object jsonFile)
    {
        return DeserialisLevelFromJsonFile(jsonFile.ToString());
    } // Deserialises a json file to retrun a level
    public Level DeserialisLevelFromJsonFile(string jsonText) 
    {
        return JsonConvert.DeserializeObject<Level>(jsonText, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
    }

    public string SerializeLevelToJson (Level level)
    {
       return JsonConvert.SerializeObject(level, Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
    }



   


    private string GetLevelPath()
    {
        return Application.dataPath + "/Resources/" + lastResourceLocation;


    }

   // [ContextMenu("Create")]
    //public void CreateLevel()
    //{
    //    Level level = levelBeingEdited;

    //    string json = JsonUtility.ToJson(level);

    //    // level.levelName = json;

    //    File.WriteAllText(GetLevelPath() + level.levelName + ".json", json);
    //}

    

    // [ContextMenu("Create Newtonsoft Json")]
    //public void CreateLevel_Newtonsoft()
    //{
    //    Level level = levelBeingEdited;
    //    string json = SerializeLevelToJson(level);


    //    File.WriteAllText(GetLevelPath() + level.levelName + ".json", json);
    //}



    
    public void SaveLevelFile (Level level)  // Prompts you to save the level as Serialised json
    {
        string saveName = "My New Level"; 

        if (level == null || level.levelName == "")
        {
            saveName = "My New Level";
        }  else
        {
            saveName = level.levelName.Replace(".json", "");
        }

        string fileLocation = EditorUtility.SaveFilePanel("Select Level Location", lastResourceLocation, saveName,  "json");

        string levelFileName;
        string[] splitPath = fileLocation.Split('/');
        levelFileName = splitPath[splitPath.Length - 1];
        levelFileName = levelFileName.Replace(".json", "");

        level.levelName = levelFileName;

        string json = SerializeLevelToJson(level);
       
        lastResourceLocation = fileLocation;

        File.WriteAllText(fileLocation, json); 
    } 
    [ContextMenu("Save Level File")] public void SaveLevelFile() //Inspecter overload
    {
        SaveLevelFile(levelBeingEdited);

        UpdateInspectorUI();
    } 


    [ContextMenu("Load Level File")] //  loads a level from a Serialised json file (prompt overload for file location)
    public Level LoadLevelFile()
    {
        string filePath = EditorUtility.OpenFilePanel("Select Level Location", lastResourceLocation, "json");

        levelBeingEdited = LoadLevelFile(filePath);

        lastResourceLocation = filePath;

        UpdateInspectorUI();

        return levelBeingEdited;
        
    }

    
    public Level LoadLevelFile(string path, bool fromResources = false) 
    {
        if (fromResources) // goes from the Resources folder if true... might make an enum with an automatic mode too
        {
            path = Application.dataPath + "/Resources/" + path;
        }

        if (File.Exists(path))
        {

            string dataAsJson = File.ReadAllText(path);

            ///TextAsset loadedJson = (TextAsset)Resources.Load(path, typeof(TextAsset));
            if (dataAsJson != null)
            {

                Level returnLevel = DeserialisLevelFromJsonFile(dataAsJson);
                if (returnLevel != null)


                    return returnLevel;

                else Debug.LogWarning("Failed to convert Json to Level at path: " + path);
            }
            else Debug.LogWarning("Failed to load Json from path: " + path);
        }
        else Debug.LogWarning("No file found at path: " + path);

       
        return null;


        
    }





    //// [ContextMenu("Select File Location")]
    //public void GetFileLocationPanel()
    //{
    //    string path = GetLevelPath();
    //    path = EditorUtility.OpenFolderPanel("Select Level Location", lastResourceLocation, "");
   
    //    lastResourceLocation = path;

    //    Debug.Log("resouce location set to " + path);
    //}


    private void OnValidate()
    {
        UpdateInspectorUI();

    }




    private void UpdateInspectorUI ()
    {
        foreach (MapElement mapElements in levelBeingEdited.hexs)
        {
            mapElements.UpdateDisplayName();
        }
    }











    //  private string SimplifyFilePath(string complexPath) // Application.dataPath/Resources/Levels/Test/  = /Levels/Test/
    //{
    //    string[] splitPath = complexPath.Split(new string[] { Application.dataPath + "/Resources/" }, System.StringSplitOptions.None);
    //    if (splitPath.Length < 1)
    //    {
    //        Debug.LogError("Location is not a child of the resource folder!!!");
    //        return null;
    //    }
    //    else
    //    {
    //        return splitPath[1];
    //    }


    //}   


}










