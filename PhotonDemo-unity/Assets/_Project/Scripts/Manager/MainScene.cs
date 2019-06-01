using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    #region Variables

    [Header("Settings")]
    [SerializeField]
    private GameObject m_panelL;
    [SerializeField]
    private GameObject m_panelR;
    [SerializeField]
    private GameObject m_panelLoading;

    [SerializeField]
    private TMP_InputField m_namePlayer;
    [SerializeField]
    private TMP_InputField m_nameRoom;

    [SerializeField]
    private TextMeshProUGUI m_nickName;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        m_panelL.SetActive(true);

        SubscriveEvent();
    }

    private void OnDestroy()
    {
        UnSubscriveEvent();
    }

    private void SubscriveEvent()
    {
        Conn.Instance.LoginAction += Login;
    }

    private void UnSubscriveEvent()
    {
        Conn.Instance.LoginAction -= Login;
    }

    public void Login()
    {
        m_panelL.SetActive(false);
        m_panelLoading.SetActive(true);
    }

}
