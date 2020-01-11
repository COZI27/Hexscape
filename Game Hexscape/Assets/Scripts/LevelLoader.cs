using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEditor;


#if (UNITY_EDITOR) 
[ExecuteInEditMode]
#endif
public class LevelLoader : MonoBehaviour
{

    private static LevelLoader instance;
    public static LevelLoader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<LevelLoader>();
                if (instance == null) Debug.LogError("No instance of LevelLoader was found.");
            }
            return instance;
        }
    }

    [SerializeField] public Level levelBeingEdited;
    private string lastResourceLocation = "/Resources/Levels/";

    private void Awake()
    {
        lastResourceLocation = Application.dataPath + "/Resources/Levels/"; 
        instance = this;
    }

#if (UNITY_EDITOR)
    [ContextMenu("Add Hex with attribute")]
    public void AddAttributeHexToLevel()
    {
        NewHexAttributeEditorWindow.ShowhexAttributeWindow();
    }
#endif

    public void AddHexToLevel(HexTypeEnum hexType, Vector2 location)
    {
        throw new System.NotImplementedException();
    }


    public void AddHexToLevel(HexTypeEnum hexType, Vector2 location, ElementAttribute attribute)
    {
        throw new System.NotImplementedException("TODO: use hex location to modify the hex tile at that location(?)");


        //levelBeingEdited.hexs[1] = new MapElement(HexTypeEnum.HexTile_MenuOptionEdit, new Vector2Int(1, 0), new MenuButtonElementAttribute(Command.Edit));
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
        // TODO: Find way to save in builds

#if (UNITY_EDITOR)

        string saveName = "My New Level"; 

        if (level == null || level.levelName == "")
        {
            saveName = "My New Level";
        }  else
        {
            if (level != null && level.levelName != null) saveName = level.levelName.Replace(".json", "");
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
#endif
    }

#if (UNITY_EDITOR)
    [ContextMenu("Save Level File")] public void SaveLevelFile() //Inspecter overload
    {
        SaveLevelFile(levelBeingEdited);

        UpdateInspectorUI();
    }
#endif

#if (UNITY_EDITOR)

    [ContextMenu("Load Level File")] //  loads a level from a Serialised json file (prompt overload for file location)
    public Level LoadLevelFile()
    {
        string filePath = EditorUtility.OpenFilePanel("Select Level Location", lastResourceLocation, "json");

        levelBeingEdited = LoadLevelFile(filePath);

        lastResourceLocation = filePath;

        UpdateInspectorUI();

        if (levelBeingEdited == null)  lastResourceLocation = Application.dataPath + "/Resources/Levels/"; 

        return levelBeingEdited;
        
    }

#endif


    public Level LoadLevelFile(string path, bool fromResources = false) 
    {
        string appendedPath = path;
//#if UNITY_EDITOR
//        Debug.Log("UNITY_EDITOR");
//        appendedPath = Path.Combine("Assets/Resources/", path);
//#endif
//#if UNITY_ANDROID
//        Debug.Log("UNITY_ANDROID");
//        //appendedPath = Path.Combine(Application.dataPath, path);
//#endif
        if (Application.platform == RuntimePlatform.WindowsEditor)
            appendedPath = Path.Combine("Assets/Resources/", path);

        if (Application.platform == RuntimePlatform.Android)
            appendedPath = Path.Combine("file://", Application.dataPath, "!/assets/", path);
        //"jar:file://" + Application.dataPath + "!/assets/alphabet.txt";


            // TODO: store datapath globally for ready access. Can be assigned on init

        if (fromResources) // goes from the Resources folder if true... might make an enum with an automatic mode too
        {
            //appendedPath = "file://" + Application.dataPath + path;
            //Debug.Log("Application.DataPath =  " + Application.dataPath);
        }
        

        GameObject TEMPOBJ = GameObject.Find("DebugText");
        if (TEMPOBJ != null) TEMPOBJ.GetComponent<Text>().text = "Found Debug Text";


        TextAsset levelText = Resources.Load<TextAsset>(path);
        if (levelText != null)
        {
            if (TEMPOBJ != null) TEMPOBJ.GetComponent<Text>().text = "levelText found!";

        }



        levelText = Resources.Load<TextAsset>(path);
        levelText = (TextAsset)Resources.Load(path, typeof(TextAsset));

        if (levelText != null)
        {
            if (TEMPOBJ != null) TEMPOBJ.GetComponent<Text>().text = "levelText found!";
            string dataAsJson = levelText.ToString();//File.ReadAllText(path);



            if (dataAsJson != null)
            {
                Debug.Log("dataAsJson != null");



                Level returnLevel = DeserialisLevelFromJsonFile(dataAsJson);
                if (returnLevel != null)
                {

                    if (TEMPOBJ != null) TEMPOBJ.GetComponentInChildren<Text>().text = "Found Level";
                    return returnLevel;
                }

                else
                {
                    Debug.Log("Failed to convert Json to Level at path: " + path);
                    if (TEMPOBJ != null) TEMPOBJ.GetComponentInChildren<Text>().text = "Failed to convert Json to Level at path: " + path;
                }
            }
            else
            {
                Debug.Log("Failed to load Json from path: " + path);
                if (TEMPOBJ != null) TEMPOBJ.GetComponentInChildren<Text>().text = "Failed to load Json from path: " + path;
            }

        }
        else
        {
            Debug.Log("No valid file found at path: " + path);
            if (TEMPOBJ != null) TEMPOBJ.GetComponentInChildren<Text>().text = "No file found at path: " + path;
        }


        //if (System.IO.File.Exists(appendedPath))
        //{
        //    Debug.Log("System.IO.File.Exists(path)");
        //    string dataAsJson = File.ReadAllText(appendedPath);

        //    ///TextAsset loadedJson = (TextAsset)Resources.Load(path, typeof(TextAsset));
        //    if (dataAsJson != null)
        //    {
        //        Debug.Log("dataAsJson != null");

        //        Level returnLevel = DeserialisLevelFromJsonFile(dataAsJson);
        //        if (returnLevel != null)
        //        {
        //            if (TEMPOBJ != null) TEMPOBJ.GetComponentInChildren<Text>().text = "Found Level";
        //            return returnLevel;
        //        }

        //        else
        //        {
        //            Debug.Log("Failed to convert Json to Level at path: " + appendedPath);
        //            if (TEMPOBJ != null) TEMPOBJ.GetComponentInChildren<Text>().text = "Failed to convert Json to Level at path: " + path;
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("Failed to load Json from path: " + appendedPath);
        //        if (TEMPOBJ != null) TEMPOBJ.GetComponentInChildren<Text>().text = "Failed to load Json from path: " + appendedPath;
        //    }
        //}
        //else
        //{
        //    Debug.Log("No file found at path: " + appendedPath);
        //    if (TEMPOBJ != null) TEMPOBJ.GetComponentInChildren<Text>().text = "No file found at path: " + appendedPath;
        //}



       
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

#if (UNITY_EDITOR)
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
#endif











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










