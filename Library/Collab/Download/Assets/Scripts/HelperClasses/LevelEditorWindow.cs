﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

#if (UNITY_EDITOR)

public class LevelEditorWindow : EditorWindow
{

    private enum EEditorMode
    {
        Draw, // Standard click to place & rightclick to remove functionality
        //Paint
        Inspect // Inspect elements in the scene view and edit or copy their properties
    }

    private static LevelEditorWindow instance = null;

    private Vector3 worldMousePos;

    private string hexGhostPath = "Prefabs/HexTile_EditorGhost";

    private Vector3? foundWorldPos;

    private GameObject cursorHex;

    private static GUIStyle ToggleButtonStyleNormal = null;
    private static GUIStyle ToggleButtonStyleToggled = null;

    Vector2 scrollPos = Vector2.zero;


    private int currentLayer = 0;

    List<List<MapElement>> mapElementsLocalRef; // Used by the editor for user feedback info

    Level levelBeingEdited; // Reference for saving and Loading

    //HexTypeEnum hexType;
    //List <ElementAttribute> attributes; // The Attribute instances 



    Vector3 worldLoc;

    ElementInfo currentElementInfo;


    //Dictionary<string, System.Type> attributeTypeDict;
    AttributeFinder attFinder;



    #region Attribute variables
    //string[] attributeChoices; // Available choices

    class AttributeChoice
    {
        public int choiceIndex = 0;
        public string[] attributeChoices = null;
        public ElementAttribute attributeInstance = null;

        public bool enabled = true;
        public bool compatible = true;
    }

    List<AttributeChoice> attributeChoiceFields;
    //int choiceIndex = 0;

    #endregion



    void OnEnable()
    {

        if (instance == null)
        {
            System.Type inspectorType = System.Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
            instance = EditorWindow.GetWindow<LevelEditorWindow>(new System.Type[] { inspectorType });
        }
        if (attFinder == null) attFinder = new AttributeFinder();

        Tools.current = Tool.None;

        UpdateHexCursorObject();

        SceneView.RepaintAll();
    }

    void OnDisable()
    {
        DestroyImmediate(cursorHex);
    }



    static LevelEditorWindow()
    {
        EditorApplication.quitting += CleanupSpawnedHexes; // Delegated to destroy stored hexes as the application quits
    }

    static void CleanupSpawnedHexes()
    {

        MapSpawner.Instance.ClearAllGrids();

        //HexagonGrid grid = MapSpawner.Instance.GetCurrentGrid();

        //if (grid.GetComponentsInChildren<Hex>().Length > 0)
        //{
        //    foreach (Hex hex in grid.GetComponentsInChildren<Hex>())
        //    {
        //        hex.gameObject.SetActive(false);
        //        HexBank.Instance.AddDisabledHex(hex.gameObject);
        //    }
        //}
    }



    [MenuItem("Window/Level Editor")]
    static void Init()
    {
        // Adds editor window as a tab to the inspector window 
        System.Type inspectorType = System.Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
        instance = EditorWindow.GetWindow<LevelEditorWindow>(new System.Type[] { inspectorType });
        instance.Show();

        LevelEditorWindow.instance.attFinder = new AttributeFinder();
        
        LevelEditorWindow.instance.mapElementsLocalRef = new List<List<MapElement>>();       
        for (int i = 0; i < MapSpawner.MAX_LEVEL_LAYERS; i++)
        {
            LevelEditorWindow.instance.mapElementsLocalRef.Add( new List<MapElement>());
        }

        LevelEditorWindow.instance.currentElementInfo.iconIndex = 0;
    }


    [ExecuteInEditMode]
    void Update()
    {
        DrawMousePosition();
    }

    private void CheckAllAttributesCompatible()
    {
        //if (attributeChoiceFields != null)
        //{
        //    for (int i = 0; i < attributeChoiceFields.Count; i++)
        //    {
        //        bool isCompatible = LevelEditorWindow.instance.attFinder.GetCompatible(attributeChoiceFields[i].attributeChoices[attributeChoiceFields[i].choiceIndex], hexType);

        //        attributeChoiceFields[i].compatible = isCompatible;

        //        if (isCompatible == false && attributeChoiceFields[i].enabled == true)
        //        {
        //            attributeChoiceFields[i].enabled = false;
        //        }

        //        var tmp = attributeChoiceFields[i];
        //        UpdateAttributeChoices(ref tmp);

        //        }
        //}

    }


    // Window has been selected
    void OnFocus()
    {
        UpdateHexCursorObject();

        // Remove delegate listener if it has previously been assigned.
        SceneView.duringSceneGui -= this.OnSceneGUI;
        SceneView.duringSceneGui += this.OnSceneGUI;
        //LastTool = Tools.current;
        //Tools.current = Tool.None;

        FindPreviewTexture();
       
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
        //HexBank.Instance.Cleanup();

        if (cursorHex != null) DestroyImmediate(cursorHex);

        // When the window is destroyed, remove the delegate
        // so that it will no longer do any drawing.
        SceneView.duringSceneGui -= this.OnSceneGUI;


    }


    //bool drawModeButtonEnabled = true;
    //bool deleteModeButtonEnabled = false;

    // TODO: Add SelectMode - will enable the selection of a hex in the scene and display its properties - should also allow editing and copying
    // Select multiple tiles? Copy, CUt and Paste?


    int toolbarIndex;
    EEditorMode[] toolbarOptions = { EEditorMode.Draw, EEditorMode.Inspect };

    EEditorMode GetEditMode()
    {
        return toolbarOptions[toolbarIndex];
    }

    private void OnGUI()
    {
        DisplayFileOptions();

        toolbarIndex = GUILayout.Toolbar(toolbarIndex, System.Enum.GetNames(typeof(EEditorMode)));



        switch (GetEditMode())
        {
            case EEditorMode.Draw:

                DisplayHexLocation();

                DisplayHexTypeOptions();

                DisplayIconRotationOptions();

                DisplayHexColourOptions();

                DisplayAttributeOptions();

                break;
            case EEditorMode.Inspect:

                DisplaySelectedMapElement();
                // Display Selected Hex's Values

                //if(chnagesHaveBeenMade)
                // Apply changs button - replace existing tile
                // Revert Changes button - reload existing tile
                break;
        };


        Repaint();

    }


    void DisplayToolOptions()
    {
        if (ToggleButtonStyleNormal == null)
        {
            ToggleButtonStyleNormal = "Button";
            ToggleButtonStyleToggled = new GUIStyle(ToggleButtonStyleNormal);
            ToggleButtonStyleToggled.normal.background = ToggleButtonStyleToggled.active.background;
        }

        GUILayout.BeginHorizontal();


        //TODO: Add Load/Save Preset option(?) <<<<


        GUILayout.EndHorizontal();
    }

    int currBackGolourInd = 0;
    Color[] backgroundColourArray = new Color[2]
    {
        new Color(0.8f, 1.0f, 1.0f, 0.1f),

        new Color(1f, 0.8f, 1f, 0.1f)
    };
    Texture previewTexture;


    void DisplayHexTypeOptions()
    {
        GUILayout.Height(100);

        DisplayGUISpacer();
        GUILayout.Label("Hex Icon", EditorStyles.boldLabel);
        GUILayout.Space(20);

        //GUILayoutOption layout = new GUILayoutOption();

        GUILayout.BeginHorizontal();


        bool doUpdateDisplay = false;


        string[] atlasNames = IconAtlasDB.GetDatabaseKeys();
        if (currentElementInfo.iconAtlasName == null) currentElementInfo.iconAtlasName = atlasNames[0];
        string prevAtlas = currentElementInfo.iconAtlasName;

        
        int preAtlasIndex = System.Array.FindIndex(atlasNames, row => row == prevAtlas);
        int newIndex = EditorGUILayout.Popup(preAtlasIndex, atlasNames);
        if (newIndex != preAtlasIndex)
        {
            currentElementInfo.iconAtlasName = atlasNames[newIndex];
            doUpdateDisplay = true;
        }


        int prevIconIndex = currentElementInfo.iconIndex;
        int newIconIndex = EditorGUILayout.Popup(prevIconIndex, IconAtlasDB.GetAtlasIconNames(currentElementInfo.iconAtlasName));
        if (newIconIndex != prevIconIndex)
        {
            currentElementInfo.iconIndex = newIconIndex;
            doUpdateDisplay = true;
        }

        
        if (doUpdateDisplay)
        {
            UpdateHexCursorObject();
            //CheckAllAttributesCompatible();
            //TODO: Update Options
            //FindPreviewTexture();
        }


        //iconAtlasName = (HexTypeEnum)EditorGUILayout.Popup("Type of Hex", hexType);
        //if (hexType != prevHexType)
        //{
        //    UpdateHexCursorObject();
        //    CheckAllAttributesCompatible();

        //    //TODO: Update Options
        //    FindPreviewTexture();
        //}
        //if (previewTexture == null) FindPreviewTexture();

        //GUILayout.BeginHorizontal();
        //GUILayout.Space(Screen.width / 2 - 100);
        //if (previewTexture != null) GUILayout.Label(previewTexture);
        //GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();

    }


    private bool doSnapRotation = true;
    void DisplayIconRotationOptions()
    {
        DisplayGUISpacer();
        GUILayout.Label("Icon Rotation", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();

        doSnapRotation = EditorGUILayout.Toggle("Snap rotation:", doSnapRotation);

        float prevRotation = currentElementInfo.iconRotation;
        float newRotation = EditorGUILayout.Slider(prevRotation, 0, 360);
        if (prevRotation != newRotation)
        {

            if (doSnapRotation)           
                newRotation = Mathf.CeilToInt(newRotation  * 1.0f / 30) * 30;
            
            currentElementInfo.iconRotation = newRotation;
            UpdateHexCursorObject();
        }
        GUILayout.EndHorizontal();
    }

    void DisplayHexColourOptions()
    {
        DisplayGUISpacer();
        GUILayout.Label("Emissive Colours", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();

        bool doUpdateDisplay = false;

        GUILayout.BeginVertical();
      //  GUILayout.Label("Icon Emissive Colour");
        Color prevColour = currentElementInfo.iconColour;     
        Color newColour =  EditorGUILayout.ColorField(new GUIContent("Icon Emissive Colour"), prevColour, true, true, true);
        if (newColour != prevColour)
        {
            currentElementInfo.iconColour = newColour;
            doUpdateDisplay = true;
        }
        GUILayout.EndVertical();


        GUILayout.BeginVertical();
       // GUILayout.Label("Base Emissive Colour");
        prevColour = currentElementInfo.baseColour;     
        newColour = EditorGUILayout.ColorField(new GUIContent("Base Emissive Colour"), prevColour, true, true, true);
        if (newColour != prevColour)
        {
            currentElementInfo.baseColour = newColour;
            doUpdateDisplay = true;
        }

        GUILayout.EndVertical();
        if (doUpdateDisplay)
        {
            UpdateHexCursorObject();
        }

        //string prevAtlas = currentElementInfo.iconAtlasName;


        //int preAtlasIndex = System.Array.FindIndex(atlasNames, row => row == prevAtlas);
        //int newIndex = EditorGUILayout.Popup(preAtlasIndex, atlasNames);
        //if (newIndex != preAtlasIndex)
        //{
        //    currentElementInfo.iconAtlasName = atlasNames[newIndex];
        //    doUpdateDisplay = true;
        //}

        GUILayout.EndHorizontal();



        GUILayout.BeginHorizontal();

        GUILayout.EndHorizontal();

       
    }

    private void DisplayHexLocation()
    {
        DisplayGUISpacer();
        GUILayout.Label("Hex Location", EditorStyles.boldLabel);

        GUILayout.BeginVertical();

        EditorGUILayout.Vector2Field("Grid Position", currentElementInfo.gridLoc);

        //int prevMapLayer = currentLayer;
        int newMapLayer = currentLayer;

        Color defaultColour = GUI.backgroundColor;
        Color selectecColour = new Color(0.4f, 0.6f, 0.8f);

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.onHover.textColor = Color.blue;

        GUILayoutOption[] buttonLayoutOptions = new GUILayoutOption[]
        {
            GUILayout.MinWidth(150),
            GUILayout.MaxWidth(150),
        };


        //GUILayout.BeginHorizontal();
        //GUILayout.Label("Toggle Visible");
        //GUILayout.Label("Select Layer to Edit");
        //GUILayout.EndHorizontal();

        for (int i = 0; i < MapSpawner.MAX_LEVEL_LAYERS; i++)
        {
            GUILayout.BeginHorizontal();
            bool isHidden = MapSpawner.Instance.GetGridFromLayer(i).GetIsHidden();
            string iconContent = isHidden ? "d_animationvisibilitytoggleoff" : "d_animationvisibilitytoggleon";
            if (GUILayout.Button(EditorGUIUtility.IconContent(iconContent), buttonStyle, buttonLayoutOptions))
            {
                isHidden = !isHidden;
                MapSpawner.Instance.GetGridFromLayer(i).SetIsHidden(isHidden);
            }
            

                if (currentLayer == i) GUI.backgroundColor = selectecColour;
            if (GUILayout.Button("LAYER " + i, buttonStyle, buttonLayoutOptions))
                newMapLayer = i;
            GUI.backgroundColor = defaultColour;

            GUILayout.EndHorizontal();
        }

        if (currentLayer != newMapLayer)
            ChangeMapLayer(newMapLayer);
        

        GUILayout.EndVertical();
    }


    void FindPreviewTexture()
    {
        //foreach (GameObject h in HexBank.Instance.hexPrefabs)
        //{
        //    ////Debug.Log(h.gameObject.name);
        //    //if (h.GetComponent<Hex>().GetTypeOfHex() == hexType)            
        //    //    previewTexture = AssetPreview.GetAssetPreview(h);
        //}
    }

    #region Attribute Methods
    void DisplayAttributeOptions()
    {

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.onHover.textColor = Color.blue;

        GUILayoutOption[] buttonLayoutOptions = new GUILayoutOption[]
        {
            GUILayout.MinWidth(150),
            GUILayout.MaxWidth(150),
        };


        #region Attribute Display

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(instance.position.width), GUILayout.Height(instance.position.height - 350));

        currBackGolourInd = 0;
        if (attributeChoiceFields != null)
        {
            for (int i = 0; i < attributeChoiceFields.Count; i++)
            {
                GUILayout.BeginHorizontal();

                var tmp = attributeChoiceFields[i];
                DisplayAttribute(ref tmp);
                attributeChoiceFields[i] = tmp;

                GUILayout.EndHorizontal();
            }
        }

        #region Choice Removal
        if (choicesToRemove != null)
        {
            foreach (AttributeChoice c in choicesToRemove)
            {
                int index = attributeChoiceFields.IndexOf(c);
                if (index != -1)
                    attributeChoiceFields.Remove(c);
            }
        }
        #endregion


        GUILayout.BeginHorizontal();
        GUILayout.Space(Screen.width / 2 - 1 / 2);
        if (GUILayout.Button(EditorGUIUtility.IconContent("d_AS Badge New"), buttonStyle, buttonLayoutOptions))
            AddAttribute();

        GUILayout.EndHorizontal();

        buttonStyle.alignment = TextAnchor.LowerLeft;

        EditorGUILayout.EndScrollView();




        #endregion





    }

    void DisplayAttribute(ref AttributeChoice choice)
    {

        GUIStyle gsAlterQuest = new GUIStyle();

        currBackGolourInd = currBackGolourInd == 0 ? 1 : 0;
        gsAlterQuest.normal.background = MakeTex(1, 1, backgroundColourArray[currBackGolourInd]);

        #region LeftColumn
        GUILayout.BeginHorizontal(gsAlterQuest);

        GUIStyle style = new GUIStyle();
        style.fixedWidth = 20;
        style.fixedHeight = 20;

        #region Compatibility Warning
        if (!choice.compatible)

            GUILayout.Label(EditorGUIUtility.IconContent("console.warnicon"), style);
        else
            GUILayout.Space(23);
        #endregion


        choice.enabled = EditorGUILayout.Toggle(choice.enabled);


        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);

        if (GUILayout.Button(EditorGUIUtility.IconContent("d_AS Badge Delete"), buttonStyle))
            RemoveAttribute(ref choice);

        GUI.enabled = choice.enabled;

        GUILayout.Space(40);
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.BeginVertical(gsAlterQuest);




        if (choice.attributeChoices == null)
        {
            UpdateAttributeChoices(ref choice);
            choice.attributeInstance = attFinder.InstantiateNewAttibute(choice.attributeChoices[choice.choiceIndex]);
        }

        int choiceIndex = choice.choiceIndex;
        choiceIndex = EditorGUILayout.Popup(choiceIndex, choice.attributeChoices);


        if (choice.choiceIndex != choiceIndex)
        {
            choice.choiceIndex = choiceIndex;
            if (choice.attributeChoices[choiceIndex] != "")
            {
                choice.attributeInstance = attFinder.InstantiateNewAttibute(choice.attributeChoices[choice.choiceIndex]);
                UpdateAttributeChoices(ref choice);
                CheckAllAttributesCompatible();
            }
        }


        GUILayout.Space(10);

        if (choice.attributeInstance != null) choice.attributeInstance.DisplayEditorAttributeOptions();


        GUILayout.EndVertical();



        GUI.enabled = true;

        GUILayout.Space(20);

    }

    private void UpdateAttributeChoices(ref AttributeChoice choiceToUpdate)
    {
        List<string> foundChoices = new List<string>();

        int currentIndex = choiceToUpdate.choiceIndex;
        bool isCompatible;

        foundChoices.Add("");
        if (attFinder == null)
        {
            Debug.LogWarning("AtributeFinder is null.");
            return;
        }
        for (int index = 0; index < LevelEditorWindow.instance.attFinder.GetAttributesDict().Count; index++)
        {
            var item = LevelEditorWindow.instance.attFinder.GetAttributesDict().ElementAt(index);
            var itemKey = item.Key;
            var itemValue = item.Value;


            //Check whether the attribute is already in use 
            bool attributeExists = false;
            foreach (AttributeChoice c in attributeChoiceFields)
                if (c.attributeChoices != null && (string)itemKey == c.attributeChoices[c.choiceIndex])
                    attributeExists = true;

            if (!attributeExists)
            {
                //isCompatible = attFinder.GetCompatible(itemKey, hexType);
                /*if (isCompatible) */foundChoices.Add((string)itemKey);
            }
        }

        if (choiceToUpdate.attributeChoices != null)
        {
            foundChoices.Insert(choiceToUpdate.choiceIndex, choiceToUpdate.attributeChoices[choiceToUpdate.choiceIndex]);
        }
        choiceToUpdate.attributeChoices = foundChoices.ToArray();

    }

    void AddAttribute()
    {
        if (attributeChoiceFields == null) attributeChoiceFields = new List<AttributeChoice>();
        attributeChoiceFields.Add(new AttributeChoice());
    }


    List<AttributeChoice> choicesToRemove;
    void RemoveAttribute(ref AttributeChoice choiceToRemove)
    {
        // Choices to be removed are added to a list so that they can be removed after the loop displaying their values has concluded
        if (choicesToRemove == null) choicesToRemove = new List<AttributeChoice>();
        choicesToRemove.Add(choiceToRemove);
    }
    #endregion



    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }


    void DisplayGUISpacer()
    {
        GUILayout.Space(10);

        Rect rect = EditorGUILayout.GetControlRect(false, 1);
        rect.height = 1;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }


    #region File Methods

    void DisplayFileOptions()
    {

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.onHover.textColor = Color.blue;

        GUILayoutOption[] buttonLayoutOptions = new GUILayoutOption[]
        {
            GUILayout.MinWidth(150),
            GUILayout.MaxWidth(150),
        };

        #region Save Load Clear Buttons
        GUILayout.BeginArea(new Rect((Screen.width / 2) - 230, 0, 500, 100));
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save Level", buttonStyle, buttonLayoutOptions))
            SaveLevel();
        if (GUILayout.Button("Load Level", buttonStyle, buttonLayoutOptions))
            LoadLevel();
        if (GUILayout.Button("New Level", buttonStyle, buttonLayoutOptions))
            NewLevel();

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
        GUILayout.Space(40);

        #endregion
    }

    void SaveLevel()
    {

        HexagonGrid grid = MapSpawner.Instance.GetCurrentGrid();


        //TODO: Hex probably shouldn't contain ElementAttributes - should be stored in Map Element. Does this need refactoring?

        if (levelBeingEdited == null) levelBeingEdited = new Level();


        MapElement[][] elementsToSave = new MapElement[mapElementsLocalRef.Count][];
        for (int i = 0; i < elementsToSave.Length; i++) 
        //foreach(MapElement[] e in elementsToSave)
        {
            elementsToSave[i] = mapElementsLocalRef[i].ToArray();
        }
        levelBeingEdited.hexs = elementsToSave;

        //for (int i = 0; i < mapElementsLocalRef.Count; i++)
        //{

        //    levelBeingEdited.hexs[i] = mapElementsLocalRef[i].ToArray();
        //}

        LevelLoader.Instance.SaveLevelFile(levelBeingEdited); // will make it so folders to where you can save it are limited for player input

        AssetDatabase.Refresh();

        GUIUtility.ExitGUI(); // Fixes "EndLayoutGroup: BeginLayoutGroup must be called first" error by preventing subsequent GUI functions from evaluation for the remainder of the GUI loop.
    }

    void LoadLevel()
    {
        //TODO: Display warning prompt
        Level loadedLevel = LevelLoader.Instance.LoadLevelFile();
        if (loadedLevel != null)
        {
            foreach (Hex hex in MapSpawner.Instance.GetCurrentGrid().GetComponentsInChildren<Hex>())
            {
                hex.gameObject.SetActive(false);
                HexBank.Instance.AddDisabledHex(hex.gameObject);
            }

            levelBeingEdited = loadedLevel;

            MapSpawner.Instance.SpawnLevel(
                levelBeingEdited,
                MapSpawner.Instance.GetCurrentGrid().transform.position + new Vector3(0, 30, 0),
                false
                );

            //mapElementsLocalRef.Clear();

            for (int i = 0; i < mapElementsLocalRef.Count; i++)
            {
                mapElementsLocalRef[i].Clear();
                mapElementsLocalRef[i] = levelBeingEdited.hexs[i].ToList();
                //mapElementsLocalRef[currentLayer] = levelBeingEdited.hexs[currentLayer].ToList();
            }
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
                mapElementsLocalRef.Clear();
            }
        }
        else
        {
            CleanupSpawnedHexes();
            UpdateHexCursorObject();

            levelBeingEdited = new Level(); //<<<<<<

        }
    }

    #endregion

    #region Scene Hex Methods
    private void AddHex()
    {
        List<ElementAttribute> attributes = new List<ElementAttribute>();
        if (attributeChoiceFields != null && attributeChoiceFields.Count >= 1)
        {
            foreach (AttributeChoice c in attributeChoiceFields)
            {
                if (c.enabled && c.attributeInstance != null)
                {
                    ElementAttribute clone = c.attributeInstance.Clone();
                    attributes.Add(clone);
                }
            }
        }

       //MapElement element = new MapElement(currentElementInfo.gridLoc, currentElementInfo.iconAtlasName, currentElementInfo.iconIndex, attributes);
       MapElement element = new MapElement(currentElementInfo, attributes);
       MapSpawner.Instance.SpawnAHex(element, currentLayer);

        mapElementsLocalRef[currentLayer].Add(element);
    }

    private void RemoveHex()
    {
        foreach(MapElement element in mapElementsLocalRef[currentLayer])
        {
            if (element.gridPos == currentElementInfo.gridLoc)
            {
                if (mapElementsLocalRef[currentLayer].Contains(element))
                    mapElementsLocalRef[currentLayer].Remove(element);
               break;
            }
        }

        MapSpawner.Instance.RemoveHexAtPoint(currentElementInfo.gridLoc);
        
    }

    private void UpdateHexCursorObject()
    {
        if (cursorHex != null)
        {
            HexMatComponent newmat = cursorHex.GetComponent<HexMatComponent>()/*.sharedMaterials*/;
            newmat.SetAtlas(currentElementInfo.iconAtlasName);
            newmat.SetMaterialIcon(currentElementInfo.iconIndex);
            newmat.SetEmissionColour(currentElementInfo.baseColour, currentElementInfo.iconColour);
            newmat.SetIconRotation(currentElementInfo.iconRotation);
        }
        else
        {
            cursorHex = Instantiate(Resources.Load(hexGhostPath) as GameObject);

            currentElementInfo.baseColour = cursorHex.GetComponent<MeshRenderer>().sharedMaterials[1].GetColor(AtlasMatConstants.BASE_COLOUR_REF);
            currentElementInfo.iconColour = cursorHex.GetComponent<MeshRenderer>().sharedMaterials[1].GetColor(AtlasMatConstants.ICON_COLOUR_REF);

        }
    }

    private void DrawMousePosition()
    {
        if (cursorHex != null && foundWorldPos != null)
        {
            cursorHex.transform.position = foundWorldPos.Value + new Vector3(0, 0.2f, 0);
            cursorHex.transform.rotation = MapSpawner.Instance.GetCurrentGrid().GetGridRotation();
        }
    }


    private void ChangeMapLayer(int newLayer)
    {

        MapSpawner.Instance.ChangeMapLayer(newLayer);

        currentLayer = newLayer;
    }

    #endregion

    void OnSceneGUI(SceneView sceneView)
    {

        // Overrides default Unity Editor Window input controls
        if (Event.current.type == EventType.Layout)
            HandleUtility.AddDefaultControl(0);


        switch (GetEditMode())
        {
            case EEditorMode.Draw:
                HandleDrawModeInput();
                break;
            case EEditorMode.Inspect:
                HandleInspectModeInput();
                break;
        }


        SceneView.RepaintAll();
    }

    private void HandleDrawModeInput()
    {
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

                Vector2Int targetGridPos = MapSpawner.Instance.GetCurrentGrid().WorldToCell(mousePos);
                foundWorldPos = MapSpawner.Instance.GetCurrentGrid().CellToWorld(targetGridPos);
                if (foundWorldPos == null) return; // No grid cell found

                worldMousePos = foundWorldPos.Value;
                worldMousePos.y = MapSpawner.Instance.GetCurrentGrid().transform.position.y;
                currentElementInfo.gridLoc = new Vector2Int(targetGridPos.x, targetGridPos.y);
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
        }
    }

    int selectedMapElementIndex;

    private void HandleInspectModeInput()
    {
        Event e = Event.current;

        //TODO: Highlight selected hex

        Vector2Int targetGridPos;
        if (e.type == EventType.MouseMove)
        {
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Vector3 mousePos;

            Plane plane = new Plane(Vector3.up, 0);

            float distance;
            if (plane.Raycast(mouseRay, out distance))   // outs the required distance
            {
                mousePos = mouseRay.GetPoint(distance);

                targetGridPos = MapSpawner.Instance.GetCurrentGrid().WorldToCell(mousePos);
                foundWorldPos = MapSpawner.Instance.GetCurrentGrid().CellToWorld(targetGridPos);
                if (foundWorldPos == null) return; // No grid cell found

                worldMousePos = foundWorldPos.Value;
                worldMousePos.y = MapSpawner.Instance.GetCurrentGrid().transform.position.y;
                currentElementInfo.gridLoc = new Vector2Int(targetGridPos.x, targetGridPos.y);
                worldLoc = foundWorldPos.Value;
            }
        }

        int previousSelectedIndex = selectedMapElementIndex;
        if (e.type == EventType.MouseDrag || e.type == EventType.MouseDown) // e.button == 0) 
        {
            if (e.button == 0)
            {
                foreach (MapElement m in mapElementsLocalRef[currentLayer])
                {
                    if (m.gridPos == currentElementInfo.gridLoc)
                    {
                        // element being inspected = m;
                        selectedMapElementIndex = mapElementsLocalRef[currentLayer].IndexOf(m);
                        break;
                    }
                }
            }
            else if (e.button == 1)
            {
               // RemoveHex();
            }
        }

        if (previousSelectedIndex != selectedMapElementIndex) // New Element Selected
        {

        }
    }

    bool enableEditing = false;

    private void DisplaySelectedMapElement()
    {

        //TODO: Show disabled when hover over, and enable on click for editing. 
        //      Apply or revert changes to selected map element, then unselect


        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Enable Editing for Map Element: ");
        enableEditing = EditorGUILayout.Toggle(enableEditing);
        GUILayout.EndHorizontal();

        GUI.enabled = enableEditing;

        if (mapElementsLocalRef != null && mapElementsLocalRef[currentLayer].Count > selectedMapElementIndex)
        {
            //mapElementsLocalRef[currentLayer][selectedMapElementIndex].hexType = (HexTypeEnum)EditorGUILayout.EnumPopup("Type of Hex", mapElementsLocalRef[currentLayer][selectedMapElementIndex].hexType);


            string[] atlasNames = IconAtlasDB.GetDatabaseKeys();
            int selectedIndex = System.Array.FindIndex(atlasNames, row => row == mapElementsLocalRef[currentLayer][selectedMapElementIndex].iconAtlasName);
            int newIndex = EditorGUILayout.Popup(selectedIndex, atlasNames);
            if (newIndex != selectedIndex)
            {
                mapElementsLocalRef[currentLayer][selectedMapElementIndex].iconAtlasName = atlasNames[newIndex];
            }

            mapElementsLocalRef[currentLayer][selectedMapElementIndex].iconIndex = EditorGUILayout.Popup(mapElementsLocalRef[currentLayer][selectedMapElementIndex].iconIndex, IconAtlasDB.GetAtlasIconNames(mapElementsLocalRef[currentLayer][selectedMapElementIndex].iconAtlasName));


            if (mapElementsLocalRef[currentLayer][selectedMapElementIndex].hexAttributes != null)
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(instance.position.width), GUILayout.Height(instance.position.height - 350));

                for (int i = 0; i < mapElementsLocalRef[currentLayer][selectedMapElementIndex].hexAttributes.Count; i++)
                {

                    ElementAttribute tmp = (ElementAttribute)mapElementsLocalRef[currentLayer][selectedMapElementIndex].hexAttributes[i];
                    //DisplayAttribute(ref tmp);

                    var attToDisplay = tmp.GetType();
                    tmp.DisplayEditorAttributeOptions();

                }

                EditorGUILayout.EndScrollView();
            }

      
        }

        //if (GUILayout.Button("Apply"))

        //if (GUILayout.Button("Revert"))


                GUILayout.EndVertical();

        GUI.enabled = true;
    }



    private void DrawSceneAttributeLabels()
    {
        /*
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
        */
    }


}

#endif