using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System;

public class Conn : MonoBehaviourPunCallbacks
{
    #region Variables

    public static Conn Instance;

    [Header("Settings")]

    #region Actions

    public Action LoginAction;
    public Action OnConnectedAction;
    public Action OnJoinedRoomAction;
    public Action OnDisconnectedAction;

    #endregion

    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void EnterLogin(string namePlayer)
    {
        PhotonNetwork.NickName = namePlayer;
        PhotonNetwork.ConnectUsingSettings();

        LoginAction?.Invoke();
    }

    public void CreateRoom(string nameRoom)
    {
        Debug.Log(" JoinOrCreateRoom !");
        PhotonNetwork.JoinOrCreateRoom(nameRoom, new RoomOptions(), TypedLobby.Default);
    }

    #region PhotonMethods

    public override void OnConnectedToMaster()
    {
        Debug.Log(" Conectado !");

        OnConnectedAction?.Invoke();
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log(" Join Lobby !");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log(" Conexão Perdida !");

        OnDisconnectedAction?.Invoke();
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

        OnJoinedRoomAction?.Invoke();

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(Constants.SceneGame);
        }
    }

    #endregion
}
