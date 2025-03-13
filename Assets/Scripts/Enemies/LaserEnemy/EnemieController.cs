using Steerings;
using UnityEngine;

public class EnemieController : MonoBehaviour
{
    public BlackboardEnemies m_blackboardEnemies;
    public LineRenderer m_lineRenderer;
    //private Seek m_seek;
    public SteeringContext m_steeringContext;
    private CircleCollider2D m_circleCollider2D;
    public LayerMask m_layerMask;
    public bool m_makingDamage = true;
    public float m_damageOfSmallEnemi = 1.0f;
    public float m_damageOfBigEnemi = 5.0f;

    //public Animation m_animation;
    public GameObject m_fireEffectPrefab;
    private AudioSource m_audioSource;

    void Start()
    {
        // m_seek = GetComponent<Seek>();
        m_steeringContext = GetComponent<SteeringContext>();
        m_blackboardEnemies = GetComponent<BlackboardEnemies>();
        m_lineRenderer = gameObject.AddComponent<LineRenderer>();
        m_circleCollider2D = GetComponent<CircleCollider2D>();
        m_audioSource = GetComponent<AudioSource>();


        m_lineRenderer.positionCount = 2;
        m_lineRenderer.startWidth = 1f;
        m_lineRenderer.endWidth = 1f;
        m_lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        m_lineRenderer.startColor = Color.blue;
        m_lineRenderer.endColor = Color.green;
    }

    public void ShootPlayer(GameObject target)
    {
        if (m_blackboardEnemies.m_isBigEnemy)
        {
            m_lineRenderer.startWidth = 3f;
            m_lineRenderer.endWidth = 3f;
            m_lineRenderer.startColor = Color.red;
            m_lineRenderer.endColor = Color.black;
        }
        m_lineRenderer.enabled = true;
        m_blackboardEnemies.m_timer += Time.deltaTime;
        Vector3 l_origin = transform.position;
        Vector3 l_direction = (target.transform.position - l_origin);

        Vector3 l_endPosition = l_origin + l_direction;

        m_lineRenderer.SetPosition(0, l_origin);
        m_lineRenderer.SetPosition(1, l_endPosition);

        if (m_blackboardEnemies.m_timer >= m_blackboardEnemies.m_rayDurationTime)
        {
            m_lineRenderer.enabled = false;
        }

        RaycastHit2D l_hit = Physics2D.Raycast(l_origin, l_direction, 50.0f, m_layerMask);

        if (l_hit)
        {
            if (l_hit.collider.CompareTag("Player") && m_makingDamage)
            {
                if (m_blackboardEnemies.m_isBigEnemy)
                {
                    l_hit.collider.GetComponent<PlayerBlackBoard>().Damage(m_damageOfBigEnemi);
                }
                else
                {
                    l_hit.collider.GetComponent<PlayerBlackBoard>().Damage(m_damageOfSmallEnemi);
                }
                m_makingDamage = false;
            }
        }
    }

    public void TakeDamage(float damage)
    {

        if (m_blackboardEnemies.m_isBigEnemy == false)
        {
            m_blackboardEnemies.m_smallEnemiesLife -= damage;
        }
        else
        {
            m_blackboardEnemies.m_bigEnemiesLife -= damage;
        }


    }
    public void MergeAlternateEnemies()
    {
        if (GuillemGroupManager.m_instance.members.Count < 2)
            return;

        for (int i = GuillemGroupManager.m_instance.members.Count - 1; i > 0; i -= 2)
        {
            Merging(GuillemGroupManager.m_instance.members[i].GetComponent<SteeringContext>(), GuillemGroupManager.m_instance.members[i - 1]);
        }
    }
    public void Merging(SteeringContext me, GameObject target)
    {
        SeekP1.GetLinearAcceleration(me, target, m_steeringContext.maxAcceleration);
        Destroy(target);
        me.transform.localScale = m_blackboardEnemies.m_originalScale * 3;
        m_circleCollider2D.radius = 3.0f;
        m_steeringContext.maxSpeed = m_blackboardEnemies.m_originalSpeed * 3;
        m_steeringContext.seekWeight += 0.2f;

    }



    public void Dead()
    {
        GameObject fireEffect = Instantiate(m_fireEffectPrefab, transform.position, Quaternion.identity);
        m_audioSource.clip = m_blackboardEnemies.m_explosionSound;
        m_audioSource.Play();
        Destroy(gameObject, 0.5f);
        Destroy(fireEffect, 1f);

        if (m_blackboardEnemies.m_isBigEnemy)
        {
            GameManager.m_instance.m_passLevel -= 2;
        }
        else
        {
            GameManager.m_instance.m_passLevel -= 1;
        }

        GuillemGroupManager.m_instance.members.Remove(gameObject);
    }



}
