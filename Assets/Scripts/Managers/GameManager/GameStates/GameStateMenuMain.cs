using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMenuMain : GameStateBase {

    private enum EMainMenuEnum
    {
        MainMenu,
        Options
    }

    //string pathMainMenu = "Levels/Menus";

    string pathMainMenu = "Levels/Menus/MainMenu";

    // Could assemble array of menus here and then load them by index value in the LoadNextLevel method?
    //string[] menuLevelPaths =
    //{
    //    "",
    //    "",
    //};

    //int nextLevelIndex = 0;

    private EMainMenuEnum currentMenu;

    public GameStateMenuMain()
    {
        Debug.Log("GameStateMenu Constructor");
        InitialiseStateTransitions();
    }

    protected override void InitialiseStateTransitions()
    {
        stateTransitions = new Dictionary<Command, TransitionData<GameStateBase>>
        {
            { Command.Begin, new TransitionData<GameStateBase>(typeof(GameStateEndlessPuzzle))  },
            { Command.Pause, new TransitionData<GameStateBase>(typeof(GameStateEndlessSimple))  },
            { Command.Resume, new TransitionData<GameStateBase>(typeof(GameStateEndlessPuzzle))  },
            { Command.Info, new TransitionData<GameStateBase>(typeof(GameStateEndless))  },
            { Command.End, new TransitionData<GameStateBase>(typeof(GameStateEndless))  },
            { Command.Edit, new TransitionData<GameStateBase>(typeof(GameStateEdit))  },
            { Command.Highscore, new TransitionData<GameStateBase>(typeof(GameStateHighScoreTable))  },
        };
    }

    public override bool CleanupGameState()
    {
        //throw new System.NotImplementedException();

        return true;
    }


    public override void HandleCommand(Command command)
    {
        base.HandleCommand(command);

        switch (command)
        {
            case Command.Begin:
                break;
            case Command.End:
                break;
            case Command.Pause:
                break;
            case Command.Resume:
                break;
            case Command.QuitLevel:
                break;
            case Command.Edit:
                break;
            case Command.Highscore:
                break;
            case Command.NextMenu:
                break;
            case Command.BackMenu:
                switch (currentMenu)
                {
                    case EMainMenuEnum.MainMenu:
                        break;
                    case EMainMenuEnum.Options:

                        break;
                };
                break;
            case Command.Login:
                break;
            case Command.NewUser:
                break;
            case Command.Skip:
                break;
            case Command.Info:
                break;
            case Command.Options:
                DisplayMenu(EMainMenuEnum.Options);
                break;
        }
    }

    private void DisplayMenu(EMainMenuEnum menuToDisplay)
    {
        currentMenu = menuToDisplay;

        switch (menuToDisplay)
        {
            case EMainMenuEnum.MainMenu:
                MainMenu();
                break;
            case EMainMenuEnum.Options:
                OptionsMenu();
                break;

                // ChangeUserMenu()
        }
    }

    public override void HexDigEvent(Hex hex)
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
        return;
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
        Camera.main.gameObject.GetComponent<CameraTunnel>().enabled = true;

        DisplayMenu(EMainMenuEnum.MainMenu);

        //MapSpawner.Instance.SpawnHexs(mainMenuLevel, GameManager.instance.GetPlayerBall().transform.position - new Vector3(0, -30, 0), false/* offsetValue */);
        //Vector3 mapPosition = MapSpawner.Instance.GetCurrentMapHolder().transform.position;
        //mapPosition += new Vector3(0, -5, 0);
        //GameManager.instance.GetPlayerBall().transform.position = mapPosition; // ballPosition;
        //GameManager.instance.GetPlayerBall().SetActive(false);

    }

    private void MainMenu()
    {
        Level loadedLevel = LevelLoader.Instance.LoadLevelFile(pathMainMenu);
        if (loadedLevel != null)
        {
            CreateLevel(
                loadedLevel,
                -30,
                false,
                false
            );

        }
        else throw new System.Exception("Exception: GameStateMainMenu LevelLoad Failed");
    }

    private void OptionsMenu()
    {
        DisplayMenu(EMainMenuEnum.MainMenu); // temp

    }


    // Use this for initialization
    private void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
