using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightAI : MonoBehaviour {

    private void Start()
    {
        ChessAIGameManager.instance.OnGameStarted.AddListener(SwitchOn);
        ChessAIGameManager.instance.OnGameEnded.AddListener(SwitchOff);
    }

    private void SwitchOn() {
        GetComponent<Light>().enabled = true;
        StartCoroutine("SpotLightStartAnimation");
    }

    private void SwitchOff() {
        GetComponent<Light>().enabled = false;
    }

    IEnumerator SpotLightStartAnimation() {
        for (int i = 1; i <= 240; i += 2) {
            GetComponent<Light>().spotAngle = i / 4;
            yield return null;
        }
    }

}
