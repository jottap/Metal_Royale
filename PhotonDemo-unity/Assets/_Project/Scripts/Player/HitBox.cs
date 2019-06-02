using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField] private Collider2D m_OwnPlayerCollider2D;
    [SerializeField] private PlayerMovement m_OwnPlayerMovement;
    [SerializeField] private float m_CollisionDuration = 0.4F;

    private float m_CollisionTimer = 0F;

    private void Update()
    {
        m_CollisionTimer += Time.deltaTime;
        if (m_CollisionTimer >= m_CollisionDuration)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        m_CollisionTimer = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_OwnPlayerCollider2D != collision)
        {
            PlayerSkillSet playerSkillSet = collision.gameObject.GetComponent<PlayerSkillSet>();
            if (playerSkillSet != null)
            {
                playerSkillSet.HitAndStun(m_OwnPlayerMovement.CharacterDirection);
                playerSkillSet.ReleaseSkillSet();
            }
        }
    }
}
