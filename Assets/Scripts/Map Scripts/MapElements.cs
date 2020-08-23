using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Runtime;
using Newtonsoft.Json;


public class AttributeFinder
{

    private Dictionary<string, System.Type> foundAttributeTypeDict;

    public Dictionary<string, System.Type> GetAttributesDict()
    {
        Dictionary<string, System.Type> returnDict = foundAttributeTypeDict;
        return returnDict;
    }

    public AttributeFinder()
    {
        foundAttributeTypeDict = FindAttributeTypesDictionary();
    }

    //public bool GetCompatible(string attributeTypeName, HexTypeEnum hexType)
    //{

    //    if (foundAttributeTypeDict.ContainsKey(attributeTypeName))
    //    {
    //        var item = foundAttributeTypeDict[attributeTypeName];

    //        var methodInfo = item.GetMethod("GetCompatible");
    //        if (methodInfo != null)
    //        {
    //            var value = methodInfo.Invoke(null, new object[] { hexType });
    //            return (bool)value;
    //        }
    //        else return true;
    //    }
    //    else return true;
    //}

    public Dictionary<string, System.Type> FindAttributeTypesDictionary()
    {
        var types = System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(ElementAttribute));


        Dictionary<string, System.Type> returnDict = new Dictionary<string, System.Type>();

        //lementAttribute[] attributeTypes = new ElementAttribute[types.Length];
        for (int i = 0; i < types.Length; i++)
        {
            if (types[i].IsSubclassOf(typeof(ElementAttribute)) && !types[i].IsAbstract)
            {

                ElementAttribute newAtribute = (ElementAttribute)System.Activator.CreateInstance(types[i]);
                if (newAtribute != null) returnDict.Add(newAtribute.GetDisplayName(), newAtribute.GetType()/*typeof(ElementAttribute)*/);
                //Debug.Log(newAtribute.GetDisplayName() + ", " + typeof(ElementAttribute));
            }
        }

        return returnDict;
    }

    /* Returns a new instance of ElementAttribute with the relative default values to the HexType */
    public ElementAttribute InstantiateNewAttibute(string attrubuteTypeName/*, HexTypeEnum type*/)
    {
        //https://docs.microsoft.com/en-us/dotnet/api/system.activator.createinstance?view=netcore-3.1
        //CreateInstance(Type, Object[]) // where Object[] is found by GetDefaultParametersForAttribute();

        System.Object[] parameters = GetDefaultParametersForAttribute(attrubuteTypeName/*, type*/);

        if (parameters == null) parameters = new object[0]; // If no parameters are found, then the default constructor will be used

        for (int i = 0; i < parameters.Length; i++)
        {
            // Newtonsoft deserialises integers as System.Int64, but the program uses int32, so we convert where necessary to avoid a Type mismatch
            if (parameters[i] is System.Int64)
            {
                parameters[i] = System.Convert.ToInt32(parameters[i]);
            }
        }

        if (foundAttributeTypeDict.ContainsKey(attrubuteTypeName))
        {
            object newAtribute = System.Activator.CreateInstance(foundAttributeTypeDict[attrubuteTypeName], parameters);
            return (ElementAttribute)newAtribute;
        }
        else return null;


    }

    private System.Object[] GetDefaultParametersForAttribute(string attrubuteTypeName/*, HexTypeEnum type*/)
    {
        AttributeArgsLoader argsLoader = new AttributeArgsLoader();
        return argsLoader.GetArgsForAttribute(attrubuteTypeName/*, type.ToString()*/);
    }

}


public struct ElementInfo
{
    public string elementName;
    public string iconAtlasName;
    public int iconIndex;
    public Vector2Int gridLoc;
    public int mapLayer;
    public SerializableColor iconColour;
    public SerializableColor baseColour;
    public float iconRotation;
}

/* Map Elements contain the information for an individual tile on the board within a Level*/
[System.Serializable]
public class MapElement
{
    public string displayName; // The name used to identify element presets

    public Vector2Int gridPos;
    public int mapLayer;

    public string iconAtlasName;
    public int iconIndex;

    public SerializableColor baseColour;
    public SerializableColor iconColour;

    public float iconRotation;

    [JsonProperty(PropertyName = "HexAttributes")]
    public List<ElementAttribute> hexAttributes = new List<ElementAttribute>();




    public Hex GetHex()
    {
        throw new System.NotImplementedException();
        //return HexBank.Instance.GetHexFromType(hexType); ;
    }

    [JsonConstructor]
    public MapElement(string displayName,  Vector2Int gridPos, int mapLayer, string iconAtlasName, int iconIndex, SerializableColor baseColour, SerializableColor iconColour, float iconRotation, List<ElementAttribute> hexAttributes = null)
    {
        this.displayName = displayName;
        this.gridPos = gridPos;
        this.mapLayer = mapLayer;
        this.iconAtlasName = iconAtlasName;
        this.iconIndex = iconIndex;
        this.baseColour = baseColour;
        this.iconColour = iconColour;
        this.iconRotation = iconRotation;
        if (hexAttributes == null) this.hexAttributes = new List<ElementAttribute>();
        else this.hexAttributes = hexAttributes;
    }

    public MapElement(ElementInfo mapElementInfo, List<ElementAttribute> hexAttributes = null)
    {
        displayName = mapElementInfo.elementName;
        gridPos = mapElementInfo.gridLoc;
        mapLayer = mapElementInfo.mapLayer;
        iconAtlasName = mapElementInfo.iconAtlasName;
        iconIndex = mapElementInfo.iconIndex;
        baseColour = mapElementInfo.baseColour;
        iconColour = mapElementInfo.iconColour;
        iconRotation = mapElementInfo.iconRotation;
        if (hexAttributes == null) hexAttributes = new List<ElementAttribute>();

        this.hexAttributes = hexAttributes;
    }


    #region Operrator overrides

    // Equals opertor overrides currently only compare displaYName and are intended for use by the presetLoader
    public static bool operator ==(MapElement x, MapElement y)
    {
        if (object.ReferenceEquals(y, null))
        {
            if (object.ReferenceEquals(x, null))
            {
                return true;
            }
            return false;
        }




        return (x.displayName == y.displayName);
    }

    public static bool operator !=(MapElement x, MapElement y)
    {

        return !(x == y);
        return (x.displayName != y.displayName);
    }


    public override bool Equals(object obj)
    {
        if (!(obj is MapElement))
            return false;

        MapElement mys = (MapElement)obj;

        return (this.displayName == mys.displayName);
    }

    #endregion


}

#region ElementAttributes



[System.Serializable]
public class DigitElementAttribute : ElementAttribute
{

    [SerializeField]
    public int leadingZeroCount;

    [SerializeField]
    public int numberToDisplay;


    public override string GetDisplayName()
    {
        return this.GetType().ToString();
    }


    public DigitElementAttribute()
    {
        this.leadingZeroCount = 0;
        this.numberToDisplay = 0;
    }

    #region Cloning
    public override ElementAttribute Clone()
    {
        return new DigitElementAttribute(this);
    }

    protected DigitElementAttribute(DigitElementAttribute other) : base(other)
    {
        this.leadingZeroCount = other.leadingZeroCount;
        this.numberToDisplay = other.numberToDisplay;
    }
    #endregion




    public DigitElementAttribute(int leadingZeroCount = 0, int numberToDisplay = 0)
    {
        this.leadingZeroCount = leadingZeroCount;
        this.numberToDisplay = numberToDisplay;
    }

    //public static bool GetCompatible(HexTypeEnum type)
    //{
    //    return HexTypes.IsDigitType(type) && !HexTypes.IsMenuHexType(type) && !HexTypes.IsPlayType(type);
    //}



    public override void AddAttributeToHex(Hex hexInstance)
    {
       // hexInstance.AddAttribute(this);

        DigitComponent component = hexInstance.gameObject.AddComponent<DigitComponent>();
        component.leadingZeroCount = this.leadingZeroCount;
        component.numberToDisplay = this.numberToDisplay;


        // NOTE: We may want some kind of delegate/ listener system for the number to automatically update its display upon vakue change
        // https://answers.unity.com/questions/1206632/trigger-event-on-variable-change.html
        //https://answers.unity.com/questions/1021048/unity-editor-inspector-delegate-function-pointer.html
    }

    public override void DisplayEditorAttributeOptions(/*ElementAttribute currAttributeVals, out ElementAttribute newAttributeVals*/)
    {
        //newAttributeVals = currAttributeVals;

        // DigitElementAttribute digitAttribute = (DigitElementAttribute)currAttributeVals;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Number To Display:");
        GUILayout.Space(40);
        if (GUILayout.Button("-"))
            this.numberToDisplay--;
        this.numberToDisplay = EditorGUILayout.IntField(this.numberToDisplay);
        if (GUILayout.Button("+"))
            this.numberToDisplay++;

        if (this.numberToDisplay <= -1) this.numberToDisplay = 0;
        GUILayout.Space(40);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Leading Zero Count:");
        GUILayout.Space(40);
        GUILayout.Label(this.leadingZeroCount.ToString(), EditorStyles.largeLabel);
        this.leadingZeroCount = (int)GUILayout.HorizontalSlider(this.leadingZeroCount, -10, 10);
        GUILayout.Space(40);
        GUILayout.EndHorizontal();
    }

    //public override System.Object[] GetElementParams()
    //{
    //    return new System.Object[] { leadingZeroCount, numberToDisplay };
    //}


}


[System.Serializable]
public class MenuButtonElementAttribute : ElementAttribute
{

    [SerializeField]
    public Command commandToCall;


    public override string GetDisplayName()
    {
        return this.GetType().ToString();
    }

    public MenuButtonElementAttribute()
    {

    }

    public MenuButtonElementAttribute(Command commandToCall)
    {
        this.commandToCall = commandToCall;
    }

    public MenuButtonElementAttribute(int commandIndex)
    {
        this.commandToCall = (Command)commandIndex;
    }

    #region Cloning
    public override ElementAttribute Clone()
    {
        return new MenuButtonElementAttribute(this);
    }

    protected MenuButtonElementAttribute(MenuButtonElementAttribute other) : base(other)
    {
        this.commandToCall = other.commandToCall;
    }
    #endregion


    //public static bool GetCompatible(HexTypeEnum type)
    //{
    //    return !HexTypes.IsDigitType(type) && HexTypes.IsMenuHexType(type) && !HexTypes.IsPlayType(type);
    //}




    public override void AddAttributeToHex(Hex hexInstance)
    {
        hexInstance.clickedEvent.AddListener(() =>
            {
                GameManager.instance.ProcessCommand(commandToCall);
                hexInstance.DigHex(false); // Temp

            });
    }

    public override void DisplayEditorAttributeOptions(/*ElementAttribute currAttributeVals, out ElementAttribute newAttributeVals*/)
    {

        //GUILayout.BeginHorizontal();
        ////GUILayout.Label("Menu Stuff");
        //GUILayout.Space(40);

        //GUILayout.EndHorizontal();
        //newAttributeVals = currAttributeVals;


        //MenuButtonElementAttribute menuAttribute = (MenuButtonElementAttribute)currAttributeVals;
        this.commandToCall = (Command)EditorGUILayout.EnumPopup("Button Command", this.commandToCall);


        //newAttributeVals = new MenuButtonElementAttribute(menuAttribute.commandToCall);
    }

    //public override System.Object[] GetElementParams()
    //{
    //    return new System.Object[] { commandToCall };
    //}
}


public class DestroyOnExitAttribute : ElementAttribute
{

    public DestroyOnExitAttribute()
    {

    }


    public override void AddAttributeToHex(Hex hexInstance)
    {
        //throw new System.NotImplementedException();
        GUILayout.Label("I AM A SOURCE");
        DestroyOnExitComponent component = hexInstance.gameObject.AddComponent<DestroyOnExitComponent>();
        //component.SpawnCollectible(objectToSpawnSerialisedName);

    }



    #region Cloning
    public override ElementAttribute Clone()
    {
        return new DestroyOnExitAttribute(this);
    }


    protected DestroyOnExitAttribute(DestroyOnExitAttribute other) : base(other)
    {
        //this.objectToSpawn = other.objectToSpawn;
        //this.objectToSpawnSerialisedName = other.objectToSpawnSerialisedName;
    }
    #endregion

    public override void DisplayEditorAttributeOptions()
    {
       // throw new System.NotImplementedException();
    }

    public override string GetDisplayName()
    {
        return this.GetType().ToString();
    }

    //public override object[] GetElementParams()
    //{
    //    return null;
    //}
}



public class PowerSwitchAttribute : ElementAttribute
{
    bool initialPowerState;

    public PowerSwitchAttribute()
    {

    }

    public PowerSwitchAttribute(bool initialPowerState)
    {
        this.initialPowerState = initialPowerState;
    }

    public override void AddAttributeToHex(Hex hexInstance)
    {
        PowerSwitchComponent component = hexInstance.gameObject.AddComponent<PowerSwitchComponent>();
    }

    #region Cloning
    public override ElementAttribute Clone()
    {
        return new PowerSwitchAttribute(this);
    }

    protected PowerSwitchAttribute(PowerSwitchAttribute other) : base(other)
    {
        this.initialPowerState = other.initialPowerState;
        //this.objectToSpawnSerialisedName = other.objectToSpawnSerialisedName;
    }
    #endregion


    public override void DisplayEditorAttributeOptions()
    {

        EditorGUILayout.BeginHorizontal();

        initialPowerState = GUILayout.Toggle(initialPowerState, (initialPowerState ? EditorGUIUtility.IconContent("d_winbtn_mac_min") : EditorGUIUtility.IconContent("d_winbtn_mac_inact")));
        
        EditorGUILayout.EndHorizontal();
    }

    public override string GetDisplayName()
    {
        return this.GetType().ToString();
    }

    //public override object[] GetElementParams()
    //{
    //    throw new System.NotImplementedException();
    //}
}





public class PowerSourceAttribute : ElementAttribute
{
    //bool initialPowerState;
    //powerType
    public PowerSourceAttribute()
    {

    }

    public override void AddAttributeToHex(Hex hexInstance)
    {
        PowerSourceComponent component = hexInstance.gameObject.AddComponent<PowerSourceComponent>();
    }

    #region Cloning
    public override ElementAttribute Clone()
    {
        return new PowerSourceAttribute(this);
    }

    protected PowerSourceAttribute(PowerSourceAttribute other) : base(other)
    {
        //this.initialPowerState = other.initialPowerState;
        //this.objectToSpawnSerialisedName = other.objectToSpawnSerialisedName;
    }
    #endregion


    public override void DisplayEditorAttributeOptions()
    {
        EditorGUILayout.BeginHorizontal();

        //initialPowerState = GUILayout.Toggle(initialPowerState, (initialPowerState ? EditorGUIUtility.IconContent("d_winbtn_mac_min") : EditorGUIUtility.IconContent("d_winbtn_mac_inact")));
        GUILayout.Label("I AM THE SOURCE");
        EditorGUILayout.EndHorizontal();
    }

    public override string GetDisplayName()
    {
        return this.GetType().ToString();
    }

    //public override object[] GetElementParams()
    //{
    //    throw new System.NotImplementedException();
    //}
}





public class PowerConductorAttribute : ElementAttribute
{
    //bool initialPowerState;

    public PowerConductorAttribute()
    {

    }



    public override void AddAttributeToHex(Hex hexInstance)
    {
        Debug.Log("PowerConductorAttribute::AddAttributeToHex ");
        ConductorComponent component = hexInstance.gameObject.AddComponent<ConductorComponent>();
    }

    #region Cloning
    public override ElementAttribute Clone()
    {
        return new PowerConductorAttribute(this);
    }

    protected PowerConductorAttribute(PowerConductorAttribute other) : base(other)
    {
       // this.initialPowerState = other.initialPowerState;
        //this.objectToSpawnSerialisedName = other.objectToSpawnSerialisedName;
    }
    #endregion


    public override void DisplayEditorAttributeOptions()
    {

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("I AM THE CONDUCTOR");
        // initialPowerState = GUILayout.Toggle(initialPowerState, (initialPowerState ? EditorGUIUtility.IconContent("d_winbtn_mac_min") : EditorGUIUtility.IconContent("d_winbtn_mac_inact")));

        EditorGUILayout.EndHorizontal();
    }

    public override string GetDisplayName()
    {
        return this.GetType().ToString();
    }

    //public override object[] GetElementParams()
    //{
    //    throw new System.NotImplementedException();
    //}
}

#endregion

