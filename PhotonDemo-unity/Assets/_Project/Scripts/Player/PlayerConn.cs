using UnityEngine;
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
                m_score = value;
                this.GetComponent<PhotonView>().RPC("ScoreSetPhoton", RpcTarget.All, new object[] { value });
            }
        }
    }

    

    public TextMeshProUGUI NameLabel { get => m_textMeshProUGUI; set => m_textMeshProUGUI = value; }

    [Header("HUD")]
    [SerializeField]
    private ScoreHud ScoreHud;

    private int m_scoreMax = 10;

    #endregion

    public void Init()
    {
        this.GetComponent<PhotonView>().RPC("InitPhoton", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    public void InitPhoton()
    {
        m_pv = GetComponent<PhotonView>();
        NameLabel.text = m_pv.Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        m_canvas.transform.LookAt(Camera.main.transform);

        if (transform.position.y <= -8)
        {
            transform.position = new Vector3(Random.Range(-5, 5), 0, 0);
        }
    }

    public void GetItem()
    {
        Score++;
    }

    [PunRPC]
    public void ScoreSetPhoton(int value)
    {
        ScoreHud.ScoreSet(value);
    }

}
