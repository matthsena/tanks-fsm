using System.Collections;
using UnityEngine;

public class FSM : TankMovement 
{
    // Ponto de destino
    protected Vector3 destPos;
    // Array de pontos definidos como pontos de patrulha 
    protected GameObject[] pointList;
    // 'pseudo' funções que sofreção override em outra classe
    protected virtual void Initialize() { }
    protected virtual void FSMUpdate () { }
    protected virtual void FSMFixedUpdate() { }
    // No método start chama a função Initialize
    void Start() 
    {
        Initialize();
    }
    // No método update chama a função FSM Update
    void Update() 
    {
        FSMUpdate();
    }
    // No método fixed update chama a FSM Fixed Update
    void FixedUpdate() {
        FSMFixedUpdate();
    }
}