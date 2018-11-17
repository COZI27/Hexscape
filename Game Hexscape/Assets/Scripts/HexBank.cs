using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HexBank : MonoBehaviour
{

    public static HexBank instance;

    public GameObject[] hexPrefabs;

    public Hex.DestroyState GetTypeAtIndex(int index)
    {
        return hexPrefabs[index].GetComponent<Hex>().destroyType;
    }

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

    public void AddDisabledHex(GameObject hexObject)
    {
        Hex hex = hexObject.GetComponent<Hex>();

        if (disableHexTypes.Exists(x => x.hexType == hex.destroyType))
        {
            disableHexTypes.Find(x => x.hexType == hex.destroyType).disabledHexObjects.Add(hexObject);
        }
        else
        {
            disableHexTypes.Add(new HexTypeHolder(hex.destroyType));
            disableHexTypes.Find(x => x.hexType == hex.destroyType).disabledHexObjects.Add(hexObject);
        }
    }

    public GameObject GetDisabledHex(Hex.DestroyState hexType, Vector3 position, Transform parent)
    {
        Quaternion rotation = Quaternion.Euler(-0, 0 ,0);

        GameObject target;
        if (disableHexTypes.Exists(x => x.hexType == hexType) && disableHexTypes.Find(x => x.hexType == hexType).disabledHexObjects.Count != 0)
        {
          target = disableHexTypes.Find(x => x.hexType == hexType).PullFirstHexObject();
            
        } else
        {
            GameObject newPrefab = hexPrefabs.ToList().Find(x => x.GetComponent<Hex>().destroyType == hexType);
            target = Instantiate(newPrefab);
        }

        target.transform.parent = parent;
        target.transform.SetPositionAndRotation(position, rotation);

        target.SetActive(true);

        return target;
    }
   


    [System.Serializable]
    public class HexTypeHolder
    {
        public Hex.DestroyState hexType;
        public List<GameObject> disabledHexObjects = new List<GameObject>();

        public GameObject PullFirstHexObject()
        {
            GameObject targetObject = disabledHexObjects.First();
            disabledHexObjects.Remove(targetObject);
            return targetObject;
        }


        public HexTypeHolder(Hex.DestroyState newHexType)
        {
            this.hexType = newHexType;
        }
    }
}
