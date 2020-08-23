using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO; //temp
using System.Linq;
using UnityEditor;
using UnityEngine;





public static class IconAtlasDB
{

    [System.Serializable]
    public class AtlasData
    {
        public string atlasName;
        public string[] iconNames;
        public string emissAtlasPath;
        public string normalAtlasNormPath;


        public AtlasData()
        {

        }

        public AtlasData(string name, string[] iconNames, string emissionPath, string normalPath)
        {

            atlasName = name;
            this.iconNames = iconNames;
            emissAtlasPath = emissionPath;
            normalAtlasNormPath = normalPath;

        }

        //public AtlasData(AtlasData dataEntry)
        //{
        //    atlasName = dataEntry.atlasName;
        //    iconNames = dataEntry.iconNames;
        //    emissAtlasPath = dataEntry.emissAtlasPath;
        //    normalAtlasNormPath = dataEntry.normalAtlasNormPath;
        //}
    }



    public static void InitialiseDB()
    {
        PopulateDatabase();
    }



    private static Dictionary<string, AtlasData> DatabaseEntries;
    private static string databasePath = "Database/IconAtlasDB/";


    private static void PopulateDatabase()
    {

        DatabaseEntries = new Dictionary<string, AtlasData>();

        TextAsset[] textAssets = Resources.LoadAll<TextAsset>(databasePath);

        foreach (TextAsset t in textAssets)
        {
            string dataAsjson = t.ToString();
            AtlasData dataEntry = JsonConvert.DeserializeObject<AtlasData>(dataAsjson);

            // Check the deserialised object is valid
            if (dataEntry != null && dataEntry.atlasName != null && dataEntry.iconNames != null && dataEntry.emissAtlasPath != null && dataEntry.normalAtlasNormPath != null)
            {
                DatabaseEntries.Add(dataEntry.atlasName, dataEntry);
            }
        }
    }

    public static string[] GetDatabaseKeys()
    {
        if (DatabaseEntries == null) PopulateDatabase();
        return DatabaseEntries.Keys.ToArray();
    }

    public static string[] GetAtlasIconNames(string atlasName)
    {
        return DatabaseEntries[atlasName].iconNames;
    }

    public static Texture[] LoadAtlas(string atlasName)
    {
        if (DatabaseEntries == null) PopulateDatabase();
        if (DatabaseEntries != null)
        {
            Texture emissTexture = Resources.Load<Texture>(DatabaseEntries[atlasName].emissAtlasPath);

            Texture normalTexture = Resources.Load<Texture>(DatabaseEntries[atlasName].normalAtlasNormPath);
            if (emissTexture != null && normalTexture != null)
            {
                return new Texture[2]
                {
                emissTexture,
                normalTexture
                };
            }
            else return null;
        }
        else return null;
        
    }




    public static void SerialiseNewEntry() // Temproary Class for writing data to folder
    {

        AtlasData data = new AtlasData(
            "Menu_Icons_01",
            new string[16] {
            "Play",
            "Pause",
            "Power",
            "Skip",
            "Settings",
            "Options",
            "Information",
            "Unlocks",
            "Login",
            "New User",
            "_",
            "_",
            "_",
            "_",
            "_",
            "_"
            },
            "Textures/Sheets/IconSheet_Menu",
            "Textures/Sheets/IconSheet_Menu_Normal"
            );







        Debug.Log(data.atlasName + ", " + data.iconNames.Length);


        string json = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });

        //string fileLocation = Application.dataPath + "/Resources/"  + databasePath + "/" + data.atlasName;
        string lastResourceLocation = Application.dataPath + "/Resources/Databse";
        string fileLocation = EditorUtility.SaveFilePanel("Select DB Location", lastResourceLocation, data.atlasName, "json");

        File.WriteAllText(fileLocation, json);

    }


}





public static class AtlasMatConstants
{
    public const string TILE_INDEX_REF = "TileIndex";
    public const string ICON_COLOUR_REF = "IconColour";
    public const string ICON_ALPHA_REF = "IconAlpha";
    public const string BASE_COLOUR_REF = "BaseColour";
    public const string BASE_ALPHA_REF = "BaseAlpha";
    public const string ICON_ROT_REF = "IconRotation";
}

[ExecuteInEditMode]
public class HexMatComponent : MonoBehaviour
{

    Material hexMat;
    Material tempMaterial;

    private string currentLoadedAtlasName;


    public HexMatComponent()
    {

    }





    [ExecuteInEditMode]
    void Start()
    {
#if UNITY_EDITOR
        hexMat =  new Material(GetComponent<MeshRenderer>().sharedMaterials[1]);
        GetComponent<MeshRenderer>().sharedMaterials[1] = hexMat;
        hexMat  = GetComponent<MeshRenderer>().sharedMaterials[1];
#endif

        if (hexMat == null) hexMat = this.GetComponent<MeshRenderer>().materials[1];


        IconAtlasDB.InitialiseDB(); //TEMP!
        
    }

    [ExecuteInEditMode]
    private void OnEnable()
    {

        if (Application.isPlaying)
        {
             hexMat = this.GetComponent<MeshRenderer>().materials[1];
        }
        else if (Application.isEditor)
        {
            hexMat = new Material(GetComponent<MeshRenderer>().sharedMaterials[1]);
            GetComponent<MeshRenderer>().sharedMaterials[1] = hexMat;
            hexMat = GetComponent<MeshRenderer>().sharedMaterials[1];
        }


    }

    void Update()
    {

    }

    public void SetEmissionColour(Color col)
    {
        if (hexMat != null)
        {
            if (Application.isPlaying)
            {
                hexMat.SetColor(AtlasMatConstants.BASE_COLOUR_REF, col);
                hexMat.SetColor(AtlasMatConstants.ICON_COLOUR_REF, col);
            }
            else // In Editor mode- require for the level editor
            {
                MeshRenderer renderer = GetComponent<MeshRenderer>();

                Material[] newMaterials = new Material[2];
                newMaterials[0] = new Material(renderer.sharedMaterials[0]);
                newMaterials[1] = new Material(renderer.sharedMaterials[1]);
               
                newMaterials[1].SetColor(AtlasMatConstants.BASE_COLOUR_REF, col);
                newMaterials[1].SetColor(AtlasMatConstants.ICON_COLOUR_REF, col);
                renderer.sharedMaterials = newMaterials;
            }
        }
    }

    public void SetEmissionColour(Color? baseColour = null, Color? iconColour = null)
    {
        if (hexMat != null)
        {
            if (Application.isPlaying)
            {
                if (baseColour != null)              
                    hexMat.SetColor(AtlasMatConstants.BASE_COLOUR_REF, baseColour.Value);
                if (iconColour != null)
                    hexMat.SetColor(AtlasMatConstants.ICON_COLOUR_REF, iconColour.Value);
            }
            else // In Editor mode- require for the level editor
            {
                MeshRenderer renderer = GetComponent<MeshRenderer>();

                Material[] newMaterials = new Material[2];
                newMaterials[0] = new Material(renderer.sharedMaterials[0]);
                newMaterials[1] = new Material(renderer.sharedMaterials[1]);
                if (baseColour != null)
                    newMaterials[1].SetColor(AtlasMatConstants.BASE_COLOUR_REF, baseColour.Value);
                if (iconColour != null)
                    newMaterials[1].SetColor(AtlasMatConstants.ICON_COLOUR_REF, iconColour.Value);
                renderer.sharedMaterials = newMaterials;
            }
        }

    }

    public void SetIconRotation(float rotValue)
    {
        if (hexMat != null)
        {
            if (Application.isPlaying)
                hexMat.SetFloat(AtlasMatConstants.ICON_ROT_REF, rotValue);
            else
            {
                MeshRenderer renderer = GetComponent<MeshRenderer>();

                Material[] newMaterials = new Material[2];
                newMaterials[0] = new Material(renderer.sharedMaterials[0]);
                newMaterials[1] = new Material(renderer.sharedMaterials[1]);
                newMaterials[1].SetFloat(AtlasMatConstants.ICON_ROT_REF, rotValue);
                renderer.sharedMaterials = newMaterials;
            }
        }
    }

    public void SetAtlas(string atlasName)
    {
        if (hexMat != null)
        {
            if (atlasName != currentLoadedAtlasName)
            {
                Texture[] textures = IconAtlasDB.LoadAtlas(atlasName);
                if (Application.isPlaying)
                {
                    if (textures[0] != null)
                        hexMat.SetTexture("IconAtlas", textures[0]);
                    if (textures[1] != null)
                        hexMat.SetTexture("IconAtlasNormal", textures[1]);
                }
                else // In Editor mode- require for the level editor
                {
                    MeshRenderer renderer = GetComponent<MeshRenderer>();

                    Material[] newMaterials = new Material[2];
                    newMaterials[0] = new Material(renderer.sharedMaterials[0]);
                    newMaterials[1] = new Material(renderer.sharedMaterials[1]);


                    if (textures[0] != null)
                        newMaterials[1].SetTexture("IconAtlas", textures[0]);
                    if (textures[1] != null)
                        newMaterials[1].SetTexture("IconAtlasNormal", textures[1]);
                    renderer.sharedMaterials = newMaterials;
                }
            }
        }
    }

    public void SetAtlas(Texture[] textures)
    {
        if (hexMat != null)
        {
            hexMat.SetTexture("IconAtlas", textures[0]);
            hexMat.SetTexture("IconAtlasNormal", textures[1]);
        }
    }

    public void SetMaterialIcon(int iconIndex)
    {
        if (hexMat != null)
        {

            if (Application.isPlaying)
            {
                hexMat.SetInt(AtlasMatConstants.TILE_INDEX_REF, iconIndex);
            }
            else // In Editor mode- require for the level editor
            {
                MeshRenderer renderer = GetComponent<MeshRenderer>();

                Material[] newMaterials = new Material[2];
                newMaterials[0] = new Material(renderer.sharedMaterials[0]);
                newMaterials[1] = new Material(renderer.sharedMaterials[1]);

                newMaterials[1].SetInt(AtlasMatConstants.TILE_INDEX_REF, iconIndex);
                renderer.sharedMaterials = newMaterials;
            }
        }
    }

    public void HidematerialIcon() {
        if (hexMat != null)
        {
            hexMat.SetInt(AtlasMatConstants.TILE_INDEX_REF, 15);
        }
    }


    //public void ToggleIconEmission()
    //{


    //    System.Action<string, float> act = (r, v) => hexMat.SetFloat(r, v);

    //    if (iconAlphaOn)
    //    {            
    //        StartCoroutine(Tween(act, AtlasMatConstants.ICON_ALPHA_REF, 0, 1, 1));
    //    }
    //    else
    //    {

    //        StartCoroutine(Tween(act, AtlasMatConstants.ICON_ALPHA_REF, 1, 0, 1));
    //    }


    //    iconAlphaOn = !iconAlphaOn;
    //}




    private IEnumerator Tween(System.Action<string, float> var, string fieldRef, float aa, float zz, float duration)
    {
        float sT = Time.time;
        float eT = sT + duration;

        while (Time.time < eT)
        {
            float t = (Time.time - sT) / duration;
            var(fieldRef, Mathf.SmoothStep(aa, zz, t));
            yield return null;
        }

        var(fieldRef, zz);

    }



}



