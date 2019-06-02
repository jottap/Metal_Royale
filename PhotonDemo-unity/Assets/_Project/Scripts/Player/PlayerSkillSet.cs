using UnityEngine;

public class PlayerSkillSet : MonoBehaviour
{
    private PlayerMovement m_PlayerMovement;
    private PlayerConn m_PlayerConn;

    private bool m_CanPerformGuitarrada;
    private bool m_CanPerformMetalPower;

    [SerializeField] private Collider2D m_HitBoxCollider2D;
    private float m_HitBoxPosition = 0.015f;

    private void Awake()
    {
        m_PlayerMovement = transform.GetComponent<PlayerMovement>();
        m_PlayerConn = transform.GetComponent<PlayerConn>();

        m_CanPerformGuitarrada = true;
        m_CanPerformMetalPower = true;
    }

    private void Update()
    {
        if (m_PlayerMovement.ItIsMe())
        {
            //if (Input.GetKeyDown(KeyCode.F))
            //{
            //    if (m_CanUseMetalPower)
            //    {
            //        if (m_PlayerConn.Score == m_PlayerConn.MaxScore)
            //        {
            //            MetalPower();
            //        }
            //        else
            //        {
            //            UltraMetalPower();
            //        }
            //    }
            //}
            //else
            if (Input.GetKeyDown(KeyCode.G))
            {
                if (m_CanPerformGuitarrada && m_PlayerMovement.IsGrounded())
                {
                    m_CanPerformGuitarrada = false;
                    //Todo Realizar animação da Guitarrada
                    //Todo abilitar o hitbox na animação
                    EnableHitBox();
                }
            }
        }
    }

    // método deve ser chamado como evento na animação de Guitarrada
    public void EnableHitBox()
    {
        m_HitBoxCollider2D.gameObject.SetActive(true);
    }

    public void ReleaseSkillSet()
    {
        m_CanPerformGuitarrada = true;
        m_CanPerformMetalPower = true;
    }

    public void HitAndStun(Vector2 hitDirection)
    {
        m_PlayerMovement.IsStunned = true;
        m_PlayerMovement.TakeHit(hitDirection);
    }

    private void PerformMetalPower()
    {

    }

    private void PerformHeavyMetalPower()
    {

    }
}
