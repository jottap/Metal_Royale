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
    [SerializeField] private ParticleSystem m_HitEffect;
    [SerializeField] private GameObject m_StunnedEffect;
    [SerializeField] private LayerMask platformsLayerMask;
    [SerializeField] private float m_JumpVelocity = 24F;
    [SerializeField] private float m_MoveSpeed = 6F;
    [SerializeField] private float m_MidAirControl = 3F;
    [SerializeField] private int m_AirJumpMax = 1;

    //private Player_Base playerBase;
    private PlayerSkillSet m_PlayerSkillSet;
    private Rigidbody2D m_Rigidbody2d;
    private Collider2D m_BoxCollider2d;
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
    private float StunMaxTime = 0.6F;
    private float StunTimer = 0F;

    private bool m_IsWinner;
    public bool IsWinner
    {
        get => m_IsWinner;
        set
        {
            m_IsWinner = value;
            m_PlayerSkillSet.CanPerformSkill = !value;
        }
    }

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
        m_BoxCollider2d = transform.GetComponent<Collider2D>();
        m_PhotonView = GetComponent<PhotonView>();
        CharacterDirection = Vector2.right;
        IsStunned = false;
        IsWinner = false;
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

        if (value == false)
        {
            m_PlayerSkillSet.ReleaseSkillSet();
        }
    }

    void Update()
    {
        if (ItIsMe())
        {
            bool isPerformingSkillSet = !m_PlayerSkillSet.CanPerformSkill;
            if (!IsStunned && !isPerformingSkillSet && !IsWinner)
            {
                bool isGrounded = IsGrounded;
                m_AnimatorController.SetBool("IsGrounded", isGrounded);

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
            else if (IsStunned)
            {
                StunTimer += Time.deltaTime;
                if (StunTimer >= StunMaxTime)
                {
                    IsStunned = false;
                    StunTimer = 0F;
                    StopPlayer();
                }
            }
            else if (isPerformingSkillSet)
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
    public void TakeHit(Vector2 hitDirection, float hitForce)
    {
        m_HitEffect.Play();
        m_Rigidbody2d.velocity = hitDirection * hitForce;
    }

    private void PerformJump()
    {
        m_AnimatorController.SetTrigger("Jump");
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

    public void WinGame()
    {
        IsWinner = true;
        this.GetComponent<PhotonView>().RPC("PerformWin", RpcTarget.All);
    }

    [PunRPC]
    public void PerformWin()
    {
        m_AnimatorController.SetTrigger("Win");
    }

    private void StopPlayer()
    {
        m_Rigidbody2d.velocity = new Vector2(0, m_Rigidbody2d.velocity.y);
        this.GetComponent<PhotonView>().RPC("SetVelocity", RpcTarget.All, new object[] { 0 });
    }
}
