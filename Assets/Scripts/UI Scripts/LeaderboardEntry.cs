using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;



public class LeaderboardEntry : MonoBehaviour
{
    public LeaderboardEntry()
    {

    }

    private void Start()
    {       
        StartCoroutine(RetrieveEntry());
    }

    [SerializeField]
    Text rank = null, score = null, level = null, uName = null;

    Leaderboard.EntryData data;

    //bool hasTretrievedData = false;

    [SerializeField]
    private int index;

    private IEnumerator RetrieveEntry()
    {
        // yield so the object's transform can be modified by the parent before the index is established
        yield return new WaitForEndOfFrame();

        RectTransform rect = this.gameObject.GetComponent<RectTransform>();
        if (rect != null)
        {
            index = rect.GetSiblingIndex();

            Leaderboard.LeaderboardDisplay leaderboard = transform.root.GetComponentInChildren<Leaderboard.LeaderboardDisplay>();
            if (leaderboard != null)
            {
                data = leaderboard.RetrieveEntryData(index);
                if (data == null)
                {
                    //leaderboard.NotifyEntryDestroy(index);
                    Destroy(this.gameObject);

                    //yield and make another request?

                    //if (index > 0) // Leave the first index in play for instantiating copies from
                    //{

                    //   Destroy(this.gameObject); // Temp solution for removing unused entries. 
                    //}
                }
                else
                {

                    ApplyData();
                }
            }

        }
        yield return null;
    }

    private void OnDestroy()
    {
        RectTransform rect = this.gameObject.GetComponent<RectTransform>();
        if (rect != null)
        {
            index = rect.GetSiblingIndex();
            Leaderboard.LeaderboardDisplay leaderboard = transform.root.GetComponentInChildren<Leaderboard.LeaderboardDisplay>();
            if (leaderboard != null)
            {
                leaderboard.NotifyEntryDestroy(index);
            }
        }

    }


    private void ApplyData()
    {
        if (data == null)
        {
            if (rank != null) rank.text = "";
            if (score != null) score.text = "";
            if (level != null) level.text = "";
            if (name != null) uName.text = "";        
        }
        else
        {
            if (rank != null) rank.text = data.playerId.ToString();
            if (score != null) score.text = data.highScore.ToString();
            if (level != null) level.text = data.highLevel.ToString();
            if (name != null) uName.text = data.playerName.ToString();

            rank.enabled = true;
            score.enabled = true;
            level.enabled = true;
            uName.enabled = true;
        }

    }
}
