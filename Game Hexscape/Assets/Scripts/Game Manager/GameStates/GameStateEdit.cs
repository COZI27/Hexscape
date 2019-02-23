using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateEdit : GameStateBase {

    Level[] levels;

    private int currentLevelIndex = 0;

    public GameStateEdit()
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
        Level currentLevel = levels[currentLevelIndex];

        MapSpawner.instance.SpawnHexs(currentLevel, GameManager.instance.GetPlayerBall().transform.position - new Vector3(0, -30, 0), false/* offsetValue */);

        Vector3 mapPosition = MapSpawner.instance.GetCurrentMapHolder().transform.position;
        mapPosition += new Vector3(0, -5, 0);
        GameManager.instance.GetPlayerBall().transform.position = mapPosition; // ballPosition;
        GameManager.instance.GetPlayerBall().SetActive(false);
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
       levels = LevelGetter.instance.GetLevelsFrom("Levels/Endless");
       Level currentLevel = levels[currentLevelIndex];

        MapSpawner.instance.SpawnHexs(currentLevel, GameManager.instance.GetPlayerBall().transform.position - new Vector3(0, -30, 0), false/* offsetValue */);

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
    public override void StateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentLevelIndex < levels.Length - 1)
            {
                currentLevelIndex++;
                LoadNextLevel();

                GameManager.instance.scoreUI.SetLevel(currentLevelIndex);
            }
            
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentLevelIndex > 0)
            {
                currentLevelIndex--;
                LoadNextLevel();

                GameManager.instance.scoreUI.SetLevel(currentLevelIndex);
            }
        }


     

    }



}
