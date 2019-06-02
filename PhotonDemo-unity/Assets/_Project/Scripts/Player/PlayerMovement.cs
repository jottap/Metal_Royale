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
    private PlayerSkillSet m_PlayerSkillSet;
    private Rigidbody2D m_Rigidbody2d;
    private BoxCollider2D m_BoxCollider2d;
    private PhotonView m_PhotonView;
    private int m_AirJumpCount = 0;

    //Todo Remove m_ArrowHelper
    [SerializeField] private GameObject m_ArrowHelper;
    private Vector3 m_ArrowHelperPosition = new Vector3(0.008F, 0.002F, 0F);
    private Vector3 m_ArrowHelperRotation = new Vector3(0F, 0F, 20F);
    private Vector3 m_ArrowHelperScale = new Vector3(0.002F, -0.002F, 1F);
    private Vector3 m_N_ArrowHelperPosition = new Vector3(-0.008F, 0.002F, 0F);
    private Vector3 m_N_ArrowHelperRotation = new Vector3(0F, 0F, -20F);
    private Vector3 m_N_ArrowHelperScale = new Vector3(-0.002F, -0.002F, 1F);

    private Vector2 m_CharacterDirection;
    public Vector2 CharacterDirection {
        get
        {
            return m_CharacterDirection;
        }

        set
        {
            if (m_CharacterDirection != value)
            {
                m_CharacterDirection = value;

                bool isRightDirection = CharacterDirection == Vector2.right;
                if (isRightDirection)
                {
                    m_ArrowHelper.transform.localPosition = m_ArrowHelperPosition;
                    m_ArrowHelper.transform.localRotation = Quaternion.Euler(m_ArrowHelperRotation);
                    m_ArrowHelper.transform.localScale = m_ArrowHelperScale;
                }
                else
                {
                    m_ArrowHelper.transform.localPosition = m_N_ArrowHelperPosition;
                    m_ArrowHelper.transform.localRotation = Quaternion.Euler(m_N_ArrowHelperRotation);
                    m_ArrowHelper.transform.localScale = m_N_ArrowHelperScale;
                }
            }
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
        m_PlayerSkillSet = transform.GetComponent<PlayerSkillSet>();
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
            bool isPerformingSkillSet = !m_PlayerSkillSet.CanPerformGuitarrada || !m_PlayerSkillSet.CanPerformMetalPower;
            if (!isPerformingSkillSet)
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
            else
            {
                StopPlayer();
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
                StopPlayer();
            }
        }
    }

    private void StopPlayer()
    {
        m_Rigidbody2d.velocity = new Vector2(0, m_Rigidbody2d.velocity.y);
    }
}
