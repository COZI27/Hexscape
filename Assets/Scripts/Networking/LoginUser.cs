using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class LoginUser
{

    public class LoginUserResponse
    {
        public LoginUserResponse(int id, string name, string response)
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
    //private string phpAddTheItemScriptLocation = "/DBAccessScripts/LogIn.php"; // The Location of the PHP script for adding an item

    private string url = "https://hexit.000webhostapp.com/DBAccessScripts/LogIn.php";

    public void AttemptLoginUser(string username, string password, System.Action<LoginUserResponse> callBack)
    {
        GameManager.instance.StartCoroutine(LoginUserFromDB(username, password, callBack));
    }

    private IEnumerator LoginUserFromDB(string username, string password, System.Action<LoginUserResponse> callBack)
    {

        yield return new WaitForSeconds(0.5f);

        WWWForm form = new WWWForm();
        form.AddField("usernamePost", username);
        form.AddField("passwordPost", password);

        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {

            Debug.Log(webRequest.downloadHandler.text);
            Debug.Log(webRequest.error);
            LoginUserResponse returnEntry;
            if (webRequest.isNetworkError) returnEntry = new LoginUserResponse(0, null, "NetworkError");
            else returnEntry = new LoginUserResponse(0, null, "HTTPError");
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

            LoginUserResponse returnEntry = new LoginUserResponse(returnID, returnName, returnResponse);

            callBack(returnEntry);
        }
    }
}
