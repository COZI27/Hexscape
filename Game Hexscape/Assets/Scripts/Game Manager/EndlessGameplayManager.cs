using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;



public class EndlessGameplayManager : MonoBehaviour
{

    // bit messy at the moment, it is meant to control almost everything... the gameplay manager at the moment is mostly used for dealing with scores.
    #region singelton Junk
    public static EndlessGameplayManager instance;
    private void MakeSingleton()
    {
        if (instance == null)
        {
            instance = this;
            //  DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion



    [Space(3f)]
    [Header("Set Components:")]
    [HideInInspector] public HexColourLerp colourLerper;
    private AudioSource source;
    private RippleManager rippleManager;

    [Space(5f)]
    [Header("Levels:")]

    [SerializeField] public Level[] levels;

    [Space(5f)]
    [Header("Options:")]

    [Space(1f)]
    [Header("   - Map Options:")]
    public bool useRandomLevels = true;
    private bool editMode = false;

    [Space(3f)]
    [Header("   - Player Options:")]
    public float initialPlayerSpeed = 30f;
    public float playerSpeedIncreaseLogBase = 2f;
    public float playerSpeedIncreaseLogMultiplyer = 10f;



    [Space(5f)]
    [Header("Score UI:")]
    public ScoreUI scoreUI;


    [Space(5f)]
    [Header("Audio Clips:")]
    [SerializeField] private AudioClip digSound;
    [SerializeField] private AudioClip levelUpSound;
    [SerializeField] private AudioClip groundThudSound;







    // Level Compleation Information
    [HideInInspector] public int levelCurrentScore = 0;
    private int passScore = 0;

    private int totalScore = 0;
    public int levelIndex = 0;


    // refrence to the player, dah!
    [HideInInspector] private PlayerController player;


    private void Awake()
    {
        MakeSingleton();

        source = GetComponent<AudioSource>();
        rippleManager = GetComponent<RippleManager>();
        colourLerper = GetComponent<HexColourLerp>();

    }

    private void Start()
    {
        editMode = PlayerPrefs.GetInt("Edit Mode") == 1;


        if (editMode == true) // level edit on start... if the ball does not spawn we go into edit mode because of a helpfull bug thingo :)
        {
            Destroy(FindObjectOfType<PlayerController>().gameObject);
            scoreUI.totalScoreText.text = "EDIT MODE";
            scoreUI.totalScoreText.color = Color.red;

        }
        else
        {
            player = FindObjectOfType<PlayerController>();
            player.moveSpeed = initialPlayerSpeed;
        }

        NextLevel();
    }

    public void PlaySound(AudioClip sound, float pitch)
    {
        source.pitch = pitch;
        source.PlayOneShot(sound);
    }

    //public void NextLevelOld()
    //{
    //    levelIndex++;
    //    levelCurrentScore = 0;

    //    Level newLevel = levels[levelIndex];

    //    scoreUI.SetLevel(levelIndex);
    //    scoreUI.SetMedalState(MedalState.noMedal);
    //    scoreUI.SetScore(totalScore, levelCurrentScore, newLevel);

    //    //MapSpawner.instance.SpawnHexs(levelIndex);
    //}


    public void NextLevel()
    {

        colourLerper.DisabledColour();



        // increase the players speed
        if (player != null) player.moveSpeed = initialPlayerSpeed + (playerSpeedIncreaseLogMultiplyer * levelIndex * Mathf.Log(playerSpeedIncreaseLogBase));

        //Debug.Log(playerSpeed);

        levelIndex++;

        if (useRandomLevels)
        {

        }

        if (!editMode)
        {

            levelCurrentScore = 0;
        }


        // this is where we pull the level from... i will have to create a level generator that returns a level

        Level newLevel;



        if (useRandomLevels && !editMode)
        {
            newLevel = levels[Random.Range(0, levels.Length)];
        }
        else
        {
            newLevel = levels[levelIndex];

        }



        // for infite the score required to pass we need to add togheter all of the hexs point rewards VVV
        passScore = 0;
        newLevel.hexs.ToList().ForEach(x => passScore += x.GetHex().destroyPoints);
        //newLevel.hexs.ToList().ForEach(x => Debug.Log( x.GetHex().destroyPoints));

        // Debug.Log(passScore);

        if (!editMode)
        {
            scoreUI.SetScore(totalScore, levelCurrentScore, passScore);
        }
        scoreUI.SetLevel(levelIndex);
        scoreUI.SetMedalState(MedalState.noMedal);


        if (!editMode) {
            player.SetDestination(player.transform.position);
            MapSpawner.instance.SpawnHexs(newLevel, player.transform.position);
        } 
       

        if (levelIndex > 0)
        {
            PlaySound(levelUpSound, 1);
        }
    }

    public void PlayGroundThud()
    {
        PlaySound(groundThudSound, 1);
        rippleManager.CreateRippleThud(player.transform.position, 5f, 100);


    }

    public void GainHexDigPoints(int points)
    {


        //if (levelIndex < 1)
        //{
        //    NextLevel();
        //    return;

        //}

        totalScore += points;
        levelCurrentScore += points;

        // Level currentLevel = levels[levelIndex];


        scoreUI.SetScore(totalScore, levelCurrentScore, passScore);

        // Debug.Log("Current Level: " + levelCurrentScore + ", Pass Score: " + passScore);
        if (levelCurrentScore >= passScore)
        {
            NextLevel();
        }
        else
        {
            PlaySound(digSound, 1);
        }



    }
    public void GainHexDigPointsOld(int points)
    {
        totalScore += points;
        levelCurrentScore += points;

        Level currentLevel = levels[levelIndex];


        scoreUI.SetScore(totalScore, levelCurrentScore, currentLevel);

        /// we need to save the medal state for each level but for the time being lest just use the UI

        if (levelCurrentScore >= currentLevel.goldAmount)
        {
            scoreUI.SetMedalState(MedalState.gold);
        }
        else if (levelCurrentScore >= currentLevel.silverAmount)
        {
            scoreUI.SetMedalState(MedalState.silver);
        }
        else if (levelCurrentScore >= currentLevel.bronzeAmount)
        {
            scoreUI.SetMedalState(MedalState.bronze);
        }
        else if (levelCurrentScore >= currentLevel.passAmount)
        {
            scoreUI.SetMedalState(MedalState.passMedal);
        }
        else
        {
            scoreUI.SetMedalState(MedalState.noMedal);
        }

    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            levelIndex += 1;
            levelIndex = Mathf.Clamp(levelIndex, 0, levels.Length - 1);
            scoreUI.SetLevel(levelIndex);

            //if (editMode)
            //{
            //    MapSpawner.instance.LoadLevel();
            //}

        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            levelIndex -= 1;
            levelIndex = Mathf.Clamp(levelIndex, 0, levels.Length - 1);

            scoreUI.SetLevel(levelIndex);


            //if (editMode)
            //{
            //    MapSpawner.instance.LoadLevel();
            //}
        }


        if (Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            editMode = !editMode;

            if (editMode)
            {
                PlayerPrefs.SetInt("Edit Mode", 1);
            }
            else
            {
                PlayerPrefs.SetInt("Edit Mode", 0);
            }


        }

    }
}



[System.Serializable]
public class ScoreUI
{
    public Text totalScoreText;
    public Text currentLevelText;

    public Text levelScoreText;

    public Image passMedal, bronzeMedal, silverMedal, goldMedal;
    public Image passFill, bronzeFill, silverFill, goldFill;


    public void SetScore(int totalScore, int currentLevelScore, int passScore)
    {

        this.totalScoreText.text = totalScore.ToString();

        this.levelScoreText.text = currentLevelScore + "/" + passScore;
        this.passFill.fillAmount = (float)currentLevelScore / (float)passScore;

        this.bronzeFill.fillAmount = 0;
        this.silverFill.fillAmount = 0;
        this.goldFill.fillAmount = 0;

    }



    public void SetScore(int totalScore, int currentLevelScore, Level level)
    {
        this.totalScoreText.text = totalScore.ToString();
        this.levelScoreText.text = currentLevelScore + "/" + level.passAmount;


        // pass/current
        // bronze + pass / current 
        // silver bronze + pass / current
        this.passFill.fillAmount = (float)currentLevelScore / level.passAmount;




        this.bronzeFill.fillAmount = ((float)currentLevelScore - (float)level.passAmount) / ((float)level.bronzeAmount - (float)level.passAmount);
        this.silverFill.fillAmount = ((float)currentLevelScore - (float)level.bronzeAmount) / ((float)level.silverAmount - (float)level.bronzeAmount);
        this.goldFill.fillAmount = ((float)currentLevelScore - (float)level.silverAmount) / ((float)level.goldAmount - (float)level.silverAmount);


    }



    public void SetScore(int totalScore, int currentLevelScore, int passScore, int bronzePass, int silverPass, int goldPass)
    {
        this.totalScoreText.text = totalScore.ToString();

        this.levelScoreText.text = currentLevelScore + "/" + passScore;
        this.passFill.fillAmount = (float)currentLevelScore / (float)passScore;

    }

    public void SetLevel(int level)
    {
        currentLevelText.text = "Lv" + level;
    }

    public void SetMedalState(MedalState medalState)
    {
        if (medalState == MedalState.noMedal)
        {
            passMedal.gameObject.SetActive(false);
            bronzeMedal.gameObject.SetActive(false);
            silverMedal.gameObject.SetActive(false);
            goldMedal.gameObject.SetActive(false);
        }
        else if (medalState == MedalState.passMedal)
        {
            passMedal.gameObject.SetActive(true);
            bronzeMedal.gameObject.SetActive(false);
            silverMedal.gameObject.SetActive(false);
            goldMedal.gameObject.SetActive(false);
        }
        else if (medalState == MedalState.bronze)
        {
            passMedal.gameObject.SetActive(true);
            bronzeMedal.gameObject.SetActive(true);
            silverMedal.gameObject.SetActive(false);
            goldMedal.gameObject.SetActive(false);
        }
        else if (medalState == MedalState.silver)
        {
            passMedal.gameObject.SetActive(true);
            bronzeMedal.gameObject.SetActive(true);
            silverMedal.gameObject.SetActive(true);
            goldMedal.gameObject.SetActive(false);
        }
        else if (medalState == MedalState.gold)
        {
            passMedal.gameObject.SetActive(true);
            bronzeMedal.gameObject.SetActive(true);
            silverMedal.gameObject.SetActive(true);
            goldMedal.gameObject.SetActive(true);
        }

    }



}

public enum MedalState
{
    noMedal,
    passMedal,
    bronze,
    silver,
    gold,
}