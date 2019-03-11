using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;


public class GameStateInit : GameStateBase
{

    string pathNewProfileLevel = "Levels/Menus/RegisterProfile";

    public override void StartGameState()
    {
        CreateNewProfileMenu();
    }

    private void CreateNewProfileMenu()
    {
        CreateLevel(
            LoadLevelsFromPath(pathNewProfileLevel)[0],
            -30.0f,
            false,
            false
            //new LoadProfileLevelComponent()
            );

        DisplayNewProfileMenu();
    }


    #region Custom State Methods 

    private string gameDataFileName = "PlayerProfile.json"; // TODO: move to a more global location

    private GameObject registerUserCanvas;
    private string pathRegisterUserCanvas = "Prefabs/GUI/RegisterUserCanvas";

    // Use this for initialization
    void Start()
    {
        FindUserProfile();
    }

    private void OnDestroy()
    {
        //if (registerUserCanvas != null) Destroy(registerUserCanvas);
    }

    [ContextMenu("Delete Local Profile Data")]
    public void DeleteLocalProfileData()
    {
        string filePath = Path.Combine(Application.persistentDataPath, gameDataFileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }


    void FindUserProfile()
    {
        string filePath = Path.Combine(Application.persistentDataPath, gameDataFileName);

        if (File.Exists(filePath))
        {
            Debug.Log("UserProfile data found at " + Application.persistentDataPath + gameDataFileName);

            string dataAsJson = File.ReadAllText(filePath);
            PlayerProfile loadedData = JsonUtility.FromJson<PlayerProfile>(dataAsJson);

            GameManager.instance.loadedProfile = loadedData;

            Debug.Log("loaded profile id = " + loadedData.GetPlayerID() + ", name = " + loadedData.GetPlayerName());

            GameManager.instance.ProcessCommand(GameManager.Command.End);
        }

        else
        {
            Debug.Log("No User Profile data found. Creating a new user...");

            DisplayNewProfileMenu();

        }
    }

    void CreateNewPlayerProfile(PlayerProfile newProfile)
    {
        JsonUtility.ToJson(newProfile);

        string dataAsJson = JsonUtility.ToJson(newProfile);

        string filePath = Path.Combine(Application.persistentDataPath, gameDataFileName);

        File.WriteAllText(filePath, dataAsJson);
        //Application.persistentDataPath;
    }

    // The menu screen for creating a new user profile
    void DisplayNewProfileMenu()
    {
        registerUserCanvas = GameObject.Instantiate(Resources.Load(pathRegisterUserCanvas) as GameObject);

        if (registerUserCanvas == null) throw new System.Exception("Failed to instantiate the RegisterIserCanvas from path " + pathRegisterUserCanvas);

        Hex registerUserHexButton = MapSpawner.instance.SpawnHexAtLocation(new Vector2Int(0, -2), HexTypeEnum.HexTile_MenuOptionPlay, true);
        registerUserHexButton.clickedEvent.AddListener(() =>
        {
            HandleRegisterClick();
            registerUserHexButton.DestroyHex();
        });
    }

    void DisplayRegisterResponse()
    {
        // Display to inform user of register success/failure
    }

    void DisplayPlayOfflineMenu()
    {
        // Present the user with the choice to play offline
    }


    public void HandleRegisterClick()
    {
        Debug.Log("HandleRegisterClick called!");

        AddUser addUser = new AddUser();

        //AddUser addUser = this.gameObject.AddComponent<AddUser>();
        if (addUser != null)
        {
            InputField nameInput = registerUserCanvas.GetComponentInChildren<InputField>();
            addUser.AttemptAddNewUser(nameInput.text, RegisterUserCallback);
            Object.Destroy(registerUserCanvas);
        }
        else Debug.Log("Add user = null");

        // Attempt to add a user
        // Wait for Response
        // If succeed, then main menu, else throw error and loop/repeat
    }

    public void RegisterUserCallback(AddUser.AddUserResponse data)
    {
        Debug.Log("RegisterUserCallback called!");

        string response = data.response.Remove(data.response.Length - 1); // Remove /t char from the end of the string

        switch (response)
        {
            case "NetworkError":
                Debug.Log("Network Error");
                // Play offline
                break;
            case "NameTaken":
                Debug.Log("Name Taken");
                DisplayNewProfileMenu();
                break;
            case "Failed ":
                Debug.Log("Server Error");
                break;
            case "Success":
                PlayerProfile newProfile = new PlayerProfile(data.id.ToString(), data.name);
                Debug.Log("Successfully Added Profile" + newProfile.GetPlayerID() + ", " + newProfile.GetPlayerName());
                Debug.Log("Successfully Added Profile" + data.id.ToString() + ", " + data.name);
                CreateNewPlayerProfile(newProfile);
                if (registerUserCanvas != null) Object.Destroy(registerUserCanvas);
                FindUserProfile();
                break;
            default:
                Debug.Log("Default Error");
                Debug.Log(data.response);
                break;
        }
    }


    #endregion

}
