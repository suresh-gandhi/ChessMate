using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSparksManagerSingleton : MonoBehaviour {

    private static GameSparksManagerSingleton Instance = null;

	void Awake () {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else {
            Destroy(this.gameObject);
        }
	}
}
