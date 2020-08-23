using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;



/// <summary> HexBank <c>Point</c> Performs as a simple object pool for storing disabled hex objects.</summary>
/// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments#:~:text=XML%20are%20followed.-,Introduction,and%20two%20stars%20(%20%2F**%20).

#if (UNITY_EDITOR) 
[System.Serializable, ExecuteInEditMode, InitializeOnLoad]
#endif
public class HexBank : MonoBehaviour
{
    private static HexBank instance;
    public static HexBank Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<HexBank>();
            }
            return instance;
        }
    }

    public GameObject hexPrefab;
    private List<GameObject> disabledHexObjects = new List<GameObject>();

    public GameObject GetDisabledHex(Vector3 position, Transform parent, int recursionAccumulator = 0)
    {
        GameObject target =  null;
        Quaternion rotation = Quaternion.Euler(-0, 0, 0);

        if (disabledHexObjects.Count > 0)
        {
            target = disabledHexObjects.First();
            disabledHexObjects.Remove(target);
        }
        else
        {
            GameObject newPrefab = hexPrefab;
            target = Instantiate(newPrefab);
            target.SetActive(false);
        }


        if (target != null)
        {
            target.transform.parent = parent;
            target.transform.SetPositionAndRotation(position, rotation);

            target.SetActive(true);

            return target;
        }

        // In case a new hex fails to spawn; the method is called recursively until a target is found or recursion depth is too great.
        // This was initially added as while using this method in the unity editor - the returned 'target' would sometimes be null. 
        else if (recursionAccumulator < 5)
        {
            return GetDisabledHex(position, parent, ++recursionAccumulator);
        }
        else return null;
    }


    public void AddDisabledHex(GameObject hexObject)
    {

        Hex hex = hexObject.GetComponent<Hex>();

        BaseHexComponent[] attributeComponents = hexObject.GetComponents<BaseHexComponent>();

        //Component[] attributeComponents = hexObject.GetComponents<ElementAttribute>() as Component[];
        foreach (BaseHexComponent e in attributeComponents)
        {
            e.CleanupComponent();

#if (UNITY_EDITOR)
            Component.DestroyImmediate(e);
#endif
            if (e != null) Component.Destroy(e);

        }

        disabledHexObjects.Add(hexObject);
        hexObject.transform.parent = this.transform;
    }



    //[SerializeField]
    //public GameObject[] hexPrefabs;


    //  public List<HexTypeHolder> disableHexTypes = new List<HexTypeHolder>();

    /*
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
#if (UNITY_EDITOR)
        Cleanup();
#endif
        disableHexTypes = new List<HexTypeHolder>();
    }

#if (UNITY_EDITOR)
    static HexBank()
    {
        EditorApplication.quitting += CleanupSpawnedHexes; // Delegated to destroy stored hexes as the application quits
    }

    [ContextMenu("CleanupSpawnedHexes")]
    public void Cleanup()
    {
        CleanupSpawnedHexes();

    }
#endif

    // Destroys all hexes stored by the HexBank in the scene

    public static void CleanupSpawnedHexes()
    {
        //EditorApplication.Beep();

        foreach (HexTypeHolder holder in HexBank.Instance.disableHexTypes)
        {
            holder.ClearDisabledObjects();

        }
    }



    //public HexTypeEnum GetTypeAtIndex(int index)
    //{

    //    if (index > hexPrefabs.Length)
    //    {
    //        return hexPrefabs[0].GetComponent<Hex>().GetTypeOfHex();
    //    }
    //    else
    //    {
    //        return hexPrefabs[index].GetComponent<Hex>().GetTypeOfHex();
    //    }
    //}

    //public Hex GetHexFromType (HexTypeEnum hexType)
    //{
    //    //Debug.Log("GetHexFromType(" + hexType + ")" + " = " + hexPrefabs.ToList().Find(x => x.GetComponent<Hex>().GetTypeOfHex() == hexType).GetComponent<Hex>() );
    //    return hexPrefabs.ToList().Find(x => x.GetComponent<Hex>().GetTypeOfHex() == hexType).GetComponent<Hex>();
    //}

    public void AddDisabledHex(GameObject hexObject)
    {
        Hex hex = hexObject.GetComponent<Hex>();

        BaseHexComponent[] attributeComponents = hexObject.GetComponents<BaseHexComponent>();

        //Component[] attributeComponents = hexObject.GetComponents<ElementAttribute>() as Component[];
        foreach (BaseHexComponent e in attributeComponents)
        {
            e.CleanupComponent();

#if (UNITY_EDITOR)
            Component.DestroyImmediate(e);
#endif
           if (e != null) Component.Destroy(e);

        }



        if (disableHexTypes.Exists(x => x.hexType == hex.GetTypeOfHex()))
        {
            disableHexTypes.Find(x => x.hexType == hex.GetTypeOfHex()).disabledHexObjects.Add(hexObject);
            hexObject.transform.parent = this.transform;
        }
        else
        {
            disableHexTypes.Add(new HexTypeHolder(hex.GetTypeOfHex()));
            disableHexTypes.Find(x => x.hexType == hex.GetTypeOfHex()).disabledHexObjects.Add(hexObject);
            hexObject.transform.parent = this.transform;
        }
    }

    public GameObject GetDisabledHex(Vector3 position, Transform parent, int recursionAccumulator = 0)
    {
        Quaternion rotation = Quaternion.Euler(-0, 0, 0);


        GameObject target;
        if (disableHexTypes.Exists(x => x.hexType == hexType) && disableHexTypes.Find(x => x.hexType == hexType).disabledHexObjects.Count != 0)
        {
            target = disableHexTypes.Find(x => x.hexType == hexType).PullFirstHexObject();
        }
        else
        {
            //GameObject newPrefab = hexPrefabs.ToList().Find(x => x.GetComponent<Hex>().GetTypeOfHex() == hexType);
            GameObject newPrefab = hexPrefabs.ToList().Find(x => x.GetComponent<Hex>().GetTypeOfHex() == hexType);
            target = Instantiate(newPrefab);
            target.SetActive(false); // Set to false before the position is set in order to prevent OnEnable initiating visual effects prematurely
        }

        if (target != null)
        {
            target.transform.parent = parent;
            target.transform.SetPositionAndRotation(position, rotation);

            target.SetActive(true);

            return target;
        }


        // In case a new hex fails to spawn; the method is called recursively until a target is found or recursion depth is too great.
        // This was initially added as while using this method in the unity editor - the returned 'target' would sometimes be null. 
        else if (recursionAccumulator < 40)
        {
            Debug.Log("RecursionAcc = " + recursionAccumulator);
            return GetDisabledHex(hexType, position, parent, ++recursionAccumulator);
        }
        else return null;
    }
   


    [System.Serializable]
    public class HexTypeHolder
    {
        public HexTypeEnum hexType;
        public List<GameObject> disabledHexObjects = new List<GameObject>();

        public GameObject PullFirstHexObject()
        {
            GameObject targetObject = disabledHexObjects.First();
            disabledHexObjects.Remove(targetObject);

            return targetObject;
        }


        public HexTypeHolder(HexTypeEnum newHexType)
        {
            this.hexType = newHexType;
        }

        public void ClearDisabledObjects()
        {

            foreach (GameObject hex in disabledHexObjects)
            {


                //disabledHexObjects.Remove(hex);
                DestroyImmediate(hex);
            }

            disabledHexObjects.Clear();
        }
    }
    */


}
