using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;


public class GameStateInit : GameStateBase
{

    string pathNewProfileLevel = "Assets/Resources/Levels/Menus/LoadProfileMenuLevel.json";

    private GameObject titleParticleObject;
    private string pathTitleParticlePrefab = "Prefabs/Particles/TitleParticle";

    private GameObject registerUserCanvas;
    private string pathRegisterUserCanvas = "Prefabs/GUI/RegisterUserCanvas";


    private float titleSequenceTime = 0.0f;

    public GameStateInit()
    {
        InitialiseStateTransitions();
    }

    protected override void InitialiseStateTransitions()
    {
        stateTransitions = new Dictionary<Command, TransitionData<GameStateBase>>
        {
            { Command.Begin, new TransitionData<GameStateBase>(typeof(GameStateMenuMain))  },
            { Command.End, new TransitionData<GameStateBase>(typeof(GameStateMenuMain))  },
        };
    }

    public override void StartGameState()
    {
        ShowTitleCard();
        //IntroSequence();
        GameManager.instance.StartCoroutine(IntroSequence());

        //if (!FindUserProfile())
        //    CreateNewProfileMenu();
    }

    public override void StateUpdate()
    {

    }

    IEnumerator IntroSequence()
    {
        yield return new WaitForSeconds(2.0f);
        yield return GameManager.instance.StartCoroutine(MoveObject(titleParticleObject, new Vector3(0, 1, 0), new Vector3(0, -6, 0), 1.0f));
        yield return new WaitForSeconds(4.0f);
        yield return GameManager.instance.StartCoroutine(MoveObject(titleParticleObject, new Vector3(0, -6, 0), new Vector3(0, 25, 0), 1.0f));
        yield return new WaitForSeconds(1.0f);
        if (!FindUserProfile())
            CreateNewProfileMenu();
    }



    IEnumerator MoveObject(GameObject objectToMove, Vector3 source, Vector3 target, float overTime)
    {
        float startTime = Time.time;
        while (Time.time < startTime + overTime)
        {
            objectToMove.transform.position = Vector3.Lerp(source, target, overTime);
            yield return null;
        }
        objectToMove.transform.position = target;
    }


    private void CreateNewProfileMenu()
    {
        Level loadedLevel = LevelLoader.Instance.LoadLevelFile(pathNewProfileLevel);
        if (loadedLevel != null)
        {
            CreateLevel(
                loadedLevel, //LoadLevelsFromPath(pathNewProfileLevel)[0],
                - 30.0f,
                false,
                false
                //new LoadProfileLevelComponent()
                );

        }
        else throw new System.Exception("GameStateInit error - Level not loaded");
        DisplayNewProfileMenu();
    }


    private void ShowTitleCard()
    {
        titleParticleObject = GameObject.Instantiate(Resources.Load(pathTitleParticlePrefab) as GameObject);

        Vector3 targetPosition = Camera.main.gameObject.transform.position + new Vector3(0, 1, 0);
        titleParticleObject.transform.position = targetPosition;
    }

    //private void UpdateTitleCardPosition(Vector3 targetPosition, float rate)
    //{
    //    titleParticleObject.transform.position = Vector3.Lerp(titleParticleObject.transform.position, targetPosition, rate);
    //}


    #region Custom State Methods 

    private string gameDataFileName = "PlayerProfile.json"; // TODO: move to a more global location



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


    bool FindUserProfile()
    {

        Debug.Log("FindUserProfile called");
        string filePath = Path.Combine(Application.persistentDataPath, gameDataFileName);

        if (File.Exists(filePath))
        {
            Debug.Log("UserProfile data found at " + Application.persistentDataPath + gameDataFileName);

            string dataAsJson = File.ReadAllText(filePath);
            PlayerProfile loadedData = JsonUtility.FromJson<PlayerProfile>(dataAsJson);

            GameManager.instance.loadedProfile = loadedData;

            Debug.Log("loaded profile id = " + loadedData.GetPlayerID() + ", name = " + loadedData.GetPlayerName());

            GameManager.instance.ProcessCommand(Command.End);
            return true;
        }

        else
        {
            Debug.Log("No User Profile data found. Creating a new user...");

            DisplayNewProfileMenu();
            return false;
        }
    }

    void CreateNewPlayerProfile(PlayerProfile newProfile)
    {
        JsonUtility.ToJson(newProfile);

        string dataAsJson = JsonUtility.ToJson(newProfile);

        string filePath = Path.Combine(Application.persistentDataPath, gameDataFileName);

        Debug.Log(Application.persistentDataPath);

        File.WriteAllText(filePath, dataAsJson);
        //Application.persistentDataPath;
    }

    // The menu screen for creating a new user profile
    void DisplayNewProfileMenu()
    {
        registerUserCanvas = GameObject.Instantiate(Resources.Load(pathRegisterUserCanvas) as GameObject);

        if (registerUserCanvas == null) throw new System.Exception("Failed to instantiate the RegisterIserCanvas from path " + pathRegisterUserCanvas);

        //Hex registerUserHexButton = MapSpawner.Instance.SpawnHexAtLocation(new Vector2Int(0, -2), HexTypeEnum.HexTile_MenuOptionPlay, true);
        //registerUserHexButton.clickedEvent.AddListener(() =>
        //{
        //    HandleRegisterClick();
        //    registerUserHexButton.DestroyHex();
        //});
    }

    void DisplayRegisterResponse()
    {
        // Display to inform user of register success/failure
    }

    void DisplayPlayOfflineMenu()
    {
        // Present the user with the choice to play offline
    }

    public override void NextMenu() 
    {
        HandleRegisterClick();
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
                CreateNewProfileMenu();
                //DisplayNewProfileMenu(); // DEPRECATED
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
