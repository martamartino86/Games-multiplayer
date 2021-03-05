using System.IO;
using UnityEngine;
using Photon.Pun;

public class PhotonNetworkPlayer : MonoBehaviour
{
    PhotonView photonView;
    // voglio tenere separata l'istanza player di Photon e la rappresentazione del player,
    // perché quando mi disconnetto non voglio eliminare tutta l'istanza del player,
    // ma solo la sua rappresentazione, in modo che mantenga le sue informazioni (tipo l'id)
    public GameObject myAvatar;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        int spawnPicker = Random.Range(0, GameSetup.instance.SpawnPoints.Length);
        if (photonView.IsMine)
        {
            myAvatar = PhotonNetwork.Instantiate(
                Path.Combine("PhotonPrefabs", "PlayerAvatar"),
                GameSetup.instance.SpawnPoints[spawnPicker].position,
                GameSetup.instance.SpawnPoints[spawnPicker].rotation, 
                0);
            myAvatar.transform.SetParent(transform, true);
        }
    }

}
