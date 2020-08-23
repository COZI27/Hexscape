using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerProfile {

    public PlayerProfile(string id, string name)
    {
        playerID = id;
        playerName = name;
    }
    [SerializeField]
    private string playerName;
    public string GetPlayerName() { return playerName; }
    [SerializeField]
    private string playerID;
    public string GetPlayerID() { return playerID; }
    public int GetPlayerIDasInt() {
        int returnID;
        int.TryParse(playerID, out returnID);
        return returnID;
    }


    //Endless Stats
    public string playerHighscore;

    bool ConvertToJason() // TEMP
    {
        string playerToJason = JsonUtility.ToJson(this);
        Debug.Log(playerToJason);

        return true;
    }
}
