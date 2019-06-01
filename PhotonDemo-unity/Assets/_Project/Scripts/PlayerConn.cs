using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerConn : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private PhotonView m_pv;

    [SerializeField]
    private Canvas m_canvas;

    [SerializeField]
    private TextMeshProUGUI m_textMeshProUGUI;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        m_pv = GetComponent<PhotonView>();
        m_textMeshProUGUI.text = m_pv.Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        m_canvas.transform.LookAt(Camera.main.transform);
    }
}
