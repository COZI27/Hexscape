using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerProfile : MonoBehaviour {
    public string playerID;

    //Endless Stats
    public string playerHighscore;

    bool ConvertToJason() // TEMP
    {
        string playerToJason = JsonUtility.ToJson(this);
        Debug.Log(playerToJason);

        return true;
    }
}
