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

                if (loadedHexes[i].layer == LayerMask.NameToLayer("Hex"))
                {
                    // if (loadedHexes[i].GetComponent<Hex>())
                    // {
                    outHexes[i] = loadedHexes[i];
                    // }
                }
            }

            Debug.Log("Loaded Hex Object count = " + loadedHexes.Length);

            Debug.Log("Accepted Hex Object count = " + outHexes.Length);
            //mapSpawner.PopulateSoundEffectsArray(loadedClips, Application.isEditor);

            HexBank hexBank = ((MonoBehaviour)target).gameObject.GetComponent<HexBank>();
            if (hexBank != null)
            {

                Undo.RecordObject(hexBank, " Changed HexBank Array");


                hexBank.hexPrefabs = outHexes;

                GenerateHexTypeEnum(outHexes, "HexTypeEnum");


                EditorUtility.SetDirty(hexBank);
                serializedObject.ApplyModifiedProperties();
            }
        }
        else Debug.LogWarning("Failed to load hex prefabs. mapSpawner instance is null.");
    }


    // Replaces the enum values of the Levels MapElements hex types
    private static void ForceHexValueChange() // Use in case of emergency & Handle with care.
    {
        Level[] levels = LevelGetter.instance.GetAllLevels();

        foreach (Level l in levels)
        {
            for (int i = 0; i < l.hexs.Length; i++)
            {
                Debug.Log("Hex enum int = " + (int)l.hexs[i].hexType);
                if ((int)l.hexs[i].hexType == 3)
                {
                    l.hexs[i].hexType = (HexTypeEnum)1344549066;
                }
                else if ((int)l.hexs[i].hexType == 0)
                {
                    l.hexs[i].hexType = (HexTypeEnum)840749193;
                }


            }
                    }
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
                if (hexArray[i] != null) streamWriter.WriteLine("\t" + hexArray[i].name.ToString() + " = " + hexArray[i].name.GetHashCode() + ",");
            }
            streamWriter.WriteLine("}");
        }
        AssetDatabase.Refresh();
    }


}
#endif