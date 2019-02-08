using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is for my Level Database, if you right click  in the project tap and press create you can create a new level...
//Not sure if we will use a level database for endless but at the moment we are... 
// we might also want to seperate endless and challenge levels into diffrent children of the Level class

[CreateAssetMenu(fileName = "New Level")]
public class Level : ScriptableObject
{
   // public string levelName;
    public MapElement[] hexs;

    public int passAmount;
    public int bronzeAmount;
    public int silverAmount;
    public int goldAmount;

}

[System.Serializable]
public class MapElement
{
    public Vector2Int gridPos;
    public HexTypeEnum hexType;
    private Hex hex;

    public Hex GetHex ()
    {
        if (hex == null)
        {
            hex = HexBank.instance.GetHexFromType(hexType);
        }

        return hex;
    }

    public MapElement(HexTypeEnum hexType, Vector2Int gridPos)
    {
        this.hexType = hexType;
        this.gridPos = gridPos;
        Debug.Log("Hex type  = " + hexType);
        this.hex = HexBank.instance.GetHexFromType(hexType);
    }


}