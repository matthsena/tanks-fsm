﻿using UnityEngine;

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


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        m_OriginalPitch = m_MovementAudio.pitch;
    }
    

    private void Update()
    {
        if (!m_IsAI) {
            // Store the player's input and make sure the audio for the engine is playing.
            // Value of vertical moviment
            m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
            // Value of horizontal moviment
            m_TurnInputValue = Input.GetAxis (m_TurnAxisName);
        }
        
		EngineAudio ();
    }


    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
        // Tank is stopped
        if (Mathf.Abs (m_MovementInputValue) < 0.1f && Mathf.Abs (m_TurnInputValue) < 0.1f) 
		{
            // If the sound was engine driving change to engine Idling
			if (m_MovementAudio.clip == m_EngineDriving) {
				m_MovementAudio.clip = m_EngineIdling;
				m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch - m_PitchRange);
				m_MovementAudio.Play ();
			}
		} 
        // Tank is running 
		else 
		{
            // If the sound was engine Idling change to engine driving
			if (m_MovementAudio.clip == m_EngineIdling) {
				m_MovementAudio.clip = m_EngineDriving;
				m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch - m_PitchRange);
				m_MovementAudio.Play ();
			}
			
		}
    }


    private void FixedUpdate()
    {
        // Move and turn the tank.
		Move ();
		Turn ();
    }


    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
        // transform forward * vertical moviment * tank speed * time that it's runnig
        Vector3 moment = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

        // Current position plus the moment
		m_Rigidbody.MovePosition (m_Rigidbody.position + moment);
       
    }


    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        // Horizontal moviment * tank speed * time that it's running
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        // 3D rotation 
		Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);
        // Current rotarion * rotation based on input
		m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
    }
}