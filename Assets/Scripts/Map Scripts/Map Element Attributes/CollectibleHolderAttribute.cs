using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class CollectibleHolderElementAttribute : ElementAttribute
{

    //[SerializeField]
    //public CollectibleType CollectibleToSpawn;

    // The name of the object is stored, rather than the object itself. The name is later used to get a reference to the object
    //[JsonIgnore]
    Object objectToSpawn;

    [SerializeField]
    public string objectToSpawnSerialisedName = "null";

    public CollectibleHolderElementAttribute()
    {

    }

    #region Cloning
    public override ElementAttribute Clone()
    {
        return new CollectibleHolderElementAttribute(this);
    }

    protected CollectibleHolderElementAttribute(CollectibleHolderElementAttribute other) : base(other)
    {
        this.objectToSpawn = other.objectToSpawn;
        this.objectToSpawnSerialisedName = other.objectToSpawnSerialisedName;
    }
    #endregion

    public override string GetDisplayName()
    {
        return this.GetType().ToString();
    }

    public override void AddAttributeToHex(Hex hexInstance)
    {
        CollectibleComponent component = hexInstance.gameObject.AddComponent<CollectibleComponent>();
        //component.SpawnCollectible(objectToSpawnSerialisedName);
    }

    public override void DisplayEditorAttributeOptions()
    {
        EditorGUILayout.BeginHorizontal();
        GameObject selectedObject = EditorGUILayout.ObjectField(objectToSpawn, typeof(GameObject), false) as GameObject;
        if (selectedObject != objectToSpawn)
        {
            if (selectedObject != null && PrefabUtility.GetPrefabAssetType(selectedObject) == PrefabAssetType.Regular)
            {
                objectToSpawn = selectedObject;
                objectToSpawnSerialisedName = objectToSpawn.name;
            }
            else
            {
                objectToSpawn = null;
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}


