using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHud : MonoBehaviour
{
    [Header("Intro Panel")]
    [SerializeField] private RectTransform m_introPanel;
    [SerializeField] private TextMeshProUGUI m_introText;
    [Header("GamePlaying")]
    [SerializeField] private TextMeshProUGUI m_playerNameText;
    [SerializeField] private TextMeshProUGUI m_scoreText;
    [Header("EndScreen")]
    [SerializeField] private RectTransform m_endScreenPanel;
    [SerializeField] private TextMeshProUGUI m_endScreenPName;
    [SerializeField] private TextMeshProUGUI m_endScreenScoreText;
    [SerializeField] private GameObject[] m_camView; //Views for single or multiplayer

    public void InitHud(bool p_isSingle)
    {
        if (p_isSingle)
        {
            m_camView[0].SetActive(true);
            m_camView[1].SetActive(false);
            m_endScreenPanel.sizeDelta = new Vector2(1080, 1920);
            m_introPanel.sizeDelta = new Vector2(1080, 1920);
        }
        else
        {
            m_camView[0].SetActive(false);
            m_camView[1].SetActive(true);
            m_endScreenPanel.sizeDelta = new Vector2(1080, 960);
            m_introPanel.sizeDelta = new Vector2(1080, 960);
        }
        m_endScreenPanel.gameObject.SetActive(false);
        m_introPanel.gameObject.SetActive(true);
        m_introText.fontSize = 120;
        m_introText.text = "Waiting..";
    }

    public void DeactivateHud()
    {
        m_playerNameText.text = string.Empty;
        m_scoreText.text = string.Empty;
        
        foreach (var camview in m_camView)
        {
            camview.SetActive(false);
        }
    }

    public void SetName(string p_playerName)
    {
        m_playerNameText.text = p_playerName;

    }
    
    public void SetScore(int p_score)
    {
        m_scoreText.text = p_score.ToString();
        
    }
    
    public void SetIntroPanel(string p_text,float p_fontSize = 320)
    {
        m_introText.fontSize = p_fontSize;
        m_introText.text = p_text;
    }
    
    public void ShowFinalScore(string p_name, int p_finalScore)
    {
        m_endScreenPanel.gameObject.SetActive(true);
        m_endScreenPName.text = p_name;
        m_endScreenScoreText.text = p_finalScore.ToString();

    }
}
