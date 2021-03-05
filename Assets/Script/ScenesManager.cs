using UnityEngine;

/// <summary>
/// Si occupa di tenere traccia della scena corrente, esegue la Load della nuova scena,
/// inizializza i gameobjects quando la scena inizia, ecc.
/// </summary>
public class ScenesManager : MonoBehaviour
{
    public ScenesManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<ScenesManager>();
            return _instance;
        }
    }
    private ScenesManager _instance;

    public enum GameState
    {
        Login,
        Room
    }

    public GameState CurrentState;

    //public GameObject UserCubePrefab;

    //public PlayFabAndPhotonController userInformation;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }
    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    // Start is called before the first frame update
    void Start()
    {
        CurrentState = GameState.Login;
    }
    
    public void GotoNextScene()
    {
        CurrentState++;
        UnityEngine.SceneManagement.SceneManager.LoadScene((int)CurrentState, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1)
    {
        InitializeNewScene();
    }

    public void GotoPrevScene()
    {
        CurrentState--;
        UnityEngine.SceneManagement.SceneManager.LoadScene((int)CurrentState, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private void InitializeNewScene()
    {
        switch (CurrentState)
        {
            case GameState.Login:
                break;
            case GameState.Room:
                //GameObject newUserCube = Instantiate(UserCubePrefab);
                //newUserCube.GetComponent<UserCube>().SetUsername(userInformation.GetUsername());
                break;
        }
    }
}
