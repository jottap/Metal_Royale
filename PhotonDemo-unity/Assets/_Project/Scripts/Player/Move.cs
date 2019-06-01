using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private PhotonView m_pv;

    [SerializeField]
    private Vector2 m_dir;
    [SerializeField]
    private float m_vel = 2;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        m_pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_pv.IsMine)
            MoveInput();
    }

    private void MoveInput()
    {
        m_dir = Vector2.zero;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            m_dir += Vector2.up;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            m_dir += Vector2.left;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            m_dir += Vector2.right;
        }

        transform.Translate(m_dir * m_vel * Time.deltaTime);

    }
}
