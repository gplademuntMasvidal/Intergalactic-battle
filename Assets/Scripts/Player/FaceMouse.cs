using UnityEngine;

namespace Steerings
{
    public class FaceMouse : SteeringBehaviour
    {
        private BlackboardTurret m_blackboardTurret;

        private void Start()
        {
            m_blackboardTurret = GetComponent<BlackboardTurret>();
        }


        public override float GetAngularAcceleration()
        {
            if (m_blackboardTurret.m_mouseTarget == null) return 0;
            return GetAngularAcceleration(Context, m_blackboardTurret.m_mouseTarget);
        }

        public static float GetAngularAcceleration(SteeringContext me, GameObject target)
        {
            Vector3 l_directionToTarget = target.transform.position - me.transform.position;
            SURROGATE_TARGET.transform.rotation = Quaternion.Euler(0, 0,
                                                    Utils.VectorToOrientation(l_directionToTarget));

            return Align.GetAngularAcceleration(me, SURROGATE_TARGET);
        }

        
    }
}
