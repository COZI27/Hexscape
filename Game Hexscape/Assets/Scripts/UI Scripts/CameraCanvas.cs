using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DisplayObjectMap
{
    public enum EDisplayType
    {

        EndlessTierUp,
        HiScoreGlobal,
        HiScoreLocal,
        LoadingProfile
    }
    [SerializeField]
    public EDisplayType type;
    [SerializeField]
    public GameObject displayObj;

}


public class CameraCanvas : MonoBehaviour
{

    public static CameraCanvas instance;

    private void MakeSingleton()
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

    [SerializeField]
    GameObject particleObject;
    public GameObject GetParticleObject()
    {
        return particleObject;
    }

    GameObject currentDisplayObj;

    [SerializeField]
    public DisplayObjectMap[] displayObjectsInspectorArr;
    private Dictionary<DisplayObjectMap.EDisplayType, GameObject> displayObjectsDict;

    // Start is called before the first frame update
    void Start()
    {
        MakeSingleton();
        displayObjectsDict = new Dictionary<DisplayObjectMap.EDisplayType, GameObject>();
        foreach (DisplayObjectMap mapObj in displayObjectsInspectorArr)
        {
                displayObjectsDict.Add(mapObj.type, mapObj.displayObj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeDisplayType(DisplayObjectMap.EDisplayType newType)
    {
        GameObject foundObj;
        displayObjectsDict.TryGetValue(newType, out foundObj);
        if (foundObj != null)
        {
            if (currentDisplayObj != null) currentDisplayObj.SetActive(false);
            currentDisplayObj = foundObj;
            currentDisplayObj.SetActive(true);
        }
    }

    void OnValidate()
    {
        int size = System.Enum.GetNames(typeof(DisplayObjectMap.EDisplayType)).Length;

        if (displayObjectsInspectorArr.Length != size)
        {
            System.Array.Resize(ref displayObjectsInspectorArr, size);
        }

        int i = 0;
        foreach (DisplayObjectMap.EDisplayType val in System.Enum.GetValues(typeof(DisplayObjectMap.EDisplayType)))
        {
            displayObjectsInspectorArr[i].type = val;
            i++;
        }
    }
}
