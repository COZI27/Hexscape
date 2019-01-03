using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMenuMain : GameStateBase {

    Level mainMenuLevel;

    public GameStateMenuMain()
    {

    }

    public override void CleanupGameState()
    {
        //throw new System.NotImplementedException();
    }

    public override void HexDigEvent()
    {
        //throw new System.NotImplementedException();
    }

    public override void LoadNextLevel()
    {
        throw new System.NotImplementedException();
    }

    public override void Pause()
    {
        throw new System.NotImplementedException();
    }

    public override void PlayClickSound()
    {
        throw new System.NotImplementedException();
    }

    public override void PlayGroundThud()
    {
        throw new System.NotImplementedException();
    }

    public override void Resume()
    {
        throw new System.NotImplementedException();
    }

    public override void StartGameState()
    {
        Level[] levels = Resources.LoadAll<Level>("Levels/Menus");
        mainMenuLevel = levels[0];

        MapSpawner.instance.SpawnHexs(mainMenuLevel, GameManager.instance.GetPlayerBall().transform.position - new Vector3(0, -30, 0)/* offsetValue */);

        Vector3 mapPosition = MapSpawner.instance.GetCurrentMapHolder().transform.position;
        mapPosition += new Vector3(0, -5, 0);
        GameManager.instance.GetPlayerBall().transform.position = mapPosition; // ballPosition;
        GameManager.instance.GetPlayerBall().SetActive(false);
       
    }

    protected override void HandleInput()
    {
        throw new System.NotImplementedException();
    }

    protected override void InitialiseClickSounds()
    {
        throw new System.NotImplementedException();
    }

    // Use this for initialization
    private void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
