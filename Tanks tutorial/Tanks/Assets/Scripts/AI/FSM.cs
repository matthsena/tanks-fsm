using System.Collections;
using UnityEngine;

public class FSM : TankMovement 
{
    protected Vector3 destPos;
    protected GameObject[] pointList;

    protected virtual void Initialize() { }
    protected virtual void FSMUpdate () { }

    protected virtual void FSMFixedUpdate() { }

    void Start() 
    {
        Initialize();
    }

    void Update() 
    {
        FSMUpdate();
    }
    void FixedUpdate() {
        FSMFixedUpdate();
    }
}