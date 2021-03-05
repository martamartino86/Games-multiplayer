using UnityEngine;

public class MultiplayerSettings : /*Photon.*/MonoBehaviour
{
    public static MultiplayerSettings Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<MultiplayerSettings>();
            return _instance;
        }
    }
    private static MultiplayerSettings _instance;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void ConnectToMaster()
    {
        //Debug.Log("connected to master? " + PhotonNetwork.ConnectUsingSettings("0.1"));
    }

}
