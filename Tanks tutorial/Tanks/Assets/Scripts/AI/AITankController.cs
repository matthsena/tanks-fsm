using System;
using UnityEngine;
using UnityEngine.AI;

public class AITankController : FSM 
{
    public TankShooting tankShooter;
    public TankHealth tankHealth;
    private bool isDead = false;
    private float elapsedTime = 0.0f;
    private float shootRate = 0.75f;
    private GameObject player = null;
    private NavMeshAgent navMeshAgent;
    private float _distancia; 
    public enum FSMState 
    {
        None, Patrol, Attack, Dead,
    }
    // Current state that the NPC is reaching
    public FSMState curState;

    //Initialize the finite state machine for the NPC tank
    protected override void Initialize() 
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        // Get list of points 
        pointList = GameObject.FindGameObjectsWithTag("PatrolPoint");

        int rndIndex = UnityEngine.Random.Range(0, pointList.Length);
        destPos = pointList[rndIndex].transform.position;
    }

    protected override void FSMUpdate()
    {
        switch (curState)
        {
            case FSMState.Patrol:
                UpdatePatrolState();
                break;
            case FSMState.Attack:
                UpdateAttackState();
                break;
            case FSMState.Dead:
                UpdateDeadState();
                break;
        }
        elapsedTime += Time.deltaTime;

        // go to dead state is no health left
       if (tankHealth.m_CurrentHealth <= 0)
            curState = FSMState.Dead;
    }
    private void UpdateDeadState() 
    {
        if (!isDead) {
            Debug.Log(" Dead");
        }
    }
    private void UpdateAttackState()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, 15.0f, LayerMask.GetMask("Players"));
        if (players.Length == 0)
        {
            curState = FSMState.Patrol;
            player = null;
            navMeshAgent.enabled = true;
            return;
        } 
        player = players[0].gameObject;
        Vector3 _direction = (player.transform.position - transform.position).normalized;
        Quaternion _lookRotation = Quaternion.LookRotation(_direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * 3); 
        if (elapsedTime > shootRate)
        {
           this.tankShooter.Fire();
            elapsedTime = 0;
        }
    }

    private void UpdatePatrolState()
    {
       
        Collider[] players = Physics.OverlapSphere(transform.position, 15.0f, LayerMask.GetMask("Players"));
        
        if (players.Length > 1)
        {
            Debug.Log(players.Length);
            curState = FSMState.Attack;
            player = players[0].gameObject;
            navMeshAgent.enabled = false;
            return;
        }
        if (IsInCurrentRange(destPos))
        {
            int rndIndex = UnityEngine.Random.Range(0, pointList.Length);
            destPos = pointList[rndIndex].transform.position;
        }
        navMeshAgent.destination = destPos;
    }
    protected bool IsInCurrentRange (Vector3 pos)
    {
        float xPos = Mathf.Abs(pos.x - transform.position.x);
        float zPos = Mathf.Abs(pos.z - transform.position.z);
        if (xPos <= 5 && zPos <= 5) return true;
        return false;
    }
 
}