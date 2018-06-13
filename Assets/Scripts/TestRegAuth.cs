using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestRegAuth : MonoBehaviour {

    public InputField userNameIF, passwordIF;
    public GameObject testRegAuthUIGameObject;

    public GameObject boardGraphicsGameObject;
    public GameObject ManagerGameObject;

    private string playerId;

    void Awake() {
        GameSparks.Api.Messages.ChallengeStartedMessage.Listener += OnChallengeStarted;
        GameSparks.Api.Messages.ChallengeTurnTakenMessage.Listener += OnChallengeTurnTaken;
    }

    public void OnClick_LogInButton() {
        AuthenticatePlayer();
    }

    void AuthenticatePlayer() {
        new GameSparks.Api.Requests.AuthenticationRequest().SetUserName(userNameIF.text).SetPassword(passwordIF.text).Send((response) => {
            if (!response.HasErrors)
            {
                Debug.Log("Player Authenticated...");
                playerId = response.UserId;
                DisableUI();
                SendMatchMakingRequest();
            }
            else
            {
                Debug.Log("Error Authenticating Player...");
                RegisterPlayer();
            }
        });
    }

    void RegisterPlayer() {
         new GameSparks.Api.Requests.RegistrationRequest()
        .SetDisplayName(userNameIF.text)
        .SetPassword(passwordIF.text)
        .SetUserName(userNameIF.text)
        .Send((response) => {
          if (!response.HasErrors) {
              Debug.Log("Player Registered");
                AuthenticatePlayer();  // This is for Disabling UI and SendingMatchRequest
            }
          else
            {
              Debug.Log("Error Registering Player");
            }
            }
        );
    }

    void DisableUI() {
        testRegAuthUIGameObject.SetActive(false);
    }

    void SendMatchMakingRequest() {
        new GameSparks.Api.Requests.MatchmakingRequest()
        .SetMatchShortCode("Test_Match")
        .SetSkill(0)
        .Send(OnMatchmakingSuccess, OnMatchmakingError);
    }

    private void OnMatchmakingSuccess(GameSparks.Api.Responses.MatchmakingResponse response)
    {
        Debug.Log("On Matchmaking Success");
    }

    private void OnMatchmakingError(GameSparks.Api.Responses.MatchmakingResponse response)
    {
        Debug.Log("OnMatchMaking Error");
    }

    private void OnChallengeStarted(GameSparks.Api.Messages.ChallengeStartedMessage message)
    {
        Debug.Log("On challenge started");
        boardGraphicsGameObject.SetActive(true);
        ManagerGameObject.SetActive(true);
        
        // string challengeId = message.Challenge.ChallengeId;
        // If it is white player
        if ( playerId == message.Challenge.Challenger.Id )            // It means that we are the challenger(white)
        {
         
        }
        else  // If it is black player                              
        {
            // Change the orientation
            boardGraphicsGameObject.transform.Rotate(new Vector3(0, 180f, 0));
            // Disable the Chess Input Script of Manager
            ManagerGameObject.GetComponent<ChessInput>().enabled = false;
        }
        //ChallengeStarted.Invoke();
        
    }


    void OnChallengeTurnTaken(GameSparks.Api.Messages.ChallengeTurnTakenMessage message) {

    }

}
