using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NetworkClientHandler;

public class UIManager : MonoBehaviour
{

    [SerializeField] GameObject m_introPanel;


    private void Start()
    {

    }


    //for simulation only
    private void Update()
    {

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

}
