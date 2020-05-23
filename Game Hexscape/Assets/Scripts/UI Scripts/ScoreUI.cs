using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

[System.Serializable]
public class ScoreUI : MonoBehaviour
{


    public Text totalScoreText;
    public Text currentTierText;
    public GameObject scoreIncreaseObj;

    public GameObject fillBarObject;
    private Image fillBar;
    private GameObject upArrowObj, downArrowObj;


    private int numberOfIncreaseObjects = 8;
    private Queue<GameObject> scoreIncreaseObjQueue;

    void Start() {
        if (fillBarObject != null) fillBar = fillBarObject.GetComponent<Image>();

        upArrowObj = transform.Find("MultiUpArrow").gameObject;
        downArrowObj = transform.Find("MultiDownArrow").gameObject;

        //
        if (scoreIncreaseObj != null)
        {
            scoreIncreaseObjQueue = new Queue<GameObject>();
            scoreIncreaseObjQueue.Enqueue(scoreIncreaseObj);
            for (int i = 0; i < numberOfIncreaseObjects - 1; i++)
            {
                GameObject newObj = GameObject.Instantiate(scoreIncreaseObj);
  
                newObj.transform.parent = scoreIncreaseObj.transform.parent;
                newObj.transform.localPosition = scoreIncreaseObj.transform.localPosition;
                newObj.transform.rotation = scoreIncreaseObj.transform.rotation;
                newObj.transform.localScale = scoreIncreaseObj.transform.localScale;
                scoreIncreaseObjQueue.Enqueue(newObj);
            }
        }
        //
    }



    // Fill value should be between 0 and 1
    //public void SetFillValue(float val)
    //{
    //    if (fillBar != null)
    //    {
    //        Debug.Log("fillBar not null. Val  = " + val);
    //        fillBar.fillAmount = val;
    //    }
    //}


    public void SetScore(int totalScore)
    {
        this.totalScoreText.text = totalScore.ToString();
    }
    public void SetScore(int totalScore, int currentTier)
    {
        this.totalScoreText.text = totalScore.ToString("D8");
        this.currentTierText.text = "x" + currentTier.ToString();
    }
    public void SetTier(int currentTier)
    {
        this.currentTierText.text = "x"+ currentTier.ToString();
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

    public void FlickerMultiplierArrow(bool upArrow, int framesPerFlicker = 3, float duraton = 3.0f)
    {
        StartCoroutine(FlickerMultiArrow(upArrow, framesPerFlicker, duraton));
    }

    private IEnumerator FlickerMultiArrow(bool upArrow, int framesPerFlicker, float duration)
    {

        int frameCount = framesPerFlicker;

        while (duration > 0)
        {
            duration -= Time.deltaTime;
            frameCount--;
            if (frameCount == 0)
            {
                frameCount = framesPerFlicker;

                
                if (upArrow && upArrowObj != null)
                    upArrowObj.SetActive(!upArrowObj.activeInHierarchy);
                if (!upArrow && downArrowObj != null)
                    downArrowObj.SetActive(!downArrowObj.activeInHierarchy);
            }
            yield return null;
        }
        if (upArrow && upArrowObj != null)
            upArrowObj.SetActive(false);
        if (!upArrow && downArrowObj != null)
            downArrowObj.SetActive(false);
    }


    public void ShowScoreIncreaseText(int score)
    {
        //Debug.Log("ShowScoreIncreaseText");
        if (scoreIncreaseObjQueue.Count > 0)
        {
            GameObject obj = scoreIncreaseObjQueue.Dequeue();
            StartCoroutine(PlayScoreIncrementEffect(obj, score));
        }
        //else delay?
    }

    IEnumerator PlayScoreIncrementEffect(GameObject obj, int score, float duration = 2.0f)
    {
        obj.SetActive(true);
        RectTransform r = obj.GetComponent<RectTransform>();
        Text t = obj.GetComponent<Text>();
        t.text = score.ToString();
        Color textCol = t.color;

        Vector3 resetPos = r.localPosition;
        Vector3 targetPosiVal = resetPos + new Vector3(0,10,0);


        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            textCol.a = Mathf.Lerp(textCol.a, 0, (elapsedTime / duration));
            t.color = textCol;
            r.localPosition = Vector3.Lerp(r.localPosition, targetPosiVal, (elapsedTime / duration));
            yield return null;
        }


        obj.SetActive(false);
        r.localPosition = resetPos;
        textCol.a = 1;
        t.color = textCol;
        scoreIncreaseObjQueue.Enqueue(obj);
        yield return null;
    }
}
