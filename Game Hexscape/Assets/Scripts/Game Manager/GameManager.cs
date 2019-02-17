﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public ScoreUI scoreUI;

    private void MakeSingleton()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public enum Command // NOTE: Values are still subject to change
    {
        NextLevel,

        Begin,
        End,

        Pause,
        Resume,

        QuitLevel
    }

    //public GameStateBase initialGameState = GameStateMenuMain;

    // private Stack<GameStateBase> ActiveGameStates;
    private GameStateBase currentGameState;
    public GameStateBase.GameSessionData GetGameSessionData()
    {
        return currentGameState.GetSessionData();
    }

    public float ballYOffset = -10; // TEMP
    //public Transform ballTransform; // TEMP
    public GameObject endlessManagerObject; // TEMP

    [SerializeField]
    private GameObject playerBallObject;

    public GameObject GetPlayerBall() {
        return playerBallObject;
    }


    private void Awake()
    {
        MakeSingleton();
        InitialiseTransitions();
    }

    // Use this for initialization
    void Start () {



        if (currentGameState == null)
        {
            ChangeGameState( new GameStateMenuMain () ) ;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}




    class StateTransition<TOne,TTwo> 
        //where TOne : System.Type
        where TTwo : struct, System.IConvertible // Intended to be an enum
    {

        readonly TOne keyOne;
        readonly TTwo keyTwo;

        public StateTransition(TOne keyOne, TTwo keyTwo)
        {
            this.keyOne = keyOne;
            this.keyTwo = keyTwo;
        }

        public override int GetHashCode()
        {
            // Note on hash code overriding: https://www.baeldung.com/java-hashcode
            //return 17 + 31 * CurrentState.GetHashCode() + 31 * Command.GetHashCode();
            int returnHashCode = 17;
            returnHashCode = returnHashCode * 23 + keyOne.GetHashCode();
            returnHashCode = returnHashCode * 23 + keyTwo.GetHashCode();
            return returnHashCode;
        }

        public override bool Equals(object obj)
        {
            StateTransition<TOne, TTwo> other = obj as StateTransition<TOne, TTwo>;
            return other != null && Object.Equals(this.keyOne, other.keyOne) && Object.Equals(this.keyTwo, other.keyTwo); // TODO: ensure object.equals works in this context
            //return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command;
        }
    }

    // transitions contains the types of 
    Dictionary<StateTransition<System.Type, Command>, System.Type> transitions;

    private void InitialiseTransitions()
    {

        transitions = new Dictionary<StateTransition<System.Type, Command>, System.Type>
        {
            /*Main Menu*/
            { new StateTransition<System.Type, Command>(typeof(GameStateMenuMain), Command.Begin), typeof(GameStateEndless)  },
            /*Endless*/
            { new StateTransition<System.Type, Command>(typeof(GameStateEndless), Command.QuitLevel), typeof(GameStateMenuMain)  },
            { new StateTransition<System.Type, Command>(typeof(GameStateEndless), Command.End), typeof(GameStateEndlessScoreboard)  },
           // { new StateTransition<System.Type, Command>(typeof(GameStateEndless), Command.End), typeof(GameStateMenuMain)  },
            /*Endless Scoreboard*/
            { new StateTransition<System.Type, Command>(typeof(GameStateEndlessScoreboard), Command.End), typeof(GameStateMenuMain)  },
            { new StateTransition<System.Type, Command>(typeof(GameStateEndlessScoreboard), Command.Begin), typeof(GameStateMenuMain)  }
        };

        // NOTE: Could change the value of an entry at runtime, if necessary
        // NOTE: It would also be possible to use a function/ delegate call or other type for the value
        //      https://stackoverflow.com/questions/20983342/how-to-store-a-type-not-type-object-for-future-use
        //      https://social.msdn.microsoft.com/Forums/en-US/0e4a2fc8-1db3-4093-8b83-83c598044917/syntax-help-calling-a-delegate-from-a-dictionary?forum=csharplanguage
        // TODO: prevent/remove duplicate values (could use a custom "Add" method to check for conflicts)
        // Could also use this or a wrapper to ensure that the transition value type is a child of GameStateBase


        
    }





    //public enum Events { LaunchApplication, CloseApplication, OpenMainMenu, OpenScoreMenu, StartGame, QuitGame }

    #region External Events

    public bool ProcessCommand(Command newCommand)
    {
        StateTransition<System.Type, Command> transitionToFind = new StateTransition<System.Type, Command>(currentGameState.GetType(), newCommand);

        if (transitions.ContainsKey(transitionToFind)) // Check whether a rule exists for this Gamestate
        {
            //GameStateBase newState =  (GameStateBase)System.Activator.CreateInstance( typeof(GameStateEndless) );
            GameStateBase newState = (GameStateBase)System.Activator.CreateInstance(transitions[transitionToFind]);
            //System.DateTime dateTime = (System.DateTime)System.Activator.CreateInstance(typeof(System.DateTime));


            ChangeGameState(newState);

            return true;
        }
        else
        {
            Debug.LogWarning("GameManager: Command process failed - no possible transition found");
            return false;
        }
    }

    public void DigEvent(int points)
    {
        // TODO: Consider whether/ how this could be moved into process command. Do we need any parameters?
        if (points > 0) // TEMP - Temproarily fixes issue with indestructible tiles adding score. The original approach passed '0' as a parameter from the tiles score.
            currentGameState.HexDigEvent();
    }

    public void ClickEvent()
    {
        currentGameState.PlayClickSound();
    }

    public void BallLandEvent()
    {
        currentGameState.PlayGroundThud();
    }

    #endregion External Events

    #region Internal Events
    private void ChangeGameState(GameStateBase newGameState)
    {
        // TODO: Share gamestate data here if needed


        if (currentGameState != null)
        {
            GameStateBase.GameSessionData previousSessionData = currentGameState.GetSessionData();
            newGameState.PassSessionData(previousSessionData);
            currentGameState.CleanupGameState();
        }

        currentGameState = newGameState;



        if (playerBallObject == null) throw new System.Exception("playerBallObject not found by GameManager.");


        currentGameState.StartGameState();
    }
    #endregion Internal Events
}
