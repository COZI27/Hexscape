using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class UploadUserScore {

    private string phpScriptsFolder = "https://Hexit.000webhostapp.com"; // The Location where all PHP scripts are stored 
    private string phpAddTheItemScriptLocation = "/DBAccessScripts/SetHighDeathScore.php"; // The Location of the PHP script for adding an item

    private string scoreToDisplay;

    private string url = "https://hexit.000webhostapp.com/DBAccessScripts/SetHighDeathScore.php";

    //[ContextMenu("Add Item")]  // Calls The AddItemToDB Coroutine from the inspector
    public void UploadScore(ScoreBoardEntry data)
    {

        GameManager.instance.StartCoroutine(UploadScoreCoroutine(data));
    }

    private IEnumerator UploadScoreCoroutine(ScoreBoardEntry data)
    {
        WWWForm form = new WWWForm();
        form.AddField("IDPost", data.playerId);
        form.AddField("levelNumPost", data.highLevel);
        form.AddField("highscorePost", data.highScore);

        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            Debug.Log("Score Upload Complete!");
            Debug.Log(webRequest.url);
            Debug.Log(url);
            Debug.Log(webRequest.downloadHandler.text);
        }

    }
}
