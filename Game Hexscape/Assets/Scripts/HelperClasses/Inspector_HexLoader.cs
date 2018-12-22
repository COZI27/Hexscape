#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(MapSpawner))]
public class Inspector_HexLoader : Editor
{

    public string directoryToLoad = "Prefabs/HexPrefabs";

    MapSpawner mapSpawner;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Reload Hex Tiles"))
        {
            LoadHexTiles();
        }
    }


    void LoadHexTiles()
    {
        mapSpawner = ((MonoBehaviour)target).gameObject.GetComponent<MapSpawner>();
        if (mapSpawner != null)
        {

            GameObject[] loadedHexes = Resources.LoadAll<GameObject>(directoryToLoad.ToString());
            GameObject[] outHexes = new GameObject[loadedHexes.Length];
            for (int i = 0; i < loadedHexes.Length; i++)
            {
               // if (loadedHexes[i].GetComponent<Hex>())
               // {
                    outHexes[i] = loadedHexes[i];
               // }
            }

            Debug.Log("Loaded Hex Object count = " + loadedHexes.Length);
            //mapSpawner.PopulateSoundEffectsArray(loadedClips, Application.isEditor);

            GenerateHexTypeEnum(outHexes, "HexTypeEnum");
        }
        else Debug.LogWarning("Failed to load hex prefabs. mapSpawner instance is null.");
    }


    public static void GenerateHexTypeEnum(GameObject[] hexArray, string enumName)
    {
        string filePathAndName = "Assets/Scripts/Enums/" + enumName + ".cs"; //The folder Scripts/Enums/ is expected to exist

        using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
        {
            streamWriter.WriteLine("public enum " + enumName);
            streamWriter.WriteLine("{");
            for (int i = 0; i < hexArray.Length; i++)
            {
                if (hexArray[i] != null) streamWriter.WriteLine("\t" + hexArray[i].name.ToString() + ",");
            }
            streamWriter.WriteLine("}");
        }
        AssetDatabase.Refresh();
    }


}
#endif