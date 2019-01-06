using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class GameStateEndless : GameStateBase
{
    [SerializeField] public Level[] levels;

    [Space(5f)]
    [Header("Options:")]
    [Space(1f)]
    [Header("   - Map Options:")]
    public bool useRandomLevels = false;
    private bool editMode = false; // TEMP - TODO: Create 'EditModeState'

    [Space(3f)]
    [Header("   - Player Options:")]
    public float initialPlayerSpeed = 30f;
    public float playerSpeedIncreaseLogBase = 2f;
    public float playerSpeedIncreaseLogMultiplyer = 10f;

    PlayerController playerController;

    public GameStateEndless()
    {

    }

    void Start()
    {
        

    }

    void Update()
    {

    }

    public override void StartGameState()
    {
        Debug.Log("GameStateEndless: Start Game State ");
        currentSessionData = new GameSessionData();
        PopulateLevelsArray();

        InitialiseClickSounds();


        Vector3 mapPosition = MapSpawner.instance.GetCurrentMapHolder().transform.position;
        mapPosition += new Vector3(0, 10, 0);
        GameManager.instance.GetPlayerBall().transform.position = mapPosition; // ballPosition;
        GameManager.instance.GetPlayerBall().SetActive(true);


        editMode = PlayerPrefs.GetInt("Edit Mode") == 1;
        if (editMode == true) // level edit on start... if the ball does not spawn we go into edit mode because of a helpfull bug thingo :)
        {
            Object.Destroy(Object.FindObjectOfType<PlayerController>().gameObject);
            //scoreUI.totalScoreText.text = "EDIT MODE";
            //scoreUI.totalScoreText.color = Color.red;

        }
        else
        {

            Debug.Log("Finding Player...");

            playerController = GameManager.instance.GetPlayerBall().GetComponent<PlayerController>();

 
            if (playerController != null) playerController.moveSpeed = initialPlayerSpeed;
        }

        LoadNextLevel();


    }

    public override void CleanupGameState()
    {
        // ...
  
    }

    protected override void InitialiseClickSounds()
    {
        hexClickSounds = new SoundEffectEnum[5]
        {
            SoundEffectEnum.ES_01,
            SoundEffectEnum.ES_02,
            SoundEffectEnum.ES_03,
            SoundEffectEnum.ES_04,
            SoundEffectEnum.ES_05
        };
    }

    public override void PlayClickSound()
    {
        if (hexClickSounds.Length > 0)
        {
            int scaleIndex = currentSessionData.levelScore % hexClickSounds.Length;
            int pitch = (currentSessionData.levelScore / hexClickSounds.Length) + 1;

            AudioManager.instance.PlaySoundEffect(hexClickSounds[scaleIndex], pitch);
        }
    }


    protected override void HandleInput()
    {
        throw new System.NotImplementedException();
    }

    public override void LoadNextLevel()
    {
        ++currentSessionData.levelIndex;

        Level newLevel = useRandomLevels ? levels[Random.Range(0, levels.Length)] : levels[currentSessionData.levelIndex];

        currentSessionData.levelScore = 0;
        currentSessionData.passScore = 0;
        newLevel.hexs.ToList().ForEach(x => currentSessionData.passScore += x.GetHex().destroyPoints);


        //scoreUI.ModifyValues(currentSessionData);
        //scoreUI.SetMedalState(MedalState.noMedal);
        if (playerController != null)
        {
            playerController.SetDestination(playerController.transform.position);
            playerController.moveSpeed = initialPlayerSpeed + (playerSpeedIncreaseLogMultiplyer * currentSessionData.levelIndex * ( 10 * Mathf.Log(playerSpeedIncreaseLogBase)));
        }

        MapSpawner.instance.SpawnHexs(newLevel, playerController.transform.position);

        UpdateScore();

        
    }

    public override void HexDigEvent()
    {
        currentSessionData.levelScore += 1; // sound uses level score WTF?
        currentSessionData.totalScore += 1;


        //scoreUI.SetScore(totalScore, levelCurrentScore, passScore);

        // Debug.Log("Current Level: " + levelCurrentScore + ", Pass Score: " + passScore);
        if (currentSessionData.levelScore >= currentSessionData.passScore)
        {
            Debug.Log("HexDigEvent: LoadNextLevel");
            LoadNextLevel();
        }

        UpdateScore();
    }

    public override void PlayGroundThud()
    {
        AudioManager.instance.PlaySoundEffect(SoundEffectEnum.Ground_Hit_Thud);
        RippleManager.instance.CreateRippleThud(playerController.gameObject.transform.position, 5f, 100);
    }

    public override void Pause()
    {
        throw new System.NotImplementedException();
    }

    public override void Resume()
    {
        throw new System.NotImplementedException();
    }


    private void PopulateLevelsArray()
    {
        levels = Resources.LoadAll<Level>("Levels/Endless");
        Debug.Log("Loaded Level count = " + levels.Length);
    }



    /// updating the UI

    

    private void UpdateScore ()
    {
        GameManager.instance.scoreUI.SetScore(currentSessionData.totalScore, currentSessionData.levelScore, currentSessionData.passScore);
    }
}



