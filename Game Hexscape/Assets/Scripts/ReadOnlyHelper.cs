using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Just stole this from the internet 

public class ReadOnlyAttribute : PropertyAttribute
{

}


[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        
        EditorGUI.PropertyField(position, property, label, true);
        GUI.color = Color.grey + Color.green/6; // i added this just for shits and giggles   
        GUI.enabled = true;
    }
}
