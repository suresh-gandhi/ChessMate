using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class _TestsGround : MonoBehaviour {

	void Start() {
		Debug.LogError ("Test");
		BoardGround.SetPositionFromFen (DefinitionsGround.startFen,true);

	}

	void Update() {
		Debug.LogError ("Test");
		BoardGround.MakeMove(new MoveGeneratorGround().GetMoves(false,false).GetMove(0),true);
		if (Input.GetKeyDown (KeyCode.Space)) {

			MoveGeneratorGround.Debug();
		}

	}
	

}
