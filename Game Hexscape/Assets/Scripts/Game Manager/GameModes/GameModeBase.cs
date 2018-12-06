using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameModeBase : MonoBehaviour {

    public struct GameSessionData
    {
        public int totalScore; // The total score for this session
        public int levelScore; // The achieved on the current level
        public int levelIndex;
        public int passScore;
    }

    protected GameSessionData currentSessionData; // TEMP: Subject to change/relocation

    protected SoundEffectEnum[] hexClickSounds;


    public abstract void StartGameMode();

    // Used to predefine click sounds for this game mode.
    protected abstract void InitialiseClickSounds();

    // Used to play predefined click sounds. 
    public abstract void PlayClickSound();

    // Intended to be used for listing for other kinds of input, such as key presses
    protected abstract void HandleInput(); 

    public abstract void LoadNextLevel();

    public abstract void PlayGroundThud(); // TODO: Consider whether this method is required at all - could the ripple effect be triggered elsewhere, such as the grid?

    public abstract void HexDigEvent();
}

