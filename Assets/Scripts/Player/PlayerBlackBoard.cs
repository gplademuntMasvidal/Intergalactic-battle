using UnityEngine;
using UnityEngine.UI;

public class PlayerBlackBoard : MonoBehaviour
{
    public Slider m_HUDHealth;
    public float m_lerpSpeed = 2.0f;
    public float m_addMaxSpeed = 1.0f;
    public float m_currentVelocity = 0.0f;
    public float m_maxHealth = 101.0f;
    public float m_currentHealt;
    public float m_damage;
    public bool m_takingDamage;
    public float m_minSpeed;
    public float m_maxSpeed;

    private void Start()
    {
        m_HUDHealth.value = m_maxHealth;
        m_currentHealt = m_maxHealth;
    }

    private void Update()
    {
        if (m_HUDHealth.value != m_currentHealt)
        {
            m_HUDHealth.value = Mathf.SmoothDamp(m_HUDHealth.value, m_currentHealt,
            ref m_currentVelocity, m_lerpSpeed * Time.deltaTime);
        }
    }

    public void Damage(float damage)
    {
        m_damage = damage;
        m_takingDamage = true;
    }

}
