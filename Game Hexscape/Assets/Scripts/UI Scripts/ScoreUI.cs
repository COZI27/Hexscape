using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class ScoreUI : MonoBehaviour
{


    public Text totalScoreText;
    public Text currentLevelText;

    //public Text levelScoreText;

    //public Image passMedal, bronzeMedal, silverMedal, goldMedal;
    //public Image passFill, bronzeFill, silverFill, goldFill;

    public GameObject fillBarObject;
    private Image fillBar;

    void Start() {
        if (fillBarObject != null) fillBar = fillBarObject.GetComponent<Image>();
    }



    public void SetScore(int totalScore, int currentLevelScore, int passScore)
    {

        //this.totalScoreText.text = totalScore.ToString();

        //this.levelScoreText.text = currentLevelScore + "/" + passScore;
        //this.passFill.fillAmount = (float)currentLevelScore / (float)passScore;

        //this.bronzeFill.fillAmount = 0;
        //this.silverFill.fillAmount = 0;
        //this.goldFill.fillAmount = 0;

    }

    // Fill value should be between 0 and 1
    public void SetFillValue(float val)
    {
        if (fillBar != null)
        {
            Debug.Log("fillBar not null. Val  = " + val);
            fillBar.fillAmount = val;
        }
    }



    public void SetScore(int totalScore, int currentLevelScore, Level level)
    {
        //this.totalScoreText.text = totalScore.ToString();
    }



    public void SetScore(int totalScore, int currentLevelScore, int passScore, int bronzePass, int silverPass, int goldPass)
    {
        //this.totalScoreText.text = totalScore.ToString();

        //this.levelScoreText.text = currentLevelScore + "/" + passScore;
        //this.passFill.fillAmount = (float)currentLevelScore / (float)passScore;

    }

    public void SetLevel(int level)
    {
        //currentLevelText.text = "Lv" + level;
    }

    //public void SetMedalState(MedalState medalState)
    //{
        //if (medalState == MedalState.noMedal)
        //{
        //    passMedal.gameObject.SetActive(false);
        //    bronzeMedal.gameObject.SetActive(false);
        //    silverMedal.gameObject.SetActive(false);
        //    goldMedal.gameObject.SetActive(false);
        //}
        //else if (medalState == MedalState.passMedal)
        //{
        //    passMedal.gameObject.SetActive(true);
        //    bronzeMedal.gameObject.SetActive(false);
        //    silverMedal.gameObject.SetActive(false);
        //    goldMedal.gameObject.SetActive(false);
        //}
        //else if (medalState == MedalState.bronze)
        //{
        //    passMedal.gameObject.SetActive(true);
        //    bronzeMedal.gameObject.SetActive(true);
        //    silverMedal.gameObject.SetActive(false);
        //    goldMedal.gameObject.SetActive(false);
        //}
        //else if (medalState == MedalState.silver)
        //{
        //    passMedal.gameObject.SetActive(true);
        //    bronzeMedal.gameObject.SetActive(true);
        //    silverMedal.gameObject.SetActive(true);
        //    goldMedal.gameObject.SetActive(false);
        //}
        //else if (medalState == MedalState.gold)
        //{
        //    passMedal.gameObject.SetActive(true);
        //    bronzeMedal.gameObject.SetActive(true);
        //    silverMedal.gameObject.SetActive(true);
        //    goldMedal.gameObject.SetActive(true);
        //}

    //}



}

//public enum MedalState
//{
//    noMedal,
//    passMedal,
//    bronze,
//    silver,
//    gold,
//}
