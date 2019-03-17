using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateBase {

    public struct GameSessionData
    {
        public int totalScore; // The total score for this session
        public int levelScore; // The achieved on the current level
        public int levelIndex;
        public int passScore;
    }

    protected GameSessionData currentSessionData; // TEMP?: Subject to change/relocation
    public GameSessionData GetSessionData() { return currentSessionData; }

    protected SoundEffectEnum[] hexClickSounds;

    #region State Initialisation
    public virtual void StartGameState()
    {
        throw new System.NotImplementedException();
    }

    public virtual void StateUpdate()
    {
        //throw new System.NotImplementedException();
    }

    public virtual void PassSessionData(GameSessionData data)
    {
        //throw new System.NotImplementedException();
    }

    // Used to predefine click sounds for this game mode.
    protected virtual void InitialiseClickSounds()
    {
        throw new System.NotImplementedException();
    }

    public virtual void CleanupGameState()
    {
        //throw new System.NotImplementedException();
    }
    #endregion

    public virtual void Pause()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Resume()
    {
        throw new System.NotImplementedException();
    }

    // Used to play predefined click sounds. 
    public virtual void PlayClickSound()
    {
    //    throw new System.NotImplementedException();
    }

    // Intended to be used for listing for other kinds of input, such as key presses
    protected virtual void HandleInput()
    {
        throw new System.NotImplementedException();
    }

    public virtual void LoadNextLevel()
    {
        throw new System.NotImplementedException();
    }

    public virtual void PlayGroundThud() 
    {
        throw new System.NotImplementedException();
    } // TODO: Consider whether this method is required at all - could the ripple effect be triggered elsewhere, such as the grid?

    public virtual void HexDigEvent()
    {
        throw new System.NotImplementedException();
    }

    protected Level[] LoadLevelsFromPath(string pathToLoad)
    {
        //Level levelToReturn = Resources.Load<Level>(pathToLoad);

        Level[] levels = LevelLoader.instance.GetLevelsFrom(pathToLoad);

        if (levels != null)
            return levels;
        else
        {
            throw new System.Exception("Failed to load Level Asset from path at '" + pathToLoad + "' for " + GetType() + ".");
        }
    }

    //protected Leve LoadLevelFromPath(string pathToLoad)
    //{
    //    //Level levelToReturn = Resources.Load<Level>(pathToLoad);

    //    Level levels = LevelLoader.instance.GetLevelFrom(pathToLoad);

    //    if (levels != null)
    //        return levels;
    //    else
    //    {
    //        throw new System.Exception("Failed to load Level Asset from path at '" + pathToLoad + "' for " + GetType() + ".");
    //    }
    //}


    protected bool CreateLevel(Level levelToCreate, float verticalOffset, bool setBallEnabled, bool allowRandomMapRotation)
    {
        //Level newProfileLevel = LoadLevelFromPath(pathNewProfileLevel);

        MapSpawner.instance.SpawnHexs(
            levelToCreate, 
            GameManager.instance.GetPlayerBall().transform.position - new Vector3(0, -30, 0), 
            allowRandomMapRotation
            );

        Vector3 mapPosition = MapSpawner.instance.GetCurrentMapHolder().transform.position;
        GameManager.instance.GetPlayerBall().transform.position = mapPosition;
        GameManager.instance.GetPlayerBall().SetActive(setBallEnabled);

        return true;
    }

    //protected bool CreateLevel<T>(Level levelToCreate, float verticalOffset, bool setBallEnabled, bool allowRandomMapRotation, T componentToAdd) where T : Component
    //{
    //    CreateLevel(levelToCreate, verticalOffset, setBallEnabled, allowRandomMapRotation);
    //    GameObject currentLevelObject = MapSpawner.instance.GetCurrentMapHolder();
    //    if (currentLevelObject != null)
    //    {
    //        if (newComponent == null) Debug.LogWarning(" Failed to add " + componentToAdd.GetType() + " to " + currentLevelObject.name + ".");
    //    }
    //    return true;
    //}
}

