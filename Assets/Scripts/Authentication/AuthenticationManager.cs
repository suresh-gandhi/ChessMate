using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

// This is basically for managing the authentication process
public class AuthenticationManager : MonoBehaviour
{
    [SerializeField]
    private GameObject EnvironmentUIManagerGO;

    public static AuthenticationManager Instance;

    private const string GUEST = "Guest";
    private string playerId;
    private string userName = GUEST;

    private bool isAuthenticated = false;

    public UnityEvent AuthenticationStateChangeEvent = new UnityEvent();

    // Initially we will show the Authentication UI and not show LogOutUI
    // If the user is previously registered we simply authenticate and log him in
    // Else we will first send registration request and then we will authenticate to log him in
    // Once logged in we will dissapear the AuthenticationUI and display the LogOutUI
    // Now, if the user presses LogOut button he will be unauthenticated

    public bool IsAuthenticated() {
        return isAuthenticated;
    }

    void Awake() {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        // LoginButton.onClick.AddListener(OnClickLoginButton);
        // LogoutButton.onClick.AddListener(OnClickLogoutButton);
        GameSparks.Core.GS.GameSparksAuthenticated += GameSparksAuthenticatedCallback;
    }

    private void GameSparksAuthenticatedCallback(string id) {
        LogIn(id);
        // LoggedInStateDisplay();
    }

    void Start() {
        EnvironmentUIManagerGO.GetComponent<EnvironmentUIManager>()
            .LogInButtonClickedEvent.AddListener(OnClickLoginButton);
        EnvironmentUIManagerGO.GetComponent<EnvironmentUIManager>()
            .LogOutButtonClickedEvent.AddListener(OnClickLogoutButton);
        GameSparks.Core.GS.Reset();         // dont know how this works only here not in destroy. TODO: Reason to be explored later.
        userName = GUEST;
        playerId = null;
    }

    void OnDestroy()
    {
        LogOut();                   // We always LogOut here. TODO: Can be made more versatile during later stages.
        EnvironmentUIManagerGO.GetComponent<EnvironmentUIManager>()
            .LogInButtonClickedEvent.RemoveListener(OnClickLoginButton);
        EnvironmentUIManagerGO.GetComponent<EnvironmentUIManager>()
            .LogInButtonClickedEvent.RemoveListener(OnClickLogoutButton);
        // GameSparks.Core.GS.Reset();
    }

    private void OnClickLoginButton() {
        // Debug.Log("Login Button Clicked");
        string username = EnvironmentUIManagerGO.GetComponent<EnvironmentUIManager>()
            .GetUserNameInputField();
        string password = EnvironmentUIManagerGO.GetComponent<EnvironmentUIManager>()
            .GetPasswordInputField();
        AuthenticatePlayer(username, password);
    }

    private void LogIn(string id)
    {
        userName = EnvironmentUIManagerGO.GetComponent<EnvironmentUIManager>()
            .GetUserNameInputField();
        playerId = id;
        isAuthenticated = true;
        AuthenticationStateChangeEvent.Invoke();
    }

    private void OnClickLogoutButton()
    {
        LogOut();
    }

    private void LogOut()
    {
        GameSparks.Core.GS.Reset();         // TODO: Try Catch has to be used later for checking the network connection error.
        userName = GUEST;
        playerId = null;
        isAuthenticated = false;
        AuthenticationStateChangeEvent.Invoke();
    }

    private void AuthenticatePlayer(string username, string password)
    {
        new GameSparks.Api.Requests.AuthenticationRequest().SetUserName(username)
            .SetPassword(password).Send((response) => {
            if (!response.HasErrors)
            {
                
            }
            else
            {
                RegisterPlayer(username, password);
            }
        });
    }

    // We are making a presumption that diplay name is the same as username.
    private void RegisterPlayer(string username, string password)
    {   
        new GameSparks.Api.Requests.RegistrationRequest()
       .SetDisplayName(username)
       .SetPassword(password)
       .SetUserName(username)
       .Send((response) => {
           if (!response.HasErrors)
           {
               // Debug.Log("Registeration Successful");
               AuthenticatePlayer(username, password);  // This is for Disabling UI and SendingMatchRequest
            }
           else
           {
               // Debug.Log("There may be some network/server/alreadyregisteredusername side issues. Try authenticating again with a different username. Maybe check your internet connection as well.");
               // TODO: See the type of error here and write it descriptively afterwards. Username password can be empty as well so take care of this error handling here as well.
           }
       }
       );
    }

    public string GetUserName(){
        return userName;
    }

    public string GetPlayerId() {
        return playerId;
    }
}

