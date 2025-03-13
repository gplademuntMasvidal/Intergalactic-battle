using UnityEngine;

public class GuillemGroupManager : Steerings.GroupManager
{
    public static GuillemGroupManager m_instance { get; private set; }

    public int m_numInstances;
    public float m_delay = 0.5f;
    public GameObject m_prefab;
    //public bool around = false;
    //public GameObject attractor;

    private int m_created = 0;
    private float m_elapsedTime = 0f;

    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            m_instance = this;
        }
    }
    void Update()
    {
        Spawn();
        //Debug.Log(members.Count);
    }


    private void Spawn()
    {
        if (m_created == m_numInstances)
        {
            return;
        }
        if (m_elapsedTime < m_delay)
        {
            m_elapsedTime += Time.deltaTime;
            return;
        }

        // if this point is reached, it's time to spawn a new instance
        GameObject l_clone = Instantiate(m_prefab);
        l_clone.transform.position = transform.position;

        // if (created == 0)
        //{
        // first one and only it
        ShowRadiiPro l_shr = l_clone.GetComponent<ShowRadiiPro>();
        l_shr.componentTypeName = "Steerings.SteeringContext";
        l_shr.innerFieldName = "repulsionThreshold";
        l_shr.outerFieldName = "cohesionThreshold";
        l_shr.enabled = true;

        //}

        BlackboardEnemies l_BbeP1 = l_clone.GetComponent<BlackboardEnemies>();
        if (l_BbeP1 != null)
        {
            l_BbeP1.m_formationIndex = m_created;  // cada enemic rep l'índex segons l'ordre de creació
            l_BbeP1.m_totalEnemies = m_numInstances; // estableix el nombre total d'enemics
        }

        AddBoid(l_clone);
        m_created++;
        m_elapsedTime = 0f;
    }
}
