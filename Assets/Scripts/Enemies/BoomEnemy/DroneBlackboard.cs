using UnityEngine;
using UnityEngine.UI;

public class DroneBlackboard : MonoBehaviour
{
    public GameObject m_player;
    public float m_lives = 2f;
    public Slider m_HUDHealth;
    public float m_damageMaked = 10.0f;
    public GameObject m_fireEffectPrefab;
    public GameObject m_render;

    [Header("Patrol")]
    public GameObject m_pointA;
    public GameObject m_pointB;
    public float m_locationReachedRadius = 5f;
    public float m_initialSeekWeight = 0.2f;
    public float m_intervalBetweenTimeouts = 10f;
    public float m_seekIncrement = 0.2f;

    [Header("DroneBomb")]
    public float m_droneDetectionRadius = 10f;
    public float m_droneReachedRadius = 5.0f;
    public float m_droneRangeRadius = 10.0f;
    public float m_destructionTime = 2f;
    public float m_explosionRadius = 3.0f;
    public bool m_shouldEscape;

    [Header("Audio")]
    public AudioClip m_explosionClip;
    public AudioSource m_audioSource;

    private void Awake()
    {
        m_explosionClip = Resources.Load<AudioClip>("Sounds/Explosion");
    }

    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_pointA = GameObject.FindGameObjectWithTag("A");
        m_pointB = GameObject.FindGameObjectWithTag("B");
        m_audioSource = GetComponent<AudioSource>();
        m_audioSource.clip = m_explosionClip;
    }

    private void Update()
    {
        m_HUDHealth.value = m_lives;
    }
}
