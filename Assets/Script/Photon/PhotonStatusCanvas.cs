using UnityEngine;
using UnityEngine.UI;

public class PhotonStatusCanvas : MonoBehaviour
{
    public static PhotonStatusCanvas instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<PhotonStatusCanvas>();
            return _instance;
        }
    }
    private static PhotonStatusCanvas _instance;

    public Text status;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void AppendMessage(string msg)
    {
        status.text += msg;
        status.text += "\n";
    }

}
