using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EnvironmentUIManager : MonoBehaviour {

    // We have 2 UI for each Authentication and LogOut
    [SerializeField]
    private GameObject AuthenticationCanvas, LogOutCanvas,
        InGameGroundMultiplayerCanvas;
    [SerializeField]
    private Button LoginButton, LogoutButton,
        InGameGroundMultiplayerExitButton,
        InGameGroundMultiplayerMatchmakingButton;
    [SerializeField]
    private InputField usernameInputField, passwordInputField;

    [SerializeField]
    private Text loadingText, matchNotFoundText, matchFoundText;

    public UnityEvent LogInButtonClickedEvent = new UnityEvent();
    public UnityEvent LogOutButtonClickedEvent = new UnityEvent();
    public UnityEvent InGameGroundMultiplayerExitButtonClickedEvent
        = new UnityEvent();
    public UnityEvent InGameGroundMultiplayerMatchmakingButtonClickedEvent
        = new UnityEvent();
    private AuthenticationManager authManager;

    private string textInputFieldUsername = null;
    private string textInputFieldPassword = null;

    private Coroutine loadingCoroutine;
    private Coroutine matchNotFoundTextDisplayWorkCoroutine;
    private Coroutine challengeStartedDisplayWorkCoroutine;

    void Start () {
        LoggedOutStateDisplay();

        LoginButton.onClick.AddListener(OnClickLoginButton);
        LogoutButton.onClick.AddListener(OnClickLogoutButton);

        InGameGroundMultiplayerMatchmakingButton.onClick
            .AddListener(OnClickMatchmakingGroundMultiplayerButtonListener);
        InGameGroundMultiplayerExitButton.onClick
            .AddListener(ExitGameGroundMultiplayerListener);

        authManager = AuthenticationManager.Instance;
        authManager.AuthenticationStateChangeEvent
            .AddListener(AuthenticationStateChangeResponder);

        MultiplayerTableGround.Instance.OnMultiplayerTableGroundClicked
           .AddListener(MultiplayerTableGroundClickedListener);

        MultiplayerManager.Instance.MatchNotFoundEvent
            .AddListener(MatchNotFoundUIDisplay);
        MultiplayerManager.Instance.ChallengeStartedEvent
            .AddListener(ChallengeStartedUIDisplay);      
    }

    void OnDestroy()
    {
        LoginButton.onClick.RemoveListener(OnClickLoginButton);
        LogoutButton.onClick.RemoveListener(OnClickLogoutButton);

        InGameGroundMultiplayerMatchmakingButton.onClick
          .RemoveListener(OnClickMatchmakingGroundMultiplayerButtonListener);
        InGameGroundMultiplayerExitButton.onClick
          .RemoveListener(ExitGameGroundMultiplayerListener);

        authManager.AuthenticationStateChangeEvent
            .RemoveListener(AuthenticationStateChangeResponder);

        MultiplayerTableGround.Instance.OnMultiplayerTableGroundClicked
           .RemoveListener(MultiplayerTableGroundClickedListener);

        MultiplayerManager.Instance.MatchNotFoundEvent
           .RemoveListener(MatchNotFoundUIDisplay);
        MultiplayerManager.Instance.ChallengeStartedEvent
           .RemoveListener(ChallengeStartedUIDisplay);
    }

    void OnClickLoginButton()
    {
        textInputFieldUsername = usernameInputField
            .GetComponent<InputField>().text;
        textInputFieldPassword = passwordInputField.
            GetComponent<InputField>().text;
        LogInButtonClickedEvent.Invoke();
    }
    private void LoggedInStateDisplay()
    {
        LogOutCanvas.SetActive(true);
        AuthenticationCanvas.SetActive(false);
        Debug.Log(authManager.GetUserName());           // TODO: Convert this debug to screen space ui display.   
    }
    void OnClickLogoutButton()
    {
        LogOutButtonClickedEvent.Invoke();
    }   
    private void LoggedOutStateDisplay()
    {
        LogOutCanvas.SetActive(false);
        AuthenticationCanvas.SetActive(true);
        // Debug.Log(authManager.GetUserName());           // TODO: Convert this debug to screen space ui display.
        usernameInputField.GetComponent<InputField>().Select();
        usernameInputField.GetComponent<InputField>().text = "";
        passwordInputField.GetComponent<InputField>().Select();
        passwordInputField.GetComponent<InputField>().text = "";
        textInputFieldUsername = null;
        textInputFieldPassword = null;
    }

    void OnClickMatchmakingGroundMultiplayerButtonListener()
    {
        InGameGroundMultiplayerMatchmakingButton.gameObject.SetActive(false);
        loadingCoroutine = StartCoroutine(LoadingCoroutine());
        InGameGroundMultiplayerMatchmakingButtonClickedEvent.Invoke();
    }
    IEnumerator LoadingCoroutine()
    {
        loadingText.gameObject.SetActive(true);
        while (true)
        {
            for (int i = 30; i >= 0; i--)
            {
                yield return null;
                loadingText.color = new Color(0, 0, 0, i / 30);

            }
            for (int i = 0; i <= 30; i++)
            {
                yield return null;
                loadingText.color = new Color(0, 0, 0, i / 30);
            }
        }
    }
    void ExitGameGroundMultiplayerListener()
    {
        InGameGroundMultiplayerExitButtonClickedEvent.Invoke();
        ActivateExternalUIsMultiplayer();
        DeactivateMultiplayerTableGroundInGameUIs();
        // Challenge end karna hai.
    }
    void DeactivateMultiplayerTableGroundInGameUIs()
    {
        InGameGroundMultiplayerCanvas.SetActive(false);
        InGameGroundMultiplayerMatchmakingButton.gameObject
        .SetActive(false);
        InGameGroundMultiplayerExitButton.gameObject
            .SetActive(false);
        loadingText.gameObject.SetActive(false);
        matchFoundText.gameObject.SetActive(false);
        matchNotFoundText.gameObject.SetActive(false);
    }

    void AuthenticationStateChangeResponder()
    {
        if (authManager.IsAuthenticated())
        {
            LoggedInStateDisplay();
        }
        else
        {
            LoggedOutStateDisplay();
        }
    }
    void MultiplayerTableGroundClickedListener()
    {
        DeactivateExternalUIsMultiplayer();
        ActivateMultiplayerTableGroundInGameUIs();
    }
    void ActivateMultiplayerTableGroundInGameUIs()
    {
        InGameGroundMultiplayerCanvas.SetActive(true);
        InGameGroundMultiplayerMatchmakingButton.gameObject
            .SetActive(true);
        InGameGroundMultiplayerExitButton.gameObject
            .SetActive(true);
        loadingText.gameObject.SetActive(false);
        matchFoundText.gameObject.SetActive(false);
        matchNotFoundText.gameObject.SetActive(false);
    }

    void MatchNotFoundUIDisplay()
    {
        loadingText.gameObject.SetActive(false);
        matchNotFoundTextDisplayWorkCoroutine =
            StartCoroutine(MatchNotFoundDisplayWork());
    }
    IEnumerator MatchNotFoundDisplayWork()
    {
        matchNotFoundText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        matchNotFoundText.gameObject.SetActive(false);
        InGameGroundMultiplayerMatchmakingButton.gameObject.SetActive(true);
    }
    void ChallengeStartedUIDisplay()
    {
        loadingText.gameObject.SetActive(false);
        challengeStartedDisplayWorkCoroutine = StartCoroutine(ChallengeStartedDisplayWork());

    }
    IEnumerator ChallengeStartedDisplayWork()
    {
        matchFoundText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        matchFoundText.gameObject.SetActive(false);
    }

    void DeactivateExternalUIsMultiplayer()
    {
        LogOutCanvas.SetActive(false);
    }
    void ActivateExternalUIsMultiplayer()
    {
        LogOutCanvas.SetActive(true);
    }

    public string GetUserNameInputField(){
        return textInputFieldUsername;
    }
    public string GetPasswordInputField(){
        return textInputFieldPassword;
    }
	
}
