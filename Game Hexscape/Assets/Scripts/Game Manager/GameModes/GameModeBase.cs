using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static abstract class GameStateBase : MonoBehaviour {

    public struct GameSessionData
    {
        public int totalScore; // The total score for this session
        public int levelScore; // The achieved on the current level
        public int levelIndex;
        public int passScore;
    }

    protected GameSessionData currentSessionData; // TEMP: Subject to change/relocation

    protected SoundEffectEnum[] hexClickSounds;

    #region State Initialisation
    public abstract void StartGameState();

    // Used to predefine click sounds for this game mode.
    protected abstract void InitialiseClickSounds();

    public abstract void CleanupGameState();
    #endregion

    public abstract void Pause();

    public abstract void Resume();


    #region State Actions
    // Used to play predefined click sounds. 
    public abstract void PlayClickSound();

    // Intended to be used for listing for other kinds of input, such as key presses
    protected abstract void HandleInput();

    public abstract void LoadNextLevel();

    public abstract void PlayGroundThud(); // TODO: Consider whether this method is required at all - could the ripple effect be triggered elsewhere, such as the grid?

    public abstract void HexDigEvent();
    #endregion
}

