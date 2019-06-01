using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    #region Variables

    [Header("Login")]
    [SerializeField]
    private GameObject m_panelL;
    [SerializeField]
    private TMP_InputField m_namePlayer;

    [Header("Room")]
    [SerializeField]
    private GameObject m_panelR;
    [SerializeField]
    private TMP_InputField m_nameRoom;

    [Header("Loading")]
    [SerializeField]
    private GameObject m_panelLoading;

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
        Conn.Instance.OnConnectedAction += OnConnected;
        Conn.Instance.OnJoinedRoomAction += OnJoinedRoom;
    }

    private void UnSubscriveEvent()
    {
        Conn.Instance.LoginAction -= Login;
        Conn.Instance.OnConnectedAction -= OnConnected;
        Conn.Instance.OnJoinedRoomAction -= OnJoinedRoom;
    }

    private void Login()
    {
        m_panelL.SetActive(false);
        m_panelLoading.SetActive(true);
    }

    public void OnConnected()
    {
        m_panelLoading.SetActive(false);
        m_panelR.SetActive(true);
    }

    public void OnJoinedRoom()
    {

    }

    public void EnterLogin()
    {
        Conn.Instance.EnterLogin(m_namePlayer.text);
    }

    public void CreateRoom()
    {
        Conn.Instance.CreateRoom(m_nameRoom.text);
    }

}
