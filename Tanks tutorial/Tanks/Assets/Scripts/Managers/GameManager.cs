using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;
    public float m_StartDelay = 3f;
    public float m_EndDelay = 3f;
    public CameraControl m_CameraControl;
    public Text m_MessageText;
    public GameObject m_TankPrefab;
    public GameObject m_TankAIPrefab;
    public TankManager[] m_Tanks;
    private int m_RoundNumber;
    private WaitForSeconds m_StartWait;
    private WaitForSeconds m_EndWait;
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;

    // Função para iniciar o game
    private void Start()
    {
        // Definimos um tempo de delay até começar a partida
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);
        // Chamamos função para spawnar os tanques
        SpawnAllTanks();
        // Chamamos função para definir quais serão os focos da camera
        SetCameraTargets();
        // Chamamos função para iniciar a partida
        StartCoroutine(GameLoop());
    }
    // Spawn dos tanques
    private void SpawnAllTanks()
    {
        // Entro num laço de repetição para pegar todos os tanques e spanwar eles
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            // Nesse exemplo o ultimo tanque a ser spawnado é o tanque com AI 
            if (i == m_Tanks.Length - 1)
            {
                // Logo em sua instância utiliza o Prefeb do tanque com AI
                m_Tanks[i].m_Instance =
                Instantiate(m_TankAIPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
                m_Tanks[i].m_PlayerNumber = i + 1;
                m_Tanks[i].Setup();
            }
            else
            {
                // Se não for o tanque com AI damos spanw no tanque controlado pelo player
                m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
                m_Tanks[i].m_PlayerNumber = i + 1;
                m_Tanks[i].Setup();
            }
            // Chamamos a função UpdateInfos presente no TankManager.cs para atribuir um número ao Tanque
            m_Tanks[i].UpdateInfos("P" + (i + 1));
        }
    }
    // Definimos os alvos da camêra, no caso são os tanques
    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        m_CameraControl.m_Targets = targets;
    }
    // GameLoop verifica se há vencedor no round e qual cena 
    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());
        if (m_GameWinner != null)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }
    // Iniciando o round
    private IEnumerator RoundStarting()
    {
        // Reseta os tanque e desabilita os controles
        ResetAllTanks();
        DisableTankControl();
        // coloca a camera na posição inicial
        m_CameraControl.SetStartPositionAndSize();

        m_RoundNumber++;
        // Exibe uma mensagem com o número do round
        m_MessageText.text = "ROUND " + m_RoundNumber;

        yield return m_StartWait;
    }
    // Durante o round
    private IEnumerator RoundPlaying()
    {
        // Habilita os tanques
        EnableTankControl();
        m_MessageText.text = string.Empty;
        // Enquanto tiver mais de um tanque em campo, o round não acabou
        while (!OneTankLeft())
        {

            yield return null;

        }
    }
    // Final do round
    private IEnumerator RoundEnding()
    {
        // Desabilita os tanques
        DisableTankControl();
        m_RoundWinner = null;
        // Exibe o vencedor do round
        m_RoundWinner = GetRoundWinner();

        if (m_RoundWinner != null)
        {

            m_RoundWinner.m_Wins++;
        }

        m_GameWinner = GetGameWinner();

        string message = EndMessage();
        m_MessageText.text = message;

        yield return m_EndWait;
    }

    // Verifica se existe apenas um tanque sobrando
    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }

    // Obtem o vencedor do round, que no caso é o unico tanque ativo sobrando
    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }

    // Obtem o vencedor do jogo, com base no numero de rounds que precisa vencer para se tornar o vencedor absoluto
    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }

    // Exibe a mensagem no final do round e no final do game
    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }
    // Reseta todos os tanques
    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }
    // Habilita os controladores de todos os tanques
    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }
    // Disabilita os controladores de todos os tanques
    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }
}