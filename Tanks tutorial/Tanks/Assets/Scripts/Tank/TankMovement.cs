using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;         
    public float m_Speed = 12f;            
    public float m_TurnSpeed = 180f;       
    public AudioSource m_MovementAudio;    
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      
    public float m_PitchRange = 0.2f;
    private string m_MovementAxisName;     
    private string m_TurnAxisName;         
    private Rigidbody m_Rigidbody;         
    private float m_MovementInputValue;    
    private float m_TurnInputValue;        
    private float m_OriginalPitch;         
    public bool m_IsAI;

    // Quando o objeto "acorda"
    private void Awake()
    {
        // Definimos qual seu rigidbody
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Quando o tanque está habilitado
    private void OnEnable ()
    {
        // O fica sensível a física (forças, colisões etc)
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }
    // Quando está desabilitado
    private void OnDisable ()
    {
        // Deixa o objeto imune a forças, colisões etc
        m_Rigidbody.isKinematic = true;
    }
    
    private void Start()
    {
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        m_OriginalPitch = m_MovementAudio.pitch;
    }
    
    // Nas atualizações
    private void Update()
    {   
        // Caso não seja uma AI
        if (!m_IsAI) {
            // Valor do movimento vertical
            m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
            // Valor do movimento horizontal
            m_TurnInputValue = Input.GetAxis (m_TurnAxisName);
        }
        // função para ligar o audio
		EngineAudio ();
    }
    // Reproduz o som com base na movimentação do tanque
    private void EngineAudio()
    {
        // Tanque está paradao
        if (Mathf.Abs (m_MovementInputValue) < 0.1f && Mathf.Abs (m_TurnInputValue) < 0.1f) 
		{
            //Se o som era o de movimento mudar para o de repouso
			if (m_MovementAudio.clip == m_EngineDriving) {
				m_MovementAudio.clip = m_EngineIdling;
				m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch - m_PitchRange);
				m_MovementAudio.Play ();
			}
		} 
        // Tanque está andando
		else 
		{
            // Se o som era de repouso mudar para o de movimento
			if (m_MovementAudio.clip == m_EngineIdling) {
				m_MovementAudio.clip = m_EngineDriving;
				m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch - m_PitchRange);
				m_MovementAudio.Play ();
			}
			
		}
    }
    // Atualizações fixas
    private void FixedUpdate()
    {
        // Mover e rotacionar o tanque
		Move ();
		Turn ();
    }
    // Mover
    private void Move()
    {
        // Ajustar a posição do tanque com base nos inputs do player
        Vector3 moment = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

        // A posição para qual ele irá se movimentar é a atual com a moment
		m_Rigidbody.MovePosition (m_Rigidbody.position + moment);
       
    }
    // Girar
    private void Turn()
    {
        // Ajusta a rotação com os inputs do player
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        // Rotação 3D 
		Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);
        // A rotação vai ser a atual * pela rotação 3D baseada nos inputs do plater
		m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
    }
}