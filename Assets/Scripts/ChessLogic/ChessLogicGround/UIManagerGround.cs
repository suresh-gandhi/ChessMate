using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManagerGround : MonoBehaviour {

	public GameObject clocks;
	public GameObject pgnViewer;
	public GameObject notationInputSmall;
	public GameObject fullBlindfoldUI;
	public GameObject reviewUI;
	public Text messageUI;

	public Button showBoardButton;
	public Text showBoardRemainingText;
	public GameObject reviewGameButton;
	public GameObject copyPGNButton;

	bool unhideBoardUIEveryXMoves;
	int remainingBoardRevealCount;
	bool resetUINextMove;

	int currentMessagePriority;
	bool currentMessageIsMoveOld;
	bool autoClearMessage;
	int messageCreatedOnMovePly;

	bool gameOver;
	bool inReviewMode;
	bool hideBoard;
	bool showingBoard;

	string resultStringShorthand;

	void Awake() {
		messageUI.text = "";
	}

	void Start() {
		FindObjectOfType<MoveManagerGround> ().OnMoveMade += OnMoveMade;
		FindObjectOfType<MoveManagerGround> ().OnGameOver += OnGameOver;
		Reset ();
	}

	public void SetMessage(string message, int priority, bool autoClearOnMove) {
		if (currentMessageIsMoveOld || priority > currentMessagePriority) {
			currentMessagePriority = priority;
			currentMessageIsMoveOld = false;
			messageUI.text = message;

			autoClearMessage = autoClearOnMove;
			messageCreatedOnMovePly = BoardGround.halfmoveCount;

		}
	}

	[ContextMenu("Set UI")]
	void Initialize() {
		ChessUIGround.instance.CreateBoardUI ();
		BoardGround.SetPositionFromFen (DefinitionsGround.startFen);

		Reset ();
	}

	void Reset() {
		HideAllUIElements ();
		hideBoard = false;

		switch (GameManagerGround.instance.gameMode) {
		case GameManagerGround.GameMode.Regular:
			SetVisibilityUIElements(notationInputSmall);
			SetVisibilityUIElements(true, clocks, pgnViewer);
			ChessUIGround.instance.SetHightlightLegalMoves(true);
			ChessUIGround.instance.SetBoardVisibility(true);
			ChessUIGround.instance.SetPieceVisiblity(true);
			break;
		case GameManagerGround.GameMode.BlindfoldWithBoard01:
			ChessUIGround.instance.SetPieceVisiblity(false);
			unhideBoardUIEveryXMoves = true;
			remainingBoardRevealCount = 6;
			SetVisibilityUIElements(true, pgnViewer, showBoardButton.gameObject);

			break;
		case GameManagerGround.GameMode.BlindfoldWithBoard02:
			ChessUIGround.instance.SetPieceVisiblity(false);
			unhideBoardUIEveryXMoves = true;
			remainingBoardRevealCount = 2;
			ChessUIGround.instance.SetHightlightLegalMoves(false);
			SetVisibilityUIElements(true, pgnViewer, showBoardButton.gameObject);
			break;
		case GameManagerGround.GameMode.BlindfoldSansBoard01:
			ChessUIGround.instance.SetBoardVisibility(false);
			unhideBoardUIEveryXMoves = true;
			remainingBoardRevealCount = 2;
			ChessUIGround.instance.SetHightlightLegalMoves(false);
			SetVisibilityUIElements(true, pgnViewer, notationInputSmall, showBoardButton.gameObject);
			hideBoard = true;
			break;
		case GameManagerGround.GameMode.BlindfoldSansBoard02:
			ChessUIGround.instance.SetBoardVisibility(false);
			SetVisibilityUIElements(true, notationInputSmall);
			hideBoard = true;
			//SetVisibilityUIElements(true, fullBlindfoldUI);
			break;
		}
		showBoardRemainingText.text = "(" + remainingBoardRevealCount + " remaining)";
	
	}

	void HideAllUIElements() {
		SetVisibilityUIElements (false, clocks, pgnViewer, notationInputSmall, fullBlindfoldUI, reviewUI,showBoardButton.gameObject, reviewGameButton, copyPGNButton);
	}

	void SetVisibilityUIElements(bool visibility, params GameObject[] objects) {
		for (int i =0; i <objects.Length; i ++) {
			objects[i].SetActive(visibility);
		}
	}
	
	public void EnterReviewMode() {
		if (!inReviewMode) {
			GameManagerGround.instance.gameMode = GameManagerGround.GameMode.Regular; // switch to regular mode for game review
			Reset();

			inReviewMode = true;
			SetVisibilityUIElements (true, reviewUI,copyPGNButton);
			SetVisibilityUIElements (false, clocks, notationInputSmall, reviewGameButton);
			FindObjectOfType<GameReviewerGround>().Init();
		}
	}

	public void ShowBoard() {
		if (remainingBoardRevealCount > 0 && !showingBoard) {
			showingBoard = true;
			remainingBoardRevealCount --;
			showBoardRemainingText.text = "(" + remainingBoardRevealCount + " remaining)";

			ChessUIGround.instance.SetBoardVisibility (true);
			ChessUIGround.instance.SetPieceVisiblity (true);
			resetUINextMove = true;
		} else if (remainingBoardRevealCount == 0) {
			showBoardButton.interactable = false;
		}
	}

	void OnMoveMade(bool white, ushort move) {
		currentMessageIsMoveOld = true;
		if (autoClearMessage) {
			if (BoardGround.halfmoveCount > messageCreatedOnMovePly) {
				messageUI.text = "";
			}
		}

		if (unhideBoardUIEveryXMoves && white && resetUINextMove) {
			showingBoard = false;
			ChessUIGround.instance.SetPieceVisiblity(false);
			if (hideBoard) {
				ChessUIGround.instance.SetBoardVisibility(false);
			}
		}
	}

	void OnGameOver(int result, DefinitionsGround.ResultType type) {
		gameOver = true;
		SetVisibilityUIElements (false, notationInputSmall, clocks, showBoardButton.gameObject);
		SetVisibilityUIElements (true, reviewGameButton);

		string resultString = "Game over";
		if (result == -1) {
			resultStringShorthand = "0-1";
			resultString = "Black wins";
		}
		else if (result == 1) {
			resultStringShorthand = "1-0";
			resultString = "White wins";
		}
		if (result != 0) { // someone has won
			if (type == DefinitionsGround.ResultType.Checkmate) {
				resultString += " by checkmate";
			}
			else if (type == DefinitionsGround.ResultType.Resignation) {
				resultString += " by resignation";
			}
			else if (type == DefinitionsGround.ResultType.Timeout) {
				resultString += " on time";
			}
		} else { // draw
			resultStringShorthand = "1/2-1/2";
			resultString = "Game drawn";

			if (type == DefinitionsGround.ResultType.Repetition) {
				resultString += " by repetition";
			}
			else if (type == DefinitionsGround.ResultType.InsufficientMaterial) {
				resultString += "; insufficient mating material";
			}
			else if (type == DefinitionsGround.ResultType.FiftyMoveRule) {
				resultString += "; 50 move rule";
			}
			else if (type == DefinitionsGround.ResultType.Stalemate) {
				resultString += " by stalemate";
			}
		}


		SetMessage (resultString, 10, false);
	}

	public void CopyPGNToClipboard() {
		string dateString = System.DateTime.Today.Year + "." + System.DateTime.Today.Month + "." + System.DateTime.Today.Day;
		string modeName = "Regular mode";
		if (GameManagerGround.gameModeIndex > 0) {
			modeName = "Blindfold Training; level " + (GameManagerGround.gameModeIndex);
		}

		string pgn = "";

		pgn += "[Event \"" +modeName+ "\"]\n";
		pgn += "[Site \"http://sebastian.itch.io/blindfoldchess\"]\n";
		pgn += "[Date \"" + dateString + "\"]\n";
		pgn += "[White \"Human\"]\n";
		pgn += "[Black \"Computer\"]\n";
		pgn += "[Result \"" + resultStringShorthand + "\"]\n";

		pgn += "\n" + PGNDisplayGround.GetGamePGN ();
		if (pgn [pgn.Length - 1] != ' ') {
			pgn += " ";
		}
		pgn += resultStringShorthand;

		TextEditor t = new TextEditor ();
		t.content = new GUIContent (pgn);
		t.SelectAll ();
		t.Copy ();
		Debug.Log (t.content.text);
	}
}
