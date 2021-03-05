using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

/// <summary>
/// Setting up connection between player and photon server;
/// allows client to join existing room or to create a new one.
/// </summary>
public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<PhotonLobby>();
            return _instance;
        }
    }
    private static PhotonLobby _instance;

    public GameObject searchForRoomsAndJoinButton; // the user searchs for available rooms
    public GameObject stopSearchingForRoomButton;

    public string Username;

    // Invoked from the Login button in the Login scene
    public void SetUsername(InputField usernameIF)
    {
        if (usernameIF.text == "")
            Username = usernameIF.placeholder.GetComponent<Text>().text;
        else
            Username = usernameIF.text;
    }

    public Text PhotonStatusText;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        // connects to master photon server
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonStatusText.text += ("Player has connected to the Photon master server\n");
        // whenever the master client calls the LoadLevel function, this level is gonna be
        // synchronized among all connected clients, so that all clients will load in
        // into the same scene at the same time (more or less)
        PhotonNetwork.AutomaticallySyncScene = true;
        searchForRoomsAndJoinButton.SetActive(true);
    }

    public void OnBtnSearchForRoomsClicked()
    {
        searchForRoomsAndJoinButton.SetActive(false);
        stopSearchingForRoomButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonStatusText.text += ("Tried to login random room but failed. Creating a new one ...\n");
        CreateRoom();
    }

    void CreateRoom()
    {
        int randomRoomName = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 10 };
        PhotonNetwork.CreateRoom("Room" + randomRoomName.ToString(), roomOps);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        PhotonStatusText.text += ("Failed to create room: " + message);
        PhotonStatusText.text += ("So trying to create a new one ...\n");
        CreateRoom();
    }

    public override void OnCreatedRoom()
    {
        PhotonStatusText.text += ("Room created!" + PhotonNetwork.CurrentRoom.Name + "\n");
        
    }

    public void OnBtnCancelClicked()
    {
        stopSearchingForRoomButton.SetActive(false);
        searchForRoomsAndJoinButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }
}
