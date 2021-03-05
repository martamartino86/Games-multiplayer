using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

/// <summary>
/// (Tutorial) diversi tipi di login con PlayFab. 
/// </summary>
public class _PlayFabLogin : MonoBehaviour
{
    private string userName;
    private string userEmail;
    private string userPassword;

    public GameObject loginPanel;
    public GameObject addLoginPanel;
    public GameObject recoveryButton;

    public void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            /*
            Please change the titleId below to your own titleId from PlayFab Game Manager.
            If you have already set the value in the Editor Extensions, this can be skipped.
            */
            PlayFabSettings.staticSettings.TitleId = "911B6";
        }
        // Recoverable Login - if the user is already registered
        if (PlayerPrefs.HasKey("EMAIL"))
        {
            userEmail = PlayerPrefs.GetString("EMAIL");
            userPassword = PlayerPrefs.GetString("PASSWORD");
            var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }
        // Anonymous Login
        else
        {
            
#if UNITY_ANDROID
            var requestAndroid = new LoginWithAndroidDeviceIDRequest { AndroidDeviceId = ReturnMobileId(), CreateAccount = true };
            PlayFabClientAPI.LoginWithAndroidDeviceID(requestAndroid, OnLoginMobileSuccess, OnLoginMobileFailure);
#endif
#if UNITY_IOS
            var requestIOS = new LoginWithIOSDeviceIDRequest { DeviceId = ReturnMobileId(), CreateAccount = true };
            PlayFabClientAPI.LoginWithIOSDeviceID(requestIOS, OnLoginMobileSuccess, OnLoginMobileFailure);
#endif
        }
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        loginPanel.SetActive(false);
        recoveryButton.SetActive(false);
    }
    private void OnLoginFailure(PlayFabError error)
    {
        // se non riesce a loggare, vuol dire che devo registrarlo (anche se non mi torna molto sta cosa, non dovrei chiederglielo?)
        var registerRequest = new RegisterPlayFabUserRequest { Email = userEmail, Password = userPassword, Username = userName };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFail);
    }

    private void OnLoginMobileSuccess(LoginResult result)
    {
        Debug.Log("[OnLoginMobileSuccess] " + result.PlayFabId);
        loginPanel.SetActive(false);
        recoveryButton.SetActive(false);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        loginPanel.SetActive(false);
    }
    private void OnRegisterFail(PlayFabError error)
    {
        PrintErrorMessage("OnRegisterFail", error);
    }

    private void OnLoginMobileFailure(PlayFabError error)
    {
        PrintErrorMessage("OnLoginAndroidFailure", error);
    }
    public void GetUserName(string userNameIn)
    {
        userName = userNameIn;
    }
    public void GetUserEmail(string emailIn)
    {
        userEmail = emailIn;
    }
    public void GetUserPassword(string passwordIn)
    {
        userPassword = passwordIn;
    }

    public void OnClickLogin()
    {
        var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    public static string ReturnMobileId()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }

    private void PrintErrorMessage(string functionName, PlayFabError error)
    {
        Debug.Log("["+ functionName + "] " + error.Error + " " + error.ErrorDetails);
    }

    public void OpenAddLogin()
    {
        addLoginPanel.SetActive(true);
    }

    public void OnClickAddLogin()
    {
        // registriamo l'utente
        var addLoginRequest = new AddUsernamePasswordRequest { Email = userEmail, Password = userPassword, Username = userName };
        PlayFabClientAPI.AddUsernamePassword(addLoginRequest, OnAddLoginSuccess, OnRegisterFail);
    }
    private void OnAddLoginSuccess(AddUsernamePasswordResult result)
    {
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        addLoginPanel.SetActive(false);
    }
}