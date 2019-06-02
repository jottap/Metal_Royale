using Photon.Pun;
using UnityEngine;

/* 
 * Basic movement from Code Monkey, visit: unitycodemonkey.com
 */

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Configuration")] [Space(10)]
    [SerializeField] private GameObject m_StunnedEffect;
    [SerializeField] private LayerMask platformsLayerMask;
    [SerializeField] private float m_JumpVelocity = 24F;
    [SerializeField] private float m_HitForce = 10F;
    [SerializeField] private float m_MoveSpeed = 6F;
    [SerializeField] private float m_MidAirControl = 3F;
    [SerializeField] private int m_AirJumpMax = 1;

    //private Player_Base playerBase;
    private Rigidbody2D m_Rigidbody2d;
    private BoxCollider2D m_BoxCollider2d;
    private PhotonView m_PhotonView;
    private int m_AirJumpCount = 0;

    private Vector2 m_CharacterDirection;
    public Vector2 CharacterDirection {
        get
        {
            return m_CharacterDirection;
        }

        set
        {
            m_CharacterDirection = value;
        }
    }

    private bool m_IsStunned;
    public bool IsStunned {
        get => m_IsStunned;
        set
        {
            m_IsStunned = value;
            m_StunnedEffect.SetActive(value);
        }
    }
    private float StunMaxTime = 1F;
    private float StunTimer = 0F;

    private void Awake()
    {
        //playerBase = gameObject.GetComponent<Player_Base>();
        m_Rigidbody2d = transform.GetComponent<Rigidbody2D>();
        m_BoxCollider2d = transform.GetComponent<BoxCollider2D>();
        m_PhotonView = GetComponent<PhotonView>();
        CharacterDirection = Vector2.right;
        IsStunned = false;
    }

    void Update()
    {
        if (ItIsMe())
        {
            if (!IsStunned)
            {
                bool isGrounded = IsGrounded();

                if (isGrounded)
                {
                    m_AirJumpCount = 0;
                }

                if (Input.GetKey(KeyCode.Space))
                {
                    if (isGrounded)
                    {
                        PerformJump();
                    }
                    else
                    {
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            if (m_AirJumpCount < m_AirJumpMax)
                            {
                                PerformJump();
                                m_AirJumpCount++;
                            }
                        }
                    }
                }

                HandleMovement_FullMidAirControl();
            }
            else
            {
                StunTimer += Time.deltaTime;
                if (StunTimer >= StunMaxTime)
                {
                    IsStunned = false;
                    StunTimer = 0F;
                }
            }
        }
    }

    public bool ItIsMe()
    {
        return m_PhotonView.IsMine;
    }

    public void TakeHit(Vector2 hitDirection)
    {
        m_Rigidbody2d.velocity = hitDirection * m_HitForce;
    }

    public bool IsGrounded()
    {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(m_BoxCollider2d.bounds.center, m_BoxCollider2d.bounds.size, 0f, Vector2.down, 0.1F, platformsLayerMask);
        return raycastHit2d.collider != null;
    }

    private void PerformJump()
    {
        m_Rigidbody2d.velocity = Vector2.up * m_JumpVelocity;
    }

    private void HandleMovement_FullMidAirControl()
    {
        float moveSpeed = m_MoveSpeed;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            m_Rigidbody2d.velocity = new Vector2(-moveSpeed, m_Rigidbody2d.velocity.y);
            CharacterDirection = Vector2.left;
        }
        else
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                m_Rigidbody2d.velocity = new Vector2(+moveSpeed, m_Rigidbody2d.velocity.y);
                CharacterDirection = Vector2.right;
            }
            else
            {
                // No keys pressed
                m_Rigidbody2d.velocity = new Vector2(0, m_Rigidbody2d.velocity.y);
            }
        }
    }
}
