using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLine : MonoBehaviour {

    public GameObject targetObject;

    private LineRenderer lineRenderer;  

    // Use this for initialization
    void Start() {
        LineRenderer foundLineComp = GetComponent<LineRenderer>();
        if (foundLineComp) lineRenderer = foundLineComp;
        else lineRenderer = this.gameObject.AddComponent<LineRenderer>();        // Add a Line Renderer to the GameObject

        // Set the width of the Line Renderer
        // line.SetWidth(0.05F, 0.05F);
        // Set the number of vertex fo the Line Renderer
        lineRenderer.SetVertexCount(2);
    }

    // Update is called once per frame
    void Update() {
        // Check if the GameObjects are not null
        if (targetObject != null) {
            // Update position of the two vertex of the Line Renderer
            lineRenderer.SetPosition(0, this.transform.position);
            lineRenderer.SetPosition(1, targetObject.transform.position);
        }
    }
}
