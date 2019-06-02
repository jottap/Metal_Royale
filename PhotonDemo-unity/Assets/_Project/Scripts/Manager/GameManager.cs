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

    public Button ButtonRespawn;
    public GameObject playerRespawn;

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
            startTimer = true;
            CustomeValue.Add("StartTime", startTime);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomeValue);
        }
        else
        {
            startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
            startTimer = true;
        }

        GameObject playerGo = PhotonNetwork.Instantiate(Constants.PlayerPrefab, new Vector3(Random.Range(-5, 5), 0, 0), Quaternion.identity);
        PlayerList.Add(playerGo.GetComponent<PlayerConn>());

        playerGo.GetComponent<PlayerConn>().Init();

        SubscriveEvent();

        StartCoroutine(GenerateItem());
    }

    void Update()
    {

        if (!startTimer) return;

        timerIncrementValue = PhotonNetwork.Time - startTime;
        m_timeLabel.text = timerIncrementValue.ToString("F0");

        if (timerIncrementValue >= timer)
        {
            //Timer Completed
            //Do What Ever You What to Do Here
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

    public void SetRespawnPlayer(GameObject player)
    {
        playerRespawn = player;
        Debug.Log(" playerRespawn : " + playerRespawn.transform.position);
        ButtonRespawn.gameObject.SetActive(true);
    }

    public void RespawnPlayer()
    {
        playerRespawn.GetComponent<PhotonView>().RPC("RespawnPlayer", RpcTarget.All);
        ButtonRespawn.gameObject.SetActive(false);
    }

}
