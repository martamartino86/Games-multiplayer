using Photon.Pun;
using UnityEngine;

public class GameButtons : MonoBehaviour
{

    public void SendStatistics()
    {
        //PlayFabController.PFC.SetStats();
        PlayFabAndPhotonController.Instance.UpdatePlayerStats_StartCloud();
    }

    public void GetLeaderboard()
    {
        PlayFabAndPhotonController.Instance.GetLeaderboard();
    }

    public void Dance()
    {
        PhotonRoom.Instance.ThisPlayer.GetComponentInChildren<AvatarSetup>().DoTheDance();
    }
}
