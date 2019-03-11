using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexButton : MonoBehaviour {

    public GameManagement.GameplayState gameplayState = GameManagement.GameplayState.endless;

    public void ClickedOn()
    {
        //GameManagement.instance.StartScene(gameplayState, transform.position);
        GameManager.instance.ProcessCommand(GameManager.Command.Begin);
        Destroy(this.gameObject);
    }


    private void OnMouseUpAsButton()
    {
       // ClickedOn();
    }
}
