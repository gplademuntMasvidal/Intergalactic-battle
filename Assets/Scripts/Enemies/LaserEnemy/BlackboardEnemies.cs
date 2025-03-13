using Steerings;
using UnityEngine;
using UnityEngine.UI;

public class BlackboardEnemies : MonoBehaviour
{

    public GameObject m_target;
    public Slider m_HUDHealth;
    public GameObject m_HUD;


    //KeepPositionP1 variables
    public float m_closeToPlayer = 70;
    public float m_formationRadius = 70f;
    public int m_formationIndex = 0;
    public int m_totalEnemies = 30;

    //ORIGINAL VALUES 
    public float m_originalSpeed;
    public float m_originalRepulsionThreshold;
    public float m_originalCohesionThreshold;
    public float m_originalAcceleration;


    //ATTACK VARIABLES
    public float m_timer;
    public float m_rayDurationTime = 5.0f;


    //Enemies variables
    public bool m_isBigEnemy = false;
    public float m_smallEnemiesLife = 1.0f;
    public float m_bigEnemiesLife = 3.0f;
    public Vector3 m_originalScale = new Vector3(3.0f, 3.0f, 1.0f);

    // public bool m_start = false;
    public float m_timeToChange = 5;
    public bool m_startFlocking = true;

    //SOUNDS
    public AudioClip m_explosionSound;

    void Start()
    {
        m_target = GameObject.FindGameObjectWithTag("Player");
        m_originalSpeed = GetComponent<SteeringContext>().maxSpeed;
        m_originalAcceleration = GetComponent<SteeringContext>().maxAcceleration;
        m_originalRepulsionThreshold = GetComponent<SteeringContext>().repulsionThreshold;
        m_originalCohesionThreshold = GetComponent<SteeringContext>().cohesionThreshold;

        m_explosionSound = Resources.Load<AudioClip>("Sounds/Explosion");
    }

    private void Update()
    {
        if (m_isBigEnemy)
        {
            m_HUDHealth.value = m_bigEnemiesLife;
        }
        else
        {
            m_HUDHealth.value = m_smallEnemiesLife;
        }
    }
}
