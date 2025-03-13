using UnityEngine;

namespace Steerings
{

    public class SeekP1 : SteeringBehaviour
    {

        public GameObject m_target;

        public float m_maxAcceleration;

        public override GameObject GetTarget()
        {
            return m_target;
        }

        public override Vector3 GetLinearAcceleration()
        {
            return SeekP1.GetLinearAcceleration(Context, m_target, m_maxAcceleration);
        }

        public static Vector3 GetLinearAcceleration(SteeringContext me,
                                                    GameObject target, float mAcceleration)
        {
            if (me == null || target == null)
            {
                Debug.LogWarning("SeekP1: 'me' o 'target' han estat destruïts!");
                return Vector3.zero; // Evita calcular amb un objecte inexistent
            }
            Vector3 l_directionToTarget;
            Vector3 l_acceleration;

            // Compute direction to target
            l_directionToTarget = target.transform.position - me.transform.position;
            l_directionToTarget.Normalize();

            // give max acceleration towards the target
            l_acceleration = l_directionToTarget * mAcceleration;

            return l_acceleration;
        }
    }
}