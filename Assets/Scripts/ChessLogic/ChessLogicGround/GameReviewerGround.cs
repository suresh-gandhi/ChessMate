using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameReviewerGround : MonoBehaviour {

	List<ushort> moveList = new List<ushort>();
	int index;

	void Start() {
		FindObjectOfType<MoveManagerGround> ().
            OnMoveMade += OnMove;
	}

	void OnMove(bool white, ushort move) {
		moveList.Add (move);
	}

	public void Init() {
		BoardGround.SetPositionFromFen (DefinitionsGround.startFen, true);
	}

	public void Next() {
		if (index < moveList.Count) {
			BoardGround.MakeMove (moveList [index], true);
			index ++;
		}
	}

	public void Previous() {
		if (index > 0) {
			index --;
			BoardGround.UnmakeMove (moveList [index], true);
		}
	}

	public void First() {
		while (index > 0) {
			index --;
			BoardGround.UnmakeMove (moveList [index], true);
		}
	}

	public void Last() {
		while (index < moveList.Count) {
			BoardGround.MakeMove (moveList [index], true);
			index ++;
		}
	}
}
