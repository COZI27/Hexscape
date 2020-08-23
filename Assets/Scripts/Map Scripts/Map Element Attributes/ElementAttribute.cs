using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ElementAttributes are used to ass additional functionality to MapElements within a Level. 
/// </summary>
/// <remarks>
/// Additional functionality for MapElements is svaed and loaded as an ElementAttribute which, when the level
/// loads, will add said functionality to their respective element via the AddAttributeToHex method.
/// </remarks>
[System.Serializable]
public abstract class ElementAttribute
{
    protected ElementAttribute()
    {

    }

    public abstract string GetDisplayName();

    public abstract void AddAttributeToHex(Hex hexInstance);

    public abstract void DisplayEditorAttributeOptions(/*ElementAttribute currAttributeVals, out ElementAttribute newAttributeVals*/);

    // Used by the Attribute Args Editor window to save parameter defaults
    //public abstract System.Object[] GetElementParams();

    #region Cloning
    // Cloning is intended to be utilised by the Level Editor when adding new MapElements and their attributes to the level.
    public abstract ElementAttribute Clone();
    protected ElementAttribute(ElementAttribute other)
    {
        //...
    }
    #endregion
}
