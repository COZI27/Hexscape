#if UNITY_EDITOR
using UnityEditor;
using System.IO;

//public class GenerateEnum
//{
//    [MenuItem("Tools/GenerateEnum")]
//    public static void Go()
//    {
//        string enumName = "SoundEffectEnum";
//        //string[] enumEntries = { "Foo", "Goo", "Hoo" };
//        string filePathAndName = "Assets/Scripts/Enums/" + enumName + ".cs"; //The folder Scripts/Enums/ is expected to exist

//        using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
//        {
//            streamWriter.WriteLine("public enum " + enumName);
//            streamWriter.WriteLine("{");
//            for (int i = 0; i < AudioManager.instance.soundEffects.Length; i++)
//            {
//                streamWriter.WriteLine("\t" + AudioManager.instance.soundEffects[i].ToString() + ",");
//            }
//            streamWriter.WriteLine("}");
//        }
//        AssetDatabase.Refresh();
//    }
//}
#endif