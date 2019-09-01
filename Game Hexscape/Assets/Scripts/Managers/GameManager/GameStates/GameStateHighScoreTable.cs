using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameStateHighScoreTable : GameStateBase
{


    GameObject scoreBoardCanvas;
    private string pathScoreBoard = "Prefabs/GUI/ScoreboardCanvas";

    private string pathHiScoreTableLevel = "Assets/Resources/Levels/Menus/HiScoreTable.json";

    private float offset = 0.5f;

    ScoreBoardEntry[] scoresToDisplay;
    int playerScoreEntryIndex; // the current player's score in the array

    public GameStateHighScoreTable()
    {
        InitialiseStateTransitions();
    }

    public override void StartGameState()
    {
        GenerateBoard();

        Level loadedLevel = LevelLoader.Instance.LoadLevelFile(pathHiScoreTableLevel);
        if (loadedLevel != null)
        {
            CreateLevel(
                loadedLevel,
                -30,
                false,
                false
            );
        }
    }

    public override void StateUpdate()
    {
    }

    protected override void InitialiseStateTransitions()
    {
        stateTransitions = new Dictionary<Command, TransitionData<GameStateBase>>
        {
            { Command.Begin, new TransitionData<GameStateBase>(typeof(GameStateMenuMain))  },
            { Command.End, new TransitionData<GameStateBase>(typeof(GameStateMenuMain))  },
        };
    }

    public override void CleanupGameState()
    {
        GameObject.Destroy(scoreBoardCanvas);
    }

    private void GenerateBoard()
    {
        scoreBoardCanvas = GameObject.Instantiate(Resources.Load(pathScoreBoard) as GameObject);
        scoreBoardCanvas.transform.Translate(MapSpawner.Instance.GetCurrentMapHolder().transform.position - new Vector3(0, 4, 0), Space.World);
        DownloadScoreData();
    }

    public void DownloadScoreData()
    {
        MakeScoreDownloadRequest();
    }

    private void MakeScoreDownloadRequest()
    {
        DownloadScore scoreDownloader = new DownloadScore();

        if (scoreDownloader != null)
        {
            scoreDownloader.GetScoresForScoreboard(GameManager.instance.loadedProfile.GetPlayerIDasInt(), Callback);


        }
    }

    public void Callback(ScoreBoardEntry[] data)
    {


        GameObject entryPrefab = scoreBoardCanvas.transform.Find("ScoreEntry").gameObject;
        //entryPrefab.SetActive(false);
        GameObject[] entries = new GameObject[data.Length];

        // if current user score not in top ten, then replace entry 10 with user score  (could handle this in php?)

        float currOffset = 0;

        for(int i = 0; i < entries.Length; i++)
        {
            entries[i] = GameObject.Instantiate(entryPrefab, scoreBoardCanvas.transform);
            //entries[i].transform.parent = scoreBoardCanvas.transform;
            entries[i].transform.position -= new Vector3(0, 0, currOffset);
            Text[] textComponents = entries[i].GetComponentsInChildren<Text>();
            textComponents[0].text = data[i].playerId.ToString();
            textComponents[1].text = data[i].highScore.ToString();
            textComponents[2].text = data[i].highLevel.ToString();
            textComponents[3].text = data[i].playerName.ToString();


            entries[i].name = entries[i].name + i.ToString();
            entries[i].SetActive(true);

            currOffset += offset;
        }
        //entries[0].SetActive(false);
        //entryPrefab.SetActive(false);
    }

    //private void MakeUserScoreDownloadRequest()
    //{
    //    DownloadScore scoreDownloader = new DownloadScore();

    //    if (scoreDownloader != null)
    //    {
    //        scoreDownloader.GetScoreForUser(GameManager.instance.loadedProfile.GetPlayerIDasInt(), Callback);


    //    }
    //}

    //public void Callback(ScoreBoardEntry data)
    //{
    //}


}
