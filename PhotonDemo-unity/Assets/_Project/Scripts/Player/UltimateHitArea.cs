using Photon.Pun;
using UnityEngine;

public class UltimateHitArea : MonoBehaviour
{
    [SerializeField] private Collider2D m_OwnPlayerCollider2D;
    [SerializeField] private PlayerMovement m_OwnPlayerMovement;
    [SerializeField] private PlayerConn m_OwnPlayerConn;
    [SerializeField] private float m_BaseHitForce = 2F;
    
    private int m_Score;

    private void OnEnable()
    {
        m_Score = m_OwnPlayerConn.Score;
        m_OwnPlayerConn.Score = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_OwnPlayerCollider2D != collision)
        {
            PlayerSkillSet playerSkillSet = collision.gameObject.GetComponent<PlayerSkillSet>();
            if (playerSkillSet != null)
            {
                float force = m_Score * m_BaseHitForce;
                playerSkillSet.GetComponent<PhotonView>().RPC("HitAndStun", RpcTarget.All, new object[] { m_OwnPlayerMovement.CharacterDirection, force });
            }
        }
    }
}
