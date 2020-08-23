using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;


public class GameStateInit : GameStateBase
{

    private enum EInitMenuEnum
    {
        PostInit,
        Login,
        NewUser
    }

    private string pathLogInLevel = "Levels/Menus/LoadProfileMenuLevel";
    string pathNewProfileLevel = "Levels/Menus/LoadProfileMenuLevel";
    string pathPostInitMenu = "Levels/Menus/PostInitMenu";
    //string pathNewProfileLevel = "Assets/Resources/Levels/Menus/LoadProfileMenuLevel.json";

    private GameObject titleParticleObject;
    private string pathTitleParticlePrefab = "Prefabs/Particles/TitleParticle";

    private GameObject currentCanvasObject;
    private string pathRegisterUserCanvas = "Prefabs/GUI/RegisterUserCanvas";

    UserCanvasScript registerCanvasScript;

    private EInitMenuEnum currentMenu;



    public GameStateInit()
    {
        InitialiseStateTransitions();
    }

    public override bool CleanupGameState()
    {
        GameObject.Destroy(currentCanvasObject);
        GameObject.Destroy(titleParticleObject);

        return true;
    }

    protected override void InitialiseStateTransitions()
    {
        stateTransitions = new Dictionary<Command, TransitionData<GameStateBase>>
        {

            //TODO: Accomodate for button click on generate profile. Must lead to checking conditions before loading net scene
            // Consider whether IsTransitionCommand is used here
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
            DisplayMenu(EInitMenuEnum.PostInit);
    }

    private void DisplayMenu(EInitMenuEnum menuToDisplay)
    {
        currentMenu = menuToDisplay;

        switch (menuToDisplay)
        {
            case EInitMenuEnum.PostInit:
                PostInitMenu();
                break;
            case EInitMenuEnum.Login:
                LogInMenu();
                break;
            case EInitMenuEnum.NewUser:
                CreateNewProfileMenu();
                break;
        }
    }

    private void PostInitMenu()
    {

        Level loadedLevel = LevelLoader.Instance.LoadLevelFile(pathPostInitMenu);
        if (loadedLevel != null)
        {
            CreateLevel(
             loadedLevel,
             -30.0f,
             false,
             false
             );
        }
        else throw new System.Exception("GameStateInit Level Lad Failed");

    }

    public override void HandleCommand(Command command)
    {
        Debug.Log("HandleCommand(" + command + ")");
        switch (command)
        {
            case Command.Login:
                DisplayMenu(EInitMenuEnum.Login);
                break;
            case Command.NewUser:
                DisplayMenu(EInitMenuEnum.NewUser);
                break;
            case Command.Skip:
                //TODO: Display "No Online scores etc" warning
                GameManager.instance.SetIsOffline(true);
                GameManager.instance.ProcessCommand(Command.End);
                break;
            case Command.NextMenu:
                switch (currentMenu)
                {
                    case EInitMenuEnum.PostInit:
                        break;
                    case EInitMenuEnum.Login:
                        LoginUser();
                        break;
                    case EInitMenuEnum.NewUser:
                        RegisterUser();
                        break;
                }
                break;
            case Command.BackMenu:
                GameManager.instance.StartCoroutine(DestroyCurrentCanvas(1));
                DisplayMenu(EInitMenuEnum.PostInit);
                break;
    }
}




    private void LogInMenu()
    {
        Level loadedLevel = LevelLoader.Instance.LoadLevelFile(pathLogInLevel);
        if (loadedLevel != null)
        {
            CreateLevel(
                loadedLevel,
                -30.0f,
                false,
                false
                );
        }

        DisplayNewProfileCanvas();
    }


    private void CreateNewProfileMenu()
    {
        Level loadedLevel = LevelLoader.Instance.LoadLevelFile(pathNewProfileLevel);
        if (loadedLevel != null)
        {
               CreateLevel(
                loadedLevel,
                - 30.0f,
                false,
                false
                );
        }
        else throw new System.Exception("GameStateInit Level Lad Failed"); 

        DisplayNewProfileCanvas();
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

    private string gameDataFileName = "PlayerProfile.json"; 



    private void OnDestroy()
    {
        //if (registerUserCanvas != null) GameObject.Destroy(registerUserCanvas);
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
        return false;
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
    void DisplayNewProfileCanvas()
    {
        Debug.Log("DisplayNewProfileMenu");
        if (currentCanvasObject == null)
        {
            currentCanvasObject = GameObject.Instantiate(Resources.Load(pathRegisterUserCanvas) as GameObject);
            currentCanvasObject.GetComponentInChildren<Animator>().SetTrigger("Show");

            registerCanvasScript = currentCanvasObject.GetComponent<UserCanvasScript>();
        }
        else Debug.Log("CANVAS NOT NULL");
        
    }

    void DisplayPlayOfflineMenu()
    {
        // Present the user with the choice to play offline
    }


    private void RegisterUser()
    {
        AddUser addUser = new AddUser();

        if (addUser != null)
        {
            InputField[] fields = currentCanvasObject.GetComponentsInChildren<InputField>();

            InputField nameInput = fields[1];
            InputField passwordInput = fields[2];
            Debug.Log("AttemptAddNewUser using " + nameInput.text + " & " + passwordInput.text);
            addUser.AttemptAddNewUser(nameInput.text, passwordInput.text, RegisterUserCallback);
            registerCanvasScript.HideInputFields();
            //GameManager.instance.StartCoroutine(DestroyCurrentCanvas());
        }
        else Debug.Log("Add user = null");

        // Attempt to add a user
        // Wait for Response
        // If succeed, then main menu, else throw error and loop/repeat
    }

    private void LoginUser()
    {
        LoginUser loginUser = new LoginUser();

        if (loginUser != null)
        {
            InputField[] fields = currentCanvasObject.GetComponentsInChildren<InputField>();

            InputField nameInput = fields[1];
            InputField passwordInput = fields[2];
            Debug.Log("AttemptAddNewUser using " + nameInput.text + " & " + passwordInput.text);
            loginUser.AttemptLoginUser(nameInput.text, passwordInput.text, LoginUserCallback);
            //Object.Destroy(currentCanvasObject);

            registerCanvasScript.HideInputFields();
            //GameManager.instance.StartCoroutine(DestroyCurrentCanvas());
        }
        else Debug.Log("Add user = null");
    }

    private IEnumerator DestroyCurrentCanvas(float delay)
    {
        if (currentCanvasObject != null)
        {
            registerCanvasScript.HideResponseField();
            registerCanvasScript.HideInputFields();
            yield return new WaitForSeconds(delay);

            if (currentCanvasObject != null) Object.Destroy(currentCanvasObject);
        }
    }



    public void RegisterUserCallback(AddUser.AddUserResponse data)
    {
        string response = data.response.Remove(data.response.Length - 1); // Remove /t char from the end of the string

        switch (response)
        {
            case "NetworkError":
                Debug.Log("Network Error");
                registerCanvasScript.DisiplayResponseMessage("Network Error");
                registerCanvasScript.ShowInputFields();
                CreateNewProfileMenu();
                break;
            case "HTTPError":
                Debug.Log("HTTPError");
                registerCanvasScript.DisiplayResponseMessage("HTTP Error");
                registerCanvasScript.ShowInputFields();
                CreateNewProfileMenu();
                break;
            case "NameTaken":
                registerCanvasScript.DisiplayResponseMessage("Username Taken");
                registerCanvasScript.ShowInputFields();
                CreateNewProfileMenu();
                break;
            case "Failed ":
                registerCanvasScript.DisiplayResponseMessage("Server Error");
                registerCanvasScript.ShowInputFields();
                CreateNewProfileMenu();
                break;
            case "Success":
                PlayerProfile newProfile = new PlayerProfile(data.id.ToString(), data.name);
                CreateNewPlayerProfile(newProfile);
                registerCanvasScript.DisiplayResponseMessage("Register User Success");
                GameManager.instance.StartCoroutine(DestroyCurrentCanvas(3));
                FindUserProfile();
                break;
            default:
                registerCanvasScript.ShowInputFields();
                CreateNewProfileMenu();
                break;
        }
    }

    public void LoginUserCallback(LoginUser.LoginUserResponse data)
    {

        string response = data.response.Remove(data.response.Length - 1); // Remove /t char from the end of the string
        switch (response)
        {
            case "NetworkError":
                registerCanvasScript.DisiplayResponseMessage("Network Error");
                registerCanvasScript.ShowInputFields();
                CreateNewProfileMenu();
                break;
            case "HTTPError":
                registerCanvasScript.DisiplayResponseMessage("HTTP Error");
                registerCanvasScript.ShowInputFields();
                CreateNewProfileMenu();
                break;
            case "IncorrectPassword":
                registerCanvasScript.DisiplayResponseMessage("Incorrect Password");
                registerCanvasScript.ShowInputFields();
                CreateNewProfileMenu();
                break;
            case "UnknownUsername":
                registerCanvasScript.DisiplayResponseMessage("Unknown Username");
                registerCanvasScript.ShowInputFields();
                CreateNewProfileMenu();
                break;
            case "Failed":
                registerCanvasScript.DisiplayResponseMessage("Server Error");
                registerCanvasScript.ShowInputFields();
                CreateNewProfileMenu();
                break;
            case "Success":
                PlayerProfile newProfile = new PlayerProfile(data.id.ToString(), data.name);
                CreateNewPlayerProfile(newProfile);
                registerCanvasScript.DisiplayResponseMessage("Login Succesful");
                GameManager.instance.StartCoroutine(DestroyCurrentCanvas(3));
                FindUserProfile();
                break;
            default:
                registerCanvasScript.ShowInputFields();
                CreateNewProfileMenu();
                break;
        }
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

    #endregion

}
