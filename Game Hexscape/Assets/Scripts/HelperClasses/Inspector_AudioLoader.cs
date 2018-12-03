using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(AudioManager))]
public class Inspector_AudioLoader : Editor
{

    public string directoryToLoad = "Sounds";

    AudioManager audioManager;

    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        if (GUILayout.Button("Reload Audio"))
        {
            LoadSoundEffects();
        }
    }


    void LoadSoundEffects()
    {
        audioManager = ((MonoBehaviour)target).gameObject.GetComponent<AudioManager>();

        AudioClip[] loadedClips = Resources.LoadAll<AudioClip>("Sounds");

        Debug.Log("Loaded clip count = " + loadedClips.Length);
        audioManager.SetSoundEffectArraySize(loadedClips.Length - 1, true);
        audioManager.soundEffects = loadedClips;

        //textureMap = Resources.Load("Tiles02") as Texture2D;
        // baseMat = new Material(Shader.Find("Diffuse"));
        //GenerateMaterials();

        GenerateAudioEnum(loadedClips);
    }



    public static void GenerateAudioEnum(AudioClip[] audioArray)
    {
        string enumName = "SoundEffectEnum";
        //string[] enumEntries = { "Foo", "Goo", "Hoo" };
        string filePathAndName = "Assets/Scripts/Enums/" + enumName + ".cs"; //The folder Scripts/Enums/ is expected to exist

        using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
        {
            streamWriter.WriteLine("public enum " + enumName);
            streamWriter.WriteLine("{");
            for (int i = 0; i < audioArray.Length; i++)
            {
                streamWriter.WriteLine("\t" + audioArray[i].name.ToString() + ",");
            }
            streamWriter.WriteLine("}");
        }
        AssetDatabase.Refresh();
    }


}
