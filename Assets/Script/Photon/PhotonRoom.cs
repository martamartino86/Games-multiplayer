using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
// Photon Unity Networking
using Photon.Pun;
using Photon.Realtime;
using System;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    // Room info 
    public static PhotonRoom Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<PhotonRoom>();
            return _instance;
        }
    }
    private static PhotonRoom _instance;
    
    // serve a mandare msg tra i vari client via Photon
    private PhotonView photonView;

    public GameObject ThisPlayer;

    public bool isGameLoaded;
    public int myNumberInRoom;

    public int multiplayerScene;
    public int currentScene;
    //public int currentRoom;
    //public bool readyToCount;
    //public bool readyToStart;
    //public float startingTime;
    //public float lessThanMaxPlayer;
    //public float atMaxPlayers;
    //public float timeToStart;

    Player[] photonPlayers;

    int playersInRoom;
    int playerInGame;

    private void Awake()
    {
        // la PhotonRoom ha anche la componente PhotonView (che ha un identificatore unico e 
        // che serve ad essere identificata da Photon). Quando instanzio un gameobject
        // con PhotonView e DontDestroyOnLoad, e riapro la stessa scena, avrò 2 gameobject uguali.
        // Photon mi dà un errore, ma non è un problema, perché cancella l'istanza più vecchia.
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        // registra questo oggetto in modo da riceverne le callback
        PhotonNetwork.AddCallbackTarget(this);
        // quando la scena Unity è caricata, inizio a fare cose
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        ThisPlayer = null;
        //readyToCount = false;
        //readyToStart = false;
        //lessThanMaxPlayer = startingTime;
        //atMaxPlayers = 6;
        //timeToStart = startingTime;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        //photonPlayers = PhotonNetwork.PlayerList;
        //playersInRoom = photonPlayers.Length;
        //myNumberInRoom = playersInRoom;
        //PhotonNetwork.NickName = myNumberInRoom.ToString();
        PhotonStatusCanvas.instance.AppendMessage("Am I master client? " + PhotonNetwork.IsMasterClient.ToString() + "\n");

        PhotonStatusCanvas.instance.AppendMessage("Room joined! " + PhotonNetwork.CurrentRoom.Name + 
            "\nLaunching scene ...\n");

        if (PhotonNetwork.IsMasterClient)
            StartGame();
    }

    void StartGame()
    {
        PhotonNetwork.LoadLevel(multiplayerScene);
    }

    private void OnSceneFinishedLoading(Scene arg0, LoadSceneMode arg1)
    {
        currentScene = arg0.buildIndex;
        // questo evento viene sollevato a prescindere dalla scena che sto caricando.
        // controllo se sono nella scena multiplayer e, in caso positivo, creo il player.
        if (currentScene == multiplayerScene)
        {
            CreatePlayer();
        }
        //isGameLoaded = true;
        //photonView.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
        //if (MultiplayerSetting.multiplayerSetting.delayStart)
        //{
        //    photonView.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
        //}
        //else
        //{
        //    RPC_CreatePlayer();
        //}
    }

    void CreatePlayer()
    {
        // create player network controller, but not player character
        ThisPlayer = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), transform.position, Quaternion.identity, 0);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom++;
        //MultiplayerSetting.multiPlayerSetting.maxPlayer
        //if (playersInRoom > 1)
        //{
        //    readyToCount = true;
        //}
        //if (playersInRoom == MultiplayerSetting.multiPlayerSetting.maxPlayer)
        //{
        //    readyToStart = true;
        //    if (!PhotonNetwork.IsMasterClient)
        //        return;
        //    PhotonNetwork.CurrentRoom.IsOpen = false;
        //}
    }
    
    //[PunRPC]
    //private void RPC_LoadedGameScene()
    //{
    //    playerInGame++;
    //    if (playerInGame == PhotonNetwork.PlayerList.Length)
    //    {
    //        // (assumendo di essere l'host) dico a tutti i client di creare i propri player
    //        photonView.RPC("RPC_CreatePlayer", RpcTarget.All);
    //    }
    //}

    //[PunRPC]
    //private void RPC_CreatePlayer()
    //{
    //    // istanzio il mio prefab
    //    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), transform.position, Quaternion.identity, 0);
    //}

}
