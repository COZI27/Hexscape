using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    void DisplayScores()
    {
        int levelValue;
        int scoreValue;
        GetScoresAndCompareToPrevious(out levelValue, out scoreValue);

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

    // Compares the current score to previous scores
    void GetScoresAndCompareToPrevious(out int returnLevel, out int returnScore)
    {

        GameStateBase.GameSessionData sessionData = GameManager.instance.GetGameSessionData();

        Debug.Log(sessionData.levelIndex);

        returnLevel = sessionData.levelIndex;
        returnScore = sessionData.totalScore;

        // TODO: store scores locally in the event that the server is not accessible, or the player chooses not to host their
        //          scores on teh server. In which case, we should compare the current score to the local score and the server?
        //          Possible issue here is that players could potentially hack their own value into the local score value to be
        //          pushed to the server. 

        /* Server Stuff 
        scoreDownloader = this.gameObject.AddComponent<DownloadScore>();
        if (scoreDownloader != null)
        {
            int downloadedScore;
            int downloadedLevel;

            scoreDownloader.GetScoreForUser(1, out downloadedScore, out downloadedLevel);

            if (downloadedScore < sessionData.totalScore) // TEMP
            {
                scoreUploader = this.gameObject.AddComponent<UploadUserScore>();
                scoreUploader.UploadScore();

                // Display "New Best Score"
            }
            if (downloadedLevel < sessionData.levelIndex) // TEMP
            {
                scoreUploader = this.gameObject.AddComponent<UploadUserScore>();
                scoreUploader.UploadScore();

                // Display "New Best Level"
            }
        }
        */
    }

    int[] ConvertIntToArray(int inValue)
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
