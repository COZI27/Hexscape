using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelEditorWindow : EditorWindow
{
    [SerializeField]
    private HexagonGrid grid;

    private Vector3 worldMousePos;

    //public LayerMask hexMask;


    private Vector3? foundWorldPos;

    private GameObject cursorHex;


    Tool LastTool = Tool.None;

    


    HexTypeEnum hexType;
    ElementAttribute attribute;
    Vector2Int gridLoc;
    Vector3 worldLoc;

    string[] attributeChoices = { "optionA", "optionB" };
    int choiceIndex = 0;


    Level levelBeingEdited;

    void OnEnable()
    {
        LastTool = Tools.current;
        Tools.current = Tool.None;

        hexType = HexTypeEnum.HexTile_ClickDestroy; // Sets the initially chosen hex type
    }

    void OnDisable()
    {
        Tools.current = LastTool;
    }



    private static GUIStyle ToggleButtonStyleNormal = null;
    private static GUIStyle ToggleButtonStyleToggled = null;


    static LevelEditorWindow()
    {
        EditorApplication.quitting += CleanupSpawnedHexes; // Delegated to destroy stored hexes as the application quits
    }

    static void CleanupSpawnedHexes()
    {
        List<MapElement> mapElements = new List<MapElement>();

        HexagonGrid grid = MapSpawner.Instance.grid;

        if (grid.GetComponentsInChildren<Hex>().Length > 0)
        {
            // TODO: Would you like to save prompt
            foreach (Hex hex in grid.GetComponentsInChildren<Hex>())
            {
                hex.gameObject.SetActive(false);
                HexBank.Instance.AddDisabledHex(hex.gameObject);
            }
        }
    }





    [MenuItem("Window/Level Editor")]
    static void Init()
    {
        LevelEditorWindow window = (LevelEditorWindow)EditorWindow.GetWindow(typeof(LevelEditorWindow));

        window.Show();
    }


    [ExecuteInEditMode]
    void Update()
    {
        DrawMousePosition();
    }


    // Window has been selected
    void OnFocus()
    {
        if (grid == null) grid = MapSpawner.Instance.grid;
        UpdateHexCursorObject();

        // Remove delegate listener if it has previously been assigned.

        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        // Add (or re-add) the delegate.
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;

        //LastTool = Tools.current;
        //Tools.current = Tool.None;
    }

    private void OnLostFocus()
    {
        //DisableCursorHex();
        //SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }

    void OnDestroy()
    {


        CleanupSpawnedHexes();


        // When the window is destroyed, remove the delegate
        // so that it will no longer do any drawing.
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;

        //Tools.current = LastTool;

        DisableCursorHex();
    }



    void DisableCursorHex()
    {
        cursorHex.SetActive(false);
        if (cursorHex != null) HexBank.Instance.AddDisabledHex(cursorHex);
        cursorHex = null;
    }


    bool drawModeButtonEnabled = true;
    bool deleteModeButtonEnabled = false;

    // TODO: Add SelectMode - will enable the selection of a hex in the scene and display its properties - should also allow editing and copying
    // Select multiple tiles? Copy, CUt and Paste?

    private void OnGUI()
    {
        DisplayToggle();

        if (drawModeButtonEnabled)
        {
            DisplayHexAttributeOptions();
        }
    }



    void DisplayToggle()
    {
        if (ToggleButtonStyleNormal == null)
        {
            ToggleButtonStyleNormal = "Button";
            ToggleButtonStyleToggled = new GUIStyle(ToggleButtonStyleNormal);
            ToggleButtonStyleToggled.normal.background = ToggleButtonStyleToggled.active.background;
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Draw", drawModeButtonEnabled ? ToggleButtonStyleToggled : ToggleButtonStyleNormal))
        {
            drawModeButtonEnabled = true;
            deleteModeButtonEnabled = false;

            cursorTexture = (Texture2D)EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSplat").image;
            Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
        }

        if (GUILayout.Button("Delete", deleteModeButtonEnabled ? ToggleButtonStyleToggled : ToggleButtonStyleNormal))
        {
            drawModeButtonEnabled = false;
            deleteModeButtonEnabled = true;
        }


        GUILayout.EndHorizontal();
    }


    void DisplayHexAttributeOptions()
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

        //location = EditorGUILayout.Vector2Field("Hex Location", location);
        EditorGUILayout.Vector2Field("Hex Location", gridLoc);

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
        if (GUILayout.Button("Save Level", buttonStyle, buttonLayoutOptions))
            SaveLevel();
        if (GUILayout.Button("Load Level", buttonStyle, buttonLayoutOptions))
            LoadLevel();
        if (GUILayout.Button("New Level", buttonStyle, buttonLayoutOptions))
            NewLevel();

        //GUILayout.Space(this.position.width - 310);


        GUILayout.EndHorizontal();

        GUILayout.Space(40);


        UpdateAttributeChoices();
        if (prevHexType != hexType) UpdateHexCursorObject();
        Repaint();
    }

    void SaveLevel()
    {
        List<MapElement> mapElements = new List<MapElement>();

        HexagonGrid grid = MapSpawner.Instance.grid;

        foreach (Hex hex in grid.GetComponentsInChildren<Hex>())
        {
            if (hex.gameObject == cursorHex) continue; // Don't add the cursor hex to the save file

            mapElements.Add(new MapElement(
                hex.typeOfHex,
                new Vector2Int(grid.WorldToCell(hex.transform.position).x, grid.WorldToCell(hex.transform.position).y),
                hex.hexAttribute)
                );

        }

        Debug.Log(levelBeingEdited);
        if (levelBeingEdited == null) levelBeingEdited = new Level();
        levelBeingEdited.hexs = mapElements.ToArray();

        LevelLoader.Instance.SaveLevelFile(levelBeingEdited); // will make it so folders to where you can save it are limited for player input


    }

    void LoadLevel()
    {
        //TODO: Display warning prompt
        Level loadedLevel = LevelLoader.Instance.LoadLevelFile();
        if (loadedLevel != null)
        {

            List<MapElement> mapElements = new List<MapElement>();

            HexagonGrid grid = MapSpawner.Instance.grid;

            foreach (Hex hex in grid.GetComponentsInChildren<Hex>())
            {
                hex.gameObject.SetActive(false);
                HexBank.Instance.AddDisabledHex(hex.gameObject);
            }

            levelBeingEdited = loadedLevel;

            Debug.Log(levelBeingEdited);

            MapSpawner.Instance.SpawnHexs(
                levelBeingEdited,
                grid.transform.position + new Vector3(0, 30, 0),
                false
                );
        }
    }

    void NewLevel()
    {
        //TODO: Display warning prompt
        if (EditorUtility.DisplayDialog("Create new level?",
            "Are you sure you want to clear " + levelBeingEdited.levelName
            + "?", "Clear", "Cancel")) {

            CleanupSpawnedHexes();
            UpdateHexCursorObject();
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
        digitAttribute.leadingZeroCount = (int)GUILayout.HorizontalSlider(digitAttribute.leadingZeroCount, -10, 10);

        GUILayout.EndHorizontal();
    }

    private void ShowLayout_MenuButtonAttribute()
    {
        if (attribute == null) return;
        MenuButtonElementAttribute digitAttribute = (MenuButtonElementAttribute)attribute;
        digitAttribute.commandToCall = (Command)EditorGUILayout.EnumPopup("Button Command", digitAttribute.commandToCall);
    }

    private void AddHex()
    {
        //RemoveHex();
        MapElement element = new MapElement(hexType, gridLoc, attribute);
        MapSpawner.Instance.SpawnAHex(element);
    }

    private void RemoveHex()
    {
        MapSpawner.Instance.RemoveHexAtPoint(gridLoc);
    }

    private void UpdateAttributeChoices()
    {
        string[] foundChoices = HexTypes.GetCompatibleAttrributes(hexType);
        attributeChoices = new string[foundChoices.Length + 1];
        attributeChoices[0] = "None";
        for (int i = 0; i < foundChoices.Length; ++i)
        {
            attributeChoices[i + 1] = foundChoices[i];
        }
    }



    public Texture2D cursorTexture; // = (Texture2D)EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSplat").image;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    void OnSceneGUI(SceneView sceneView)
    {
        if (grid == null)
        {
            Debug.LogWarning("No HexagonGrid found by LevelEditor");
            return;
        }

        Event e = Event.current;

        if (e.type == EventType.MouseMove)
        {
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Vector3 mousePos;

            Plane plane = new Plane(Vector3.up, 0);

            float distance;
            if (plane.Raycast(mouseRay, out distance))   // outs the required distance
            {
                mousePos = mouseRay.GetPoint(distance);

                Vector2Int targetGridPos = grid.WorldToCell(mousePos);
                foundWorldPos = grid.CellToWorld(targetGridPos);
                if (foundWorldPos == null) return; // No grid cell found

                worldMousePos = foundWorldPos.Value;
                worldMousePos.y = MapSpawner.Instance.grid.transform.position.y;
                gridLoc = new Vector2Int(targetGridPos.x, targetGridPos.y);
                worldLoc = foundWorldPos.Value;
            }

        }

        if (e.type == EventType.MouseDrag || e.type == EventType.MouseDown) // e.button == 0) 
        {
            if (e.button == 0)
            {
                AddHex();
            }
            else if (e.button == 1)
            {
                RemoveHex();
            }
            e.Use();

        }


        SceneView.RepaintAll();
    }

    private void UpdateHexCursorObject()
    {
        if (cursorHex != null)
        {
            cursorHex.gameObject.SetActive(false);
            HexBank.Instance.AddDisabledHex(cursorHex);
        }

        cursorHex = HexBank.Instance.GetDisabledHex(hexType, Vector3.zero, grid.transform);
    } 



    private void DrawMousePosition()
    {
        if (cursorHex != null && foundWorldPos != null)
        {
            cursorHex.transform.position = foundWorldPos.Value;
            cursorHex.transform.rotation = grid.GetGridRotation();
        }
    }

}
