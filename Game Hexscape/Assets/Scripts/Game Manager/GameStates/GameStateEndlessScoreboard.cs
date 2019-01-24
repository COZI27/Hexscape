using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateEndlessScoreboard : GameStateBase
{
    Level scoreBoardLevel;

    //Note: Might be able to use one Scoreboard state class and have it behave differently depending on context, such as what type of game state was in use previosuly

    public override void StartGameState()
    {
        Level[] levels = Resources.LoadAll<Level>("Levels/Menus");
        scoreBoardLevel = levels[0];

        MapSpawner.instance.SpawnHexs(scoreBoardLevel, GameManager.instance.GetPlayerBall().transform.position - new Vector3(0, -30, 0), false);
        GameObject currentLevelObject = MapSpawner.instance.GetCurrentMapHolder();
        currentLevelObject.AddComponent<ScoreboardLevelComponent>();

        Vector3 mapPosition = currentLevelObject.transform.position;
        mapPosition += new Vector3(0, -5, 0);
        GameManager.instance.GetPlayerBall().transform.position = mapPosition; // ballPosition;
        GameManager.instance.GetPlayerBall().SetActive(false);
    }

    public override void PlayClickSound()
    {
        base.PlayClickSound();
    }

    public override void HexDigEvent()
    {
        //throw new System.NotImplementedException();
    }
}
