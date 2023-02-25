using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NetworkClientHandler;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance => _instance;
    [Header("Players")]
    [SerializeField] Player m_player;

    [Header("UI")]

    [SerializeField] GameObject m_endScreenMenu;
    [SerializeField] GameObject playerLifeParent;
    [SerializeField] Transform[] playerLiveObjs;

    [SerializeField] TextMeshProUGUI m_totalScoreText;
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
        m_endScreenMenu.SetActive(true);
        m_totalScoreText.text = GameManager.instance.GetScore().ToString();
    }

    public void ShowIntroPanel(bool p_show) {
        m_introPanel.SetActive(p_show);
        GameManager.instance.ChangeGameState(GameState.GAME_COUNTDOWN);
    }
}
