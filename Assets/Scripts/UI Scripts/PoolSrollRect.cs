using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(PoolSrollRect))]
public class MenuButtonEditor : Editor
{

    private PoolSrollRect scrollRectRef = null;
    public override void OnInspectorGUI()
    {
        //RectTransform objectToSpawn = (RectTransform)EditorGUILayout.ObjectField(new GUIContent("ObjectToSpawn", "Add object to fracture"), objectToSpawn, typeof(RectTransform), false);


        scrollRectRef = (PoolSrollRect)target;
        scrollRectRef.objectToSpawn = (RectTransform)EditorGUILayout.ObjectField(new GUIContent("ObjectToSpawn", "Add object to fracture"), scrollRectRef.objectToSpawn, typeof(RectTransform), false);

        scrollRectRef.disableInstantiation = EditorGUILayout.Toggle("Disable Object Instantiation", scrollRectRef.disableInstantiation);
        // Show default inspector property editor
        base.OnInspectorGUI();
        //DrawDefaultInspector();
    }
}


public class PoolSrollRect : ScrollRect
{

    [HideInInspector]
    public bool
    initOnAwake = false;

    protected RectTransform t
    {
        get
        {
            if (_t == null)
                _t = GetComponent<RectTransform>();
            return _t;
        }
    }

    private RectTransform _t;

    RectTransform[] entryPool;

    [SerializeField]
    public GameObject listPrefab;


    [SerializeField]
    public RectTransform objectToSpawn;

    [SerializeField]
    public bool disableInstantiation = false; //TEMP

    private string spawnName;

    private Vector2 dragOffset = Vector2.zero;

    private bool init;


    new void Awake()
    {
        if (!Application.isPlaying)
            return;

        if (initOnAwake)
            Init();
    }

    public void Init()
    {
        /*
         * Establish sibling count
         * Populate with children
         * add children to pool
         */
        init = true;

        if (objectToSpawn == null)
        {
            objectToSpawn = content.GetChild(0).GetComponent<RectTransform>();
            if (objectToSpawn == null)
            {
                Debug.LogWarning("ScrollRect failed to initialise: No Object to spawn found.");
                return;
            }
            else
            {
                spawnName = objectToSpawn.name;

                float containerSize = 0; // Container size is the sum of entry sizes in the container
                                         //Filling up the scrollview with initial items
                while (containerSize < GetDimension(t.sizeDelta))
                {
                    RectTransform nextItem = NewItemAtEnd();
                    containerSize += GetSize(nextItem);
                }


                // Replace "prefab" object in order to run its constructor if necessary
                Destroy(objectToSpawn.gameObject);
                NewItemAtEnd();
            }
        }
        else
        {
            foreach (Transform child in content.transform)
                Destroy(child.gameObject);

            spawnName = objectToSpawn.name;

            float containerSize = 0; // Container size is the sum of entry sizes in the container
                                     //Filling up the scrollview with initial items

            while (containerSize < GetDimension(t.sizeDelta))
            {
                RectTransform nextItem = NewItemAtEnd();
                containerSize += GetSize(nextItem);
            }
        }

    }


    // Update is called once per frame
    void Update()
    {
        UpdateContents();

    }

    void UpdateContents()
    {
        if (!Application.isPlaying || !init)
            return;


        if (GetDimension(content.sizeDelta) - (GetDimension(content.localPosition) * OneOrMinusOne()) < GetDimension(t.sizeDelta))
        {        
            NewItemAtEnd();
            //margin is used to Destroy objects. We add them at half the margin (if we do it at full margin, we continuously add and delete objects)
        }
        else if (GetDimension(content.localPosition) * OneOrMinusOne() < (GetDimension(t.sizeDelta) * 0.5f))
        {

            NewItemAtStart();
            //Using else because when items get added, sometimes the properties in UnityGUI are only updated at the end of the frame.
            //Only Destroy objects if nothing new was added (also nice performance saver while scrolling fast).
        }
        else
        {
            

            //Looping through all items.
            foreach (RectTransform child in content)
            {
                //We Destroy an item from the end if it's too far
                if (GetPos(child) > GetDimension(t.sizeDelta))
                {
                    //Debug.Log("if");
                    Destroy(child.gameObject);
                    //We update the container position, since after we delete something from the top, the container moves all of it's content up
                    content.localPosition -= (Vector3)GetVector(GetSize(child));
                    dragOffset -= GetVector(GetSize(child));
                }
                else if (GetPos(child) < -(GetDimension(t.sizeDelta) + GetSize(child)))
                {
                    //Debug.Log("else if");
                    Destroy(child.gameObject);
                }
            }
        }


    }

    private RectTransform NewItemAtStart()
    {
        //Debug.Log("NewItemAtStart");
        RectTransform newItem = InstantiateNextItem();
        if (!disableInstantiation) newItem.SetAsFirstSibling();
        if (!disableInstantiation) content.localPosition += (Vector3)GetVector(GetSize(newItem));
        if (!disableInstantiation) dragOffset += GetVector(GetSize(newItem));
        return newItem;
    }

    private RectTransform NewItemAtEnd()
    {
        //Debug.Log("NewItemAtEnd");
        RectTransform newItem = InstantiateNextItem();
        return newItem;
    }

    private RectTransform InstantiateNextItem()
    {
        if (objectToSpawn == null) {
            Debug.Log("Finding new objectToSpawn...  / DIABLED");
            //objectToSpawn = content.GetChild(Mathf.FloorToInt(content.childCount / 2)).GetComponent<RectTransform>();
        }
       
        RectTransform nextItem = Instantiate(objectToSpawn) as RectTransform;       
        //nextItem.name = spawnName;
        nextItem.transform.SetParent(content.transform, false);
        nextItem.gameObject.SetActive(true);
        return nextItem;
    }


    #region overrides
    public override void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        dragOffset = Vector2.zero;
        base.OnBeginDrag(eventData);
    }

    public override void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        //TEMP method until I found a better solution
        if (dragOffset != Vector2.zero)
        {
            OnEndDrag(eventData);
            OnBeginDrag(eventData);
            //dragOffset = Vector2.zero;
        }

        base.OnDrag(eventData);
    }
    #endregion


    #region CalcMethods

    protected float GetSize(RectTransform item)
    {
        return item.GetComponent<LayoutElement>().minHeight + content.GetComponent<VerticalLayoutGroup>().spacing;
    }

    protected float GetDimension(Vector2 vector)
    {
        return vector.y;
    }

    protected Vector2 GetVector(float value)
    {
        return new Vector2(0, value);
    }

    protected float GetPos(RectTransform item)
    {
        return item.localPosition.y + content.localPosition.y;
    }

    protected int OneOrMinusOne()
    {
        return 1;
    }

    #endregion



    #region convenience

    private void Subtract(ref int i)
    {
        i--;
        if (i == -1)
        {
            // i = prefabItems.Length - 1;
            i = 0;
        }
    }

    private void Add(ref int i)
    {
        i++;
        //if (i == prefabItems.Length)
        //{
        //   // i = 0;
        //}
        i = 0;
    }
    #endregion


}
