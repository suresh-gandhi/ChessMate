using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GManager : MonoBehaviour {

    void Awake() {
        // CGSWaypoint.instance.OnOccupied.AddListener(ChessGameStart);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ChessGameStart() {
        Debug.Log("Start the chess game");
    }
}
