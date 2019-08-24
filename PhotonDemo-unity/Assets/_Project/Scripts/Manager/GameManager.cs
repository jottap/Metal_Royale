using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class GameManager : MonoBehaviour
{
    #region Variables

    public static GameManager Instance;

    public const string PLAYER_PREFAB = "Player_{0}";

    [Header("Settings")]
    [SerializeField]
    private List<PlayerConn> m_PlayerList;
    public List<PlayerConn> PlayerList { get => m_PlayerList; set => m_PlayerList = value; }
    public bool IsGameStarted { get => m_isGameStarted; set => m_isGameStarted = value; }

    [SerializeField]
    private float m_timeRespawnItem;

    [SerializeField]
    private Button ButtonRespawn;
    [SerializeField]
    private Button ButtonStartGame;

    [SerializeField]
    private GameObject WaintingStart;

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

    [SerializeField]
    private List<PlayerConn> m_PlayerListLive;

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
            CustomeValue.Add("IsGameStarted", IsGameStarted);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomeValue);
            ButtonStartGame.gameObject.SetActive(true);

        }
        else
        {
            startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
            IsGameStarted = bool.Parse(PhotonNetwork.CurrentRoom.CustomProperties["IsGameStarted"].ToString());
            startTimer = bool.Parse(PhotonNetwork.CurrentRoom.CustomProperties["startTimer"].ToString());
        }

        GameObject playerGo = PhotonNetwork.Instantiate(string.Format(PLAYER_PREFAB, CharacterSelection.charId + 1), new Vector3(Random.Range(-5, 5), 0, 0), Quaternion.identity);

        playerGo.GetComponent<PlayerConn>().Init();
        SubscriveEvent();

    }

    void Update()
    {

        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
            IsGameStarted = bool.Parse(PhotonNetwork.CurrentRoom.CustomProperties["IsGameStarted"].ToString());
            startTimer = bool.Parse(PhotonNetwork.CurrentRoom.CustomProperties["startTimer"].ToString());
        }

        if (!startTimer)
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.Count > 0 && (bool)PhotonNetwork.LocalPlayer.CustomProperties["IsDeath"])
                WaintingStart.gameObject.SetActive(true);

            return;
        }

        WaintingStart.gameObject.SetActive(!startTimer);

        timerIncrementValue = PhotonNetwork.Time - startTime;
        m_timeLabel.text = (timer - timerIncrementValue).ToString("F0");

        if (timerIncrementValue >= timer)
        {
            StopGame();
        }
    }

    public void CheckDeath()
    {
        int i = 0;
        foreach (var item in PhotonNetwork.PlayerList)
        {
            Debug.Log(" IsDeath :: " + item.CustomProperties["VIEWID"]);
            Debug.Log(" IsDeath :: " + ((bool)item.CustomProperties["IsDeath"]));
            Debug.Log(" IsDeath :: " + item.UserId);
            Debug.Log(" IsDeath :: " + item.ActorNumber);

            if (((bool)item.CustomProperties["IsDeath"]))
            {
                PlayerConn aux = PlayerList.FirstOrDefault(x => x.GetComponent<PhotonView>().Owner.UserId == item.UserId);
                m_PlayerListLive.Remove(aux);
            }
        }

        //Empate
        if (m_PlayerListLive.Count == 0)
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
                ButtonStartGame.gameObject.SetActive(true);
        }
        else
        {
            if (m_PlayerListLive.Count == 1)
            {
                //WIN
                m_PlayerListLive[0].GetComponent<PlayerMovement>().WinGame();
                if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    ButtonStartGame.gameObject.SetActive(true);
            }
        }
    }

    public IEnumerator GenerateItem()
    {
        while (true && IsGameStarted)
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
        m_PlayerListLive = new List<PlayerConn>(m_PlayerList);

        IsGameStarted = true;
        ButtonStartGame.gameObject.SetActive(false);
        startTime = PhotonNetwork.Time;
        startTimer = true;

        ExitGames.Client.Photon.Hashtable CustomeValue = new ExitGames.Client.Photon.Hashtable();
        startTime = PhotonNetwork.Time;

        CustomeValue.Add("StartTime", startTime);
        CustomeValue.Add("startTimer", startTimer);
        CustomeValue.Add("IsGameStarted", IsGameStarted);


        PhotonNetwork.CurrentRoom.SetCustomProperties(CustomeValue);
        StartCoroutine(GenerateItem());

        foreach (var item in PlayerList)
        {
            item.GetComponent<PhotonView>().RPC("RespawnPlayer", RpcTarget.All);
        }
    }

    public void StopGame()
    {
        IsGameStarted = false;
        ButtonRespawn.gameObject.SetActive(false);

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            ButtonStartGame.gameObject.SetActive(true);
            CheckDeath();
        }

        startTimer = false;

        ExitGames.Client.Photon.Hashtable CustomeValue = new ExitGames.Client.Photon.Hashtable();
        startTime = PhotonNetwork.Time;

        CustomeValue.Add("StartTime", startTime);
        CustomeValue.Add("startTimer", startTimer);
        CustomeValue.Add("IsGameStarted", IsGameStarted);

        PhotonNetwork.CurrentRoom.SetCustomProperties(CustomeValue);
    }

    public void SetRespawnPlayer(GameObject player)
    {
        playerRespawn = player;
        Debug.Log(" playerRespawn : " + playerRespawn.transform.position);
        if (startTimer)
        {
            ButtonRespawn.gameObject.SetActive(true);
        }
        else
        {
            CheckDeath();
        }
    }

    public void RespawnPlayer()
    {
        playerRespawn.GetComponent<PhotonView>().RPC("RespawnPlayer", RpcTarget.All);
        playerRespawn.gameObject.SetActive(true);
        ButtonRespawn.gameObject.SetActive(false);
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
