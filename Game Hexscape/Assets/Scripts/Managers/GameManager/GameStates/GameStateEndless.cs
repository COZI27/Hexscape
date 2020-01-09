using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public class EnergyMetre
{

    public EnergyMetre(float initialEnergy = 90)
    {
        currentEnergyLevel = initialEnergy;
    }

    public delegate void OnMaxEnergyReached();
    public event OnMaxEnergyReached onMaxEnergyReached;

    public UnityEvent maxEnergyReached = new UnityEvent();


    private float currentEnergyLevel;
    public float GetCurrentEnergy() { return currentEnergyLevel; }

    public float GetCurrentEnergyNormalised()
    {
        return (currentEnergyLevel - 0) / (energyMax - 0);      
    }

    private float deteriorationRate = 0.05f;

    private float peakDeteriorationExponent = 1.02f;

    private float energyPeak = 100;

    private int energyMax = 120;
    public int GetEnergyMax() { return energyMax; }


    public void AddEnergy(float amountToAdd)
    {
        currentEnergyLevel += amountToAdd;
        if (currentEnergyLevel >= energyMax)
        {
            currentEnergyLevel = energyMax;
            maxEnergyReached.Invoke();
        }

    }

    public void DrainEmergy()
    {
        if (currentEnergyLevel > energyPeak)
        {
            //currentEnergyLevel -= GetPow(deteriorationRate, currentEnergyLevel);
            currentEnergyLevel -= deteriorationRate + (Mathf.Pow(peakDeteriorationExponent, currentEnergyLevel - energyPeak) - peakDeteriorationExponent);

        }
        else currentEnergyLevel -= deteriorationRate;
    }

    private static float GetPow(float baseNum, float powNum)
    {
        float result = 1;

        for (int i=0; i < powNum; i++)
            result = result * baseNum;
       
        return result;
    }


}

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
    //public float initialPlayerSpeed = 30f; // Replaced with initial Energy value
    public float playerSpeedIncreaseLogBase = 2f;
    public float playerSpeedIncreaseLogMultiplyer = 2.5f;

    public float playerKillzoneOffset = 40f;

    float initialEnergy = 70.0f;

    #region PostProcessingAttributes
    // NOTE: could be moved to a struct - could permit the manager to update levels on behalf of state?
    float minSaturation = -70;
    float maxSaturation = 20;

    float minBrightness = -50;
    float maxBrightness = 50;
    #endregion


    HexTunnelEnergy energyMetreTunnel;
   
    PlayerController playerController;

    EnergyMetre energyMetre;

    public GameStateEndless()
    {
        InitialiseStateTransitions();

        energyMetre = new EnergyMetre(initialEnergy);

        energyMetre.maxEnergyReached.AddListener(() =>
        {
            MaxEnergyReachedListener();
            //HandleRegisterClick();
            //GameManager.instance.ProcessCommand(commandToCall);
        });
    }

    public override void StateUpdate()
    {
        energyMetre.DrainEmergy();

        //GameManager.instance.scoreUI.SetFillValue(  energyMetre.GetCurrentEnergyNormalised());
        energyMetreTunnel.SetEnergyValue(energyMetre.GetCurrentEnergy());


        UpdatePostProcesser();

        if (playerController.gameObject.transform.position.y < MapSpawner.Instance.GetCurrentMapHolder().transform.position.y - playerKillzoneOffset)
        {
            //throw new System.Exception("BALL HAS FALLEN");

            LoadNextLevel();

            //GameManager.instance.ProcessCommand(Command.End);
        }

        //Debug.Log(energyMetre.GetCurrentEnergy());

        playerController.moveSpeed = /*initialPlayerSpeed +*/ (energyMetre.GetCurrentEnergy() * (playerSpeedIncreaseLogMultiplyer * Mathf.Log(playerSpeedIncreaseLogBase)));

        //playerController.moveSpeed = initialPlayerSpeed + (currentSessionData.levelIndex * (playerSpeedIncreaseLogMultiplyer * Mathf.Log(playerSpeedIncreaseLogBase)));

        if (energyMetre.GetCurrentEnergy() <= 0) GameManager.instance.StartCoroutine(EndGame());
    }

    float currentColourBoost = 0;
    float colourBoostToAddOnDig = 40;
    float colourBoostDeclineExponent = 0.4f;

    float colourBoostMin = -70;
    float colourBoostMax = 30;

    private void UpdatePostProcesser()
    {
        if (currentColourBoost > 0)
            currentColourBoost -= Mathf.Pow(currentColourBoost, colourBoostDeclineExponent);
        else currentColourBoost = 0;

        float currentEnergy = energyMetre.GetCurrentEnergy() - 50 + currentColourBoost;    

        PostProcessingManager.instance.ModifyColourGrading(currentEnergy, currentEnergy);
    }

    private void AddToColourBoost(float amountToAdd)
    {
        currentColourBoost += amountToAdd;
    }

    private void MaxEnergyReachedListener()
    {
        Debug.Log("MAX ENERGY");
        //Multiplier
        //Reset Energy (lerp drain?)
    }


    protected override void InitialiseStateTransitions()
    {
        stateTransitions = new Dictionary<Command, TransitionData<GameStateBase>>
        {
            { Command.Begin, new TransitionData<GameStateBase>(typeof(GameStateEndlessScoreboard))  },
            { Command.End, new TransitionData<GameStateBase>(typeof(GameStateEndlessScoreboard))  },
            //{ Command.Pause, new TransitionData<GameStateBase>(typeof(GameStatePauseMenu), true) } // Example of loading a pause menu - Endless game state is set to be preserved. TODO: Consider how best to decide when to destroy a preserved state.
        };
    }

    public override void StartGameState()
    {
        Debug.Log("GameStateEndless: Start Game State ");
        currentSessionData = new GameSessionData();
        PopulateLevelsArray();

        InitialiseClickSounds();


        Vector3 mapPosition = MapSpawner.Instance.GetCurrentMapHolder().transform.position;
        mapPosition += new Vector3(0, 10, 0);
        //GameManager.instance.GetPlayerBall().transform.position = mapPosition; // ballPosition;
        GameManager.instance.GetPlayerBall().SetActive(true);


        energyMetreTunnel = GameManager.instance.gameObject.AddComponent<HexTunnelEnergy>();
        energyMetreTunnel.Initialise(8, 30, 7);
        //energyMetreTunnel.edgeValue = energyMetre.GetEnergyMax() / 6;



        editMode = PlayerPrefs.GetInt("Edit Mode") == 1;
        if (editMode == true) // level edit on start... if the ball does not spawn we go into edit mode because of a helpfull bug thingo :)
        {
            Object.Destroy(Object.FindObjectOfType<PlayerController>().gameObject);
            //scoreUI.totalScoreText.text = "EDIT MODE";
            //scoreUI.totalScoreText.color = Color.red;

        }
        else
        {

            playerController = GameManager.instance.GetPlayerBall().GetComponent<PlayerController>();
        }

        LoadNextLevel();

    }

    public override void CleanupGameState()
    {
        // ...
        
    }

    IEnumerator EndGame()
    {
        //DROP RINGS
        //RESET POST PROC (could be handled by state change on manager?)
        //END STATE

        PostProcessingManager.instance.ResetPostProcessor();
        GameManager.instance.ProcessCommand(Command.End);

        energyMetreTunnel.FallDestroy();
        yield return null;
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


    public override void LoadNextLevel()
    {

        ColourManager.instance.ChangePalette();
        
        Level newLevel = useRandomLevels ? levels[Random.Range(0, levels.Length)] : levels[currentSessionData.levelIndex % levels.Length];

        ++currentSessionData.levelIndex;

        currentSessionData.levelScore = 0;
        currentSessionData.passScore = 0;
        //newLevel.hexs.ToList().ForEach(x => currentSessionData.passScore += x.GetHex().destroyPoints);

        if (playerController != null)
        {
            playerController.SetDestination(playerController.transform.position);
            //playerController.moveSpeed = initialPlayerSpeed + ( currentSessionData.levelIndex * (playerSpeedIncreaseLogMultiplyer * Mathf.Log(playerSpeedIncreaseLogBase)));
        }

        MapSpawner.Instance.SpawnHexs(newLevel, playerController.transform.position);

        UpdateScore();
   
    }

    public override void HexDigEvent()
    {
        currentSessionData.levelScore += 1;
        currentSessionData.totalScore += 1;

        energyMetre.AddEnergy(4);

        AddToColourBoost(colourBoostToAddOnDig);

        if (currentSessionData.levelScore >= currentSessionData.passScore)
        {
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
        //levels = Resources.LoadAll<Level>("Levels/Endless") as Level;
        //Debug.Log("Loaded Level count = " + levels.Length);

        levels = LevelLoader.Instance.GetLevelsFrom("Levels/Endless");
    }




    private void UpdateScore ()
    {
        //GameManager.instance.scoreUI.SetScore(currentSessionData.totalScore, currentSessionData.levelScore, currentSessionData.passScore);
    }
}



