using Photon.Pun;
using UnityEngine;

public class BoardSymbolInstance : MonoBehaviour, IPunInstantiateMagicCallback
{
    void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = gameObject.GetPhotonView().InstantiationData;
        if (data != null && data.Length == 1)
        {
            transform.SetParent(GameObject.Find((string)data[0]).transform);
        }
    }
}