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
    [SerializeField] bool m_gameStarted = false;
    // Start is called before the first frame update
    void Start()
    {
        m_currentTimeCountDown = m_startTime;
        m_gameStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_gameStarted)
            return;

        m_currentTimeCountDown -= Time.deltaTime * m_multiplier;
        if (m_currentTimeCountDown <= 0)
        {
            m_currentTimeCountDown = 0;
            m_gameStarted = true;

            //Call GameManager to Start
            Debug.Log("GAME START");
            GameManager.instance.ChangeGameState(GameState.GAME_START);
        }

        m_countdownText.text = m_currentTimeCountDown.ToString("##");

    }
}
