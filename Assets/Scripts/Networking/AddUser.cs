using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class AddUser
{

    public class AddUserResponse
    {
        public AddUserResponse(int id, string name, string response)
        {
            this.id = id;
            this.name = name;
            this.response = response;
        }

        public int id;
        public string name;
        public string response;

    }


    //private string phpScriptsFolder = "https://Hexit.000webhostapp.com"; // The Location where all PHP scripts are stored 
    //private string phpAddTheItemScriptLocation = "/DBAccessScripts/InsertUser.php"; // The Location of the PHP script for adding an item
    private string url = "https://hexit.000webhostapp.com/DBAccessScripts/InsertUser.php";

    [ContextMenu("Add Item")]  // Calls The AddItemToDB Coroutine from the inspector
    public void AttemptAddNewUser(string username, string password, System.Action<AddUserResponse> callBack)
    {
        GameManager.instance.StartCoroutine(AddUserToDB(username, password, callBack));
    }

    public void BeginAddNewUser(string username, string password, System.Action<AddUserResponse> callBack)
    {
        GameManager.instance.StartCoroutine(AddUserToDB(username, password, callBack));
    }

    private IEnumerator AddUserToDB(string username, string password, System.Action<AddUserResponse> callBack)
    {
        yield return new WaitForSeconds(0.5f);

        WWWForm form = new WWWForm();
        form.AddField("usernamePost", username); 
        form.AddField("passwordPost", password);

        UnityWebRequest webRequest = UnityWebRequest.Post(/*phpScriptsFolder + phpAddTheItemScriptLocation*/ url, form);

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {

            Debug.Log(webRequest.downloadHandler.text);
            Debug.Log(webRequest.error);
            AddUserResponse returnEntry;
            if (webRequest.isNetworkError) returnEntry = new AddUserResponse(0, null, "NetworkError" );
            else returnEntry = new AddUserResponse(0, null, "HTTPError" );
            callBack(returnEntry);
        }
        else
        {
            Debug.Log("Form upload complete!");
            string entry = webRequest.downloadHandler.text;

            if (!entry.Contains("\t"))
                throw new System.Exception("Improperly formatted data from database " + entry);     // name\tid\tresponse\n // TODO: throw ServerError and continue offline

            string[] temp = entry.Split('\t');

            int returnID;
            string returnName, returnResponse;

            returnName = temp[1];
            returnResponse = temp[2];

            int.TryParse(temp[0], out returnID);
            //int.TryParse(temp[1], out returnName);
            //int.TryParse(temp[2], out returnResponse);

            AddUserResponse returnEntry = new AddUserResponse(returnID, returnName, returnResponse);

            callBack(returnEntry);
        }


    }

    //the string passed ParseResponseString MUST be of the form:
    // name\tid\tresponse\n
    //public void ParseResponseString(string responseString)
    //{
    //    string[] splitScores = responseString.Trim().Split('\n');
    //    if (responseString == string.Empty)
    //        return;
    //    int count = 0;
    //    foreach (string entry in splitScores)
    //    {
    //        //throw an error if the string is not properly formatted
    //        if (!entry.Contains("\t"))
    //            throw new System.Exception("Improperly formatted data from database " + entry);
    //        string[] temp = entry.Split('\t');
    //        count++;
    //    }
    //}

}
