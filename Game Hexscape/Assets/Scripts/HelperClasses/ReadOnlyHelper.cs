using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR) 

[System.Serializable]
public class ReadOnlyAttribute : PropertyAttribute
{

}




[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.stringValue != "")
        {

            return EditorGUI.GetPropertyHeight(property, label, false);

        }

        return 0;

    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        GUI.enabled = false;


        if (property.stringValue == "" || property.stringValue == null)
        {
            

        }
        else
        {
            //Debug.Log(property.stringValue);
            EditorGUI.PropertyField(position, property, label, true);

        }

        GUI.enabled = true;


    }
}
#endif