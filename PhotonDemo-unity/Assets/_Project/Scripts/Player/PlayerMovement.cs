using Photon.Pun;
using UnityEngine;

/* 
 * Basic movement from Code Monkey, visit: unitycodemonkey.com
 */

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Configuration")] [Space(10)]
    [SerializeField] private LayerMask platformsLayerMask;
    [SerializeField] private float m_JumpVelocity = 24F;
    [SerializeField] private float m_MoveSpeed = 6F;
    [SerializeField] private float m_MidAirControl = 3F;
    [SerializeField] private int m_AirJumpMax = 1;

    //private Player_Base playerBase;
    private Rigidbody2D m_Rigidbody2d;
    private BoxCollider2D m_BoxCollider2d;
    private PhotonView m_PhotonView;
    private int m_AirJumpCount = 0;

    private float GetHorizontalInput { get => Input.GetAxisRaw("Horizontal"); }
    private bool GetJumpInput { get => Input.GetButtonDown("Jump"); }
    private bool IsGrounded
    {
        get {
            RaycastHit2D raycastHit2d = Physics2D.BoxCast(m_BoxCollider2d.bounds.center, m_BoxCollider2d.bounds.size, 0f, Vector2.down, 0.1F, platformsLayerMask);
            return raycastHit2d.collider != null;
        }
    }

    private void Awake()
    {
        //playerBase = gameObject.GetComponent<Player_Base>();
        m_Rigidbody2d = transform.GetComponent<Rigidbody2D>();
        m_BoxCollider2d = transform.GetComponent<BoxCollider2D>();
        m_PhotonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (m_PhotonView.IsMine)
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
            //HandleMovement_SomeMidAirControl();
            //HandleMovement_NoMidAirControl();

            // Set Animations
            //if (IsGrounded())
            //{
            //    if (rigidbody2d.velocity.x == 0)
            //    {
            //        playerBase.PlayIdleAnim();
            //    }
            //    else
            //    {
            //        playerBase.PlayMoveAnim(new Vector2(rigidbody2d.velocity.x, 0f));
            //    }
            //}
            //else
            //{
            //    playerBase.PlayJumpAnim(rigidbody2d.velocity);
            //}
        }
    }

    

    private void PerformJump()
    {
        m_Rigidbody2d.velocity = Vector2.up * m_JumpVelocity;
    }

    private void HandleMovement_FullMidAirControl()
    {
        float moveSpeed = m_MoveSpeed;

        m_Rigidbody2d.velocity = new Vector2(GetHorizontalInput * m_MoveSpeed, m_Rigidbody2d.velocity.y);

    }

    private void HandleMovement_SomeMidAirControl()
    {
        float moveSpeed = m_MoveSpeed;
        float midAirControl = m_MidAirControl;

        if (IsGrounded)
        {
            m_Rigidbody2d.velocity = new Vector2(GetHorizontalInput * m_MoveSpeed, m_Rigidbody2d.velocity.y);
        }
        else {
            m_Rigidbody2d.velocity += new Vector2((GetHorizontalInput * m_MoveSpeed) * midAirControl * Time.deltaTime, 0);
            m_Rigidbody2d.velocity = new Vector2(Mathf.Clamp(m_Rigidbody2d.velocity.x, -moveSpeed, +moveSpeed), m_Rigidbody2d.velocity.y);
        }

    }

    private void HandleMovement_NoMidAirControl()
    {
        if (IsGrounded)
        {
            float moveSpeed = m_MoveSpeed;

            m_Rigidbody2d.velocity = new Vector2(GetHorizontalInput * m_MoveSpeed, m_Rigidbody2d.velocity.y);

        }
    }
}
