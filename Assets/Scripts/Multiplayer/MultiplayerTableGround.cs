using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultiplayerTableGround : MonoBehaviour {

    public static MultiplayerTableGround Instance;

    public UnityEvent OnMultiplayerTableGroundClicked = new UnityEvent();

    void Awake() {
        if(!Instance){
            Instance = this;
        }
        else{
            Destroy(this.gameObject);
        }
    }

    public void OnClickMultiplayerTableGround() {
        OnMultiplayerTableGroundClicked.Invoke();
    }

}
