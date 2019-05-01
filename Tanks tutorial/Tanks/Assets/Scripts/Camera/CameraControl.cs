using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;                 
    public float m_ScreenEdgeBuffer = 4f;           
    public float m_MinSize = 6.5f;                  
    [HideInInspector] public Transform[] m_Targets; 
    private Camera m_Camera;                        
    private float m_ZoomSpeed;                      
    private Vector3 m_MoveVelocity;                 
    private Vector3 m_DesiredPosition;              

    // Assim que a função começa a ser executada
    private void Awake()
    {
        // A variável camera vai pegar como valor um componente do tipo Camera
        m_Camera = GetComponentInChildren<Camera>();
    }
    // Definindo atualizações fixas chamando métodos para zoom e para movimentar a camera
    private void FixedUpdate()
    {
        Move();
        Zoom();
    }
    // Mover
    private void Move()
    {
        // Invocamos a função para saber a média de posição entre os tanques
        FindAveragePosition();
        // Mudamos a posição da camera
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }

    // Obter média da posição
    private void FindAveragePosition()
    {
        // Obtemos a média da posição entre todos os players que estão ativos
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            averagePos += m_Targets[i].position;
            numTargets++;
        }

        if (numTargets > 0)
            averagePos /= numTargets;

        averagePos.y = transform.position.y;

        m_DesiredPosition = averagePos;
    }

    // Função para o zoom na camera
    private void Zoom()
    {
        // chamamos uma função para definir para qual valor devemos mudar o zoom
        float requiredSize = FindRequiredSize();
        // Definimos o tamanho ortografico da camera, esse é metade do tamanho da visão vertical
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }
    // Função para determinar o valor requerido de zoom
    private float FindRequiredSize()
    {
        // 
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

        float size = 0f;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);
        }
        
        size += m_ScreenEdgeBuffer;

        size = Mathf.Max(size, m_MinSize);

        return size;
    }

    // Definimos a posição e zoom iniciais da camera
    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();
    }
}