using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class DownloadScore : MonoBehaviour {

    private string phpScriptsFolder = "https://Hexit.000webhostapp.com"; // The Location where all PHP scripts are stored 
    private string phpAddTheItemScriptLocation = "/DBAccessScripts/InsertUser.php"; // The Location of the PHP script for adding an item

    private string scoreToDisplay;

    private int scoreToReturn = 0;
    private int levelToReturn = 0;

    private string url = "https://hexit.000webhostapp.com/DBAccessScripts/GetUserStats.php";

    //[ContextMenu("Add Item")]  // Calls The AddItemToDB Coroutine from the inspector
    public void GetScoreForUser(int userID, out int returnScore, out int returnLevel)
    {
        StartCoroutine(GetUserScore());

        returnScore = scoreToReturn;
        returnLevel = levelToReturn;
    }

    //public void GetScoreForUser()
    //{
    //    StartCoroutine(GetUserScore());
    //}

    private IEnumerator GetUserScore()
    {
        int userID = 1; // TEMP

        WWWForm form = new WWWForm();
        form.AddField("IDPost", userID);
        //form.AddField("usernamePost", name);
        //form.AddField("passwordPost", password);
        UnityWebRequest webRequest = UnityWebRequest.Post(/*phpScriptsFolder + phpAddTheItemScriptLocation*/ url, form);
        //UnityWebRequest webRequest = UnityWebRequest.Post(/*phpScriptsFolder + phpAddTheItemScriptLocation*/ url, form);

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {

            Debug.Log(webRequest.downloadHandler.text);
            Debug.Log(webRequest.error);

        }
        else
        {
            byte[] results = webRequest.downloadHandler.data;


            //scoreToDisplay =
            Debug.Log("Score Download Complete!");
            Debug.Log(webRequest.downloadHandler.text);
            if (results.Length > 0)
            {
                string test = System.Text.Encoding.ASCII.GetString(results);
                Debug.Log(test);

                Debug.Log(webRequest.ToString());

                for (int i = 0; i < results.Length; i++)
                {
                    //Debug.Log(results[i]);
                    //string test = System.Text.Encoding.ASCII.GetString(results);
                    //Debug.Log(test);
                }
            }
            else Debug.Log("Score Results Empty");
        }

    }
}
