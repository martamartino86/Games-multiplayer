using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class Chat : MonoBehaviour
{
    public TextMeshProUGUI chat;
    public InputField message;
    
    private PhotonView pv;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    public void SendChatMessageToAll()
    {
        pv.RPC("RPC_ReceivedMessage", RpcTarget.All, PhotonLobby.Instance.Username, message.text);
        resetMessageText();
    }

    void resetMessageText()
    {
        message.text = "";
    }

    [PunRPC]
    void RPC_ReceivedMessage(string senderViewId, string msg)
    {
        chat.text += "[<b>" + senderViewId + "</b>] " + msg + "\n";
    }
}
