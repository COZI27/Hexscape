using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMenuMain : GameStateBase {

    string pathMainMenu = "Levels/Menus";


    // Could assemble array of menus here and then load them by index value in the LoadNextLevel method?
    //string[] menuLevelPaths =
    //{
    //    "",
    //    "",
    //};

    //int nextLevelIndex = 0;

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

        CreateLevel(
        LoadLevelsFromPath(pathMainMenu)[0],
        -30,
        false,
        false
        );

        Hex registerUserHexButton = MapSpawner.instance.SpawnHexAtLocation(new Vector2Int(0, 0), HexTypeEnum.HexTile_MenuOptionPlay, true);
        registerUserHexButton.clickedEvent.AddListener(() =>
        {
            GameManager.instance.ProcessCommand(GameManager.Command.Begin);
            //HandleRegisterClick();
            registerUserHexButton.DestroyHex();
        });

        //MapSpawner.instance.SpawnHexs(mainMenuLevel, GameManager.instance.GetPlayerBall().transform.position - new Vector3(0, -30, 0), false/* offsetValue */);
        //Vector3 mapPosition = MapSpawner.instance.GetCurrentMapHolder().transform.position;
        //mapPosition += new Vector3(0, -5, 0);
        //GameManager.instance.GetPlayerBall().transform.position = mapPosition; // ballPosition;
        //GameManager.instance.GetPlayerBall().SetActive(false);

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
