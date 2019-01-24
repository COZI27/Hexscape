using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class AddUser : MonoBehaviour
{

    private string phpScriptsFolder = "https://Hexit.000webhostapp.com"; // The Location where all PHP scripts are stored 
    private string phpAddTheItemScriptLocation = "/DBAccessScripts/InsertUser.php"; // The Location of the PHP script for adding an item


    private string url = "https://hexit.000webhostapp.com/DBAccessScripts/InsertUser.php";

    [ContextMenu("Add Item")]  // Calls The AddItemToDB Coroutine from the inspector
    public void AddTheItem()
    {
        StartCoroutine(AddUserToDB("nametest", "passtest"));
    }

    public void AddTheItem(string name, string password)
    {
        StartCoroutine(AddUserToDB(name, password));
    }

    private IEnumerator AddUserToDB(string name, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("usernamePost", name); 
        form.AddField("passwordPost", password);

        UnityWebRequest webRequest = UnityWebRequest.Post(/*phpScriptsFolder + phpAddTheItemScriptLocation*/ url, form);

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {

            Debug.Log(webRequest.downloadHandler.text);
            Debug.Log(webRequest.error);

        }
        else
        {
            Debug.Log("Form upload complete!");
        }

    }
}
