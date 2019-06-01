using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class Conn : MonoBehaviourPunCallbacks
{
    #region Variables

    [Header("Settings")]
    [SerializeField]
    private GameObject m_panelL;
    [SerializeField]
    private GameObject m_panelR;

    [SerializeField]
    private TMP_InputField m_namePlayer;
    [SerializeField]
    private TMP_InputField m_nameRoom;

    [SerializeField]
    private TextMeshProUGUI m_nickName;

    [SerializeField]
    private GameObject m_playerPrefab;

    #endregion

    public void Login()
    {
        PhotonNetwork.NickName = m_namePlayer.text;
        PhotonNetwork.ConnectUsingSettings();

        m_panelL.SetActive(false);
        m_panelR.SetActive(true);
    }

    public void CreateRoom()
    {
        Debug.Log(" JoinOrCreateRoom !");
        PhotonNetwork.JoinOrCreateRoom(m_nameRoom.text, new RoomOptions(), TypedLobby.Default);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PhotonNetwork.Disconnect();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(" Conectado !");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log(" Join Lobby !");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log(" Conexão Perdida !");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(" Não entrou em nenhuma sala!");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(" Não entrou em nenhuma sala!");
    }

    public override void OnJoinedRoom()
    {
        Debug.LogFormat(" Entrei em uma sala !");
        Debug.LogFormat("Name Room : [{0}] Players Count : [{1}], NickName : [{2}]", PhotonNetwork.CurrentRoom.Name, PhotonNetwork.CurrentRoom.PlayerCount, PhotonNetwork.NickName);
        m_nickName.text = PhotonNetwork.NickName;

        m_panelR.SetActive(false);
        PhotonNetwork.Instantiate(m_playerPrefab.name, new Vector3(Random.Range(-5, 5), 0, 0), Quaternion.identity, 0);
    }

}
