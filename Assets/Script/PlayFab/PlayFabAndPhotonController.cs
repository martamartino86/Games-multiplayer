using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// Mia classe Login di prova. Usa il codice di login di playfab + photon
/// </summary>
public class PlayFabAndPhotonController : MonoBehaviourPunCallbacks
{
    public static PlayFabAndPhotonController Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<PlayFabAndPhotonController>();
            return _instance;
        }
    }
    private static PlayFabAndPhotonController _instance;

    private string _username;
    private bool _register;

    private string _playFabPlayerIdCache;

    private void OnEnable()
    {
        base.OnEnable();
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = "911B6";
        }

        _username = "";
        _register = false;

        //        // Anonymous login
        //        var requestPc = new LoginWithCustomIDRequest { CustomId = ReturnId() };
        //        PlayFabClientAPI.LoginWithCustomID(requestPc, OnLoginSuccess, OnLoginFail);
        //#if UNITY_ANDROID
        //        var requestAndroid = new LoginWithAndroidDeviceIDRequest { AndroidDeviceId = ReturnMobileId(), CreateAccount = true };
        //        PlayFabClientAPI.LoginWithAndroidDeviceID(requestAndroid, OnLoginMobileSuccess, OnLoginMobileFailure);
        //#endif
        //#if UNITY_IOS
        //        var requestIOS = new LoginWithIOSDeviceIDRequest { DeviceId = ReturnMobileId(), CreateAccount = true };
        //        PlayFabClientAPI.LoginWithIOSDeviceID(requestIOS, OnLoginMobileSuccess, OnLoginMobileFailure);
        //#endif
        
    }

    /*
     * Step 1
     * We authenticate current PlayFab user normally.
     * In this case we use LoginWithCustomID API call for simplicity.
     * You can absolutely use any Login method you want.
     * We use PlayFabSettings.DeviceUniqueIdentifier as our custom ID.
     * We pass RequestPhotonToken as a callback to be our next step, if
     * authentication was successful.
     */
    private void AuthenticateWithPlayFab()
    {
        Debug.Log("PlayFab authenticating using Custom ID...");

        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            CreateAccount = true,
            CustomId = PlayFabSettings.DeviceUniqueIdentifier
        }, RequestPhotonToken, OnPlayFabError);
    }

    /*
    * Step 2
    * We request Photon authentication token from PlayFab.
    * This is a crucial step, because Photon uses different authentication tokens
    * than PlayFab. Thus, you cannot directly use PlayFab SessionTicket and
    * you need to explicitly request a token. This API call requires you to
    * pass Photon App ID. App ID may be hard coded, but, in this example,
    * We are accessing it using convenient static field on PhotonNetwork class
    * We pass in AuthenticateWithPhoton as a callback to be our next step, if
    * we have acquired token successfully
    */
    private void RequestPhotonToken(LoginResult obj)
    {
        LogMessage("PlayFab authenticated. Requesting photon token...");
        //We can player PlayFabId. This will come in handy during next step
        _playFabPlayerIdCache = obj.PlayFabId;

        PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest()
        {
            PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime // non so se è giusto, non trovava il ref
        }, AuthenticateWithPhoton, OnPlayFabError);

        //
        //GetStats();
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest { DisplayName = _username }, OnDisplayName, OnErrorDisplayName);
    }

    private void OnErrorDisplayName(PlayFabError obj)
    {
        PrintErrorMessage("OnErrorDisplayName", obj);
    }

    private void OnDisplayName(UpdateUserTitleDisplayNameResult obj)
    {
        Debug.Log("OnDisplayName - setup correctly");
    }

    /*
     * Step 3
     * This is the final and the simplest step. We create new AuthenticationValues instance.
     * This class describes how to authenticate a players inside Photon environment.
     */
    private void AuthenticateWithPhoton(GetPhotonAuthenticationTokenResult obj)
    {
        LogMessage("Photon token acquired: " + obj.PhotonCustomAuthenticationToken + "  Authentication complete.");

        //We set AuthType to custom, meaning we bring our own, PlayFab authentication procedure.
        var customAuth = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };
        //We add "username" parameter. Do not let it confuse you: PlayFab is expecting this parameter to contain player PlayFab ID (!) and not username.
        customAuth.AddAuthParameter("username", _playFabPlayerIdCache);    // expected by PlayFab custom auth service

        //We add "token" parameter. PlayFab expects it to contain Photon Authentication Token issues to your during previous step.
        customAuth.AddAuthParameter("token", obj.PhotonCustomAuthenticationToken);

        //We finally tell Photon to use this authentication parameters throughout the entire application.
        PhotonNetwork.AuthValues = customAuth;
        // aggiunta mia perché mi sa che alla fine non si connetteva mica a photon (ma non so mica se serva davvero)
        PhotonNetwork.ConnectUsingSettings();
    }
    
    // callback di ConnectUsingSettings
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        // una volta fatto questo passaggio, provo a joinare una stanza e, se non esiste, chiedo al server di crearmela
        PhotonNetwork.JoinRandomRoom();
    }


    // callback di JoinRandomRoom
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("Failed to join room. Creating a new room ...");
        CreateRoom();
    }

    void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 2, IsVisible = true };
        int randomId = Random.Range(0, 40000);
        PhotonNetwork.CreateRoom("LaMiaStanza", roomOptions, TypedLobby.Default);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print("Failed to create a room (" + message + "). Trying with a new one ...");
        CreateRoom();
    }

    private void OnPlayFabError(PlayFabError obj)
    {
        LogMessage(obj.GenerateErrorReport());
    }

    public void LogMessage(string message)
    {
        Debug.Log("PlayFab + Photon Example: " + message);
    }
    
    //private void OnLoginFail(PlayFabError obj)
    //{
    //    PrintErrorMessage("OnLoginFail", obj);
    //    Debug.Log("Probably this is the first connection, so let's register the user after the insertion of the name");
    //    _register = true;
    //}

    //private void OnLoginSuccess(LoginResult obj)
    //{
    //    Debug.Log("Known user successfully login: " + obj.PlayFabId);
    //}
    //private void RegisterNewUserWithName()
    //{
    //    var registerRequest = new RegisterPlayFabUserRequest { Username = _username };
    //    PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFail);
    //}
    //private void OnRegisterSuccess(RegisterPlayFabUserResult obj)
    //{
    //    Debug.Log("Registration user successful: " + obj.PlayFabId);
    //}

    //private void OnRegisterFail(PlayFabError obj)
    //{
    //    PrintErrorMessage("OnRegisterFail", obj);
    //}

    //private static string ReturnId()
    //{
    //    return SystemInfo.deviceUniqueIdentifier;
    //}


    public string GetUsername()
    {
        return _username;
    }
    public void SetUsername(InputField usernameInputField)
    {
        _username = usernameInputField.text;
        //RegisterNewUserWithName();
        AuthenticateWithPlayFab();
    }

    public int playerLevel;
    public int gameLevel;
    public int playerHealth;
    public int playerDamage;
    public int playerHighScore;

    #region PlayerStats

    public void SetStats()
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate { StatisticName = "PlayerLevel", Value = playerLevel },
                new StatisticUpdate { StatisticName = "GameLevel", Value = gameLevel },
                new StatisticUpdate { StatisticName = "PlayerDamage", Value = playerDamage },
                new StatisticUpdate { StatisticName = "PlayerHighScore", Value = playerHighScore },
            }
        },
        result => { Debug.Log("User statistics updated"); },
        error => { Debug.LogError(error.GenerateErrorReport()); });
    }

    void GetStats()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStats,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    void OnGetStats(GetPlayerStatisticsResult result)
    {
        Debug.Log("Received the following Statistics:");
        foreach (var eachStat in result.Statistics)
        {
            Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
            switch (eachStat.StatisticName)
            {
                case "PlayerLevel":
                    playerLevel = eachStat.Value;
                    break;
                case "GameLevel":
                    gameLevel = eachStat.Value;
                    break;
                case "PlayerDamage":
                    playerDamage = eachStat.Value;
                    break;
                case "PlayerHighScore":
                    playerHighScore = eachStat.Value;
                    break;
            }
        }
    }

    // Build the request object and access the API
    public void UpdatePlayerStats_StartCloud()
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdatePlayerStats", // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new { Level = playerLevel, highScore = playerHighScore }, // The parameter provided to your function
            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        }, OnCloudUpdateStats, OnErrorShared);
    }

    private static void OnCloudUpdateStats(ExecuteCloudScriptResult result)
    {
        // CloudScript returns arbitrary results, so you have to evaluate them one step and one parameter at a time
        Debug.Log(PlayFabSimpleJson.SerializeObject(result.FunctionResult));
        JsonObject jsonResult = (JsonObject)result.FunctionResult;
        object messageValue;
        jsonResult.TryGetValue("messageValue", out messageValue); // note how "messageValue" directly corresponds to the JSON values set in CloudScript
        Debug.Log((string)messageValue);
    }

    private static void OnErrorShared(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    #endregion PlayerStats

    #region Leaderboard
    public void GetLeaderboard()
    {
        var requestLeaderboard = new GetLeaderboardRequest { StartPosition = 0, StatisticName = "PlayerHighScore", MaxResultsCount = 20 };
        PlayFabClientAPI.GetLeaderboard(requestLeaderboard, OnGetLeaderboard, OnErrorLeaderboard);
    }
    void OnGetLeaderboard(GetLeaderboardResult result)
    {
        foreach (PlayerLeaderboardEntry player in result.Leaderboard)
        {
            Debug.Log(player.DisplayName + ": " + player.StatValue);
        }
    }
    void OnErrorLeaderboard(PlayFabError error)
    {
        PrintErrorMessage("OnErrorLeaderboard", error);
    }
    #endregion

    private void PrintErrorMessage(string functionName, PlayFabError error)
    {
        Debug.Log("[" + functionName + "] " + error.Error + " " + error.ErrorDetails);
    }
}
