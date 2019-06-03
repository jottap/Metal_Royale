using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    #region Variables

    public static GameManager Instance;

    [Header("Settings")]
    [SerializeField]
    private List<PlayerConn> m_PlayerList;
    public List<PlayerConn> PlayerList { get => m_PlayerList; set => m_PlayerList = value; }

    [SerializeField]
    private float m_timeRespawnItem;

    [SerializeField]
    private Button ButtonRespawn;
    [SerializeField]
    private Button ButtonStartGame;

    [SerializeField]
    private GameObject playerRespawn;

    [SerializeField]
    private bool m_isGameStarted;


    [Header("Time")]
    [SerializeField]
    private double startTime = 0;
    [SerializeField]
    private double timerIncrementValue = 0;
    [SerializeField]
    private TextMeshProUGUI m_timeLabel;
    [SerializeField]
    private double timer = 60;
    [SerializeField]
    private bool startTimer;

    [Header("Debug")]
    [SerializeField]
    private TextMeshProUGUI m_DebugLabel;

    #endregion

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable CustomeValue = new ExitGames.Client.Photon.Hashtable();
            startTime = PhotonNetwork.Time;
            startTimer = false;
            CustomeValue.Add("StartTime", startTime);
            CustomeValue.Add("startTimer", startTimer);
            CustomeValue.Add("IsGameStarted", m_isGameStarted);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomeValue);
            ButtonStartGame.gameObject.SetActive(true);

            StartCoroutine(GenerateItem());
        }
        else
        {
            startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
            m_isGameStarted = bool.Parse(PhotonNetwork.CurrentRoom.CustomProperties["IsGameStarted"].ToString());
            startTimer = bool.Parse(PhotonNetwork.CurrentRoom.CustomProperties["startTimer"].ToString());
        }

        GameObject playerGo = PhotonNetwork.Instantiate(Constants.PlayerPrefab, new Vector3(Random.Range(-5, 5), 0, 0), Quaternion.identity);
        PlayerList.Add(playerGo.GetComponent<PlayerConn>());

        playerGo.GetComponent<PlayerConn>().Init();

        SubscriveEvent();

        if (m_isGameStarted)
        {
            playerGo.gameObject.SetActive(false);
        }
        else
        {
            playerGo.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
            m_isGameStarted = bool.Parse(PhotonNetwork.CurrentRoom.CustomProperties["IsGameStarted"].ToString());
            startTimer = bool.Parse(PhotonNetwork.CurrentRoom.CustomProperties["startTimer"].ToString());
        }
        SetVar();

        if (!startTimer) return;

        timerIncrementValue = PhotonNetwork.Time - startTime;
        m_timeLabel.text = timerIncrementValue.ToString("F0");

        if (timerIncrementValue >= timer)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            {

            }
            StopGame();
        }
    }

    public IEnumerator GenerateItem()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_timeRespawnItem);
            PhotonNetwork.Instantiate(Constants.ItemPrefab, new Vector3(Random.Range(-5, 5), 7, 0), Quaternion.identity);
        }
    }

    private void OnDestroy()
    {
        UnSubscriveEvent();
    }

    private void SubscriveEvent()
    {
        Conn.Instance.OnDisconnectedAction += LoadMain;
    }

    private void UnSubscriveEvent()
    {
        Conn.Instance.OnDisconnectedAction -= LoadMain;
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public void LoadMain()
    {
        SceneManager.LoadScene(Constants.MainScene);
    }

    public void StartGame()
    {
        m_isGameStarted = true;
        ButtonStartGame.gameObject.SetActive(false);
        startTime = PhotonNetwork.Time;
        startTimer = true;

        ExitGames.Client.Photon.Hashtable CustomeValue = new ExitGames.Client.Photon.Hashtable();
        startTime = PhotonNetwork.Time;

        CustomeValue.Add("StartTime", startTime);
        CustomeValue.Add("startTimer", startTimer);
        CustomeValue.Add("IsGameStarted", m_isGameStarted);

        PhotonNetwork.CurrentRoom.SetCustomProperties(CustomeValue);
    }

    public void StopGame()
    {
        m_isGameStarted = false;
        ButtonStartGame.gameObject.SetActive(true);

        ExitGames.Client.Photon.Hashtable CustomeValue = new ExitGames.Client.Photon.Hashtable();
        startTime = PhotonNetwork.Time;

        CustomeValue.Add("StartTime", startTime);
        CustomeValue.Add("startTimer", startTimer);
        CustomeValue.Add("IsGameStarted", m_isGameStarted);

        PhotonNetwork.CurrentRoom.SetCustomProperties(CustomeValue);
    }

    public void SetRespawnPlayer(GameObject player)
    {
        playerRespawn = player;
        Debug.Log(" playerRespawn : " + playerRespawn.transform.position);
        ButtonRespawn.gameObject.SetActive(true);
    }

    public void RespawnPlayer()
    {
        playerRespawn.GetComponent<PhotonView>().RPC("RespawnPlayer", RpcTarget.MasterClient);
        playerRespawn.gameObject.SetActive(true);
        ButtonRespawn.gameObject.SetActive(false);
    }

    public void SetVar()
    {
        m_DebugLabel.text = string.Format(" startTime : {0}\n  m_isGameStarted : {1}\n  startTimer : {2} \n ", startTime, m_isGameStarted, startTimer);
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            Debug.Log(stream.Count);
        }

        else
        {
        }
    }
}
