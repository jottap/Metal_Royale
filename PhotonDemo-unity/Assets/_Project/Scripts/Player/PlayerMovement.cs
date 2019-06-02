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

    private bool IsGrounded()
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
        }
        else
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                m_Rigidbody2d.velocity = new Vector2(+moveSpeed, m_Rigidbody2d.velocity.y);
            }
            else
            {
                // No keys pressed
                m_Rigidbody2d.velocity = new Vector2(0, m_Rigidbody2d.velocity.y);
            }
        }
    }

    private void HandleMovement_SomeMidAirControl()
    {
        float moveSpeed = m_MoveSpeed;
        float midAirControl = m_MidAirControl;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (IsGrounded())
            {
                m_Rigidbody2d.velocity = new Vector2(-moveSpeed, m_Rigidbody2d.velocity.y);
            }
            else
            {
                m_Rigidbody2d.velocity += new Vector2(-moveSpeed * midAirControl * Time.deltaTime, 0);
                m_Rigidbody2d.velocity = new Vector2(Mathf.Clamp(m_Rigidbody2d.velocity.x, -moveSpeed, +moveSpeed), m_Rigidbody2d.velocity.y);
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                if (IsGrounded())
                {
                    m_Rigidbody2d.velocity = new Vector2(+moveSpeed, m_Rigidbody2d.velocity.y);
                }
                else
                {
                    m_Rigidbody2d.velocity += new Vector2(+moveSpeed * midAirControl * Time.deltaTime, 0);
                    m_Rigidbody2d.velocity = new Vector2(Mathf.Clamp(m_Rigidbody2d.velocity.x, -moveSpeed, +moveSpeed), m_Rigidbody2d.velocity.y);
                }
            }
            else
            {
                // No keys pressed
                if (IsGrounded())
                {
                    m_Rigidbody2d.velocity = new Vector2(0, m_Rigidbody2d.velocity.y);
                }
            }
        }
    }

    private void HandleMovement_NoMidAirControl()
    {
        if (IsGrounded())
        {
            float moveSpeed = m_MoveSpeed;
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                m_Rigidbody2d.velocity = new Vector2(-moveSpeed, m_Rigidbody2d.velocity.y);
            }
            else
            {
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    m_Rigidbody2d.velocity = new Vector2(+moveSpeed, m_Rigidbody2d.velocity.y);
                }
                else
                {
                    // No keys pressed
                    m_Rigidbody2d.velocity = new Vector2(0, m_Rigidbody2d.velocity.y);
                }
            }
        }
    }
}
