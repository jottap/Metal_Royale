using Photon.Pun;
using UnityEngine;

public class PlayerSkillSet : MonoBehaviour
{
    private PlayerMovement m_PlayerMovement;
    private PlayerConn m_PlayerConn;

    public bool CanPerformGuitarrada { get; set; }
    public bool CanPerformMetalPower { get; set; }

    [SerializeField] private Collider2D m_HitBoxCollider2D;

    private void Awake()
    {
        m_PlayerMovement = transform.GetComponent<PlayerMovement>();
        m_PlayerConn = transform.GetComponent<PlayerConn>();

        CanPerformGuitarrada = true;
        CanPerformMetalPower = true;
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
                    this.GetComponent<PhotonView>().RPC("SetTrigger", RpcTarget.All);
                }
            }
        }
    }

    [PunRPC]
    public void SetTrigger()
    {
        m_PlayerMovement.AnimatorController.SetTrigger("Attack");
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
        this.GetComponent<PhotonView>().RPC("TakeHit", RpcTarget.All, new object[] { hitDirection });
    }

    private void PerformMetalPower()
    {

    }

    private void PerformHeavyMetalPower()
    {

    }
}
