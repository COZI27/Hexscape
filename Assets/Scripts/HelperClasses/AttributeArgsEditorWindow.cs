using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using UnityEngine;



// Editor window for modifying the default parameters of ElementAttributes
public class AttributeArgsEditorWindow : EditorWindow
{

    private static AttributeArgsEditorWindow instance = null;

    string[] attributeChoices = { "optionA", "optionB" };   
    Object[] args;

    ElementAttribute currentAttribute;


    int choiceIndex = 0;

    [MenuItem("Window/Attribute Args Editor")]
    static void Init()
    {
        System.Type inspectorType = System.Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
        instance = EditorWindow.GetWindow<AttributeArgsEditorWindow>();
        instance.Show();

        AttributeArgsEditorWindow.instance.LoadChoices();
    }

    private void LoadChoices()
    {
        AttributeFinder attributeFinder = new AttributeFinder();
        Dictionary<string, System.Type> foundAttributes = attributeFinder.GetAttributesDict();

        attributeChoices = foundAttributes.Keys.ToArray();
    }

    void OnEnable()
    {
        EditorWindow thisWindow = GetWindow<AttributeArgsEditorWindow>("Modify Attribute Default Values");
        thisWindow.minSize = new Vector2(440, 300);
    }

    void OnDisable()
    {

    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Attribute");

        int prevChoiceIndex = choiceIndex;
        choiceIndex = EditorGUILayout.Popup(this.choiceIndex, attributeChoices);

        GUILayout.EndHorizontal();
        GUILayout.Space(40);


        //hexType = (HexTypeEnum)EditorGUILayout.EnumPopup("Type of Hex", hexType);


        bool doCreateNewAttribute = prevChoiceIndex != choiceIndex  || currentAttribute == null;

        if (doCreateNewAttribute)
        {
            AttributeFinder attributeFinder = new AttributeFinder();
            //currentAttribute = attributeFinder.InstantiateNewAttibute(attributeChoices[choiceIndex], hexType);
        }

        currentAttribute.DisplayEditorAttributeOptions();

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.onHover.textColor = Color.green;

        GUILayoutOption[] buttonLayoutOptions = new GUILayoutOption[]
        {
            GUILayout.MinWidth(150),
            GUILayout.MaxWidth(150),
        };


       // GUILayout.Label(hexType.ToString() + "IsPlayType = " + HexTypes.IsPlayType(hexType));

        if (GUILayout.Button("SaveAttributeArgs", buttonStyle, buttonLayoutOptions))
            SaveAttributeArgs();




    }


    private void SaveAttributeArgs()
    {
        AttributeArgsLoader loader = new AttributeArgsLoader();

        EditorApplication.Beep();
      
       // loader.SaveAttributeArgs(attributeChoices[choiceIndex], currentAttribute.GetElementParams() );
    }




}
