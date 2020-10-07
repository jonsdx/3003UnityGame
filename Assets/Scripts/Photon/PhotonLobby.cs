using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonLobby : MonoBehaviourPunCallbacks
{

    // Singleton which can be referenced by other scripts
    public static PhotonLobby lobby;
    public GameObject battleButton;
    public GameObject cancelButton;

    // initialise the singleton PhotonLobby
    private void Awake(){
        lobby = this;
        GameObject.Find("Student_Default").SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // connects to master photon server once the lobby is initialized (ie. when the script is run when the scene that this script is attached to runs)

    }

    // This will automatically run once the master client is connected
    public override void OnConnectedToMaster(){
        Debug.Log("Player is connected to the Photon Master Server");
        PhotonNetwork.AutomaticallySyncScene = true;
        battleButton.SetActive(true);

    }

    // Joins a random room
    public void OnBattleButtonClicked(){
        battleButton.SetActive(false);
        cancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();

    }

    // If no room available, create one
    public override void OnJoinRandomFailed(short returnCode, string message){
        Debug.Log("Tried to join random room but failed.");
        CreateRoom();
    }

    void CreateRoom(){
        int randomRoomName = Random.Range(0,1000);
        RoomOptions roomOps = new RoomOptions() {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = (byte)MultiplayerSettings.multiplayerSettings.maxPlayers
        };
        PhotonNetwork.CreateRoom("Room " + randomRoomName, roomOps);
    }

    public override void OnJoinedRoom(){
        Debug.Log("Joined room successfully");    
    }

    // If we fail to create a new room, possibly if the room that we are trying to create already exists because room number assigned is random
    public override void OnCreateRoomFailed(short returnCode, string message){
        CreateRoom();   // just run the create room function again and hope it doesnt create the same room again
    }

    public void OnCancelButtonClicked(){
        cancelButton.SetActive(false);
        battleButton.SetActive(true);
        // Leave the room if we want to stop entering the room
        PhotonNetwork.LeaveRoom();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
