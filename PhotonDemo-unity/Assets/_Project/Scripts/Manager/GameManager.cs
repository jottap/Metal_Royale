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

        GameObject playerGo = PhotonNetwork.Instantiate(Constants.PlayerPrefab, new Vector3(Random.Range(-5, 5), 0, 0), Quaternion.identity);

        playerGo.GetComponent<PlayerConn>().Init();
        SubscriveEvent();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            foreach (var item in PhotonNetwork.PlayerList)
            {
                Debug.Log("item.CustomProperties.Count >>> " + item.CustomProperties.Count);
                Debug.Log("item.CustomProperties.Keys.GetType >>> " + item.CustomProperties.Keys.GetType());
                Debug.Log(" IsDeath :: " + ((bool)item.CustomProperties["PlayerCoon"]));

                foreach (var entry in item.CustomProperties)
                {
                    Debug.Log(" KEY >>> " + entry.Key);
                    Debug.Log(" Value >>> " + entry.Value);
                }
            }
        }

        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
            IsGameStarted = bool.Parse(PhotonNetwork.CurrentRoom.CustomProperties["IsGameStarted"].ToString());
            startTimer = bool.Parse(PhotonNetwork.CurrentRoom.CustomProperties["startTimer"].ToString());
        }
        SetVar();

        if (!startTimer)
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.Count > 0 && (bool)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCoon"])
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
            Debug.Log(" IsDeath :: " + ((bool)item.CustomProperties["PlayerCoon"]));
        }

        foreach (var item in PlayerList)
        {
            item.GetComponent<PhotonView>().RPC("SetRespawn", RpcTarget.All);
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
            item.GetComponent<PhotonView>().RPC("SetRespawn", RpcTarget.All);
        }

        //foreach (var item in PhotonNetwork.PlayerList)
        //{
        //    Debug.Log(" IsDeath :: " + ((PlayerConn)item.CustomProperties["PlayerCoon"]).IsDeath);
        //}
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
        ButtonRespawn.gameObject.SetActive(true);
    }

    public void RespawnPlayer()
    {
        playerRespawn.GetComponent<PhotonView>().RPC("RespawnPlayer", RpcTarget.All);
        playerRespawn.gameObject.SetActive(true);
        ButtonRespawn.gameObject.SetActive(false);
    }

    public void SetVar()
    {
        m_DebugLabel.text = string.Format(" startTime : {0}\n  m_isGameStarted : {1}\n  startTimer : {2} \n ", startTime, IsGameStarted, startTimer);
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
