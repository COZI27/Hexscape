using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;



/* 
 Basic gamemode... like the og prototype... 
 Has a click energy system to prevent spam clicking

Falling off ends the game

Speed goes up over time to make the game harder... 
 */





public sealed class GameStateEndlessSimple : GameStateBase
{
 
    private TierSpeedLogCurve speedCurve = new TierSpeedLogCurve(2.6f, 4.4f, -3f);

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
    
    public float playerSpeedIncreaseLogMultiplyer = 45f;

    public float playerKillzoneOffset = 40f;


    HexTunnelEnergy energyMetreTunnel;
    PlayerController playerController;

    private float currentClickEnergy;

    float maxClickEnergy = 3f;
    float clickEnergyDrain = 1f;
    float clickEnergyRechargePerSecondBase = 1f;
    float clickEnergyRechargePerSecondPlayerSpeedMod = 0.001f;
    float GetClickEnergyRecharge()
    {
        return clickEnergyRechargePerSecondBase +  (clickEnergyRechargePerSecondPlayerSpeedMod * playerController.moveSpeed );
    }



    #region PostProcessingAttributes
    // NOTE: could be moved to a struct - could permit the manager to update levels on behalf of state?
    float minSaturation = -20;
    float maxSaturation = 20;

    float minBrightness = -20;
    float maxBrightness = 50;
    #endregion


   
    

    public GameStateEndlessSimple()
    {
        InitialiseStateTransitions();
        GameManager.instance.SetIngameHudActive(true);

        currentClickEnergy = maxClickEnergy;
    }

    public override void StateUpdate()
    {
       
        //GameManager.instance.scoreUI.SetFillValue(  energyMetre.GetCurrentEnergyNormalised());
        energyMetreTunnel.SetEnergyFill((float) currentClickEnergy/ (float) maxClickEnergy, (float)currentClickEnergy / (float)maxClickEnergy);

        


        if (playerController.gameObject.transform.position.y < MapSpawner.Instance.GetCurrentMapHolder().transform.position.y - playerKillzoneOffset)
        {

            if ((currentSessionData.levelScore >= currentSessionData.passScore))
            {
                LoadNextLevel();
            }
            else
            {
                Debug.Log("END GAME");
                EndGame();
            }

               
         
        }

      

        //if (((currentSessionData.levelScore < currentSessionData.passScore)) && playerController.gameObject.transform.position.y < MapSpawner.Instance.GetCurrentMapHolder().transform.position.y)
        //{
        //    ColourManager.instance.SetGrayPallet();
        //} else
        //{
        //    ColourManager.instance.SetGrayPallet(false);
        //}



        //   playerController.moveSpeed = /*initialPlayerSpeed +*/ (energyMetre.GetCurrentEnergy() * (playerSpeedIncreaseLogMultiplyer * Mathf.Log(playerSpeedIncreaseLogBase)));
        float nplayerSpeed = /*playerSpeedIncreaseLogBase + */speedCurve.GetY(currentSessionData.levelIndex) * playerSpeedIncreaseLogMultiplyer;
        if (playerController.moveSpeed != nplayerSpeed)
        {
            playerController.moveSpeed = nplayerSpeed;
        }
        //   if (energyMetre.GetCurrentEnergy() <= 0) GameManager.instance.StartCoroutine(EndGame());




        float tempcurrentClickEnergy = currentClickEnergy + GetClickEnergyRecharge() * Time.deltaTime;


        if (tempcurrentClickEnergy >= clickEnergyDrain && currentClickEnergy < clickEnergyDrain)
        {
            SetPostProcesser(true);
            MapSpawner.Instance.EnableMapInteraction();
        }

        currentClickEnergy = tempcurrentClickEnergy;
        currentClickEnergy = Mathf.Clamp(currentClickEnergy, 0, maxClickEnergy);


    }

    float currentColourBoost = 0;
    float colourBoostToAddOnDig = 40;
    float colourBoostDeclineExponent = 0.4f;

    float colourBoostMin = -70;
    float colourBoostMax = 30;

    private void SetPostProcesser(bool charged)
    {

        if (charged)
        {
            PostProcessingManager.instance.ModifyColourGrading(maxSaturation, maxBrightness);
        }
        else
        {
            PostProcessingManager.instance.ModifyColourGrading(minSaturation, minBrightness);
        }


      

      
    }

    private void AddToColourBoost(float amountToAdd)
    {
        currentColourBoost += amountToAdd;
    }

    private void MaxEnergyReachedListener()
    {

        energyMetreTunnel.PlayTierUpEffect();
        ColourManager.instance.ChangePalette();
    }



    protected override void InitialiseStateTransitions()
    {
        stateTransitions = new Dictionary<Command, TransitionData<GameStateBase>>
        {
            { Command.Begin, new TransitionData<GameStateBase>(typeof(GameStateEndlessScoreboard))  },
            { Command.End, new TransitionData<GameStateBase>(typeof(GameStateMenuMain))  },
            //{ Command.Pause, new TransitionData<GameStateBase>(typeof(GameStatePauseMenu), true) } // Example of loading a pause menu - Endless game state is set to be preserved. TODO: Consider how best to decide when to destroy a preserved state.
        };
    }

    public override void StartGameState()
    {
        Debug.Log("GameStateEndlesSimple: Start Game State ");
        currentSessionData = new GameSessionData();
        PopulateLevelsArray();

        InitialiseClickSounds();


        Vector3 mapPosition = MapSpawner.Instance.GetCurrentMapHolder().transform.position;
        mapPosition += new Vector3(0, 10, 0);
        //GameManager.instance.GetPlayerBall().transform.position = mapPosition; // ballPosition;
        GameManager.instance.GetPlayerBall().SetActive(true);


        energyMetreTunnel = GameManager.instance.gameObject.AddComponent<HexTunnelEnergy>();
        energyMetreTunnel.Initialise(8, 30, 9);
        //energyMetreTunnel.edgeValue = energyMetre.GetEnergyMax() / 6;



        editMode = PlayerPrefs.GetInt("Edit Mode") == 1;
        if (editMode == true) // level edit on start... if the ball does not spawn we go into edit mode because of a helpfull bug thingo :)
        {
            Object.Destroy(Object.FindObjectOfType<PlayerController>().gameObject);
        }
        else
        {
            playerController = GameManager.instance.GetPlayerBall().GetComponent<PlayerController>();
        }

        LoadNextLevel();

    }

    public override bool CleanupGameState()
    {
        // ...

        UnityEngine.Object.Destroy(energyMetreTunnel);
        Debug.Log("CLEAN GAME STATE!");

        GameManager.instance.SetIngameHudActive(false);

        return true;
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



    private void InitialiseClickSounds()
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

        currentClickEnergy = maxClickEnergy;

        //ColourManager.instance.ChangePalette();

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
        currentSessionData.levelScore ++;
        currentSessionData.totalScore++;
        
        int scoreToAdd = 1;

        GameManager.instance.scoreUI.ShowScoreIncreaseText(scoreToAdd);

        currentClickEnergy -= clickEnergyDrain;
        currentClickEnergy = Mathf.Clamp(currentClickEnergy, 0, maxClickEnergy);

        AddToColourBoost(colourBoostToAddOnDig);

        if (currentSessionData.levelScore >= currentSessionData.passScore) // Level Cleared
        {
            if (playerController.transform.position.y >= MapSpawner.Instance.grid.transform.position.y)
            {
              //  energyMetre.AddEnergy(energyIncreaseLevelDigAbove);


            } else
            {
               // energyMetre.AddEnergy(energyIncreaseLevelDigBelow);
            }
           
            LoadNextLevel();

        }
        UpdateScore();


        if (currentClickEnergy < clickEnergyDrain)
        {
            SetPostProcesser(false);
            MapSpawner.Instance.DisableMapInteraction();
        }

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




    private void UpdateScore()
    {
        GameManager.instance.scoreUI.SetScore(currentSessionData.totalScore, currentSessionData.levelIndex, true);
    }

    internal class TierSpeedLogCurve
    {

        public TierSpeedLogCurve(float a = 2.7f, float b = 4.5f, float c = -3.1f)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }
        public float a, b, c;

        public float GetY(float x)
        {
            float y = (a * (Mathf.Log(x + b))) + c;

            return (y);
        }
    }
}



