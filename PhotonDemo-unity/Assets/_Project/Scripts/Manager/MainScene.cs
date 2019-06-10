using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScene : MonoBehaviour
{
    [SerializeField] private GameObject m_boardObject;

    [Space(10)]
    [Header("Login")]
    [SerializeField] private GameObject m_panelLogin;
    [SerializeField] private TMP_InputField m_inputFieldName;
    [SerializeField] private TextMeshProUGUI m_tmpWarningName;

    [Space(10)]
    [Header("Room")]
    [SerializeField] private GameObject m_panelRoom;
    [SerializeField] private TMP_InputField m_inputFieldIdRoom;
    [SerializeField] private TextMeshProUGUI m_tmpWarningRoom;

    [Space(10)]
    [Header("Loading")]
    [SerializeField] private GameObject m_panelLoading;

    [Space(10)]
    [Header("ACTION BUTTONS")]
    [SerializeField] private GameObject m_btnsPanel;
    [Space(10)]
    [SerializeField] private Button m_btnPositive;
    [SerializeField] private TextMeshProUGUI m_labelPositive;
    [Space(10)]
    [SerializeField] private Button m_btnNegative;
    [SerializeField] private TextMeshProUGUI m_labelNegative;

    [Space(10)]
    [Header("TUTORIAL")]
    [SerializeField] private GameObject m_tutorialPanel;

    [Space(10)]
    [Header("OTHERS")]
    [SerializeField] private float m_startDelay = 4;


    private string m_userNickname;
    private string m_roomId;

    private const string USER_NAME_NULL = "You must enter your nickname.";
    private const string ROOM_ID_NULL = "You must enter a valid room id.";



    // Start is called before the first frame update
    void Start()
    {
        m_tmpWarningName.text = "";
        m_tmpWarningRoom.text = "";
        CloseAllPanels();
        
        SubscriveEvent();
        m_boardObject.SetActive(false);
        StartCoroutine(StartDelay());
    }

    private IEnumerator StartDelay() {
        yield return new WaitForSeconds(m_startDelay);
        m_boardObject.SetActive(true);
        ActivePanelLogin();

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
        ActiveLoadingScreen();
    }

    public void OnConnected()
    {
        ActivePanelRoom();
    }

    public void OnJoinedRoom()
    {

    }

    private void ActivePanelLogin() {
        CloseAllPanels();
        m_panelLogin.SetActive(true);

        m_btnsPanel.SetActive(true);
        m_tmpWarningName.text = "";

        m_btnPositive.gameObject.SetActive(true);
        m_labelPositive.text = "Enter";
        m_btnPositive.onClick.RemoveAllListeners();
        m_btnPositive.onClick.AddListener(() => EnterLogin());

        m_btnNegative.gameObject.SetActive(true);
        m_labelNegative.text = "Tutoriial";
        m_btnNegative.onClick.RemoveAllListeners();
        m_btnNegative.onClick.AddListener(() => ActiveTutorialScreen());
    }

    private void ActiveTutorialScreen() {

        CloseAllPanels();
        m_tutorialPanel.SetActive(true);

        m_btnsPanel.SetActive(true);

        m_btnNegative.gameObject.SetActive(true);
        m_labelNegative.text = "Return";
        m_btnNegative.onClick.RemoveAllListeners();
        m_btnNegative.onClick.AddListener(() => ActivePanelLogin());
    }

    private void ActiveLoadingScreen() {
        CloseAllPanels();
        m_panelLoading.SetActive(true);

    }

    private void ActivePanelRoom()
    {
        CloseAllPanels();
        m_panelRoom.SetActive(true);

        m_btnsPanel.SetActive(true);
        m_tmpWarningRoom.text = "";

        m_btnPositive.gameObject.SetActive(true);
        m_labelPositive.text = "Enter";
        m_btnPositive.onClick.RemoveAllListeners();
        m_btnPositive.onClick.AddListener(() => CreateRoom());

        m_btnNegative.gameObject.SetActive(true);
        m_labelNegative.text = "return";
        m_btnNegative.onClick.RemoveAllListeners();
        m_btnNegative.onClick.AddListener(() => {
            PhotonNetwork.Disconnect();
            ActivePanelLogin();

        });
    }

    private void CloseAllPanels() {
        m_panelLogin.SetActive(false);
        m_panelRoom.SetActive(false);
        m_panelLoading.SetActive(false);
        m_btnPositive.gameObject.SetActive(false);
        m_btnNegative.gameObject.SetActive(false);
        m_btnsPanel.SetActive(false);
        m_tutorialPanel.SetActive(false);
    }


    public void EnterLogin()
    {
        m_userNickname = m_inputFieldName.text;

        if (!string.IsNullOrEmpty(m_userNickname))
        {
            Conn.Instance.EnterLogin(m_userNickname);
        }
        else {
            m_tmpWarningName.text = USER_NAME_NULL;
        }
    }

    public void CreateRoom()
    {
        m_roomId = m_inputFieldIdRoom.text;

        if (!string.IsNullOrEmpty(m_roomId))
        {
            Conn.Instance.CreateRoom(m_roomId);
        }
        else
        {
            m_tmpWarningRoom.text = ROOM_ID_NULL;
        }

    }

}
