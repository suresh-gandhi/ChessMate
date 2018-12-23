using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraTeleportation : MonoBehaviour {

    [SerializeField]
    private GameObject EnvironmentUIManagerGO;

    [SerializeField]
    private Transform MultiplayerGroundPlayPosition, 
        GroundMultiplayerExitTransform;

    [SerializeField]
    private Transform MultiplayerTopPlayPosition;

    [SerializeField]
    private Transform AIPlayPosition;

    void Start () {
        MultiplayerTableGround.Instance.OnMultiplayerTableGroundClicked
             .AddListener(MultiplayerTableGroundClickedListener);
        EnvironmentUIManagerGO.GetComponent<EnvironmentUIManager>()
            .InGameGroundMultiplayerExitButtonClickedEvent.
            AddListener(ExitMultiplayerGroundClickedListener);
	}

    void MultiplayerTableGroundClickedListener() {
        if (!Camera.main)
        {
            Debug.Log("Main camera doesn't exist and it has to be " +
                "handled through an exception.");
        }
        else
        {
            Camera.main.transform.parent.transform.position
                = MultiplayerGroundPlayPosition.transform.position;
        }
    }

    void ExitMultiplayerGroundClickedListener() {
        if (!Camera.main)
        {
            Debug.Log("Main camera doesn't exist and it has to be " +
                "handled through an exception.");
        }
        else
        {
            Camera.main.transform.parent.transform.position
                = GroundMultiplayerExitTransform.position;
        }
    }

    void MultiplayerTableTopClickedListener()
    {
        if (!Camera.main)
        {
            Debug.Log("Main camera doesn't exist and it has to be " +
                "handled through an exception.");
        }
        else
        {
            // TODO
        }
    }

    void AITableClickedListener()
    {
        if (!Camera.main)
        {
            Debug.Log("Main camera doesn't exist and it has to be " +
                "handled through an exception.");
        }
        else
        {
            // TODO
        }
    }
}
