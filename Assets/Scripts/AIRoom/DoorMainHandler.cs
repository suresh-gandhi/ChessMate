using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DoorMainHandler : MonoBehaviour {

    private Animator doorMainController;

    private enum State {
        Opened,
        Closed
    }

    private State state;

    private void Awake()
    {
        doorMainController = GetComponent<Animator>();
        state = State.Closed;
    }

    public void OnClick_DoorMain() {
        // If it was earlier closed
        if (state == State.Closed)
        {
            state = State.Opened;
            Open();
        }
        // If it was earlier opened
        else {
            state = State.Closed;
            Close();
        }
    }


    private void Open() {
        // Debug.Log("Door is opening");
        if (doorMainController.GetCurrentAnimatorStateInfo(0).IsName("DoorIdleAnimation")){
            doorMainController.SetBool("doorOpened", true);
        }
    }

    private void Close() {
        // Debug.Log("Door is closing");
        if (doorMainController.GetCurrentAnimatorStateInfo(0).IsName("DoorOpenedAnimation")) {
            doorMainController.SetBool("doorOpened", false);
        }
    }
    
}
