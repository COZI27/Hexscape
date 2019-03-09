using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameStateInit : GameStateBase
{

    string pathNewProfileLevel = "Levels/Menus/RegisterProfile";

    public override void StartGameState()
    {
        CreateNewProfileMenu();
    }

    private void CreateNewProfileMenu()
    {
        CreateLevel(
            LoadLevelFromPath(pathNewProfileLevel),
            -30.0f,
            false,
            false,
            new LoadProfileLevelComponent()
            );
    }
}
