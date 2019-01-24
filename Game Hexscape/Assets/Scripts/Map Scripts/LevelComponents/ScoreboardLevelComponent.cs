using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardLevelComponent : BaseLevelComponent {

	// Use this for initialization
	void Start () {
        DisplayScores();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void DisplayScores()
    {
        MapSpawner.instance.SpawnHexAtLocation(0, 0, HexTypeEnum.HexTile_MenuOption, true);
    }

}
