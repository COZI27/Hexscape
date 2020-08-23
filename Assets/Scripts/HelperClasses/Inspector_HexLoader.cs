#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


// Loads hex prefabs from the defined directory and appends the hexTypeEnum with any new prefabs
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


                //hexBank.hexPrefabs = outHexes;

                GenerateHexTypeEnum(outHexes, "HexTypeEnum");


                EditorUtility.SetDirty(hexBank);
                serializedObject.ApplyModifiedProperties();
            }
        }
        else Debug.LogWarning("Failed to load hex prefabs. mapSpawner instance is null.");
    }


    // Replaces the enum values of the Levels MapElements hex types
    private static void ForceHexValueChange() // Use in case of emergency & Handle with care - has the potential to damage or break multiple levels
    {
        //Level[] levels = LevelLoader.Instance.GetAllLevels("Levels/Endless");

        //foreach (Level l in levels)
        //{
        //    foreach (MapElement[] m in l.hexs)
        //    for (int i = 0; i < m.Length; i++)
        //    {
        //        Debug.Log("Hex enum int = " + (int)m[i].hexType);
        //        if ((int)m[i].hexType == 3)
        //        {
        //            m[i].hexType = (HexTypeEnum)1344549066;
        //        }
        //        else if ((int)m[i].hexType == 0)
        //        {
        //            m[i].hexType = (HexTypeEnum)840749193;
        //        }
        //    }
        //}
    }


    public static void GenerateHexTypeEnum(GameObject[] hexArray, string enumName)
    {
        //string filePathAndName = "Assets/Scripts/Enums/" + enumName + ".cs"; //The folder Scripts/Enums/ is expected to exist

        //using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
        //{
        //    streamWriter.WriteLine("public enum " + enumName);
        //    streamWriter.WriteLine("{");
        //    for (int i = 0; i < hexArray.Length; i++)
        //    {
        //        if (hexArray[i] != null) streamWriter.WriteLine("\t" + hexArray[i].name.ToString() + " = " + hexArray[i].name.GetHashCode() + ",");
        //        //(HexTypeEnum)hexArray[i].name.ToString();
        //    }
        //    streamWriter.WriteLine("}");
        //}
        //AssetDatabase.Refresh();

        //for (int i = 0; i < hexArray.Length; i++)
        //{
        //    hexArray[i].GetComponent<Hex>().SetTypeOfHex( (HexTypeEnum)hexArray[i].name.GetHashCode() );
        //}
    }


}
#endif