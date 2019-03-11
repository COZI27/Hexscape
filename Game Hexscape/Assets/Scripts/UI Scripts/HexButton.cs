using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexButton : MonoBehaviour {

   
    public GameManager.Command command = GameManager.Command.Begin;

    

    public void ClickedOn()
    {
        //GameManagement.instance.StartScene(gameplayState, transform.position);
        GameManager.instance.ProcessCommand(command);
        Destroy(this.gameObject);
    }


    private void OnMouseUpAsButton()
    {
       // ClickedOn();
    }
}
