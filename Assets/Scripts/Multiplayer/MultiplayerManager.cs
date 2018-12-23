using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Api.Messages;
using GameSparks.Core;


public class MultiplayerManager : MonoBehaviour
{

    [SerializeField]
    private GameObject EnvironmentUIManagerGO;

    public static MultiplayerManager Instance;

    //public UnityEvent MatmakingFoundDummyEvent = new UnityEvent();
    //public UnityEvent MatchFoundEvent = new UnityEvent();
    public UnityEvent ChallengeStartedEvent = new UnityEvent();
    public UnityEvent MatchNotFoundEvent = new UnityEvent();
    public UnityEvent ChallengeTurnTakenEvent = new UnityEvent();

    private bool isMyTurn;
    private bool isWhite;
    private bool isChallengeActive;

    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        EnvironmentUIManagerGO.GetComponent<EnvironmentUIManager>()
            .InGameGroundMultiplayerMatchmakingButtonClickedEvent
            .AddListener(MatchmakingButtonClickedResponder);

            MatchNotFoundMessage.Listener += OnMatchNotFound;
        ChallengeStartedMessage.Listener += OnChallengeStarted;
        ChallengeTurnTakenMessage.Listener += OnChallengeTurnTaken;
    }

    void OnDestroy() {
        EnvironmentUIManagerGO.GetComponent<EnvironmentUIManager>()
            .InGameGroundMultiplayerMatchmakingButtonClickedEvent
            .RemoveListener(MatchmakingButtonClickedResponder);

        MatchNotFoundMessage.Listener -= OnMatchNotFound;
        ChallengeStartedMessage.Listener -= OnChallengeStarted;
        ChallengeTurnTakenMessage.Listener -= OnChallengeTurnTaken;
    }

    private void MatchmakingButtonClickedResponder()
    {
        MatchmakingRequest request = new MatchmakingRequest();
        request.SetMatchShortCode("Test_Match");
        request.SetSkill(0);
        request.Send(OnMatchmakingResponseSuccess, OnMatchmakingResponseError);
    }
    private void OnMatchmakingResponseSuccess(MatchmakingResponse response)
    {
        Debug.Log("OnMatchmakingResponseSuccess");

    }
    private void OnMatchmakingResponseError(MatchmakingResponse response)
    {
        Debug.Log("OnMatchmakingResponseError");
        MatchNotFoundEvent.Invoke();
    }

    private void OnMatchNotFound(MatchNotFoundMessage message) {
        Debug.Log("OnMatchNotFound");
        MatchNotFoundEvent.Invoke();
    }
    private void OnChallengeStarted(ChallengeStartedMessage message){
        Debug.Log("OnChallengeStarted");
        isChallengeActive = true;
        // more work todo in chessinput etc.
        // Show animation maybe same as AIRoom
        // get turn variable from challenge message
        if (message.Challenge.NextPlayer == AuthenticationManager
            .Instance.GetPlayerId()){
            isWhite = true;
            isMyTurn = true;
            Debug.Log("I am white and it is my turn");
        }
        else {
            isWhite = false;
            isMyTurn = false;
            Debug.Log("I am black and it is not my turn");
        }
        ChallengeStartedEvent.Invoke();
        // accordingly rotate chess board and display UI that you are white black
        // OR move camera's transform to opposite position

        //**** make chessinput(OR piece layer) active according to turn too


        // Once right player took a move:- 
        // Call Move function of this script from line 50 of HumanPlayerGround
        // Update turn variable also in Move function
        // also add game ending code in exit buttonclickedevent
    }
    private void OnChallengeTurnTaken(ChallengeTurnTakenMessage message) {
        isMyTurn = !isMyTurn;
        ChallengeTurnTakenEvent.Invoke();
    }

    private void OnChallengeWon(ChallengeWonMessage message){
        Debug.Log("Inside OnChallengeWon(TODO)");
        isChallengeActive = false;
    }
    private void OnChallengeLost(ChallengeLostMessage message) {
        Debug.Log("Inside OnChallengeLost(TODO)");
        isChallengeActive = false;
    }

    public bool GetIsMyTurn() {
        return isMyTurn;
    }
    public bool GetIsWhite(){
        return isWhite;
    }
    public bool GetIsChallengeActive() {
        return isChallengeActive;
    }

}
    //IEnumerator MatchmakingRequestDummyCoroutine() {
    //    Debug.Log("Before sending the matchmaking request");
    //    yield return new WaitForSeconds(5.0f);
    //    Debug.Log("Just after getting the matchmaking request");
    //    MatmakingFoundDummyEvent.Invoke();
    //}

// Subscribe to InGameGroundMultiplayerMatchmakingButtonClickedEvent of EUIM
// To send matchmaking request and show loading loading and dissapear InGameGroundMultiplayerCanvas
// If challenge started then
//                     ENV UI MANAGER                  
//                          1. loading loading disappear
//                          2  matchmaking button deactivate because already in match
//                          3. UI Flow custom wala kuch dimaag lagakar
//                     RAYCASTING MANAGER
//                          1. piece activate baaki no disturbance         
//                     MISCELLANEOUS 
//                          1. display things like activating stop watch
//                          2. lighting effects
// Else if match not found then
//                     sirf vo jo function hai uska ulta + inform
//                     karna hai user ko ki match nahi mila hai
//                     - pehle matchnotfound dikhao phir canvas active
//
