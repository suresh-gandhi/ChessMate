using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChessAIGameManager : MonoBehaviour {

    public static ChessAIGameManager instance;

    [SerializeField]
    private GameObject BoardGraphicsGObject;

    [SerializeField]
    private GameObject ManagerGObject;

    [SerializeField]
    private GameObject InGameInteractionUI;

    [SerializeField]
    private GameObject MovesDisplayUI;

    [SerializeField]
    private GameObject MovesHeadingCanvas;

    public UnityEvent OnPreAIGameSetupDone = new UnityEvent();

    public UnityEvent OnGameStarted = new UnityEvent();
    public UnityEvent OnGameEnded = new UnityEvent();

    private void Awake()
    {
        if (instance == null) {
            instance = this;
        }
        else{
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        ComputerChair.instance.OnChairClicked.AddListener(Preprocessing);
        // ComputerAIScreen.instance.OnInitialInteractionCompleted.AddListener();
        ComputerAIScreen.instance.OnLoadingCompleted.AddListener(StartTheGame);
    }

    private void StartTheGame(){
        OnGameStarted.Invoke();
        // BoardGraphicsGObject.SetActive(true);        // This will be on from the start.
        ManagerGObject.GetComponent<ChessInput>().enabled = true;
        // ComputerScreenCanvasGObject.SetActive(true);  // Will see this later
        MovesDisplayUI.SetActive(true);
        InGameInteractionUI.SetActive(false);
        MovesHeadingCanvas.SetActive(true);
    }

    private void EndTheGame() {
        OnGameEnded.Invoke();
    }

    private void Preprocessing()
    {
        PreAIGameSetup();
        OnPreAIGameSetupDone.Invoke();
    }

    private void PreAIGameSetup() {
        // Changing the camera position
        Camera.main.transform.parent.transform.position = ComputerChair.instance.transform.GetChild(0).transform.position;
        // Disabling the box collider component of the chair
        ComputerChair.instance.transform.GetComponent<BoxCollider>().enabled = false;
        // Deactivating the waypoints game object
        Waypoints.instance.transform.gameObject.SetActive(false);
    }

}
