﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class DownloadScore : MonoBehaviour {

    //private string phpScriptsFolder = "https://Hexit.000webhostapp.com"; // The Location where all PHP scripts are stored 
    //private string phpAddTheItemScriptLocation = "/DBAccessScripts/InsertUser.php"; // The Location of the PHP script for adding an item



    private string url = "https://hexit.000webhostapp.com/DBAccessScripts/GetStatsForUserID.php";

    //[ContextMenu("Add Item")]  // Calls The AddItemToDB Coroutine from the inspector
    public void GetScoreForUser(int userId, System.Action<ScoreBoardEntry> callBack)
    {
        StartCoroutine(GetUserScore(userId, callBack ));
        Debug.Log("starting cooroutine, user id = " + userId);
    }

    //public void GetScoreForUser()
    //{
    //    StartCoroutine(GetUserScore());
    //}

    private IEnumerator GetUserScore(int userId, System.Action<ScoreBoardEntry> callBack)
    {
        Debug.Log("GetUserScore user id = " + userId);
        WWWForm form = new WWWForm();
        form.AddField("userIDPost", userId);
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
            Debug.Log("Score Download Complete!");
            string entry = webRequest.downloadHandler.text;

            if (!entry.Contains("\t"))
                throw new System.Exception("Improperly formatted data from database " + entry);
            string[] temp = entry.Split('\t');

            int returnID, returnLevel, returnScore;

            int.TryParse(temp[0], out returnID);
            int.TryParse(temp[1], out returnLevel);
            int.TryParse(temp[2], out returnScore);

            Debug.Log(returnID +" | "+ returnLevel + " | " + returnScore);

            ScoreBoardEntry returnEntry = new ScoreBoardEntry(returnID, returnLevel, returnScore);
          
            callBack(returnEntry);
        }
    }


    //the string passed ParseScoreString MUST be of the form:
    // name\tscore\twhenSet\n
    public void ParseScoreString(string scoreString)
    {
        string[] splitScores = scoreString.Trim().Split('\n');
        if (scoreString == string.Empty)
            return;
        int count = 0;
        foreach (string entry in splitScores)
        {
            //throw an error if the string is not properly formatted
            if (!entry.Contains("\t"))
                throw new System.Exception("Improperly formatted data from database " + entry);
            string[] temp = entry.Split('\t');
           // highScoreList.Add(new HighScoreElement(
           //     name: temp[0], score: int.Parse(temp[1]), whenSet: DateTime.Parse(temp[2]), zeroBasedRank: count));
            count++;
        }

    }

}
