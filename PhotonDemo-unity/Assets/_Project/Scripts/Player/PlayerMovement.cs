using Photon.Pun;
using UnityEngine;

/* 
 * Basic movement from Code Monkey, visit: unitycodemonkey.com
 */

public class PlayerMovement : MonoBehaviour
{
    [Header("Sprites Configuration")]
    [Space(10)]
    [SerializeField] private GameObject m_SpritesGameObject;
    [SerializeField] private Animator m_AnimatorController;
    public Animator AnimatorController { get => m_AnimatorController; }

    private readonly Vector3 ToTheRight = new Vector3(0F, 0F, 0F);
    private readonly Vector3 ToTheLeft = new Vector3(0F, -180F, 0F);

    [Header("Player Configuration")]
    [Space(10)]
    [SerializeField] private GameObject m_StunnedEffect;
    [SerializeField] private LayerMask platformsLayerMask;
    [SerializeField] private float m_JumpVelocity = 24F;
    [SerializeField] private float m_HitForce = 30F;
    [SerializeField] private float m_MoveSpeed = 6F;
    [SerializeField] private float m_MidAirControl = 3F;
    [SerializeField] private int m_AirJumpMax = 1;

    //private Player_Base playerBase;
    private PlayerSkillSet m_PlayerSkillSet;
    private Rigidbody2D m_Rigidbody2d;
    private BoxCollider2D m_BoxCollider2d;
    private PhotonView m_PhotonView;
    private int m_AirJumpCount = 0;

    private Vector2 m_CharacterDirection;
    public Vector2 CharacterDirection
    {
        get
        {
            return m_CharacterDirection;
        }

        set
        {
            if (m_CharacterDirection != value)
            {
                this.GetComponent<PhotonView>().RPC("CharacterDirectionRPC", RpcTarget.All, new object[] { value });
            }
        }
    }

    private bool m_IsStunned;
    public bool IsStunned
    {
        get => m_IsStunned;
        set
        {
            this.GetComponent<PhotonView>().RPC("SetIsStuned", RpcTarget.All, new object[] { value });
        }
    }
    private float StunMaxTime = 1F;
    private float StunTimer = 0F;

    private float GetHorizontalInput { get => Input.GetAxisRaw("Horizontal"); }
    private bool GetJumpInput { get => Input.GetButtonDown("Jump"); }
    public bool IsGrounded
    {
        get
        {
            RaycastHit2D raycastHit2d = Physics2D.BoxCast(m_BoxCollider2d.bounds.center, m_BoxCollider2d.bounds.size, 0f, Vector2.down, 0.1F, platformsLayerMask);
            return raycastHit2d.collider != null;
        }
    }

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

    [PunRPC]
    public void CharacterDirectionRPC(Vector2 value)
    {
        m_CharacterDirection = value;

        bool isRightDirection = CharacterDirection == Vector2.right;
        if (isRightDirection)
        {
            m_SpritesGameObject.transform.localRotation = Quaternion.Euler(ToTheRight);
        }
        else
        {
            m_SpritesGameObject.transform.localRotation = Quaternion.Euler(ToTheLeft);
        }
    }

    [PunRPC]
    public void SetIsStuned(bool value)
    {
        Debug.Log(" SetStuned : " + value);
        m_IsStunned = value;
        m_StunnedEffect.SetActive(value);
        AnimatorController.SetBool("IsStunned", value);
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
                    bool isGrounded = IsGrounded;

                    if (isGrounded)
                    {
                        m_AirJumpCount = 0;
                    }

                    if (GetJumpInput)
                    {
                        if (isGrounded)
                        {
                            PerformJump();
                        }
                        else
                        {
                            if (GetJumpInput)
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
                        StopPlayer();
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

    [PunRPC]
    public void TakeHit(Vector2 hitDirection)
    {
        m_Rigidbody2d.AddForce(hitDirection * m_HitForce, ForceMode2D.Impulse);
    }

    private void PerformJump()
    {
        m_Rigidbody2d.velocity = Vector2.up * m_JumpVelocity;
    }

    private void HandleMovement_FullMidAirControl()
    {

        m_Rigidbody2d.velocity = new Vector2(GetHorizontalInput * m_MoveSpeed, m_Rigidbody2d.velocity.y);

        if (GetHorizontalInput < 0) CharacterDirection = Vector2.left;
        else if (GetHorizontalInput > 0) CharacterDirection = Vector2.right;

        this.GetComponent<PhotonView>().RPC("SetVelocity", RpcTarget.All, new object[] { (GetHorizontalInput == 0 ? 0 : 1) });
    }

    [PunRPC]
    public void SetVelocity(int value)
    {
        m_AnimatorController.SetInteger("Velocity", value);
    }

    private void StopPlayer()
    {
        m_Rigidbody2d.velocity = new Vector2(0, m_Rigidbody2d.velocity.y);
        //m_AnimatorController.SetInteger("Velocity", 0);
        this.GetComponent<PhotonView>().RPC("SetVelocity", RpcTarget.All, new object[] { 0 });
    }
}
