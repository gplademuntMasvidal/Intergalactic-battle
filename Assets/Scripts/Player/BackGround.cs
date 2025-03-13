using UnityEngine;
public class BackGround : MonoBehaviour
{
    public float m_speed;

    [SerializeField]
    private Renderer m_Renderer;

    void Update()
    {
        m_Renderer.material.mainTextureOffset += new Vector2(m_speed * Time.deltaTime, 0);
    }
}
