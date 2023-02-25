using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance => _instance;
    [SerializeField] GameObject m_endScreenMenu;

    [SerializeField] GameObject playerLifeParent;
    [SerializeField] Transform[] playerLiveObjs;

    [SerializeField] TextMeshProUGUI m_totalScoreText;
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        playerLiveObjs = playerLifeParent.GetComponentsInChildren<Transform>();
    }

    public void OnLifeChange()
    {

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
}
