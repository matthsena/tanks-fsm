using System;
using UnityEngine;
using UnityEngine.AI;
// Iniciando a classe AI TankController usando a classe FSM como base
public class AITankController : FSM
{
    // Fazendo uma referência a classe TankShooting
    public TankShooting tankShooter;
    // Fazendo uma referência a classe TankHealth
    public TankHealth tankHealth;
    private bool isDead = false;
    private float elapsedTime = 0.0f;
    // Intervalo de um tiro e outro quando a FSM estiver em estado de ataque
    private float shootRate = 0.75f;
    private GameObject player = null;
    private NavMeshAgent navMeshAgent;
    // Definindo os 4 estados possíveis de nossa FSM
    private float enableTime = 3.0f;
    private bool isEnabled = false;

    public enum FSMState
    {
        None, Patrol, Attack, Dead,
    }
    public FSMState curState;
     

    /*
    * Iniciando a FSM para o tanque que utiliza AI
    */
    protected override void Initialize()
    {

        navMeshAgent = GetComponent<NavMeshAgent>();
        // Obtendo a lista de pontos definidos como pontos de patrulha 
        pointList = GameObject.FindGameObjectsWithTag("PatrolPoint");
        // Gera um valor aleatório para definir o próximo ponto
        int rndIndex = UnityEngine.Random.Range(0, pointList.Length);
    
        destPos = pointList[rndIndex].transform.position;
        
    }


    // Update do estado FSM muda
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
        enableTime -= Time.deltaTime;
        if (enableTime < 0 && !isEnabled) {
             curState = FSMState.Patrol;
             isEnabled = true;   
        }
        // Variação de tempo
        elapsedTime += Time.deltaTime;

        // Vai para o estado morto caso a vida do tanque seja <= 0
        if (tankHealth.m_CurrentHealth <= 0)
        {
            isDead = true;
            curState = FSMState.Dead;
        }

    }
    // Caso a FSM esteja no estado morto
    private void UpdateDeadState()
    {
        // Verifica se realmente está morto
        if (isDead)
        {
            // "Desliga" a FSM deixando no estado none
            curState = FSMState.None;
            // Exibe no console a mensagem de "Morto"
            Debug.Log("Dead");
        }
    }
    // Função para ser executada no estado de ataque
    private void UpdateAttackState()
    {
        /*
        Identifica todos os GameObjects num raio de tamanho  15 
        tendo como referência o Tanque com AI com a layer Players
         */
        Collider[] players = Physics.OverlapSphere(transform.position, 15.0f, LayerMask.GetMask("Players"));
        /*
        Caso o numero de GameObjects que foi encontrado nesse raio seja <= a 1 a FSM volta para o estado patrulha
        (Esse <= a 1 é porque o tanque com AI é um GameObject com a layer 'Player')
         */
        if (players.Length <= 1)
        {
            curState = FSMState.Patrol;
            player = null;
            navMeshAgent.enabled = true;
            return;
        }
        /*
        O tanque com AI é o primeiro indice desse array que obtivemos no passo anterior e é um GameObject
        Ele identificou um outro tanque no raio que ele tem conhecimento, muda a direção da sua mira
        Para esse tanque, até mesmo enquanto o adversário se move, ele possui a habilidade de rotação
        e atira
         */
        player = players[0].gameObject;
        Vector3 _direction = (player.transform.position - transform.position).normalized;
        Quaternion _lookRotation = Quaternion.LookRotation(_direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * 3);
        /**
        Esse trecho tem a função de deixar uma pausa tempo entre cada tiro, esse tempo de pausa foi 
        definido na variavél 'shootRate' 
        */
        if (elapsedTime > shootRate)
        {
            // Executando a função tiro da classe TankShooting
            this.tankShooter.Fire();
            elapsedTime = 0;
        }
    }
    // Função para ser executada no estado de patrulha
    private void UpdatePatrolState()
    {
        /*
        Identifica todos os GameObjects num raio de tamanho  15 
        tendo como referência o Tanque com AI com a layer Players
        */
        Collider[] players = Physics.OverlapSphere(transform.position, 15.0f, LayerMask.GetMask("Players"));
        /*
        Se houver mais de um GameObject nesse raio com a layer mask 'Players' a FSM muda para o estado ataque 
        (O próprio tanque com  AI é um GameObject com essa layer, estamos identificando um além dele)
        */ 
        if (players.Length > 1)
        {
            curState = FSMState.Attack;
            player = players[0].gameObject;
            navMeshAgent.enabled = false;
            return;
        }
        
        if (IsInCurrentRange(destPos))
        {
            // Gera um valor aleatório para definir o próximo ponto
            int rndIndex = UnityEngine.Random.Range(0, pointList.Length);
            destPos = pointList[rndIndex].transform.position;
        }
        // Se dirige até o próximo destino
        navMeshAgent.destination = destPos;
    }
    // Verificar se o vetor está a menos de 6 unidades de distância
    protected bool IsInCurrentRange(Vector3 pos)
    {
        float xPos = Mathf.Abs(pos.x - transform.position.x);
        float zPos = Mathf.Abs(pos.z - transform.position.z);
        if (xPos <= 5 && zPos <= 5) return true;
        return false;
    }

}