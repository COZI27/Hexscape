﻿using System.Collections;
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

    protected SoundEffectEnum[] hexClickSounds;

    #region State Initialisation
    public virtual void StartGameState()
    {
        throw new System.NotImplementedException();
    }

    // Used to predefine click sounds for this game mode.
    protected virtual void InitialiseClickSounds()
    {
        throw new System.NotImplementedException();
    }

    public virtual void CleanupGameState()
    {
        throw new System.NotImplementedException();
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
        throw new System.NotImplementedException();
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
}
