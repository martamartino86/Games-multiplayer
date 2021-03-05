//using UnityEngine;

//public class MPGameManager : MonoBehaviour
//{
//    public MPGameManager Instance
//    {
//        get
//        {
//            if (_instance == null)
//                _instance = FindObjectOfType<MPGameManager>();
//            return _instance;
//        }
//    }
//    private MPGameManager _instance;

//    public enum GameState
//    {
//        Login,
//        Room
//    }

//    public GameState CurrentState;

//    public GameObject UserCubePrefab;

//    public PlayFabController userInformation;

//    private void Awake()
//    {
//        DontDestroyOnLoad(gameObject);
//    }

//    private void OnEnable()
//    {
//        UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;
//    }
//    private void OnDisable()
//    {
//        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
//    }

//    // Start is called before the first frame update
//    void Start()
//    {
//        CurrentState = GameState.Login;
//    }
    
//    public void NextState()
//    {
//        CurrentState++;
//        UnityEngine.SceneManagement.SceneManager.LoadScene((int)CurrentState, UnityEngine.SceneManagement.LoadSceneMode.Single);
//    }

//    private void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1)
//    {
//        InitializeNewScene();
//    }

//    public void PrevState()
//    {
//        CurrentState--;
//        UnityEngine.SceneManagement.SceneManager.LoadScene((int)CurrentState, UnityEngine.SceneManagement.LoadSceneMode.Single);
//    }

//    private void InitializeNewScene()
//    {
//        switch (CurrentState)
//        {
//            case GameState.Login:
//                break;
//            case GameState.Room:
//                GameObject newUserCube = Instantiate(UserCubePrefab);
//                newUserCube.GetComponent<UserCube>().SetUsername(userInformation.GetUsername());
//                break;
//        }
//    }
//}
