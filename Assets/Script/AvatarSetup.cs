using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class AvatarSetup : MonoBehaviour
{
    public Text myUsername;
    public Camera myCamera;
    public AudioListener myAudioListener;

    private PhotonView photonView;
    private GameObject myCharacter;

    // Start is called before the first frame update
    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        
        Debug.Log("Starting new AvatarSetup running on " + photonView.ViewID);
        
        if (photonView.IsMine)
        {
            Color randomColor = Random.ColorHSV();
            string myUsername = PhotonLobby.Instance.Username;
            
            PhotonStatusCanvas.instance.AppendMessage( "I am " + photonView.ViewID);
            PhotonStatusCanvas.instance.AppendMessage( "my username is " + myUsername);
            PhotonStatusCanvas.instance.AppendMessage( "and my color is " + randomColor);
            //PhotonStatusCanvas.instance.AppendMessage("And this should be my Player instance: " + PhotonNetwork.LocalPlayer);
            
            PhotonNetwork.LocalPlayer.NickName = myUsername;

            // AllBuffered: invia a tutti (fa in modo che anche i player che joinano successivamente
            // ricevano questo messaggio) e io lo eseguo immediatamente!
            photonView.RPC("RPC_UpdateInfo", RpcTarget.AllBuffered, randomColor.r, randomColor.g, randomColor.b, myUsername);
        }
    }

    public void DoTheDance()
    {
        photonView.RPC("RPC_Dance", RpcTarget.All, photonView.ViewID);
    }

    // This function is needed for the instantiation of the different players inside the scene.
    // If I am the one that actually made the call, I will execute it right away: so I basically will update my own information.
    // If other clients are connected, the "instances of me" will execute this function as soon as they join the room: 
    // this is how I "initialize" my self in other clients (exe)
    [PunRPC]
    void RPC_UpdateInfo(float r, float g, float b, string s)
    {
        if (photonView == null)
            PhotonStatusCanvas.instance.AppendMessage("My photonView is null\n");
        //else
        //    PhotonStatusCanvas.instance.AppendMessage("I am " + photonView.ViewID + "\n");
        //PhotonStatusCanvas.instance.AppendMessage("and I received: " + s + "\n");
        //PhotonStatusCanvas.instance.AppendMessage("and " + r + " " + g + " " + b + "\n\n");
        Material material = GetComponentInChildren<MeshRenderer>().material;
        material.color = new Color(r, g, b, 1);
        if (myUsername != null)
        {
            myUsername.text = s;
        }
        if (photonView != null && !photonView.IsMine)
        {
            // if this instance is not the local player (!pv.IsMine) I disable the camera and the audio listener
            // in order to avoid errors and warnings
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
        }
    }
    
    [PunRPC]
    void RPC_Dance(int photonViewViewId)
    {
        if (photonView == null)
            PhotonStatusCanvas.instance.AppendMessage("My photonView is null\n");
        //else
        //    PhotonStatusCanvas.instance.AppendMessage("I am " + photonView.ViewID + "\n");
        //PhotonStatusCanvas.instance.AppendMessage("and I received: DANCE\n");

        if (photonView.ViewID == photonViewViewId)
        {
            Animation anim = GetComponent<Animation>();
            Debug.Log(anim.name);
            anim.Play();
        }
    }
}
