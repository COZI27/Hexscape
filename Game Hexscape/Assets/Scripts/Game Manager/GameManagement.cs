using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour {

    public static GameManagement instance;
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

    public float ballYOffset = -10;
    public Transform ballTransform;
    public GameObject endlessManagerObject;


    public void StartScene (GameplayState gameplayState, Vector3 tilePos)
    {
        if (gameplayState == GameplayState.endless)
        {
            StartEndlessGameplay(tilePos + Vector3.up * ballYOffset);
        }
    }

    [ContextMenu("StartEndless")]
    public void StartEndlessGameplay (Vector3 ballPosition)
    {
        ballTransform.position = ballPosition;
        ballTransform.gameObject.SetActive(true);
        endlessManagerObject.SetActive(true);
    }


    private void Awake()
    {
        MakeSingleton();
    }

    public enum GameplayState
    {
        endless,
        puzzle,
        unlocks
    }

    
}
