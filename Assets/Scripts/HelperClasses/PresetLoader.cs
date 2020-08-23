using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class PresetLoader
{

    private Dictionary<string, MapElement> loadedPresetData;

    private string databasePath = "Database/HexPresets";

    public delegate void OnDataModify(string[] dataKeys);
    public event OnDataModify WhenDataModify;

    public PresetLoader ()
    {
        LoadPresetData();    
    }

    public string[] GetDataKeys()
    {
        if (loadedPresetData.Count > 0)
        {
            return loadedPresetData.Keys.ToArray();
        }
        else return new string[1] { " " };
    }


    public bool SavePreset(MapElement preset)
    {
        bool result = SavePresetData(preset);

        return result;
    }


    private bool SavePresetData(MapElement preset)
    {
        TextAsset data = (TextAsset)Resources.Load(databasePath, typeof(TextAsset));
        if (data != null)
        {
            List<MapElement> foundPresets = JsonConvert.DeserializeObject<List<MapElement>>(data.ToString(), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });
            if (foundPresets != null)
            {

                int index = foundPresets.FindIndex(ind => ind.Equals(preset));
                if (index > -1)
                {
                    foundPresets[index] = preset;
                }
                else // Add new arg
                {
                    foundPresets.Add(preset);
                }

                //TODO: Sort by Name/
                //TODO: If name does not contain slash, then add one by default
                //foundPresets = SortArguments(foundPresets);

                JsonConvert.SerializeObject(foundPresets, Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });
                string json = JsonConvert.SerializeObject(foundPresets, Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });

                File.WriteAllText(Application.dataPath + "/Resources/" + databasePath + ".json", json);
            }
            else
            {

                List<MapElement> newArgs = new List<MapElement>();
                newArgs.Add(preset);

                string json = JsonConvert.SerializeObject(newArgs, Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });

                File.WriteAllText(Application.dataPath + "/Resources/" + databasePath + ".json", json);
            }

            LoadPresetData();
            return true;
        }
        else return false;
    }

    public MapElement LoadPreset(string presetName)
    {
        if (loadedPresetData.ContainsKey(presetName))
        {
            return loadedPresetData[presetName];
        }
        else return null;
    }

    private void LoadPresetData()
    {

        AssetDatabase.Refresh();
        TextAsset data = (TextAsset)Resources.Load(databasePath, typeof(TextAsset));

        if (data == null) Debug.Log("Data is null");

        loadedPresetData = new Dictionary<string, MapElement>();
        List<MapElement> loadedData = JsonConvert.DeserializeObject<List<MapElement>>(data.ToString(), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
        if (loadedData != null)
        {
            foreach(MapElement e in loadedData)
            {
                if (e.displayName != null && !loadedPresetData.ContainsKey(e.displayName))
                {
                    loadedPresetData.Add(e.displayName, e);
                }

            }

        }

        if (WhenDataModify != null)
            WhenDataModify(GetDataKeys());
    }


    public void RemovePreset(string presetName)
    {
        if (loadedPresetData.ContainsKey(presetName))
        {
            loadedPresetData.Remove(presetName);

            List<MapElement> dataToSerialise = loadedPresetData.Values.ToList();

            string json = JsonConvert.SerializeObject(dataToSerialise, Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });

            File.WriteAllText(Application.dataPath + "/Resources/" + databasePath + ".json", json);

            LoadPresetData();
        }
        else return;
    }

}
