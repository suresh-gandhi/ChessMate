using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PGNDisplayGround : MonoBehaviour {

	public Text moveNumberUI;
	public Text moveNotationWhiteUI;
	public Text moveNotationBlackUI;
	public RectTransform contentBounds;
	public Scrollbar scroller;
	bool userControllingScrollbar;
	bool relinquishUserScrollbarControlNextMove;
	static string fullGamePGN;

	void Start() {
		fullGamePGN = "";
		moveNumberUI.text = "";
		moveNotationWhiteUI.text = "";
		moveNotationBlackUI.text = "";
		FindObjectOfType<MoveManagerGround> ().OnMoveMade += OnMove;
	}


	void OnMove(bool whiteMoved, ushort move) {

		if (relinquishUserScrollbarControlNextMove) {
			relinquishUserScrollbarControlNextMove = false;
			userControllingScrollbar = false;
		}

		if (whiteMoved) {
			fullGamePGN += (BoardGround.GetFullMoveCount () + 1) + ". " +  PGNReaderGround.NotationFromMove (move);
			moveNumberUI.text += (BoardGround.GetFullMoveCount () + 1) + ". \n";
			moveNotationWhiteUI.text += PGNReaderGround.NotationFromMove (move) + "\n";
		} else {
			fullGamePGN += " "  + PGNReaderGround.NotationFromMove (move) + " ";
			moveNotationBlackUI.text += PGNReaderGround.NotationFromMove (move) + "\n";

			if (BoardGround.GetFullMoveCount () > 14) {
				int size = -30 * (BoardGround.GetFullMoveCount ()-14);
				contentBounds.offsetMin = new Vector2 (contentBounds.offsetMin.x, size);
				contentBounds.offsetMax = new Vector2 (contentBounds.offsetMax.x, 0);
			}
		}
	}

	void LateUpdate() {
		if (!userControllingScrollbar) {
			scroller.value = 0;
		}
	}

	public void OnUseScrollbar() {
		relinquishUserScrollbarControlNextMove = false;
		userControllingScrollbar = true;
	}

	public void OnStopUsingScrollbar() {
		relinquishUserScrollbarControlNextMove = true;
	}

	public static string GetGamePGN() {
		return fullGamePGN;
	}


}
