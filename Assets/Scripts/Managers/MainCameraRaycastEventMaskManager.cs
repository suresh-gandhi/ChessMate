using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraRaycastEventMaskManager : MonoBehaviour {

    [SerializeField]
    private GameObject EnvironmentUIManagerGO;

    private static MainCameraRaycastEventMaskManager Instance;

    private AuthenticationManager authManager;
    private GvrPointerPhysicsRaycaster gvrPointerPhysicsRaycaster;

    private int presentlayerMaskBitConfig = ~0;

    private int authenticatedlayerMaskBitConfig = 0;  //  TODO: I can give them more meaningful values initially. This has to be thought upon later.    
    private int notAuthenticatedlayerMaskBitConfig = 0;     // TODO: I can given them more meaningful values initially. This has to be thought upon later.

    void Awake() {
        // This is to ensure the singleton behaviour.
        if (!Instance){
            Instance = this;
        }
        else {
            Destroy(this.gameObject);
        }
    }

	void Start () {
        // Getting the reference to the main camera whose gvrpointerphysicsraycaster's 
        // layermask we will change.
        if (Camera.main)
        {
            gvrPointerPhysicsRaycaster =
            Camera.main.gameObject.GetComponent<GvrPointerPhysicsRaycaster>();
            if (!gvrPointerPhysicsRaycaster)
            {
                Debug.Log("GVRPointerRaycaster is missing from the scene.");
            }
            else {
                string[] layerParams = {"AuthenticatedObjects", "Piece", "InGameGroundMultiplayer", "AIPiece" };   // Other layers like InGameTopMultiplayer, InGameAI shall also be removed and other pieces as well.
                ChangeGVRPointerPhysicsRaycasterEventMask(layerParams, false);
                notAuthenticatedlayerMaskBitConfig = presentlayerMaskBitConfig;
            }
        }
        else {
            Debug.Log("Main Camera is missing from the scene.");
        }
        // Subscribing to all the game state events.
        // The first event, for now, which tells when the authentication state changes.
        authManager = AuthenticationManager.Instance;
        authManager.AuthenticationStateChangeEvent.AddListener(AuthenticationStateChangeResponder);

        MultiplayerTableGround.Instance.OnMultiplayerTableGroundClicked
            .AddListener(MultiplayerTableGroundClickedListener);
        EnvironmentUIManagerGO.GetComponent<EnvironmentUIManager>()
            .InGameGroundMultiplayerExitButtonClickedEvent
            .AddListener(ExitGameMultiplayerListener);

        MultiplayerManager.Instance.ChallengeStartedEvent
            .AddListener(ChallengeTurnResponder);
        MultiplayerManager.Instance.ChallengeTurnTakenEvent
            .AddListener(ChallengeTurnResponder);

        ChessAIGameManager.instance.OnGameStarted
            .AddListener(AIGameStartedListener);
    }

    void OnDestroy()
    {
        authManager.AuthenticationStateChangeEvent
            .RemoveListener(AuthenticationStateChangeResponder);

        MultiplayerTableGround.Instance.OnMultiplayerTableGroundClicked
            .RemoveListener(MultiplayerTableGroundClickedListener);
        EnvironmentUIManagerGO.GetComponent<EnvironmentUIManager>()
            .InGameGroundMultiplayerExitButtonClickedEvent
            .RemoveListener(ExitGameMultiplayerListener);

        MultiplayerManager.Instance.ChallengeStartedEvent
            .RemoveListener(ChallengeTurnResponder);
        MultiplayerManager.Instance.ChallengeTurnTakenEvent
            .RemoveListener(ChallengeTurnResponder);

        ChessAIGameManager.instance.OnGameStarted
        .RemoveListener(AIGameStartedListener);
    }

    void AuthenticationStateChangeResponder()
    {
        if (authManager.IsAuthenticated())
        {
            string[] layerParams = { "AuthenticatedObjects" };
            ChangeGVRPointerPhysicsRaycasterEventMask(layerParams, true);
            authenticatedlayerMaskBitConfig = presentlayerMaskBitConfig;

        }
        else
        {
            string[] layerParams = { "AuthenticatedObjects", "Piece", "InGameGroundMultiplayer", "AIPiece" };  // Other layers like InGameTopMultiplayer, InGameAI shall also be removed and other pieces as well.      
            ChangeGVRPointerPhysicsRaycasterEventMask(layerParams, false);
        }
    }

    void MultiplayerTableGroundClickedListener()
    {
        presentlayerMaskBitConfig = 0;   // deactivating interaction with everything.
        string[] layerParams = { "InGameGroundMultiplayer" };
        ChangeGVRPointerPhysicsRaycasterEventMask(layerParams, true);
    }    
    void ExitGameMultiplayerListener()
    {
        // This would always be the same state or the same bit config which is the exact same as we did initially just after authenticating.
        presentlayerMaskBitConfig = authenticatedlayerMaskBitConfig;
        gvrPointerPhysicsRaycaster.eventMask = presentlayerMaskBitConfig;
    }// Only one function would suffice for both the tables as the end state would be the same(authenticated) for both of them.

    private void ChallengeTurnResponder() {
        string[] layerParams = { "Piece" };
        if (MultiplayerManager.Instance.GetIsMyTurn()){            
            ChangeGVRPointerPhysicsRaycasterEventMask(layerParams, true);
        }
        else {
            ChangeGVRPointerPhysicsRaycasterEventMask(layerParams, false);
        }
    }

    // Activates/Deactivates layerParams upon the previous configuration.
    void ChangeGVRPointerPhysicsRaycasterEventMask(string[] layerParams, bool onSwitch) {

        int layerParamsBitConfig =
                    LayerMask.GetMask(layerParams);

        if (onSwitch){
            presentlayerMaskBitConfig |= layerParamsBitConfig;
        }
        else {
            presentlayerMaskBitConfig &= ~layerParamsBitConfig;
        }

        gvrPointerPhysicsRaycaster.eventMask = presentlayerMaskBitConfig;
    }

    void AIGameStartedListener() {
        string[] layerParams = { "AIPiece" };
        ChangeGVRPointerPhysicsRaycasterEventMask(layerParams, true);
    }

    void ExitGameAIListener()
    {
        // This would depend on whether it is currently authenticated or not but it is always the same for these two cases.
        if (AuthenticationManager.Instance.IsAuthenticated())
        {
            presentlayerMaskBitConfig = authenticatedlayerMaskBitConfig;
        }
        else
        {
            presentlayerMaskBitConfig = notAuthenticatedlayerMaskBitConfig;
        }
    }

}


