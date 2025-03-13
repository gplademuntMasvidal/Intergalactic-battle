using UnityEngine;
using UnityEngine.UI;

public class BlackboardTurret : MonoBehaviour
{
    public Slider m_HUDHealth;
    public GameObject m_mouseTarget;
    public GameObject m_bulletPrefab;
    public Transform m_firePoint;
    public float m_bulletSpeed = 500f;
    public float m_bulletLifeTime = 5f;
    public bool m_shoot;

    [Header("Parameters Shoot")]
    public float m_fireRate = 0.5f;
    public float m_lastShotTime = 0f;

    //SOUNDS
    public AudioClip m_shootSound;


    private void Start()
    {
        m_shootSound = Resources.Load<AudioClip>("Sounds/Blaster");

    }

    private void Update()
    {
        if (!GameManager.m_instance.m_isPaused)
            if (Input.GetMouseButtonDown(0) && Time.time >= m_lastShotTime + m_fireRate)
            {
                m_lastShotTime = Time.time;
                m_shoot = true;
            }
    }
}
