using Photon.Pun;
using UnityEngine;

public class PlayerSkillSet : MonoBehaviour
{
    private PlayerMovement m_PlayerMovement;
    private PlayerConn m_PlayerConn;

    public bool CanPerformGuitarrada { get; set; }
    public bool CanPerformMetalPower { get; set; }

    [SerializeField] private Collider2D m_HitBoxCollider2D;
    private Vector2 m_HitBoxPosition;

    private void Awake()
    {
        m_PlayerMovement = transform.GetComponent<PlayerMovement>();
        m_PlayerConn = transform.GetComponent<PlayerConn>();

        CanPerformGuitarrada = true;
        CanPerformMetalPower = true;

        m_HitBoxPosition = new Vector2(0.021f, m_HitBoxCollider2D.transform.position.y);
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
                Debug.Log("m_CanPerformGuitarrada = " + (CanPerformGuitarrada ? "Sim" : "Não"));
                Debug.Log("m_PlayerMovement.IsGrounded() = " + (m_PlayerMovement.IsGrounded ? "Sim" : "Não"));
                if (CanPerformGuitarrada && m_PlayerMovement.IsGrounded)
                {
                    CanPerformGuitarrada = false;
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
        bool isRightDirection = m_PlayerMovement.CharacterDirection == Vector2.right;

        Vector2 hitBoxPosition = isRightDirection ? m_HitBoxPosition : -m_HitBoxPosition;
        m_HitBoxCollider2D.transform.localPosition = hitBoxPosition;
        m_HitBoxCollider2D.gameObject.SetActive(true);
    }

    public void ReleaseSkillSet()
    {
        CanPerformGuitarrada = true;
        CanPerformMetalPower = true;
    }

    [PunRPC]
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
