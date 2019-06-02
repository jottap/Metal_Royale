using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Variables

    [Header("Settings")]
    [SerializeField]
    private List<PlayerConn> m_PlayerList;
    public List<PlayerConn> PlayerList { get => m_PlayerList; set => m_PlayerList = value; }

    [SerializeField]
    private float m_timeRespawnItem;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        GameObject playerGo = PhotonNetwork.Instantiate(Constants.PlayerPrefab, new Vector3(Random.Range(-5, 5), 0, 0), Quaternion.identity);
        PlayerList.Add(playerGo.GetComponent<PlayerConn>());

        playerGo.GetComponent<PlayerConn>().Init();

        SubscriveEvent();

        StartCoroutine(GenerateItem());
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

}
