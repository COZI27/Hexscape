using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameStateHighScoreTable : GameStateBase
{


    GameObject scoreBoardCanvas;
    GameObject titleParticle;

    private string pathScoreBoard = "Prefabs/GUI/ScoreboardCanvas";

    private string pathTitleParticle = "Prefabs/Particles/TextParticle";

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


        GameObject fillRing = new GameObject("HighScoreBoard");
        fillRing.AddComponent<MeshFilter>().mesh = CreateMesh(5, 5);
        fillRing.AddComponent<MeshRenderer>(); //.material = energyFillMat;
        //fillRing.transform.SetParent(GameManager.instance.gameObject);
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
        GameObject.Destroy(titleParticle);
    }

    Mesh CreateMesh(float width, float height)
    {
        //https://catlikecoding.com/unity/tutorials/procedural-grid/

        Mesh m = new Mesh();
        m.name = "ScriptedMesh";

        int xSize = 1;
        int ySize = 4;

        int z = 0;

        Vector3[] vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        for (int i = 0, y = 0; y <= ySize; y++, z++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, y * -z, y);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
            }
        }
        m.vertices = vertices;
        m.uv = uv;

        //m.uv = new Vector2[] {
        //    new Vector2 (0, 0),
        //    new Vector2 (0, 1),
        //    new Vector2(1, 1),
        //    new Vector2 (1, 0)
        //};



        int[] triangles = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }

        m.triangles = triangles;
        //m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };

        m.RecalculateNormals();

        return m;
    }

    private void GenerateBoard()
    {

        scoreBoardCanvas = GameObject.Instantiate(Resources.Load(pathScoreBoard) as GameObject);
        scoreBoardCanvas.transform.Translate(MapSpawner.Instance.GetCurrentMapHolder().transform.position - new Vector3(0, 4, 0), Space.World);

        titleParticle = GameObject.Instantiate(Resources.Load(pathTitleParticle) as GameObject);
        titleParticle.transform.Translate(MapSpawner.Instance.GetCurrentMapHolder().transform.position - new Vector3(0, 4, -1.5f), Space.World);

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

            scoreBoardEntries = new ScoreBoardEntry[0];
            currentPlayerEntry = null;


            scoreDownloader.GetScoreForUser(GameManager.instance.loadedProfile.GetPlayerIDasInt(), CallbackUserScore);

            scoreDownloader.GetScoresForScoreboard(GameManager.instance.loadedProfile.GetPlayerIDasInt(), CallbackHighScores);


            // Store the coroutine so it can be retrieved in the event of a timeout
            waitRoutine = WaitForDownloadComplete();
            GameManager.instance.StartCoroutine(waitRoutine);
        }
    }


    ScoreBoardEntry[] scoreBoardEntries;
    ScoreBoardEntry currentPlayerEntry;

    IEnumerator waitRoutine;

    IEnumerator WaitForDownloadComplete()
    {

        bool dataReturned = false;
        while (!dataReturned)
        {
            if (scoreBoardEntries.Length > 0)
                if (currentPlayerEntry != null)
                    dataReturned = true;
                else yield return null;
            else yield return null;
        }

        

        Debug.Log("Data Retrieved:");
        Debug.Log(currentPlayerEntry);
        Debug.Log(scoreBoardEntries.Length);

        bool playerIndexFound = false;
        for (int i = 0; i < scoreBoardEntries.Length; i++)
        {
            if (scoreBoardEntries[i].playerId == currentPlayerEntry.playerId)
            {
                playerScoreEntryIndex = i;
                playerIndexFound = true;
                break;
            }
        }
        if (!playerIndexFound) // replace the lowest scored entry in the array with the current player's entry
        {
            scoreBoardEntries[scoreBoardEntries.Length - 1] = currentPlayerEntry;
            playerScoreEntryIndex = scoreBoardEntries.Length - 1;
        }

        Debug.Log("playerScoreboardIndex : " + playerScoreEntryIndex);
        Debug.Log("playerKeyID : " + currentPlayerEntry.playerId);
        Debug.Log("playerScore : " + currentPlayerEntry.highScore);
        Debug.Log("playerLevel : " + currentPlayerEntry.highLevel);
        Debug.Log("playername : " + currentPlayerEntry.playerName);




        GenerateHighScoreDisplay();
    }

    private void DisplayErrorMessage()
    {
        // Error retrieving score data.
        Debug.LogWarning("Error retrieving score data.");
    }



    public void CallbackUserScore(ScoreBoardEntry data)
    {
        currentPlayerEntry = data;
    }

    public void CallbackHighScores(ScoreBoardEntry[] data)
    {
        scoreBoardEntries = data;
    }

    private void GenerateHighScoreDisplay()
    {
        GameObject entryPrefab = scoreBoardCanvas.transform.Find("ScoreEntry").gameObject;
        //entryPrefab.SetActive(false);
        GameObject[] entries = new GameObject[scoreBoardEntries.Length];

        // if current user score not in top ten, then replace entry 10 with user score  (could handle this in php?)

        float currOffset = 0;

        for (int i = 0; i < entries.Length; i++)
        {
            entries[i] = GameObject.Instantiate(entryPrefab, scoreBoardCanvas.transform);
            //entries[i].transform.parent = scoreBoardCanvas.transform;
            entries[i].transform.position -= new Vector3(0, 0, currOffset);
            Text[] textComponents = entries[i].GetComponentsInChildren<Text>();
            textComponents[0].text = scoreBoardEntries[i].playerId.ToString();
            textComponents[1].text = scoreBoardEntries[i].highScore.ToString();
            textComponents[2].text = scoreBoardEntries[i].highLevel.ToString();
            textComponents[3].text = scoreBoardEntries[i].playerName.ToString();

            if (i == playerScoreEntryIndex)
                foreach (Text t in textComponents)
                    t.color = Color.green;

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
