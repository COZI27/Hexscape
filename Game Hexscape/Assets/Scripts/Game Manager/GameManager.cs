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
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            StateTransition<TOne, TTwo> other = obj as StateTransition<TOne, TTwo>;
            return other != null && Object.Equals(this.keyOne, other.keyOne) && Object.Equals(this.keyTwo, other.keyTwo); // TODO: ensure object.equals works in this context
            //return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command;
        }
    }

    private void InitialiseTransitions()
    {
        Dictionary < StateTransition <System.Type, Command >, System.Type> transitions;

        //string stringTemp = endlessState.GetType().Name.ToString();


        //thing = new Dictionary<string, typeof(GameStateBase)>
        //{
        //    {  stringTemp, typeof(GameStateEndless),


        //    }
        //};

        //thing.Add(stringTemp, typeof(GameStateEndless));

        transitions = new Dictionary<StateTransition<System.Type, Command>, System.Type>
        {
            { new StateTransition<System.Type, Command> (typeof(GameStateEndless), Command.Begin), typeof(GameStateEndless) }
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
