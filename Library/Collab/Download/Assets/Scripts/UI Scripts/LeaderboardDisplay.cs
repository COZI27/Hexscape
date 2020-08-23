using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;

namespace Leaderboard
{
    // The Data to be stored and passed onto the LeaderboardEntry object for display
    public class EntryData
    {
        public EntryData(int id = -1, int level = -1, int score = 000000, string name = "name")
        {
            playerId = id;
            highLevel = level;
            highScore = score;
            playerName = name;
        }

        public int playerId;
        public int highLevel;
        public int highScore;
        public string playerName;

        //NOTE: Could have additional field denoting the nature of the entry (ie currentplayer, friend, otherPlayer)
        // This could be used to apply colour or highlighting to entry

    }

    public class LeaderboardDisplay : MonoBehaviour
    {
        [SerializeField]
        GameObject scoreObjectTemplate;
 
        IEnumerator waitRoutine;
        EntryData[] scoreBoardEntries;
        EntryData currentPlayerEntry;

        private float offset = 0.3f;

        private bool downloadRequestInProgress = false;

        bool isDebugMode = false;


        int pageLength = 30; // TODO: Link value with database (could be initial returned length?)
        int pageOffset = 0;

        int databaseEndIndex = 999999; // The index of the final entry on the server - prevents further attempts to download new scores 

        int lowIndex = 1, highIndex; // NOTE: low index start index: 0 or 1?
        // low: increase when child 0 destroyed, decreased when child 0 added
        // high: increase when child N added. decreased when child N destoyed




        //int scoreOffset;
        int scoreOffsetMultiplier = 30;

        //private void UpdateEntry()
        //{
        //    int newIndex = entryObjectQueue.Count;
        //    GameObject entryToMove = entryObjectQueue.Dequeue();
        //    entryToMove.transform.SetSiblingIndex(newIndex);
        //    entryObjectQueue.Enqueue(entryToMove);

        //    // move entry to bottom
        //    // update entry data from array
        //    // Also handle reversing

        //    //ReverseQueueUtil<GameObject>.ReverseQueue(ref entryObjectQueue);

        //    //https://github.com/ivomarel/InfinityScroll/blob/master/Scripts/InfiniteScroll.cs

        //    //https://forum.unity.com/threads/infinite-scroll-with-scrollrect.265345/
        //}

        private void OnEnable()
        {
            Init();
        }

        void Init()
        {
            lowIndex = 1;
            highIndex = 0;
            pageOffset = 0;
            downloadRequestInProgress = false;
            scoreBoardEntries = new EntryData[0];
        }


        // Start is called before the first frame update
        void Start()
        {
            //if (scoreObjectTemplate == null) scoreObjectTemplate = transform.Find("ScoreEntry").gameObject;
            //entryObjectQueue = new Queue<GameObject>();

            //listScrollComponent = GetComponentInChildren<ScrollRect>();
            //listRectTransform = listScrollComponent.gameObject.GetComponent<RectTransform>();
        }


        //private void UpdateEntries()
        //{
        //    if (!Application.isPlaying/* || !init*/)
        //        return;

        //    if (GetDimension(listScrollComponent.content.sizeDelta) - (GetDimension(listScrollComponent.content.localPosition) * OneOrMinusOne()) < GetDimension(listRectTransform.sizeDelta))
        //    {
        //        NewItemAtEnd();
        //        //margin is used to Destroy objects. We add them at half the margin (if we do it at full margin, we continuously add and delete objects)
        //    }
        //    else if (GetDimension(listScrollComponent.content.localPosition) * OneOrMinusOne() < (GetDimension(listRectTransform.sizeDelta) * 0.5f))
        //    {
        //        NewItemAtStart();
        //        //Using else because when items get added, sometimes the properties in UnityGUI are only updated at the end of the frame.
        //        //Only Destroy objects if nothing new was added (also nice performance saver while scrolling fast).
        //    }
        //    else
        //    {
        //        //Looping through all items.
        //        foreach (RectTransform child in listScrollComponent.content)
        //        {
        //            //Our prefabs are inactive
        //            if (!child.gameObject.activeSelf)
        //                continue;
        //            //We Destroy an item from the end if it's too far
        //            if (GetPos(child) > GetDimension(listRectTransform.sizeDelta))
        //            {
        //                Destroy(child.gameObject);
        //                //We update the container position, since after we delete something from the top, the container moves all of it's content up
        //                listScrollComponent.content.localPosition -= (Vector3)GetVector(GetSize(child));
        //                dragOffset -= GetVector(GetSize(child));
        //                Add(ref itemTypeStart);
        //            }
        //            else if (GetPos(child) < -(GetDimension(t.sizeDelta) + GetSize(child)))
        //            {
        //                Destroy(child.gameObject);
        //                Subtract(ref itemTypeEnd);
        //            }
        //        }
        //    }

        //}

        //Scrolls the score display up(+) and down(-) depending on magnitude
        public void ScrollBoard(float magnitude)
        {
            //https://www.youtube.com/watch?v=lUun2xW6FJ4


            //ScrollRect listScrollComponent = GetComponentInChildren<ScrollRect>();
           // listScrollComponent.verticalNormalizedPosition = magnitude; // value range (0 to 1)

        }

        public void DownloadScoreData()
        {
            MakeScoreDownloadRequest(0);
        }

        private void MakeScoreDownloadRequest(int offset)
        {
            if (downloadRequestInProgress == true) return;
            downloadRequestInProgress = true;

            if(isDebugMode)
            {
                StartCoroutine(GeneratePseudoEntries(offset));
                return;
            }

            DownloadScore scoreDownloader = new DownloadScore();
            if (scoreDownloader != null)
            {
                //coreBoardEntries = new EntryData[0]; // NOTE: PROBLEM may be here - should Callback handle this?
                                                      //currentPlayerEntry = null;

                //Debug.Log("GetPlayerIDasInt = " + GameManager.instance.loadedProfile.GetPlayerIDasInt());

                //scoreDownloader.GetScoreForUser(GameManager.instance.loadedProfile.GetPlayerIDasInt(), CallbackUserScore); // TODO: Remove this and replace with index check on downloaded scores?

                if (offset > 0) // Download Next Page
                {
                    Debug.Log(" (offset > 0) MakeScoreDownloadRequest : offset = " + offset);
                    scoreDownloader.GetScoresForScoreboard(GameManager.instance.loadedProfile.GetPlayerIDasInt(), pageOffset * pageLength, pageLength, CallbackHighScoresNext);
                }
                else if (offset < 0) // DownloadScore Previous Page
                {
                    Debug.Log(" (offset < 0) MakeScoreDownloadRequest : offset = " + offset);
                    scoreDownloader.GetScoresForScoreboard(GameManager.instance.loadedProfile.GetPlayerIDasInt(), (pageOffset - 3) * pageLength, pageLength, CallbackHighScoresPrevious);

                    //  int index = (offset >= 0 ? pageLength * pageOffset + i : pageLength * (pageOffset -3) + i); TAKEN FROM GENERATE PSEUDO
                }
                else if (offset == 0)
                {
                    Debug.Log(" offset == 0) MakeScoreDownloadRequest : offset = " + offset);
                    // if (newScores.Length == pageLength + pageLength) pageOffset++;

                    PoolSrollRect scrollRect = GetComponentInChildren<PoolSrollRect>();
                    if (scrollRect != null)
                    {
                        scrollRect.Init(); // Note - could be moved
                    }

                    scoreDownloader.GetScoresForScoreboard(GameManager.instance.loadedProfile.GetPlayerIDasInt(), pageOffset * pageLength, pageLength + pageLength, CallbackHighScoresNext);
                }


                // Store the coroutine so it can be retrieved in the event of a timeout
                waitRoutine = WaitForDownloadComplete();
                GameManager.instance.StartCoroutine(waitRoutine);
            }
        }


        IEnumerator WaitForDownloadComplete()             //Deprecated?
        {


            bool dataReturned = false;
            while (!dataReturned)
            {
                if (scoreBoardEntries!= null && scoreBoardEntries.Length > 0)
                    if (currentPlayerEntry != null)
                        dataReturned = true;
                    else yield return null;
                else yield return null;
            }

            //Debug.Log("Data Retrieved:");
            //Debug.Log(currentPlayerEntry);
            //Debug.Log(scoreBoardEntries.Length);

            //bool playerIndexFound = false;
            //for (int i = 0; i < scoreBoardEntries.Length; i++)
            //{
            //    if (scoreBoardEntries[i].playerId == currentPlayerEntry.playerId)
            //    {
            //        playerScoreEntryIndex = i;
            //        playerIndexFound = true;
            //        break;
            //    }
            //}
            //if (!playerIndexFound) // replace the lowest scored entry in the array with the current player's entry
            //{
            //    scoreBoardEntries[scoreBoardEntries.Length - 1] = currentPlayerEntry;
            //    playerScoreEntryIndex = scoreBoardEntries.Length - 1;
            //}

            ////PoolSrollRect scrollRect = GetComponentInChildren<PoolSrollRect>();
            ////if (scrollRect != null)
            ////{
            ////    scrollRect.Init();
            ////}
        }


        private void DisplayErrorMessage()
        {
            // Error retrieving score data.
            Debug.LogWarning("Error retrieving score data.");
        }

        public void CallbackUserScore(EntryData data)
        {
            currentPlayerEntry = data;
        }

        public void CallbackHighScoresPrevious(EntryData[] data, DownloadScore.EDownloadStatus status)
        {
            // New Data added to start of array
            Debug.Log("CallbackHighScoresPrevious. Data length = " + data.Length);
            pageOffset--;

            EntryData[] oldData = scoreBoardEntries.Take((scoreBoardEntries.Length + 1) / 2).ToArray();


            string str = "";

            str += "OldData: ";
            foreach (EntryData n in oldData)
            {
                str += n.playerId;
                str += ",";
            }
            Debug.Log(str);
            str = "";

            str += "newData: ";
            foreach (EntryData n in data)
            {
                str += n.playerId;
                str += ",";
            }
            Debug.Log(str);
            str = "";

            EntryData[] returnData = new EntryData[oldData.Length + data.Length];
            data.CopyTo(returnData, 0);
            oldData.CopyTo(returnData, data.Length);
            scoreBoardEntries = returnData;



            str += "scoreBoardEntries: ";
            foreach (EntryData n in scoreBoardEntries)
            {
                str += n.playerId;
                str += ",";
            }
            Debug.Log(str);
            str = "";

            downloadRequestInProgress = false;
        }

        public void CallbackHighScoresNext(EntryData[] data, DownloadScore.EDownloadStatus status)
        {
            // New data added to end of array

            if (data == null)
            {
                //Do Something here? Prevent further downloads at end?
                if (status == DownloadScore.EDownloadStatus.EndOfDatabase)
                {
                    //highIndex--; // TEMP

                    databaseEndIndex = highIndex;
                }

                return;
            }




            string str = "";

            pageOffset++;

            EntryData[] oldData;

            if (scoreBoardEntries == null) scoreBoardEntries = new EntryData[0];

            if (scoreBoardEntries.Length >= pageLength * 2)
            {
                oldData = scoreBoardEntries.Skip((scoreBoardEntries.Length + 1) / 2).ToArray();
            }
            else
            {
                oldData = scoreBoardEntries;
            }


            str += "OldData: ";
            foreach (EntryData n in oldData)
            {
                str += n.playerId;
                str += ",";
            }
            Debug.Log(str);
            str = "";

            str += "newData: ";
            foreach (EntryData n in data)
            {
                str += n.playerId;
                str += ",";
            }
            Debug.Log(str);
            str = "";



            EntryData[] returnData = new EntryData[oldData.Length + data.Length];
            oldData.CopyTo(returnData, 0);
            data.CopyTo(returnData, oldData.Length);
            scoreBoardEntries = returnData;




            str += "scoreBoardEntries: ";
            foreach (EntryData n in scoreBoardEntries)
            {
                str += n.playerId;
                str += ",";
            }
            Debug.Log(str);
            str = "";


            downloadRequestInProgress = false;
        }

        //private void GenerateHighScoreDisplay()
        //{
        //    GameObject entryPrefab = scoreObjectTemplate;
        //    Transform parentTransform = scoreObjectTemplate.transform.parent;
        //    //entryPrefab.SetActive(false);
        //    GameObject[] entries = new GameObject[scoreBoardEntries.Length];

        //    // if current user score not in top ten, then replace entry 10 with user score  (could handle this in php?)

        //    float currOffset = 0;

        //    for (int i = 0; i < entries.Length; i++)
        //    {
        //        entries[i] = GameObject.Instantiate(entryPrefab, parentTransform);
        //        entryObjectQueue.Enqueue(entries[i]);

        //        //entries[i].transform.parent = scoreBoardCanvas.transform;
        //        // entries[i].transform.position -= new Vector3(0, currOffset, 0);
        //        Text[] textComponents = entries[i].GetComponentsInChildren<Text>();
        //        textComponents[0].text = scoreBoardEntries[i].playerId.ToString();
        //        textComponents[1].text = scoreBoardEntries[i].highScore.ToString();
        //        textComponents[2].text = scoreBoardEntries[i].highLevel.ToString();
        //        textComponents[3].text = scoreBoardEntries[i].playerName.ToString();

        //        if (i == playerScoreEntryIndex)
        //            foreach (Text t in textComponents)
        //                t.color = Color.green;

        //        entries[i].name = entries[i].name + i.ToString();
        //        entries[i].SetActive(true);

        //        currOffset += offset;
        //    }
        //    //entries[0].SetActive(false);
        //    entryPrefab.SetActive(false);
        //}



            
        //public EntryData RetrieveEntryData( isStart)
        //{
        //    if (scoreBoardEntries != null)
        //    {
        //        GameObject groupObj = transform.GetComponentInChildren<VerticalLayoutGroup>().gameObject;
        //        if (groupObj != null)
        //        {
        //            int noOfChildren = groupObj.transform.childCount;
        //        }

        //        return scoreBoardEntries[0];
        //    }
        //    else return null;
        //}


        public EntryData RetrieveEntryData(int siblingIndex)
        {
            if (scoreBoardEntries != null && scoreBoardEntries.Length > 0)
            {

                if (siblingIndex == 0)
                    --lowIndex;
                else
                    ++highIndex;


                if (siblingIndex == 0)
                {
                    if (lowIndex < 0)  // Top of entries reached
                    {
                       // Reset lowindex and return nulll entry 
                        Debug.Log("Top Of Board - 1");
                        //lowIndex = 0;

                        return null;

                        //NOTE: If starting scoreboard from anywher eother than top - then the low entry value will need setting to match page
                    }
                    else if (lowIndex == 0)
                    {
                        Debug.Log("Top Of Board");
                        return scoreBoardEntries[lowIndex];
                    }
                    else
                    {

                        if (lowIndex >= ((pageOffset - 2) * pageLength))
                        {
                            // TODO: Condition to see if score has been downloaded
                            int indexToGet = (lowIndex - ((pageOffset - 2) * pageLength));


                            Debug.Log("indexToGet: " + indexToGet + " = "+ " pageOffset: " + pageOffset + ". lowIndex" + lowIndex + ". ScoreBoardLength:" + scoreBoardEntries.Length);
                            return scoreBoardEntries[indexToGet];
                        }
                        else
                        {
                            //++lowIndex; // Moved to OnDestroyNotify
                            MakeScoreDownloadRequest(-1);
                            return null;
                        }

                    }
                }
                else
                {
                    if (highIndex >= databaseEndIndex)
                    {
                        return null;
                    }
                    else if (highIndex >= ((pageOffset) * pageLength))
                    {
                        //End of entries downloaded reached ...

                        // download next score page
                        //highIndex--; // Moved to OnDestroyNotify
                        MakeScoreDownloadRequest(1);
                        return null;
                    }
                    else
                    {
                        #region TEMP
                        //if (highIndex < ((pageOffset + 1) * pageLength) && highIndex >= ((pageOffset) * pageLength))
                        //{
                        //    Debug.LogWarning("if");
                        //    indexToGet = (highIndex - ((pageOffset) * pageLength)); // highIndex - 60
                        //}
                        //else if (highIndex < ((pageOffset) * pageLength) && highIndex >= ((pageOffset - 1) * pageLength))
                        //{
                        //    Debug.Log("else if ");
                        //    indexToGet = (highIndex - ((pageOffset - 2) * pageLength)); // highIndex - 30
                        //}
                        //else
                        //{
                        //    Debug.Log("else");
                        //    indexToGet = (highIndex - ((pageOffset - 2) * pageLength)); // highIndex - 0
                        //}
                        //                        Debug.Log("indexToGet: " +indexToGet);
                        #endregion

                        int indexToGet = (highIndex - ((pageOffset - /*2*/ 1) * pageLength));
                        if (isDebugMode) indexToGet = (highIndex - ((pageOffset - 2) * pageLength));
                        //Debug.Log("(highIndex - ((pageOffset - 1) * pageLength)) " + " = " + indexToGet);
                        //Debug.Log("(" + highIndex + " - " + " (( " + pageOffset + " - 1 )  * " + pageLength + ")) = " + indexToGet);
                        //Debug.Log("scoreBoardEntries.Length: " + scoreBoardEntries.Length);
                        //Debug.Log("indexToGet = " + indexToGet + " pageOffset: " + pageOffset + " pageLength: " + pageLength + ". highIndex:" + highIndex);
                        return scoreBoardEntries[indexToGet]; // TODO: "Random" exception error here. Needs investigation
                    }
                }

            }
            else
                return null;
        }


        public void NotifyEntryDestroy(int index)
        {
            if (scoreBoardEntries != null && scoreBoardEntries.Length > 0) // TODO: Consider replacing with an isInit bool or tidier system. 
            {
                if (index == 0)
                {
                    lowIndex++;
                }
                else
                {
                    highIndex--;
                }
            }
        }




        #region Debug Methods


        private IEnumerator GeneratePseudoEntries(int offset)
        {

            yield return new WaitForSeconds(2.0f); // Simulate Server Delay

            EntryData[] newScores;

            newScores = new EntryData[scoreBoardEntries == null ? pageLength + pageLength : pageLength];

            for (int i = 0; i < newScores.Length; i++)
            {
                int index = (offset >= 0 ? pageLength * pageOffset + i : pageLength * (pageOffset -3) + i);

                newScores[i] = new EntryData(index, 000, 000000, "name");
            }



            if (offset == 0)
            {
                if (newScores.Length == pageLength + pageLength) pageOffset++;

                PoolSrollRect scrollRect = GetComponentInChildren<PoolSrollRect>();
                if (scrollRect != null)
                {
                    scrollRect.Init();
                }

                CallbackHighScoresNext(newScores, 0);

            }
            else if (offset > 0)
            {
                CallbackHighScoresNext(newScores, 0);
            }
            else
            {
                CallbackHighScoresPrevious(newScores, 0);
            }

            yield return null;
        }

        #endregion


        //void OnGUI()
        //{
        //    GUI.Label(new Rect(0, 0, 200, 200), "Low Index:");
        //    GUI.Label(new Rect(0, 20, 200, 200), lowIndex.ToString());

        //    GUI.Label(new Rect(100, 0, 200, 200), "High Index:");
        //    GUI.Label(new Rect(100, 20, 200, 200), highIndex.ToString());
        //}


    }
}
