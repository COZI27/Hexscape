using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable, ExecuteInEditMode]
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
                if (instance == null) Debug.LogError("No instance of HexBank was found.");
            }
            return instance;
        }
    }


    [SerializeField]
    public GameObject[] hexPrefabs;

   

    public List<HexTypeHolder> disableHexTypes = new List<HexTypeHolder>();


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
    }

    public HexTypeEnum GetTypeAtIndex(int index)
    {
        if (index > hexPrefabs.Length)
        {
            return hexPrefabs[0].GetComponent<Hex>().typeOfHex;
        } else
        {
            return hexPrefabs[index].GetComponent<Hex>().typeOfHex;
        }
    }

    public Hex GetHexFromType (HexTypeEnum hexType)
    {
        return hexPrefabs.ToList().Find(x => x.GetComponent<Hex>().typeOfHex == hexType).GetComponent<Hex>();
    }

    public void AddDisabledHex(GameObject hexObject)
    {
        Hex hex = hexObject.GetComponent<Hex>();

        if (disableHexTypes.Exists(x => x.hexType == hex.typeOfHex))
        {
            disableHexTypes.Find(x => x.hexType == hex.typeOfHex).disabledHexObjects.Add(hexObject);
        }
        else
        {
            disableHexTypes.Add(new HexTypeHolder(hex.typeOfHex));
            disableHexTypes.Find(x => x.hexType == hex.typeOfHex).disabledHexObjects.Add(hexObject);
        }
    }

    public GameObject GetDisabledHex(HexTypeEnum hexType, Vector3 position, Transform parent)
    {
        Quaternion rotation = Quaternion.Euler(-0, 0 ,0);

        GameObject target;
        if (disableHexTypes.Exists(x => x.hexType == hexType) && disableHexTypes.Find(x => x.hexType == hexType).disabledHexObjects.Count != 0)
        {
          target = disableHexTypes.Find(x => x.hexType == hexType).PullFirstHexObject();
            
        } else
        {
            GameObject newPrefab = hexPrefabs.ToList().Find(x => x.GetComponent<Hex>().typeOfHex == hexType);
            target = Instantiate(newPrefab);
            target.SetActive(false); // Set to false before the position is set in order to prevent OnEnable initiating visual effects prematurely
        }

        target.transform.parent = parent;
        target.transform.SetPositionAndRotation(position, rotation);

        target.SetActive(true);

        return target;
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
    }
}
