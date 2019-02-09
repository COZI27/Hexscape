using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardLevelComponent : BaseLevelComponent {

    int scoreToDisplay, levelToDisplay;

    DownloadScore scoreDownloader;
    UploadUserScore scoreUploader;

    private int scoreDisplayYPos, levelDisplayYPos;

    Vector2Int[] scoreDisplayTilePos = new[] { new Vector2Int(1, 2), new Vector2Int(0, 2), new Vector2Int(-1, 2) };

    Vector2Int[] levelDisplayTilePos = new[] { new Vector2Int(2, 0), new Vector2Int(1, 0), new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-2, 0) };

    // Use this for initialization
    void Start () {

        foreach (Vector2Int pos in scoreDisplayTilePos)
        {
            MapSpawner.instance.SpawnHexAtLocation(pos, HexTypeEnum.HexTile_Digit0, true);
        }

        foreach (Vector2Int pos in levelDisplayTilePos)
        {
            MapSpawner.instance.SpawnHexAtLocation(pos, HexTypeEnum.HexTile_Digit0, true);
        }


        DisplayScores();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void DisplayScores()
    {
        //MapSpawner.instance.SpawnHexAtLocation(0, 0, HexTypeEnum.HexTile_MenuOption, true);

        scoreDownloader = this.gameObject.AddComponent<DownloadScore>();
        if (scoreDownloader != null)
        {
            int downloadedScore;
            int downloadedLevel;

            scoreDownloader.GetScoreForUser(1, out downloadedScore, out downloadedLevel);

             if (downloadedScore < 1)
            {
                scoreUploader = this.gameObject.AddComponent<UploadUserScore>();
                scoreUploader.UploadScore();
            }

        }
    }

}
