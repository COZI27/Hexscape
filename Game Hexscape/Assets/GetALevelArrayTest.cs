using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetALevelArrayTest : MonoBehaviour
{
    public Level[] levels;

    private void Start()
    {
        levels = LevelLoader.Instance.GetAllLevels();


    }

}
