using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UploadItemDemo : MonoBehaviour
{




    [SerializeField] private string itemNameInput; // Item Name Input
    [SerializeField] private int itemDamageInput; // Item Damage Input
    [SerializeField] private int itemDefenceInput; // Item Defence Input 


    private string phpScriptsFolder = "https://deflated-administra.000webhostapp.com"; // The Location where all PHP scripts are stored 
    private string phpAddTheItemScriptLocation = "/ItemDatabase/UploadItem.php"; // The Location of the PHP script for adding an item


    [ContextMenu("Add Item")]  // Calls The AddItemToDB Coroutine from the inspector
    public void AddTheItem()
    {
        StartCoroutine(AddItemToDB(itemNameInput, itemDamageInput, itemDefenceInput));
    }


    private IEnumerator AddItemToDB(string name, int damage, int defence)
    {

        // Creates a form of fields to assign a values too for name damage and defence.



        WWWForm form = new WWWForm();
        form.AddField("itemNamePost", name); // itemNamePost field on the php ItemUpload.php script is set to 'name'
        form.AddField("itemDamagePost", damage);
        form.AddField("itemDefencePost", defence);

        UnityWebRequest webRequest = UnityWebRequest.Post(phpScriptsFolder + phpAddTheItemScriptLocation, form);


        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log(webRequest.error);

        }
        else
        {
            Debug.Log("Form upload complete!");
        }


        // this uploads the item to the Data Base

        yield return webRequest;


    }



}

/*
 <?php

$server_username = "id6360098_cozi27";
$server_password = "Peanuts 27";
$dbName = "id6360098_hexscapedatabase";

$itemName = $_POST['itemNamePost'];
$itemDamage = $_POST['itemDamagePost'];
$itemDefence = $_POST['itemDefencePost'];

$conn = new mysqli($servername, $server_username, $server_password, $dbName);

if ($conn == true) {
// Connection was made.
	echo("Connection Success: <br> <br>");
} else {
// connection was not made.
die	("Connection Failed." . mysqli_connect_error() );
}



$sql = "INSERT INTO `Items` ( `Name`, `Damage`, `Defence`) 
VALUES ('".$itemName."', '".$itemDamage."', '".$itemDefence."')";

$result = mysqli_query($conn ,$sql);


if (!$result) {
	echo ("there was an error");
} else {
	echo ("Everything is okay");
}

?>






*/

