using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NetworkClientHandler;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    
    [Header("UI")]
    [SerializeField] GameObject playerLifeParent;
    [SerializeField] Transform[] playerLiveObjs;

    [SerializeField] TextMeshProUGUI m_totalScoreText;
    [SerializeField] TextMeshProUGUI m_player1NameText;

    [SerializeField] GameObject m_introPanel;

    int currentLife;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;

        //m_player.onPlayerHit += OnLifeChange;
        //m_player.OnPlayerHit += OnLifeChange;

    }
    private void Start()
    {
        Init();

        SessionsHandler.OnStartPlaying += ShowIntroPanel;
        SessionsHandler.OnPlayer1SetName += UpdateNamePlayer1;
    }


    //for simulation only
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            UpdateNamePlayer1("MABIN");
            ShowIntroPanel(false);
        }
    }

    private void OnDestroy()
    {
        SessionsHandler.OnStartPlaying -= ShowIntroPanel;
        SessionsHandler.OnPlayer1SetName -= UpdateNamePlayer1;
    }

    private void Init()
    {
        playerLiveObjs = new Transform[playerLifeParent.transform.childCount]; 
        for (int childIndex = 0; childIndex < playerLifeParent.transform.childCount; childIndex++)
        {
            playerLiveObjs[childIndex] = playerLifeParent.transform.GetChild(childIndex);
        }
        currentLife = playerLifeParent.transform.childCount;
    }

    //public void OnLifeChange(int p_life)
    public void UpdateLife()
    {
        currentLife--;
        Debug.Log($"player hit {currentLife}");
        playerLiveObjs[currentLife].gameObject.SetActive(false);
    }

    public void StartGame()
    {
        GameManager.instance.ChangeGameState(GameState.GAME_START);
    }

    public void ShowEndScreen()
    {
        // m_endScreenMenu.SetActive(true);
        // m_totalScoreText.text = GameManager.instance.GetScore().ToString();
    }

    public void ShowIntroPanel(bool p_show) {
        m_introPanel.SetActive(p_show);
        GameManager.instance.ChangeGameState(GameState.GAME_COUNTDOWN);
    }

    private void UpdateNamePlayer1(string value)
    {
        m_player1NameText.text = value;
    }
}
