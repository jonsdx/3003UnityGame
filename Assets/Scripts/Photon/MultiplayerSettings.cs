using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSettings : MonoBehaviour
{

    public static MultiplayerSettings multiplayerSettings;

    // use this to set false - continuous loading game, true - delayed start game
    public bool delayStart;
    public int maxPlayers;
    public int menuScene;
    public int multiplayerScene;
    
    private void Awake(){
        // We want our multiplayer settings to always be updated and persisting
        if(MultiplayerSettings.multiplayerSettings == null){
            MultiplayerSettings.multiplayerSettings = this;
        } else {
            if(MultiplayerSettings.multiplayerSettings != this){
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
