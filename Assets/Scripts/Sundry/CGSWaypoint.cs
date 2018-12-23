using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]             // This ensures that we have the required component if we want to use the pointer functionality.

public class CGSWaypoint : MonoBehaviour
{
    public static CGSWaypoint instance;

    public UnityEvent OnOccupied;

    // These are the three possible states that our waypoint can be during the lifetime of the game(and also discretely).
    private enum State
    {
        Idle,
        Focussed,
        Occupied
    }

    private const float NULLITY = 0.0f;

    private State _state;   // To keep track of our present state.

    void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else {
            Destroy(this.gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        _state = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(_state);
        bool occupied = Camera.main.transform.parent.transform.position == transform.position;

        // If it was occupied just and the person left the waypoint(That's we put it here in update so that we can catch it very well).
        if (!occupied && _state == State.Occupied)
        {
            _state = State.Idle;
        }

        switch (_state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Focussed:
                Focussed();
                break;
            case State.Occupied:
                Occupied();
                break;
        }
    }

    // This gets called when we hover over the collider attached to this.gameobject.
    public void Enter()
    {
        // Debug.Log("Inside Enter()");
        if (_state == State.Idle)
        {
            _state = State.Focussed;
        }
    }

    // This gets called when we exit over the collider attached to this gameobject.
    public void Exit()
    {
        // Debug.Log("Inside Exit()");
        if (_state == State.Focussed)
        {
            _state = State.Idle;
        }
    }

    // This gets called when we click on the collider attached to this gameobject.
    public void Click()
    {
        // Debug.Log("Inside Click()");
        if (_state == State.Focussed)
        {
            _state = State.Occupied;
            OnOccupied.Invoke();
            Camera.main.transform.parent.transform.position = transform.position;
        }
    }

    // What should happen when the waypoint is idle.
    void Idle()
    {
        transform.GetChild(1).transform.gameObject.SetActive(false);
        transform.localScale = Vector3.one;
    }

    // What should happen when the waypoint is focussed.
    void Focussed()
    {
        transform.GetChild(1).transform.gameObject.SetActive(true);
    }

    // What should happen when the waypoint is occupied.
    void Occupied()
    {
        transform.localScale = Vector3.one * NULLITY;
    }

}
