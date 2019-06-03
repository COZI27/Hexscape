using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTypeUIElement : MonoBehaviour
{
    public HexTypeEnum hexType;
    public int scaleLerp;

    private Vector3 defaultScale;

    private bool isCurrent;
    

    private void OnMouseUpAsButton()
    {
        GameManager.instance.EditUIHexClick(hexType);
    }

    private void OnMouseOver()
    {
        transform.localScale = defaultScale * 1.2f;
    }

    private void OnMouseExit()
    {
        if (isCurrent == false)
        {
            transform.localScale = defaultScale;
        }
        
    }

    public void NewHexPicked(HexTypeEnum hexType)
    {
        isCurrent = hexType == this.hexType;

        if (isCurrent)
        {
            transform.localScale = defaultScale * 1.2f;
        } else
        {
            transform.localScale = defaultScale;
        }
        
    }

    public void OnDestroy()
    {
        GameManager.instance.editHexPicked -= NewHexPicked;
    }

    public delegate void NewUIEditHexPicked(HexTypeEnum hexType);
    

    public void SetUp(HexTypeEnum hexType)
    {
        Hex hex = HexBank.instance.GetHexFromType(hexType);

        MeshRenderer newMeshRend = hex.GetComponent<MeshRenderer>();
        MeshRenderer oldMeshRend = GetComponent<MeshRenderer>();

        MeshFilter newMesh = hex.GetComponent<MeshFilter>();
        MeshFilter oldMesh = GetComponent<MeshFilter>();

        oldMesh.sharedMesh = newMesh.sharedMesh;
        oldMeshRend.sharedMaterials = newMeshRend.sharedMaterials;


        this.hexType = hexType;

        gameObject.name = "HEX UI: " + hexType;

        defaultScale = transform.localScale;

        GameManager.instance.editHexPicked += NewHexPicked;
    }
    
}
