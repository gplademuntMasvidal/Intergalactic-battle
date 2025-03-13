using UnityEngine;

namespace Steerings
{
    public class MousePosition : MonoBehaviour
    {
        private void Update()
        {
            Vector3 l_mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            l_mousePosition.z = 0;

            transform.position = l_mousePosition;
        }
    }
}
