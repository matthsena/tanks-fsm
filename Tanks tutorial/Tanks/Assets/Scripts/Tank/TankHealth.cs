using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;
    public float m_CurrentHealth;
    public Slider m_Slider;
    public Image m_FillImage;
    public Color m_FullHealthColor = Color.green;
    public Color m_ZeroHealthColor = Color.red;
    public GameObject m_ExplosionPrefab;
    private AudioSource m_ExplosionAudio;
    private ParticleSystem m_ExplosionParticles;
    private bool m_Dead;
    public int count = 0;
    public TankManager[] m_Tanks;
    private TankInfos m_Infos;
    // Objeto texto que exibe o número de vidas do tanque
    public Text m_HealthText;
    private Rigidbody m_Rigidbody;
    // A posição de renascimento do tank depois que esgota uma vida
    [HideInInspector] public Vector3 m_pos;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
        m_ExplosionParticles.gameObject.SetActive(false);
        // m_pos como a posição inicial (Spanw) do tanque
        // Para que quando ele perder uma vida saiba para onde voltar
        m_pos = transform.position;
    }
    private void OnEnable()
    {
        // Ao iniciar coloco o texto do m_HealthText com 3 corações a fim de representar 3 vidas
        m_HealthText.text = "♥♥♥";
        // A vida atual é a vida inicial
        m_CurrentHealth = m_StartingHealth;
        // E ele não está morto
        m_Dead = false;
        // Invoco a função para definir o slider com a vida
        SetHealthUI();
    }
    // Assim que sofre dano
    public void TakeDamage(float amount)
    {
        // A vida atual diminui
        m_CurrentHealth -= amount;
        // É atualizado o slider com a vida
        SetHealthUI();
        // Se a vida for menor ou igual a 0 e não estiver morto
        if (m_CurrentHealth <= 0f && !m_Dead)
        {
            // Chama a função que decreta sua morte
            OnDeath();
        }
    }
    // Slider de vida
    private void SetHealthUI()
    {
        // O valor é a vida atual
        m_Slider.value = m_CurrentHealth;
        // Define a cor e preenchimento do slider com base na vida atual
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }
    // Assim que morre
    private void OnDeath()
    {
        // Animação de explosão do tanque
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();
        // O contador soma mais uma morte ao tanque
        count++;
        // Caso o tanque tenha morrido 3 ou mais vezes
        if (count >= 3)
        {
            // O round acaba e o contador é zerado
            m_Dead = true;
            gameObject.SetActive(false);
            count = 0;
        }
        // Senão
        else
        {
            // O texto com os corações irá mudar para o numero de vidas que ele ainda possui
            switch (count)
            {
                case 0:
                    m_HealthText.text = "♥♥♥";
                    break;
                case 1:
                    m_HealthText.text = "♥♥";
                    break;
                case 2:
                    m_HealthText.text = "♥";
                    break;
                case 3:
                    m_HealthText.text = "Dead";
                    break;
            }
            // Invoca a função Reborn
            Reborn();
        }
    }
    // Caso ele não tenha morrido 3 ou mais vezes
    private void Reborn()
    {
        // Deixa o objeto imune a forças, colisões etc
        m_Rigidbody.isKinematic = true;
        // Retorna para a posição inicial
        transform.position = m_pos;
        // Volta com a vida "cheia", ou seja, a vida inicial
        m_CurrentHealth = m_StartingHealth;
        // O objeto volta a ser sensível a física
        m_Rigidbody.isKinematic = false;
        // Chamo função para preencher o slider de vida dele
        SetHealthUI();
    }
}