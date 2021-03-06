﻿using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerConn : MonoBehaviour
{
    #region Variables
    [Header("Settings")]
    [SerializeField]
    private PhotonView m_pv;

    [SerializeField]
    private Canvas m_canvas;

    [SerializeField]
    private TextMeshProUGUI m_textMeshProUGUI;

    [SerializeField] private int m_MaxScore = 10;
    public int MaxScore
    {
        get => m_MaxScore;
    }

    [SerializeField]
    private int m_score;
    public int Score
    {
        get => m_score;
        set
        {
            if (value <= m_MaxScore)
            {
                this.GetComponent<PhotonView>().RPC("ScoreSetPhoton", RpcTarget.All, new object[] { value });
            }
        }
    }

    public TextMeshProUGUI NameLabel { get => m_textMeshProUGUI; set => m_textMeshProUGUI = value; }

    [Header("HUD")]
    [SerializeField]
    private ScoreHud ScoreHud;

    private int m_scoreMax = 10;

    [SerializeField]
    private bool isDeath;
    public bool IsDeath
    {
        get => isDeath;
        set
        {
            isDeath = value;
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { "IsDeath", value }
        });
        }
    }

    #endregion

    public void Init()
    {
        this.GetComponent<PhotonView>().RPC("InitPhoton", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    public void StartRespawn()
    {
        if (isDeath)
        {
            IsDeath = false;
            gameObject.SetActive(true);
        }
        else
        {
            GetComponent<PlayerMovement>().RestartPlayer();
        }
    }

    [PunRPC]
    public void InitPhoton()
    {
        m_pv = GetComponent<PhotonView>();
        NameLabel.text = m_pv.Owner.NickName;

        if (m_pv.IsMine)
            NameLabel.color = Color.green;


        GameManager.Instance.PlayerList.Add(this);

        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { "IsDeath", IsDeath },
            { "VIEWID", GetComponent<PhotonView>().ViewID }
        });

        if (GameManager.Instance.IsGameStarted)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_canvas.transform.LookAt(Camera.main.transform);

        if (transform.position.y <= -8) SetRespawn();
        //this.GetComponent<PhotonView>().RPC("RespawnPlayer", RpcTarget.All);


    }

    private void SetRespawn()
    {
        //GetComponent<PhotonView>().RPC("SetRespawnPlayer", RpcTarget.MasterClient);
        if (!GetComponent<PhotonView>().IsMine)
            return;

        IsDeath = true;

        this.GetComponent<PhotonView>().RPC("SetPosition", RpcTarget.All);
        GameManager.Instance.SetRespawnPlayer(this.gameObject);
    }

    [PunRPC]
    public void SetPosition()
    {
        this.transform.position = new Vector3(Random.Range(-5, 5), 0, 0);
        gameObject.SetActive(false);
    }

    [PunRPC]
    private void RespawnPlayer()
    {
        IsDeath = false;
        gameObject.SetActive(true);
    }

    public void GetItem()
    {
        Score++;
    }

    [PunRPC]
    public void ScoreSetPhoton(int value)
    {
        m_score = value;
        ScoreHud.ScoreSet(value);
    }

}
