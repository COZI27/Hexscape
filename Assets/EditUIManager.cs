using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EditUIManager : MonoBehaviour
{

    

    private static EditUIManager instance;
    public static EditUIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<EditUIManager>();
                if (instance == null) Debug.LogError("No instance of MapSpawner was found.");
            }
            return instance;
        }
    }

    [SerializeField] private Transform panel;
    [SerializeField] private HexTypeEnum[] hexTypes;
    [SerializeField] private HexTypeUIElement hexUIPrefab;

    public void ShowPanel(bool doShow)
    {
        panel.gameObject.SetActive(doShow);
    }

    public void Start()
    {
        foreach (var item in hexTypes)
        {
            AddHexTypeUI(item);
        }
    }

    public void AddHexTypeUI (HexTypeEnum hexType)
    {
        Instantiate(hexUIPrefab, panel).SetUp(hexType);
    }
    
}


    
