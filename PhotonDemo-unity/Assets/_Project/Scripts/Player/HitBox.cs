using Photon.Pun;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField] private Collider2D m_OwnPlayerCollider2D;
    [SerializeField] private PlayerMovement m_OwnPlayerMovement;
    [SerializeField] private float m_HitForce = 4F;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_OwnPlayerCollider2D != collision)
        {
            PlayerSkillSet playerSkillSet = collision.gameObject.GetComponent<PlayerSkillSet>();
            if (playerSkillSet != null)
            {
                playerSkillSet.GetComponent<PhotonView>().RPC("HitAndStun", RpcTarget.All, new object[] { m_OwnPlayerMovement.CharacterDirection, m_HitForce });
            }
        }
    }
}
