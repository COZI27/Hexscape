using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateEndlessScoreboard : GameStateBase
{
    string pathScoreBoardLevel = "Levels/Menus/EndlessScoreboard";

    //ScoreboardLevelComponent levelComponent;

    //Note: Might be able to use one Scoreboard state class and have it behave differently depending on context, such as what type of game state was in use previosuly

    public override void StartGameState()
    {
        CreateLevel(
            LoadLevelFromPath(pathScoreBoardLevel),
            -30.0f,
            false,
            false,
            new ScoreboardLevelComponent()
            );
    }

    public override void PlayClickSound()
    {
        base.PlayClickSound();
    }

    public override void HexDigEvent()
    {
        //throw new System.NotImplementedException();
    }

    public override void PassSessionData(GameSessionData data)
    {
        currentSessionData = data;
    }

    public override void CleanupGameState()
    {
       
    }
}
