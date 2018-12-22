#if UNITY_EDITOR
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
        if (audioManager != null)
        {

            AudioClip[] loadedClips = Resources.LoadAll<AudioClip>(directoryToLoad.ToString());

            Debug.Log("Loaded Audio Clip count = " + loadedClips.Length);
            audioManager.PopulateSoundEffectsArray(loadedClips, Application.isEditor);

            GenerateAudioEnum(loadedClips, "SoundEffectEnum");
        }
        else Debug.LogWarning("Failed to load sound effects. audioManager instance is null.");
    }


    public static void GenerateAudioEnum(AudioClip[] audioArray, string enumName)
    {
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
#endif
