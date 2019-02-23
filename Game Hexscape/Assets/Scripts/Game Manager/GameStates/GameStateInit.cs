using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameStateInit : GameStateBase
{

    string pathNewProfileLevel = "";

    public override void StartGameState()
    {

        string filePath = Path.Combine(Application.persistentDataPath, "PlayerProfile.json");

        if (File.Exists(filePath))
        {
            Debug.Log("UserProfile data found at " + Application.persistentDataPath + "PlayerProfile.json");

            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(filePath);
            // Pass the json to JsonUtility, and tell it to create a GameData object from it
            PlayerProfile loadedData = JsonUtility.FromJson<PlayerProfile>(dataAsJson);
        }

        else
        {
            Debug.Log("No User Profile data found. Creating a new user...");

            // create new user
            // Load a level/ menu to request data? 
            // Should a level component handle the data?

        }

    }

    Level newProfileLevel;

    private void CreateNewProfileMenu()
    {
        Level[] levels = Resources.LoadAll<Level>("Levels/Menus");
        newProfileLevel = levels[0];

        newProfileLevel = LoadLevelFromPath(pathNewProfileLevel);

        MapSpawner.instance.SpawnHexs(newProfileLevel, GameManager.instance.GetPlayerBall().transform.position - new Vector3(0, -30, 0), false);
        GameObject currentLevelObject = MapSpawner.instance.GetCurrentMapHolder();
        currentLevelObject.AddComponent<ScoreboardLevelComponent>();

        Vector3 mapPosition = currentLevelObject.transform.position;
        mapPosition += new Vector3(0, 0, 0);
        GameManager.instance.GetPlayerBall().transform.position = mapPosition; // ballPosition;
        GameManager.instance.GetPlayerBall().SetActive(false);
    }
}
