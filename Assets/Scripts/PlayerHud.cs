using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHud : MonoBehaviour
{
    [Header("Count Down")]
    [SerializeField] private RectTransform m_countDownPanel;
    [SerializeField] private TextMeshProUGUI m_countDownText;
    [Header("GamePlaying")]
    [SerializeField] private TextMeshProUGUI m_playerNameText;
    [SerializeField] private TextMeshProUGUI m_scoreText;
    [Header("EndScreen")]
    [SerializeField] private RectTransform m_endScreenPanel;
    [SerializeField] private TextMeshProUGUI m_endScreenPName;
    [SerializeField] private TextMeshProUGUI m_endScreenScoreText;
    [SerializeField] private GameObject[] m_camView; //Views for single or multiplayer

    public void DeactivateHud()
    {
        m_playerNameText.text = string.Empty;
        m_scoreText.text = string.Empty;
        
        foreach (var camview in m_camView)
        {
            camview.SetActive(false);
        }
    }

    public void InitHud(string p_playerName,bool m_isSingle = true)
    {
        m_playerNameText.text = p_playerName;

        if (m_isSingle)
        {
            m_camView[0].SetActive(true);
            m_camView[1].SetActive(false);
            m_endScreenPanel.sizeDelta = new Vector2(1080, 1920);
            m_countDownPanel.sizeDelta = new Vector2(1080, 1920);
        }
        else
        {
            m_camView[0].SetActive(false);
            m_camView[1].SetActive(true);
            m_endScreenPanel.sizeDelta = new Vector2(1080, 960);
            m_countDownPanel.sizeDelta = new Vector2(1080, 960);
        }
        m_endScreenPanel.gameObject.SetActive(false);
        m_countDownPanel.gameObject.SetActive(true);
    }
    
    public void SetScore(int p_score)
    {
        m_scoreText.text = p_score.ToString();
        
    }
    
    public void SetCountDown(string m_countDownVal)
    {
        m_countDownText.text = m_countDownVal;
    }
    
    public void ShowFinalScore(string p_name, int p_finalScore)
    {
        m_endScreenPName.text = p_name.ToString();
        m_endScreenScoreText.text = p_finalScore.ToString();

    }
}
