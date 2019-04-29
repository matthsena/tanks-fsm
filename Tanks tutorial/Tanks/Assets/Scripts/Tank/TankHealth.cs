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
    public Text m_HealthText;
    private Rigidbody m_Rigidbody;

    [HideInInspector] public Vector3 m_pos;        // A posição de renascimento do tank depois que esgota uma vida


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        m_ExplosionParticles.gameObject.SetActive(false);

        m_pos = transform.position;
    }

    private void OnEnable()
    {

        m_HealthText.text = "♥♥♥";
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        SetHealthUI();
    }

    public void TakeDamage(float amount)
    {
        m_CurrentHealth -= amount;
        SetHealthUI();

        if (m_CurrentHealth <= 0f && !m_Dead)
        {
            OnDeath();
        }
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
    }

    private void SetHealthUI()
    {
        m_Slider.value = m_CurrentHealth;
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
        // Adjust the value and colour of the slider.
    }

    private void OnDeath()
    {
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();
        count++;

        if (count >= 3)
        {
            // Play the effects for the death of the tank and deactivate it.
            m_Dead = true;
            gameObject.SetActive(false);
            count = 0;
        }
        else
        {
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

            Reborn();
        }
    }

    private void Reborn()
    {
        m_Rigidbody.isKinematic = true;

        transform.position = m_pos;

        m_CurrentHealth = m_StartingHealth;
        SetHealthUI();

    }
}