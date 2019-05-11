using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelEditorWindow : EditorWindow
{
    [MenuItem("Window/Level Editor")]
    static void Init()
    {
        LevelEditorWindow window = (LevelEditorWindow)EditorWindow.GetWindow(typeof(LevelEditorWindow));
        window.Show();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    //void OnSceneGUI(SceneView sceneView)
    //{
    //    Vector3 mousePosition = Event.current.mousePosition;
    //    mousePosition.y = sceneView.camera.pixelHeight - mousePosition.y;
    //    mousePosition = sceneView.camera.ScreenToWorldPoint(mousePosition);
    //    mousePosition.y = -mousePosition.y;

    //    Debug.Log("Mouse Pos on Scene = " + mousePosition);
    //}
}
