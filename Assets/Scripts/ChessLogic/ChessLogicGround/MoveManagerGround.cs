using UnityEngine;
using System.Collections;
using System;

public class MoveManagerGround : MonoBehaviour {

	public enum PlayerType {Human, AI};
	public PlayerType whitePlayerType;
	public PlayerType blackPlayerType;
	public AudioClip moveAudio;
	public AudioClip captureAudio;

	PlayerGround whitePlayer;
	PlayerGround blackPlayer;

	[HideInInspector]
	public bool whiteToPlay;

	ChessInputGround boardInput;
	public event Action<bool, ushort> OnMoveMade;
	public event Action<int, DefinitionsGround.ResultType> OnGameOver; // result (-1;0;1) (black wins; draw; white wins)
	MoveGeneratorGround moveGenerator = new MoveGeneratorGround();
	bool gameOver;

    bool testCheck;

    public void CreatePlayers() {
        boardInput = GetComponent<ChessInputGround>();

        HumanPlayerGround whiteHuman = null;
        HumanPlayerGround blackHuman = null;
        AIPlayerGround whiteAI = null;
        AIPlayerGround blackAI = null;

        if (blackPlayerType == PlayerType.Human) {
            ChessUIGround.instance.SetBoardOrientation(false);
            blackHuman = new HumanPlayerGround();
            boardInput.AddPlayer(blackHuman);
        } else {
            blackAI = new AIPlayerGround();
        }

        if (whitePlayerType == PlayerType.Human) {
            ChessUIGround.instance.SetBoardOrientation(true);
            whiteHuman = new HumanPlayerGround();
            boardInput.AddPlayer(whiteHuman);
            // FindObjectOfType<NotationInput>().SetPlayer(whiteHuman);
        } else {
            whiteAI = new AIPlayerGround();
        }

        whitePlayer = (PlayerGround)whiteHuman ?? (PlayerGround)whiteAI;
        blackPlayer = (PlayerGround)blackHuman ?? (PlayerGround)blackAI;

		whitePlayer.OnMove += OnMove;
		blackPlayer.OnMove += OnMove;

		whitePlayer.Init (true);
		blackPlayer.Init (false);

		whiteToPlay = BoardGround.IsWhiteToPlay ();
		RequestMove ();
	}

	void GameOver(int result, DefinitionsGround.ResultType resultType = DefinitionsGround.ResultType.NA) {
		if (!gameOver) {
			gameOver = true;
			if (OnGameOver != null) {
				OnGameOver (result, resultType);
			}
			whitePlayer.Deactivate ();
			blackPlayer.Deactivate ();
		}
	}

	public void Resign() {
		GameOver (-1, DefinitionsGround.ResultType.Resignation);
	}

	// draw requested by player
	public void Draw() {
		if (BoardGround.ThreefoldRepetition ()) {
			GameOver (0, DefinitionsGround.ResultType.Repetition);
		} else if (BoardGround.halfmoveCountSinceLastPawnMoveOrCap >= 100) {
			GameOver(0, DefinitionsGround.ResultType.FiftyMoveRule);
		}
	}

	public void TimeOut(bool white) {
		GameOver((white)?-1:1, DefinitionsGround.ResultType.Timeout);
	}

	void OnMove(ushort move) {
		int moveToIndex = (move >> 7) & 127;
		int capturedPieceCode = BoardGround.boardArray [moveToIndex]; // get capture piece code
		if (capturedPieceCode == 0) {
			AudioSource.PlayClipAtPoint (moveAudio, Vector3.zero, 1f);
		} else {
			AudioSource.PlayClipAtPoint (captureAudio, Vector3.zero, 1f);
		}

		BoardGround.MakeMove (move, true);

		if (OnMoveMade != null) {
			OnMoveMade(whiteToPlay, move);
		}

		whiteToPlay = !whiteToPlay;

		// detect mate/stalemate
		if (moveGenerator.PositionIsMate ()) {
			GameOver (((whiteToPlay) ? -1 : 1), DefinitionsGround.ResultType.Checkmate); // player is mated
			
		} else if (moveGenerator.PositionIsStaleMate ()) {
			GameOver (0, DefinitionsGround.ResultType.Stalemate); // player is mated
		} else if (moveGenerator.InsuffientMatingMaterial ()) {
			GameOver(0, DefinitionsGround.ResultType.InsufficientMaterial);
		}
		else {

			if (whitePlayerType == PlayerType.AI && blackPlayerType == PlayerType.AI) {
				StartCoroutine (RequestMoveCoroutine (.15f)); // force delay between moves when two AI are playing
			} else {
				RequestMove ();
			}
		}
	}

	void RequestMove() {
		if (whiteToPlay) {
			whitePlayer.RequestMove ();
		} else {
			blackPlayer.RequestMove ();
		}
	}

	
	IEnumerator RequestMoveCoroutine(float delay) {
		yield return new WaitForSeconds (delay);
		if (whiteToPlay) {
			whitePlayer.RequestMove ();
		} else {
			blackPlayer.RequestMove ();
		}
	}

	void Update() {
		whitePlayer.Update ();
		blackPlayer.Update ();
	}


}
