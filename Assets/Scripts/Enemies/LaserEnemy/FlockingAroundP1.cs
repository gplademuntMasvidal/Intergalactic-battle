using UnityEngine;

namespace Steerings
{
    public class FlockingAroundP1 : SteeringBehaviour
    {

        // private BlackboardEnemies m_blackboardEnemies;
        public GameObject m_target;

        private void Start()
        {
            //m_blackboardEnemies = GetComponent<BlackboardEnemies>();
            m_target = m_target = GameObject.FindGameObjectWithTag("Player");

        }

        public override GameObject GetTarget()
        {
            return m_target;
        }

        public override Vector3 GetLinearAcceleration()
        {
            return FlockingAround.GetLinearAcceleration(Context, m_target);
        }

        public static Vector3 GetLinearAcceleration(SteeringContext me, GameObject target)
        {
            Vector3 l_seekAcc = Seek.GetLinearAcceleration(me, target);
            Vector3 l_flockingAcc = Flocking.GetLinearAcceleration(me);

            return l_seekAcc * me.seekWeight + l_flockingAcc * (1 - me.seekWeight);
        }
    }
}
