using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;       
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;    
    public Slider m_AimSlider;           
    public AudioSource m_ShootingAudio;  
    public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
    public float m_MinLaunchForce = 15f; 
    public float m_MaxLaunchForce = 30f; 
    public float m_MaxChargeTime = 0.75f;

    
    private string m_FireButton;         
    private float m_CurrentLaunchForce;  
    private float m_ChargeSpeed;         
    private bool m_Fired;                

    public bool m_IsAI;
    // Na ativação
    private void OnEnable()
    {   
        // A força do tiro será a minima
        m_CurrentLaunchForce = m_MinLaunchForce;
        // O slider com a seta será o minimo
        m_AimSlider.value = m_MinLaunchForce;
    }

    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }
    // Atualização
    private void Update()
    {
        // Se for uma AI retorna
        if (m_IsAI) {
            return;
        }
        // O slider com a força do tiro se torna a força minima
        m_AimSlider.value = m_MinLaunchForce;
        // Caso a força chegar ou ultrapassar a maxima e ainda não ter lançado
        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired) 
        {   
            // A força do tiro será a máxima
            m_CurrentLaunchForce = m_MaxLaunchForce;
            // Abre fogo
            Fire();
        }
        // Senão, verifica se o botão de tiro ainda está sendo pressionado
        // Coloca as animações e sons correspondentes
        // Define a força do tiro com base no aperto do botão
        // E finalmente atira
        else if (Input.GetButtonDown(m_FireButton))
        {
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;
            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }
        else if (Input.GetButton(m_FireButton) && !m_Fired)
        {
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
            m_AimSlider.value = m_CurrentLaunchForce;
        }
        else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
            Fire();
        }
    }
    // Função para abrir fogo
    public void Fire()
    {
        // Se for uma inteligencia artifical a força do tiro será metade da maxima
        if (m_IsAI) {
            m_CurrentLaunchForce = m_MaxLaunchForce / 2.0f;
        }
        // Calcula a velocidade da bala, ativa o audio e após isso faz com que a força do tiro volte a ser a minima
        m_Fired = true;

        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}