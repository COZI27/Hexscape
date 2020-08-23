using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;




public class DownloadScore {

    //private string phpScriptsFolder = "https://Hexit.000webhostapp.com"; // The Location where all PHP scripts are stored 
    //private string phpAddTheItemScriptLocation = "/DBAccessScripts/InsertUser.php"; // The Location of the PHP script for adding an item



    public enum EDownloadStatus
    {
        DownloadSuccess,
        EndOfDatabase,
        QueryError
    }

    private string url = "https://hexit.000webhostapp.com/DBAccessScripts/GetStatsForUserID.php";

    private string highscoreUrl = "https://hexit.000webhostapp.com/DBAccessScripts/GetHighScoreUserStatsInRange.php";

    //[ContextMenu("Add Item")]  // Calls The AddItemToDB Coroutine from the inspector
    public void GetScoreForUser(int userId, System.Action<Leaderboard.EntryData, DownloadScore.EDownloadStatus> callBack)
    {
        GameManager.instance.StartCoroutine(GetUserScore(userId, callBack ));
        //Debug.Log("starting cooroutine, user id = " + userId);
    }

    public void GetScoresForScoreboard(int userId, int offset, int pageLength, System.Action<Leaderboard.EntryData[], DownloadScore.EDownloadStatus> callBack)
    {
        GameManager.instance.StartCoroutine(GetUserHighScoresOffset(userId, offset, pageLength, callBack));
    }   

    //public void GetScoreForUser()
    //{
    //    StartCoroutine(GetUserScore());
    //}

    private IEnumerator GetUserScore(int userId, System.Action<Leaderboard.EntryData, DownloadScore.EDownloadStatus> callBack)
    {
        WWWForm form = new WWWForm();
        form.AddField("userIDPost", userId);
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
            //Debug.Log("Score Download Complete!");
            string entry = webRequest.downloadHandler.text;

            if (!entry.Contains("\t"))
                throw new System.Exception("Improperly formatted data from database " + entry);
            string[] temp = entry.Split('\t');

            int returnID, returnLevel, returnScore;
            string returnName = "anon";

            int.TryParse(temp[0], out returnID);
            int.TryParse(temp[1], out returnLevel);
            int.TryParse(temp[2], out returnScore);
            //returnName = temp[3];

            //Debug.Log(returnID +" | "+ returnLevel + " | " + returnScore);

            Leaderboard.EntryData returnEntry = new Leaderboard.EntryData(returnID, returnLevel, returnScore, returnName);



            callBack(returnEntry, EDownloadStatus.DownloadSuccess);
        }
    }

    private IEnumerator GetUserHighScoresOffset(int userId, int rowOffset, int pageLimit, System.Action<Leaderboard.EntryData[], DownloadScore.EDownloadStatus> callBack)
    {
        WWWForm form = new WWWForm();
        form.AddField("userIDPost", userId);
        form.AddField("offsetPost", rowOffset);
        form.AddField("limitPost", pageLimit);

        UnityWebRequest webRequest = UnityWebRequest.Post(highscoreUrl, form);

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {

            Debug.Log(webRequest.downloadHandler.text);
            Debug.Log(webRequest.error);

        }

        else
        {

            string entry = webRequest.downloadHandler.text;

            if (!entry.Contains("\t"))
            {
                if (entry.Contains("End"))
                {
                    Debug.Log("End of Entries");
                    callBack(null, EDownloadStatus.EndOfDatabase);
                }
                else
                {
                    callBack(null, EDownloadStatus.QueryError);
                    //throw new System.Exception("Improperly formatted data from database (QUERY CONTAINS AN ERROR " + entry);
                }
            }

            string[] splitEntries = entry.Split('\n');

            Leaderboard.EntryData[] returnEntries = new Leaderboard.EntryData[splitEntries.Length - 1];



            for (int i = 0; i < returnEntries.Length; i++)
            {

                string[] temp = splitEntries[i].Split('\t');



                int returnID, returnLevel, returnScore;
                string returnName;

                int.TryParse(temp[0], out returnID);
                int.TryParse(temp[1], out returnLevel);
                int.TryParse(temp[2], out returnScore);
                returnName = temp[3];

                //Debug.Log(returnID + " | " + returnLevel + " | " + returnScore + " | " + returnName);

                returnEntries[i] = new Leaderboard.EntryData(returnID, returnLevel, returnScore, returnName);
            }


            callBack(returnEntries, EDownloadStatus.DownloadSuccess);
        }
    }

    private IEnumerator GetAllUserHighScores(int userId, System.Action<Leaderboard.EntryData[]> callBack)
    {
        int rowOffset = 0; // Offset or "page" of sorted rows from database


        WWWForm form = new WWWForm();
        form.AddField("userIDPost", userId);
        form.AddField("offsetPost", rowOffset);

        UnityWebRequest webRequest = UnityWebRequest.Post(highscoreUrl, form);

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {

            Debug.Log(webRequest.downloadHandler.text);
            Debug.Log(webRequest.error);

        }

        else
        {

            string entry = webRequest.downloadHandler.text;

            if (!entry.Contains("\t"))
                throw new System.Exception("Improperly formatted data from database (QUERY CONTAINS AN ERROR " + entry);

            string[] splitEntries = entry.Split('\n');

            Leaderboard.EntryData[] returnEntries = new Leaderboard.EntryData[splitEntries.Length - 1];


            for (int i = 0; i < returnEntries.Length; i++)
            {

                string[] temp = splitEntries[i].Split('\t');



                int returnID, returnLevel, returnScore;
                string returnName;

                int.TryParse(temp[0], out returnID);
                int.TryParse(temp[1], out returnLevel);
                int.TryParse(temp[2], out returnScore);
                returnName = temp[3];

                //Debug.Log(returnID + " | " + returnLevel + " | " + returnScore + " | " + returnName);

                returnEntries[i] = new Leaderboard.EntryData(returnID, returnLevel, returnScore, returnName);
            }


            callBack(returnEntries);
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
