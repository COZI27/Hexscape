using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameModeEndless : GameModeBase
{




    [SerializeField] public Level[] levels;

    [Space(5f)]
    [Header("Options:")]
    [Space(1f)]
    [Header("   - Map Options:")]
    public bool useRandomLevels = true;

    [Space(3f)]
    [Header("   - Player Options:")]
    public float initialPlayerSpeed = 30f;
    public float playerSpeedIncreaseLogBase = 2f;
    public float playerSpeedIncreaseLogMultiplyer = 10f;


    private PlayerController player;


    void Start()
    {

    }

    void Update()
    {

    }

    public override void StartGameMode()
    {
        currentSessionData = new GameSessionData();


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

        currentSessionData.passScore = 0;
        newLevel.hexs.ToList().ForEach(x => currentSessionData.passScore += x.GetHex().destroyPoints);

        //scoreUI.ModifyValues(currentSessionData);
        //scoreUI.SetMedalState(MedalState.noMedal);

        player.SetDestination(player.transform.position);
        MapSpawner.instance.SpawnHexs(newLevel, player.transform.position);
    }

    public override void HexDigEvent()
    {
        throw new System.NotImplementedException();
    }

    public override void PlayGroundThud()
    {
        throw new System.NotImplementedException();
    }
}

