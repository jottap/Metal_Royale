using Photon.Pun;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField] private Collider2D m_OwnPlayerCollider2D;
    [SerializeField] private PlayerMovement m_OwnPlayerMovement;
    [SerializeField] private PlayerSkillSet m_OwnPlayerSkillSet;
    [SerializeField] private float m_CollisionDuration = 0.4F;

    private float m_CollisionTimer = 0F;

    private void Update()
    {
        m_CollisionTimer += Time.deltaTime;
        if (m_CollisionTimer >= m_CollisionDuration)
        {
            m_CollisionTimer = 0;
            gameObject.SetActive(false);
            m_OwnPlayerSkillSet.ReleaseSkillSet();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_OwnPlayerCollider2D != collision)
        {
            PlayerSkillSet playerSkillSet = collision.gameObject.GetComponent<PlayerSkillSet>();
            if (playerSkillSet != null)
            {
                playerSkillSet.GetComponent<PhotonView>().RPC("HitAndStun", RpcTarget.All, new object[] { m_OwnPlayerMovement.CharacterDirection });
                //playerSkillSet.HitAndStun(m_OwnPlayerMovement.CharacterDirection);
            }
        }
    }
}
