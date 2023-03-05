using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_countdownText;
    [SerializeField] float m_currentTimeCountDown;
    [SerializeField] float m_startTime = 3;
    [SerializeField] float m_multiplier = 1;
    [SerializeField] bool m_gameStarting = false;
    // Start is called before the first frame update
    void Start()
    {
        m_currentTimeCountDown = m_startTime;
        m_gameStarting = false;

        // GameManager.instance.onChangedGameState += OnChangeState;
    }

    // void OnChangeState(GameState p_gameState) 
    // {
    //
    //     switch (p_gameState)
    //     {
    //         case GameState.GAME_COUNTDOWN:
    //             m_gameStarting = true; break;
    //
    //         case GameState.GAME_END:
    //         default:
    //             m_gameStarting = false; break;
    //     }
    //    
    // }

    // Update is called once per frame
    void Update()
    {
        if (m_gameStarting)
        {
            m_currentTimeCountDown -= Time.deltaTime * m_multiplier;
            if (m_currentTimeCountDown <= 0)
            {
                m_currentTimeCountDown = 0;
                m_gameStarting = true;

                //Call GameManager to Start
                Debug.Log("GAME START");
                // GameManager.instance.ChangeGameState(GameState.GAME_START);
            }

            m_countdownText.text = m_currentTimeCountDown.ToString("##");
        }
    }
}
