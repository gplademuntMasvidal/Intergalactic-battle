using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Steerings
{

    public class KeepPositionP1 : SteeringBehaviour
    {

        public GameObject m_target;    
        private BlackboardEnemies m_blackboardEnemies;

        [HideInInspector]
        public float m_formationRadius;
        [HideInInspector]
        public int m_formationIndex;
        [HideInInspector]
        public int m_totalEnemies;

        private void Start()
        {
            m_blackboardEnemies = GetComponent<BlackboardEnemies>();
            m_target = m_blackboardEnemies.m_target;
        }
        public override GameObject GetTarget()
        {
            return m_target;
        }

        public override Vector3 GetLinearAcceleration()
        {
            return GetLinearAcceleration(Context, m_target, m_formationRadius, m_formationIndex, m_totalEnemies);
        }

        public static Vector3 GetLinearAcceleration(SteeringContext me, GameObject target, float radius, int index, int totalEnemies)
        {
            if (target == null)
                return Vector3.zero;

            float l_angleStep = 360f / totalEnemies;
            float l_angleDegrees = index * l_angleStep;
            float l_desiredAngle = l_angleDegrees;

            Vector3 l_desiredDirection = Utils.OrientationToVector(l_desiredAngle).normalized;
            Vector3 l_displacement = l_desiredDirection * radius;
            SURROGATE_TARGET.transform.position = target.transform.position + l_displacement;

            return Arrive.GetLinearAcceleration(me, SURROGATE_TARGET);
        }

    }
}