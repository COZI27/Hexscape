using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardLevelComponent : BaseLevelComponent {

    int scoreToDisplay, levelToDisplay;

    DownloadScore scoreDownloader;
    UploadUserScore scoreUploader;

    // Use this for initialization
    void Start () {
        DisplayScores();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void DisplayScores()
    {
        MapSpawner.instance.SpawnHexAtLocation(0, 0, HexTypeEnum.HexTile_MenuOption, true);

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
