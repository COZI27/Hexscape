using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateEdit : GameStateBase
{

    Level[] levels;

    private int currentLevelIndex = 0;

    

    

    public GameStateEdit()
    {
        InitialiseStateTransitions();
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

        MapSpawner.Instance.SpawnHexs(currentLevel, GameManager.instance.GetPlayerBall().transform.position - new Vector3(0, -30, 0), false/* offsetValue */);

       // Vector3 mapPosition = MapSpawner.Instance.GetCurrentMapHolder().transform.position;
       // mapPosition += new Vector3(0, -5, 0);
       // GameManager.instance.GetPlayerBall().transform.position = mapPosition; // ballPosition;
        GameManager.instance.GetPlayerBall().SetActive(false);
    }

    protected override void InitialiseStateTransitions()
    {
        stateTransitions = new Dictionary<Command, TransitionData<GameStateBase>>
        {
            { Command.Begin, new TransitionData<GameStateBase>(typeof(GameStateEndless))  },
            { Command.End, new TransitionData<GameStateBase>(typeof(GameStateEndless))  }
            
        };


        GameManager.instance.editHexPicked(currentEditHexType);

        EditUIManager.Instance.ShowPanel(true);
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
        Debug.Log("GAME STATE EDIT STARTED !!!");

        levels = LevelLoader.Instance.GetLevelsFrom("Levels/Endless");
        if (levels.Length == 0)
        {
            levels = new Level[1];
            levels[0] = new Level();
        }
        Level currentLevel = levels[currentLevelIndex];
        MapSpawner.Instance.SpawnHexs(currentLevel, GameManager.instance.GetPlayerBall().transform.position - new Vector3(0, -30, 0), false/* offsetValue */);

        Vector3 mapPosition = MapSpawner.Instance.GetCurrentMapHolder().transform.position;
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

        else if (Input.GetKeyDown(KeyCode.S)) //save
        {
            SaveLevel();
        }

        

        if (Input.GetMouseButtonDown(0) && MouseOverUI() == false)
        {
             
            Vector2Int mouseGridPos =  GridFinder.instance.MouseToGridPoint();

            MapElement element = new MapElement(currentEditHexType, mouseGridPos);
            MapSpawner.Instance.SpawnAHex(element);

        } else if (Input.GetMouseButtonDown(1))
        {
            Vector2Int mouseGridPos = GridFinder.instance.MouseToGridPoint();

           
            MapSpawner.Instance.RemoveHexAtPoint(mouseGridPos);

        }
       
        else if (Input.GetKeyDown(KeyCode.Delete))
        {
            MapSpawner.Instance.ClearMapGrid();
        }


    }

    //public void AddHexToGrid(HexTypeEnum type, Vector3 position)LoadLevel
    //{
    //    GameObject hexInstance = HexBank.Instance.GetDisabledHex(currentEditHexType, position, MapSpawner.Instance.grid.transform);



    //    // to do... set up the map refrence for the grid finder 
    //}


    [ContextMenu("Save Level")]
    public void SaveLevel()
    {
        List<MapElement> mapElements = new List<MapElement>();

        HexagonGrid grid = MapSpawner.Instance.grid;

        foreach (Hex hex in grid.GetComponentsInChildren<Hex>())
        {

            mapElements.Add(new MapElement(hex.typeOfHex, new Vector2Int(grid.WorldToCell(hex.transform.position).x, grid.WorldToCell(hex.transform.position).y)));

        }

        Level level = levels[currentSessionData.levelIndex];
        level.hexs = mapElements.ToArray();

        Debug.Log(level.hexs.Length);

        LevelLoader.Instance.SaveLevelFile(level); // will make it so folders to where you can save it are limited for player input


    }



}
