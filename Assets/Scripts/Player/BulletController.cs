using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float m_damage = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BOID"))
        {
            EnemieController l_enemyHealth = collision.GetComponent<EnemieController>();
            if (l_enemyHealth != null)
            {
                l_enemyHealth.TakeDamage(m_damage);
            }

            Destroy(gameObject);
        }
        else if (collision.CompareTag("Enemie"))
        {
            DroneBlackboard l_blackBoard = collision.GetComponent<DroneBlackboard>();
            if(l_blackBoard != null)
            {
                l_blackBoard.m_lives -= m_damage;
            }

            Destroy(gameObject);
        }
    }
}
