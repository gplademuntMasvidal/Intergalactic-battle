using UnityEngine;

namespace Steerings
{
    public class LookWhereYouGo : SteeringBehaviour
    {
        public override float GetAngularAcceleration()
        {
            return LookWhereYouGo.GetAngularAcceleration(Context);
        }

        public static float GetAngularAcceleration(SteeringContext me)
        {
            // Obtener la velocidad actual del objeto
            Vector3 velocity = me.GetComponent<Rigidbody2D>().velocity;

            // Si la velocidad es casi cero, no rotar
            if (velocity.magnitude < 0.01f)
                return 0;

            // Calcular el ángulo hacia donde se está moviendo el objeto
            float targetOrientation = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;

            // Rotar el objeto para que mire en la dirección en la que se está moviendo
            SURROGATE_TARGET.transform.rotation = Quaternion.Euler(0, 0, targetOrientation);

            // Usar Align para suavizar la rotación hacia la dirección del movimiento
            return Align.GetAngularAcceleration(me, SURROGATE_TARGET);
        }
    }
}
