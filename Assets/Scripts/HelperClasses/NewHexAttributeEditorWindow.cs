using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR) 

public class NewHexAttributeEditorWindow : EditorWindow // PopupWindow
{   
    /*

    ElementAttribute attribute;

    string[] attributeChoices = { "optionA", "optionB" };
    int choiceIndex = 0;
    Vector2 location;

    public static void ShowhexAttributeWindow()
    {
        EditorWindow thisWindow = GetWindow<NewHexAttributeEditorWindow>("Add Hex With attribute");
        thisWindow.minSize = new Vector2(440, 300);
    }


    private void OnGUI()
    {


        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.onHover.textColor = Color.green;


        GUILayoutOption[] buttonLayoutOptions = new GUILayoutOption[]
        {
            GUILayout.MinWidth(150),
            GUILayout.MaxWidth(150),
        };


        GUILayout.Height(100);


        HexTypeEnum prevHexType = hexType;

        GUILayout.Label("Select the type of hex and attibute you wish to add to the level.", EditorStyles.boldLabel);
        GUILayout.Space(20);

        hexType = (HexTypeEnum)EditorGUILayout.EnumPopup("Type of Hex", hexType);

        location = EditorGUILayout.Vector2Field("Hex Location", location);

        #region attribute
        GUILayout.BeginHorizontal();
        GUILayout.Label("Attribute");
        choiceIndex = EditorGUILayout.Popup(choiceIndex, attributeChoices);
        GUILayout.EndHorizontal();
        GUILayout.Space(40);
        #endregion

        switch (attributeChoices[choiceIndex])
        {
            case "None":
                break;
            case "DigitElementAttribute":
                if (attribute == null || attribute.GetType() != typeof(DigitElementAttribute))
                {
                    attribute = new DigitElementAttribute(0);
                }
                ShowLayout_DigitAttribute();
                break;
            case "MenuButtonElementAttribute":
                if (attribute == null || attribute.GetType() != typeof(MenuButtonElementAttribute))
                {
                    attribute = new MenuButtonElementAttribute(Command.NextMenu);
                }
                ShowLayout_MenuButtonAttribute();
                break;
            default:
                break;
        }

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("CloseWindow", buttonStyle, buttonLayoutOptions))
            this.Close();
        GUILayout.Space(this.position.width - 310);
        if (GUILayout.Button("Add Hex", buttonStyle, buttonLayoutOptions))
            AddHexToLevel();
        GUILayout.EndHorizontal();

        GUILayout.Space(40);


        UpdateAttributeChoices();


        // Refreshes the popup window in order to update options
        if (EditorApplication.isPlaying)
        {
            if (prevHexType != hexType)
            {
                UpdateAttributeChoices();
                Repaint();
            }
        }
    }

    private void ShowLayout_DigitAttribute()
    {
        if (attribute == null) return;
        DigitElementAttribute digitAttribute = (DigitElementAttribute)attribute;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Leading Zero Count");
        GUILayout.Space(40);
        GUILayout.Label(digitAttribute.leadingZeroCount.ToString(), EditorStyles.largeLabel);
        digitAttribute.leadingZeroCount = (int)GUILayout.HorizontalSlider(digitAttribute.leadingZeroCount, 0, 10);

        GUILayout.EndHorizontal();
    }

    private void ShowLayout_MenuButtonAttribute()
    {
        if (attribute == null) return;
        MenuButtonElementAttribute digitAttribute = (MenuButtonElementAttribute)attribute;
        digitAttribute.commandToCall = (Command)EditorGUILayout.EnumPopup("Button Command", digitAttribute.commandToCall);
    }

    private void AddHexToLevel()
    {
        //if (LevelLoader.Instance.GetHexAtPoint(Vector2(0,0))) // Check whetehr a hex exists already at location
        //{
        //    // Display 'are you sure?'
        //}

        if (EditorUtility.DisplayDialog("Add Hex to Level?",
            "Are you sure you want to add " + hexType.ToString()
            + " to " + LevelLoader.Instance.levelBeingEdited.levelName +"?", "Add", "Do Not Add"))
        {
            Debug.Log("HEX ADDED!");
            LevelLoader.Instance.AddHexToLevel(location, attribute);
            // levelBeingEdited.hexs[1] = new MapElement(HexTypeEnum.HexTile_MenuOptionEdit, new Vector2Int(1, 0), new MenuButtonElementAttribute(Command.Edit));
        }
    }

    private void UpdateAttributeChoices()
    {
        Debug.LogError("NewHexAttributeWinder::UpdateAttributeChoices() has been disabled due to use of deprecated code");

        //string[] foundChoices = HexTypes.GetCompatibleAttrributes(hexType);
        //attributeChoices = new string[foundChoices.Length + 1];
        //attributeChoices[0] = "None";
        //for (int i = 0; i < foundChoices.Length; ++i)
        //{
        //    attributeChoices[i + 1] = foundChoices[i];
        //}
    }
    */
}

#endif