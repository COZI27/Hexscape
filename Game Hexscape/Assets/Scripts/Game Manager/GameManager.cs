using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    private void MakeSingleton()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameStateBase initialGameState;

    GameStateBase CurrentGameState;


    private void Awake()
    {
        MakeSingleton();
    }

    // Use this for initialization
    void Start () {
		if (CurrentGameState == null)
        {
            ChangeGameState( new GameStateEndless() ) ;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // private Stack<GameStateBase> ActiveGameStates;



    public enum Command
    {
        Begin,
        End,
        Pause,
        Resume,
        Exit
    }

    public GameStateEndless endlessState;

    class StateTransition
    {
        readonly GameStateBase CurrentState;
        readonly Command Command;

        public StateTransition(GameStateBase currentState, Command command)
        {
            CurrentState = currentState;
            Command = command;
        }

        public override int GetHashCode()
        {
            // Note on hash code overriding: https://www.baeldung.com/java-hashcode
            //return 17 + 31 * CurrentState.GetHashCode() + 31 * Command.GetHashCode();
            int returnHashCode = 17;
            returnHashCode = returnHashCode * 23 + CurrentState.GetHashCode();
            returnHashCode = returnHashCode * 23 + Command.GetHashCode();
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            StateTransition other = obj as StateTransition;
            return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command;
        }
    }


    Dictionary<StateTransition, GameStateBase> transitions;

    Dictionary<GameStateBase, GameStateBase> thing;

    private void InitialiseTransitions()
    {

        thing = new Dictionary<GameStateBase, GameStateBase>
        {
            {endlessState, typeof(GameStateEndless) }
        };

        transitions = new Dictionary<StateTransition, GameStateBase>
        {
            { new StateTransition(GameStateEndless, Command.Begin), typeof(GameStateEndless) }
        };
    }




    public enum Events { LaunchApplication, CloseApplication, OpenMainMenu, OpenScoreMenu, StartGame, QuitGame }

    #region External Events

    public bool ProcessEvent( Events newEvent ) {
        return false;
    }

    public void LaunchGame()
    {

    }

    public void LoadMainMenu()
    {

    }

    #endregion External Events

    #region Internal Events
    private void ChangeGameState(GameStateBase newGameState)
    {
        CurrentGameState.CleanupGameState();
        CurrentGameState = newGameState;
        //System.GC.Collect();
    }
    #endregion Internal Events
}
