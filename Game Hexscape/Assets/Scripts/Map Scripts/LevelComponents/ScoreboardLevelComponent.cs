using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScoreBoardEntry
{
    public ScoreBoardEntry(int id, int level, int score)
    {
        playerId = id;
        highLevel = level;
        highScore = score;
    }

    public int playerId;
    public int highLevel;
    public int highScore;

}

public class ScoreboardLevelComponent : BaseLevelComponent {

    int scoreToDisplay, levelToDisplay;

    DownloadScore scoreDownloader;
    UploadUserScore scoreUploader;

    private int scoreDisplayYPos, levelDisplayYPos;

    Vector2Int[] levelDisplayTilePos = new[] { new Vector2Int(1, 2), new Vector2Int(0, 2), new Vector2Int(-1, 2) };

    Vector2Int[] scoreDisplayTilePos = new[] { new Vector2Int(2, 0), new Vector2Int(1, 0), new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-2, 0) };

    // Note: Digit array could be stored elsewhere (HexBank?)
    HexTypeEnum[] hexDigits = { HexTypeEnum.HexTile_Digit0,
                                HexTypeEnum.HexTile_Digit1,
                                HexTypeEnum.HexTile_Digit2,
                                HexTypeEnum.HexTile_Digit3,
                                HexTypeEnum.HexTile_Digit4,
                                HexTypeEnum.HexTile_Digit5,
                                HexTypeEnum.HexTile_Digit6,
                                HexTypeEnum.HexTile_Digit7,
                                HexTypeEnum.HexTile_Digit8,
                                HexTypeEnum.HexTile_Digit9,
    };


    // Use this for initialization
    void Start() {
        DisplayScores();
    }



    // Update is called once per frame
    void Update() {

    }

    int levelValue;
    int scoreValue;

    void DisplayScores()
    {

        GetCurrentSessionScore(out levelValue, out scoreValue);
        MakeScoreDownloadRequest();

        int[] scoreToDisplay = ConvertIntToArray(scoreValue);
        int[] levelToDisplay = ConvertIntToArray(levelValue);

        int position = scoreToDisplay.Length - 1;
        foreach (Vector2Int pos in scoreDisplayTilePos)
        {
           int hexDigitIndex = (position >= 0 ? scoreToDisplay[position] : 0);

           MapSpawner.instance.SpawnHexAtLocation(pos, hexDigits[hexDigitIndex], true);
           position--;
        }

        position = levelToDisplay.Length - 1;
        foreach (Vector2Int pos in levelDisplayTilePos)
        {
            int hexDigitIndex = (position >= 0 ? levelToDisplay[position] : 0);
            MapSpawner.instance.SpawnHexAtLocation(pos, hexDigits[hexDigitIndex], true);
            position--;
        }
    }

    void GetCurrentSessionScore(out int returnLevel, out int returnScore)
    {
        GameStateBase.GameSessionData sessionData = GameManager.instance.GetGameSessionData();

        returnLevel = sessionData.levelIndex;
        returnScore = sessionData.totalScore;
    }

    bool uploadScore = false;
    int downloadedLevelValue;
    int downloadedScoreValue;




    void MakeScoreDownloadRequest()
    {
        // TODO: store scores locally in the event that the server is not accessible, or the player chooses not to host their
        //          scores on teh server. In which case, we should compare the current score to the local score and the server?
        //          Possible issue here is that players could potentially hack their own value into the local score value to be
        //          pushed to the server. 

        scoreDownloader = this.gameObject.AddComponent<DownloadScore>();
        if (scoreDownloader != null)
        {
            scoreDownloader.GetScoreForUser(1, Callback);   
        }
    }

    public void Callback(ScoreBoardEntry data)
    {
        // Level
        if (data.highLevel < levelValue)
        {
            uploadScore = true;

            // Display "New Best Level"
            Debug.Log("New Best Level.  Old = " + data.highLevel + "  | New = " + levelValue);

        }

        // Score
        if (data.highScore < scoreValue)
        {
            uploadScore = true;

            // Display "New Best Score"
            Debug.Log("New Best Score. Old = " + data.highScore + "  | New = " + scoreValue);
        }

        if (uploadScore)
        {
            scoreUploader = this.gameObject.AddComponent<UploadUserScore>();
            scoreUploader.UploadScore(new ScoreBoardEntry(1, levelValue, scoreValue));
        }
    }



    int[] ConvertIntToArray(int inValue) // TODO: Move to helper class
    {
        if (inValue == 0) return new int[1] { 0 };

        var digits = new List<int>();

        for (; inValue != 0; inValue /= 10)
            digits.Add(inValue % 10);

        int[] arr = digits.ToArray();
        System.Array.Reverse(arr);
        return arr;
    }


}
