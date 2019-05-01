using System;
using UnityEngine;

[Serializable]
public class TankManager
{
    public Color m_PlayerColor;
    public Transform m_SpawnPoint;
    [HideInInspector] public int m_PlayerNumber;
    [HideInInspector] public string m_ColoredPlayerText;
    [HideInInspector] public GameObject m_Instance;
    [HideInInspector] public int m_Wins;

    private TankInfos m_Infos;
    private TankMovement m_Movement;
    private TankShooting m_Shooting;

    private GameObject m_CanvasGameObject;

    // No momento de setup
    public void Setup()
    {
        // Obtenho o componente de infos, movimento, tiro, canvas
        m_Infos = m_Instance.GetComponent<TankInfos>();
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;
        // Adicionando o atributo player number no movimento e no tiro
        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;
        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";
        // Renderizo os ranques e atribuo uma cor a cada um deles
        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;
        }
    }
    // Desabilito o movimento, tiro e canvas
    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;
        m_CanvasGameObject.SetActive(false);
    }
    // Habilito movimento, tiro e canvas
    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;
        m_CanvasGameObject.SetActive(true);
    }
    // Reseto o tanque
    public void Reset()
    {
        // Coloco a posição e rotação em sua posição de spawn
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;
        // Desabilito e habilito a instância
        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
    // Update infos serve para definir o texto do player
    public void UpdateInfos(String value)
    {
        // Adiciono o texto no tanque, exemplo: P1 (indica que é o player 1)
        m_Infos.m_TankInfo.text = value;
    }
}
