using UnityEngine;
using System.Collections;
using System;

public class GameManagerGround : MonoBehaviour {

	MoveManagerGround playerManager;
	public enum GameMode {Regular, BlindfoldWithBoard01, BlindfoldWithBoard02, BlindfoldSansBoard01, BlindfoldSansBoard02};
	public static int gameModeIndex;
	public static bool gameModeIndexSet;

	[Header("Game mode:")]
	public GameMode gameMode;

	[Space(15)]
	public bool regenerateOpeningBook;
	public bool useOpeningBook;
	public bool useTestPosition;

	static GameManagerGround myInstance;

	public static GameManagerGround instance {
		get {
            if (myInstance == null)
            {
                myInstance = FindObjectOfType<GameManagerGround>();
            }
            else {
                
            }
			return myInstance;
		}
	}
	
	void Start () {
		if (gameModeIndexSet) {
			gameMode = (GameMode)gameModeIndex;
		}
		if (gameMode == GameMode.Regular) {
			// GetComponent<ClockGround>().StartClock();
		}

		BoardGround.SetPositionFromFen (DefinitionsGround.gameStartFen,true);

		ZobristKeyGround.Init ();
		EvaluationGround.Init ();
		if (regenerateOpeningBook) {
			OpeningBookGeneratorGround.GenerateBook ();
		}
		if (useOpeningBook) {
			OpeningBookReaderGround.Init ();
		}

		playerManager = GetComponent<MoveManagerGround> ();

		playerManager.CreatePlayers ();

		BoardGround.SetPositionFromFen (DefinitionsGround.gameStartFen,true);

	}


	public void ReturnToMenu() {
		Application.LoadLevel ("Menu");
	}

}
