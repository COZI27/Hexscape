using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR)

public class LevelEditorWindow : EditorWindow
{
    private static LevelEditorWindow instance = null;

    [SerializeField]
    private HexagonGrid grid;

    private Vector3 worldMousePos;

    private string hexGhostPath = "Prefabs/HexTile_EditorGhost";

    private Vector3? foundWorldPos;

    private GameObject cursorHex;

    private static GUIStyle ToggleButtonStyleNormal = null;
    private static GUIStyle ToggleButtonStyleToggled = null;

    Tool LastTool = Tool.None;

    List<MapElement> mapElementsLocalRef; // Used by the editor for user feedback info

    Level levelBeingEdited;

    HexTypeEnum hexType;
    List <ElementAttribute> attributes;
    Vector2Int gridLoc;
    Vector3 worldLoc;


    Dictionary<string, System.Type> attributeTypeDict;

    #region Attribute variables
    string[] attributeChoices;
    int choiceIndex = 0;

    #endregion




    void OnEnable()
    {
        LastTool = Tools.current;
        Tools.current = Tool.None;


        hexType = HexTypeEnum.HexTile_ClickDestroy; // Sets the initially chosen hex type

        SceneView.RepaintAll();
    }

    void OnDisable()
    {
        Tools.current = LastTool;
        DestroyImmediate(cursorHex);
    }



    static LevelEditorWindow()
    {
        EditorApplication.quitting += CleanupSpawnedHexes; // Delegated to destroy stored hexes as the application quits
    }

    static void CleanupSpawnedHexes()
    {
        HexagonGrid grid = MapSpawner.Instance.grid;

        if (grid.GetComponentsInChildren<Hex>().Length > 0)
        {
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

        Debug.Log("static void Init()");

        //instance = (LevelEditorWindow)EditorWindow.GetWindow(typeof(LevelEditorWindow)); // deprecated version

        // Adds editor window as a tab to the inspector window 
        System.Type inspectorType = System.Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
        instance = EditorWindow.GetWindow<LevelEditorWindow>(new System.Type[] { inspectorType });
        instance.Show();

        AttributeFinder attributeFinder = new AttributeFinder();
        LevelEditorWindow.instance.attributeTypeDict = attributeFinder.GetAttributesDict();

        LevelEditorWindow.instance.mapElementsLocalRef = new List<MapElement>();       

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
        // Cleanup both the hexes spawned by the editor and the disabled hexes in the hexbank
        CleanupSpawnedHexes();
        HexBank.Instance.Cleanup();

        if (cursorHex != null) DestroyImmediate(cursorHex);

        // When the window is destroyed, remove the delegate
        // so that it will no longer do any drawing.
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;

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


        DebugHexAttribuesOnBoard();


        //if (editorCamera != null)
        //{
        //    // NOTE: This is not a perfect rectangle for the window.  Adjust the size to get the desired result
        //    Rect cameraRect = new Rect(0f, 0f, position.width, position.height);
        //    Handles.DrawCamera(cameraRect, editorCamera, DrawCameraMode.Textured);
        //}

        //TODO: perform input handling for editor and camera here
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
        attributes = new List<ElementAttribute>(3);



        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.onHover.textColor = Color.green;


        GUILayoutOption[] buttonLayoutOptions = new GUILayoutOption[]
        {
            GUILayout.MinWidth(150),
            GUILayout.MaxWidth(150),
        };


        GUILayout.Height(100);




        GUILayout.Label("Select the type of hex and attibute you wish to add to the level.", EditorStyles.boldLabel);
        GUILayout.Space(20);


        bool useDefaultAttributeValues = false;

        #region hexType
        HexTypeEnum prevHexType = hexType;
        hexType = (HexTypeEnum)EditorGUILayout.EnumPopup("Type of Hex", hexType);
        if (hexType != prevHexType)
        {
            UpdateHexCursorObject();
            useDefaultAttributeValues = true;
        }
        #endregion



        EditorGUILayout.Vector2Field("Hex Location", gridLoc);




        ElementAttribute defaultElementAttribute;
        UpdateAttributeChoices(useDefaultAttributeValues, out defaultElementAttribute);


        #region attribute
        GUILayout.BeginHorizontal();
        GUILayout.Label("Attribute");
        choiceIndex = EditorGUILayout.Popup(choiceIndex, attributeChoices);
        GUILayout.EndHorizontal();
        GUILayout.Space(40);
        #endregion



        //if (attribute != null && defaultElementAttribute != null &&  attribute.GetType() != defaultElementAttribute.GetType()) 
        //{
        //    Debug.Log(attribute.GetType() + "!=" + defaultElementAttribute.GetType());
        //}


        //TODO: DISPLAY DEFAULT VALUES
        // FIND TIDER WAY OF SWITCHING BETWEEN ATTRIBUTE OPTIONS TO DISPLAY

        //if (useDefaultAttributeValues) attribute = defaultElementAttribute;
        //TODO: Assign type to attribute! (Could use Dict<int, Type> ?)

        Dictionary<int, MapElement> elementDictionary = new Dictionary<int, MapElement>()
        {

          // {  0 ,  new DigitElementAttribute(0) }

        };



        //if (choiceIndex ==  0) attribute = null;
        //if (attributeChoices[choiceIndex] == "None") attribute = null;

        //switch (attributeChoices[choiceIndex])
        //{
        //    case "None":
        //        attribute = null;
        //        break;
        //    case "DigitElementAttribute":
        //        attribute.DisplayEditorAttributeOptions(attribute, out attribute);
        //        break;
        //    case "MenuButtonElementAttribute":

        //        break;
        //}
        //attribute.DisplayEditorAttributeOptions(attribute, out attribute);



        //if (false) { // "Don't enable me - I'm reference code"
        //    if (attribute == null) return;
        //    DigitElementAttribute digitAttribute = (DigitElementAttribute)attribute;

        //    GUILayout.BeginHorizontal();
        //    GUILayout.Label("Leading Zero Count");
        //    GUILayout.Space(40);
        //    GUILayout.Label(digitAttribute.leadingZeroCount.ToString(), EditorStyles.largeLabel);
        //    digitAttribute.leadingZeroCount = (int)GUILayout.HorizontalSlider(digitAttribute.leadingZeroCount, -10, 10);

        //    GUILayout.EndHorizontal();
        //}

        //switch (attributeChoices[choiceIndex])
        //    {
        //        case "None":
        //            attribute = null;
        //            break;
        //        case "DigitElementAttribute":
        //            if (attribute == null || attribute.GetType() != typeof(DigitElementAttribute) || useDefaultAttributeValues)
        //            {
        //            //attribute = new DigitElementAttribute(0);
        //            attribute = defaultElementAttribute;
        //            }
        //            ShowLayout_DigitAttribute();
        //            break;
        //        case "MenuButtonElementAttribute":
        //            if (attribute == null || attribute.GetType() != typeof(MenuButtonElementAttribute) || useDefaultAttributeValues)
        //            {
        //                //attribute = new MenuButtonElementAttribute(Command.NextMenu);
        //                attribute = defaultElementAttribute;
        //            }
        //            ShowLayout_MenuButtonAttribute();
        //            break;
        //        default:
        //            break;
        //    }

        #region Save Load Clear Buttons

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Level", buttonStyle, buttonLayoutOptions))
            SaveLevel();
        if (GUILayout.Button("Load Level", buttonStyle, buttonLayoutOptions))
            LoadLevel();
        if (GUILayout.Button("New Level", buttonStyle, buttonLayoutOptions))
            NewLevel();

        GUILayout.EndHorizontal();

        GUILayout.Space(40);

        #endregion

        Repaint();
    }

    void SaveLevel()
    {
        List<MapElement> mapElements = new List<MapElement>();

        HexagonGrid grid = MapSpawner.Instance.grid;

        foreach (Hex hex in grid.GetComponentsInChildren<Hex>())
        {
            if (hex.gameObject == cursorHex) continue; // Don't add the cursor hex to the save file

            List <ElementAttribute> tempAts = new List<ElementAttribute>();

            //foreach (var attribute in hex.hexAttributes)
            //{
            //    ElementAttribute tempAt = attribute;
            //    if (tempAt != null && tempAt.GetType() == typeof(MenuButtonElementAttribute)) Debug.Log("SaveLevel: " + ((MenuButtonElementAttribute)tempAt).commandToCall);
            //    tempAts.Add(tempAt);

               
            //}

            ElementAttribute  tempAt = hex.hexAttributes[0];
            if (tempAt != null && tempAt.GetType() == typeof(MenuButtonElementAttribute)) Debug.Log("SaveLevel: " + ((MenuButtonElementAttribute)tempAt).commandToCall);
            

            mapElements.Add(new MapElement(
                hex.typeOfHex,
                new Vector2Int(grid.WorldToCell(hex.transform.position).x, grid.WorldToCell(hex.transform.position).y),
                hex.hexAttributes)
                );

        }


        if (levelBeingEdited == null) levelBeingEdited = new Level();
        Debug.Log(levelBeingEdited + " , " + levelBeingEdited.levelName);
        levelBeingEdited.hexs = mapElements.ToArray();

        LevelLoader.Instance.SaveLevelFile(levelBeingEdited); // will make it so folders to where you can save it are limited for player input


        AssetDatabase.Refresh();
    }


    void DebugHexAttribuesOnBoard()
    {
        foreach (Hex hex in grid.GetComponentsInChildren<Hex>())
        {
            if (hex.gameObject == cursorHex) continue; // Don't add the cursor hex to the save file

            foreach (var attributes in hex.hexAttributes)
            {
              //  ElementAttribute tempAt = attributes;
            }

           // ElementAttribute tempAt = hex.hexAttribute;

        }
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

            Debug.Log(levelBeingEdited); //TODO: Add info box to window

            MapSpawner.Instance.SpawnHexs(
                levelBeingEdited,
                grid.transform.position + new Vector3(0, 30, 0),
                false
                );
        }
    }

    void NewLevel()
    {

        if (levelBeingEdited != null)
        {
            //TODO: Display warning prompt
            if (EditorUtility.DisplayDialog("Create new level?",
                "Are you sure you want to clear " + levelBeingEdited.levelName
                + "?", "Clear", "Cancel"))
            {

                CleanupSpawnedHexes();
                UpdateHexCursorObject();



                levelBeingEdited = new Level(); //<<<<<<




            }
        }
        else
        {
            CleanupSpawnedHexes();
            UpdateHexCursorObject();

            levelBeingEdited = new Level(); //<<<<<<




        }
    }


    private void ShowLayout_DigitAttribute()
    {
        ElementAttribute attribute = attributes[0];

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
        ElementAttribute attribute = attributes[0];
        if (attribute == null) return;

        MenuButtonElementAttribute menuAttribute = (MenuButtonElementAttribute)attribute;
        menuAttribute.commandToCall = (Command)EditorGUILayout.EnumPopup("Button Command", menuAttribute.commandToCall);


        attribute = new MenuButtonElementAttribute(menuAttribute.commandToCall);
    }




    private void AddHex()
    {
        MapElement element = new MapElement(hexType, gridLoc, attributes);
        MapSpawner.Instance.SpawnAHex(element);

        mapElementsLocalRef.Add(element);
    }

    private void RemoveHex()
    {
        foreach(MapElement element in mapElementsLocalRef)
        {
            if (element.gridPos == gridLoc)
            {
               mapElementsLocalRef.Remove(element);
               break;
            }
        }




        //mapElementsLocalRef = new List<MapElement>();
        //foreach (Hex hex in grid.GetComponentsInChildren<Hex>())
        //{
        //    if (hex.gameObject == cursorHex) continue; // Don't add the cursor hex to the save file


        //    ElementAttribute tempAt = hex.hexAttribute;
        //    if (tempAt != null && tempAt.GetType() == typeof(MenuButtonElementAttribute)) Debug.Log("SaveLevel: " + ((MenuButtonElementAttribute)tempAt).commandToCall);


        //    mapElementsLocalRef.Add(new MapElement(
        //        hex.typeOfHex,
        //        new Vector2Int(grid.WorldToCell(hex.transform.position).x, grid.WorldToCell(hex.transform.position).y),
        //        hex.hexAttribute)
        //        );

        //}



        MapSpawner.Instance.RemoveHexAtPoint(gridLoc);
        
    }

    private void UpdateAttributeChoices(bool useDefaults, out ElementAttribute outDefaultValues)
    {
        //ElementAttribute outDefaultValues;
        string[] foundChoices = HexTypes.GetCompatibleAttrributes(hexType, out outDefaultValues);

        string[] attributeDisplayNames = new string[LevelEditorWindow.instance.attributeTypeDict.Keys.Count];
        LevelEditorWindow.instance.attributeTypeDict.Keys.CopyTo(attributeDisplayNames, 0);

        //TODO: GetDefaults

         attributeChoices = new string[foundChoices.Length + 1];
        attributeChoices[0] = "None";
        for (int i = 0; i < foundChoices.Length; ++i)
        {
            attributeChoices[i + 1] = foundChoices[i];
        }

        if (useDefaults)
        {
            if (outDefaultValues != null)
            {
                if (attributeChoices.Length > 0)
                {
                    int index = 0;
                    foreach (string s in attributeChoices)
                    {
                        if (s == outDefaultValues.GetType().ToString())
                        {
                            choiceIndex = index;
                        }
                        else index++;
                    }
                }
            }
            else choiceIndex = 0;
        }
        else outDefaultValues = null;

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



        // Overrides default Unity Editor Window input controls
        if (Event.current.type == EventType.Layout)
            HandleUtility.AddDefaultControl(0);




        DrawSceneAttributeLabels();

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

            
            //e.Use(); // If we want to "paint" tiles, then this will need to be removed - COuld use Shift + event to enable painting and prevent the camera moving

        }


        SceneView.RepaintAll();
    }

    private void UpdateHexCursorObject()
    {
        // Pulls a temporary hex from the hex bank from which the emissive material is taken and added to the cursor hex's material
        if (cursorHex != null)
        {
            GameObject tempHex = HexBank.Instance.GetDisabledHex(hexType, Vector3.zero, grid.transform);

            Material[] newmat = cursorHex.GetComponent<Renderer>().sharedMaterials;
            newmat[1].SetTexture("_EmissionMap", tempHex.GetComponent<Renderer>().sharedMaterials[1].GetTexture("_EmissionMap"));
           
            cursorHex.GetComponent<Renderer>().sharedMaterials = newmat;
            
            tempHex.SetActive(false);
            HexBank.Instance.AddDisabledHex(tempHex);
        }
        else
        {
            cursorHex = Instantiate(Resources.Load(hexGhostPath) as GameObject); 
        }
    } 


    private void DrawMousePosition()
    {
        if (cursorHex != null && foundWorldPos != null)
        {
            cursorHex.transform.position = foundWorldPos.Value;
            cursorHex.transform.rotation = grid.GetGridRotation();
        }
    }



    private void DrawSceneAttributeLabels()
    {
        return;

        if (instance == null)
        {
            return;
        }


        Handles.color = Color.yellow;


        if (LevelEditorWindow.instance.levelBeingEdited != null)
        {
            foreach (MapElement element in levelBeingEdited.hexs)
            {

                if (element.gridPos == gridLoc) {



                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.green;
                    Handles.BeginGUI();


                    LevelEditorWindow.instance.grid.CellToWorld(gridLoc);

                    Vector3 pos = LevelEditorWindow.instance.grid.CellToWorld(element.gridPos).Value;
                    Vector2 pos2D = HandleUtility.WorldToGUIPoint(pos);

                    //TODO: Create the GUI Labes for multiple atts with their unique data bellow (as well as a button to add and remove atts) -COZI

                    //for (int i = 0; i < element.hexAttributes.Count ; i++)
                   
                    GUI.Label(new Rect(pos2D.x, pos2D.y, 100, 100), element.hexAttributes[0].ToString(), style);
                    
                   
                    Handles.EndGUI();


                    Handles.Label(LevelEditorWindow.instance.grid.CellToWorld(element.gridPos).Value, element.hexType.ToString());
                }
            }
            
        }
    }


}

#endif