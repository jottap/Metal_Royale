using Photon.Pun;
using System.Collections;
using UnityEngine;

public class PlayerSkillSet : MonoBehaviour
{
    private PlayerMovement m_PlayerMovement;

    public bool CanPerformSkill { get; set; }

    [SerializeField] private Collider2D m_HitBoxCollider2D;
    [SerializeField] private float m_UltimateDuration = 2F;

    private void Awake()
    {
        m_PlayerMovement = transform.GetComponent<PlayerMovement>();
        CanPerformSkill = true;
    }

    private void Update()
    {
        if (m_PlayerMovement.ItIsMe() && CanPerformSkill)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                CanPerformSkill = false;
                this.GetComponent<PhotonView>().RPC("SetTriggerUltimate", RpcTarget.All, new object[] { true });
            }
            else if (Input.GetKeyDown(KeyCode.Q) && m_PlayerMovement.IsGrounded)
            {
                CanPerformSkill = false;
                this.GetComponent<PhotonView>().RPC("SetTriggerAttack", RpcTarget.All);
            }
        }
    }

    public void FinishUltimate()
    {
        StartCoroutine(FinishUltimateCoroutine());
    }

    private IEnumerator FinishUltimateCoroutine()
    {
        yield return new WaitForSeconds(m_UltimateDuration);
        this.GetComponent<PhotonView>().RPC("SetTriggerUltimate", RpcTarget.All, new object[] { false });
        ReleaseSkillSet();
    }

    [PunRPC]
    public void SetTriggerAttack()
    {
        m_PlayerMovement.AnimatorController.SetTrigger("Attack");
    }

    [PunRPC]
    public void SetTriggerUltimate(bool isActive)
    {
        m_PlayerMovement.AnimatorController.SetBool("IsUltimateOn", isActive);
        if (isActive) m_PlayerMovement.AnimatorController.SetTrigger("Ulti");
    }

    public void ReleaseSkillSet()
    {
        CanPerformSkill = true;
    }

    [PunRPC]
    public void HitAndStun(Vector2 hitDirection, float hitForce)
    {
        m_PlayerMovement.IsStunned = true;
        this.GetComponent<PhotonView>().RPC("TakeHit", RpcTarget.All, new object[] { hitDirection, hitForce });
    }
}
