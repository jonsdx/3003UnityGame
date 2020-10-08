using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{

    // Create a singleton of this script
    public static PhotonRoom room;
    private PhotonView PV;
    public bool isGameLoaded;
    public int currentScene;

    // Player info
    Player[] photonPlayers;
    public int playersInRoom;
    public int myNumberInRoom;
    public int playerInGame;

    // Delayed Start
    private bool readyToCount;
    private bool readyToStart;
    public float startingTime;
    private float lessThanMaxPlayers;
    private float atMaxPlayers;
    private float timeToStart;
    public Vector2 playerPosition;
    
    void Awake(){
        if(PhotonRoom.room == null){
            PhotonRoom.room = this;
        } else{
            if(PhotonRoom.room != this){
                Destroy(PhotonRoom.room.gameObject);
                PhotonRoom.room = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnEnable(){
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable(){
        base.OnDisable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }


    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        readyToCount = false;
        readyToStart = false;
        lessThanMaxPlayers = startingTime;
        atMaxPlayers = 6;
        timeToStart = startingTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(MultiplayerSettings.multiplayerSettings.delayStart){
            if(playersInRoom == 1){
                RestartTimer();                
            }
            if(!isGameLoaded){
                if(readyToStart){
                    atMaxPlayers -= Time.deltaTime;
                    lessThanMaxPlayers = atMaxPlayers;
                    timeToStart = atMaxPlayers;
                }
                else if(readyToCount){
                    lessThanMaxPlayers -= Time.deltaTime;
                    timeToStart = lessThanMaxPlayers;
                }
                if(timeToStart <= 0){
                    StartGame();
                }
            }
        }
    }

    public override void OnJoinedRoom(){
        base.OnJoinedRoom();
        Debug.Log("Joined room successfully");    
        // Get list of players in room
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;
        // assign a nickname to each player in the room
        PhotonNetwork.NickName = myNumberInRoom.ToString();
        if(MultiplayerSettings.multiplayerSettings.delayStart){
            Debug.Log("Number of players in room currently/Max players: " + playersInRoom + "/" + MultiplayerSettings.multiplayerSettings.maxPlayers);
            if(playersInRoom > 1){
                readyToCount = true;
            }
            if(playersInRoom == MultiplayerSettings.multiplayerSettings.maxPlayers){
                readyToCount = true;
                if(!PhotonNetwork.IsMasterClient){
                    return;
                }
                // Once the maximum number of players that can be in the room is reached we automatically close the room so no other players may join
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
        else{
            StartGame();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer){
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("A new player has entered the room");
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom++;
        if(MultiplayerSettings.multiplayerSettings.delayStart){
            Debug.Log("Number of players in room currently/Max players: " + playersInRoom + "/" + MultiplayerSettings.multiplayerSettings.maxPlayers);
            if(playersInRoom > 1){
                readyToCount = true;
            }
            if(playersInRoom == MultiplayerSettings.multiplayerSettings.maxPlayers){
                readyToCount = true;
                if(!PhotonNetwork.IsMasterClient){
                    return;
                }
                // Once the maximum number of players that can be in the room is reached we automatically close the room so no other players may join
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
    }

    void StartGame(){
        isGameLoaded = true;
        if(!PhotonNetwork.IsMasterClient)
            return;
        if(MultiplayerSettings.multiplayerSettings.delayStart){
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        PhotonNetwork.LoadLevel("ExamHall1");
        GameObject.Find("Student_Default").SetActive(true);
    }

    void RestartTimer(){
        lessThanMaxPlayers = startingTime;
        timeToStart = startingTime;
        atMaxPlayers = 6;
        readyToCount = false;
        readyToStart = false;
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode){
        currentScene = scene.buildIndex;
        if(currentScene == MultiplayerSettings.multiplayerSettings.multiplayerScene){
            isGameLoaded = true;

            if(MultiplayerSettings.multiplayerSettings.delayStart){
                PV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
            }
            else{
                RPC_LoadPlayer();
            }
        }
    }

    [PunRPC]    
    private void RPC_LoadedGameScene(){
        playerInGame++;
        if(playerInGame == PhotonNetwork.PlayerList.Length){
            PV.RPC("RPC_LoadPlayer", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_LoadPlayer(){
        GameObject.Find("Student_Default").GetComponent<PlayerMovement>().StartMove();
        GameObject.Find("Student_Default").transform.position = playerPosition;
    }
}
