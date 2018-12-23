using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(BoxCollider))]
public class ComputerChair : MonoBehaviour {

    public static ComputerChair instance;

    public UnityEvent OnChairClicked = new UnityEvent();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void OnClick_ComputerChair() {
        OnChairClicked.Invoke();
    }

}
