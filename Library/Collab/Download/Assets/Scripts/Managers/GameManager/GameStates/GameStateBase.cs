using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class TransitionData<T> where T : GameStateBase //where T :  System.IConvertible
{
    public TransitionData(System.Type key, bool preserveState = false)
    {
        //this.key = key;
        this.key = key;

        //if (this.key != typeof(T)) {
        //    //Debug.Log(this.key.ToString() + " | " + typeof(T).ToString());
        //    //throw new System.InvalidOperationException();

        //}

        this.preserveState = preserveState;
    }

    readonly bool preserveState;
    public bool GetPreserveState() {
        return preserveState;
    }

    readonly System.Type key;
    public System.Type GetStateType() {
        return key;
    }

    //public GameStateBase CreateNewStateFromData()
    //{
    //    GameStateBase newState = (GameStateBase)System.Activator.CreateInstance(key);
    //    Debug.Log("New State = " + newState.GetType().ToString());
    //    return newState;
    //}

    public override int GetHashCode()
    {
        // Note on hash code overriding: https://www.baeldung.com/java-hashcode
        //return 17 + 31 * CurrentState.GetHashCode() + 31 * Command.GetHashCode();
        int returnHashCode = 17;
        returnHashCode = returnHashCode * 23 + key.GetHashCode();
        return returnHashCode;
    }

    public override bool Equals(object obj)
    {
        if (obj is TransitionData<T>)
        {
            TransitionData<T> other = (TransitionData<T>)obj;
            return this.key.Equals(other.key) && this.preserveState.Equals(other.preserveState);
        }
        else return false;
    }
}


public class GameStateBase
{

    protected Dictionary<Command, TransitionData<GameStateBase>> stateTransitions;

    protected virtual void InitialiseStateTransitions()
    {
        //stateTransitions = new Dictionary<Command, TransitionData<GameStateBase>>
        //{
        //    { Command.Begin, new TransitionData<GameStateBase>(typeof(GameStateEndless))  }
        //};

        throw new System.NotImplementedException();
    }

    public virtual bool StateTransition(Command commandToProcess, out TransitionData<GameStateBase> returnTransition)
    {
        if (stateTransitions.ContainsKey(commandToProcess))
        {
            returnTransition = stateTransitions[commandToProcess];
            return true;
        }
        returnTransition = null;
        return false;
    }


    public void ReplaceTilePassScores (int oldScore, int newScore)
    {
        currentSessionData.passScore -= oldScore;
        currentSessionData.passScore += newScore;
    }


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

    public HexTypeEnum currentEditHexType = HexTypeEnum.HexTile_ClickDestroy; // for the editor remembers the current edit thingo 

    #region State Initialisation
    public virtual void StartGameState()
    {
        throw new System.NotImplementedException();
    }

    public GameStateBase()
    {
        //InitialiseStateTransitions();
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

    public virtual void NextMenu()
    {
        throw new System.NotImplementedException();
    }


    protected Level[] LoadLevelsFromPath(string pathToLoad)
    {
        //Level levelToReturn = Resources.Load<Level>(pathToLoad);

        Level[] levels = LevelLoader.Instance.GetLevelsFrom(pathToLoad);

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

    //    Level levels = LevelLoader.Instance.GetLevelFrom(pathToLoad);

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



        if (MapSpawner.Instance != null)
        {
            MapSpawner.Instance.SpawnHexs(
                levelToCreate,
                GameManager.instance.GetPlayerBall().transform.position - new Vector3(0, -30, 0),
                allowRandomMapRotation
                );
        }
        else throw new System.Exception(" EXCEPTION: MapSpawner NULL");




        Vector3 mapPosition = MapSpawner.Instance.GetCurrentMapHolder().transform.position;
        //GameManager.instance.GetPlayerBall().transform.position = mapPosition;
        GameManager.instance.GetPlayerBall().SetActive(setBallEnabled);

        return true;
    }

    //protected bool CreateLevel<T>(Level levelToCreate, float verticalOffset, bool setBallEnabled, bool allowRandomMapRotation, T componentToAdd) where T : Component
    //{
    //    CreateLevel(levelToCreate, verticalOffset, setBallEnabled, allowRandomMapRotation);
    //    GameObject currentLevelObject = MapSpawner.Instance.GetCurrentMapHolder();
    //    if (currentLevelObject != null)
    //    {
    //        if (newComponent == null) Debug.LogWarning(" Failed to add " + componentToAdd.GetType() + " to " + currentLevelObject.name + ".");
    //    }
    //    return true;
    //}


    public bool MouseOverUI ()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}

