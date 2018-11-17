using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// Just a big F off box colider that restarts the game if the player touches it... just to make sure you dont keep falling till the cold death of the universe (even if it looks nice in the amazing tunels being created by Kris, ya legend) 

public class PlayerKillZone : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
         if (other.GetComponent<PlayerController>() != null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            // Call Restart Game function here M8
        }
    }
}
