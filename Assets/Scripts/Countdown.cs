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
    // Start is called before the first frame update
    void Start()
    {
        m_currentTimeCountDown = m_startTime;
    }

    // Update is called once per frame
    void Update()
    {
        m_currentTimeCountDown -= Time.fixedDeltaTime;
        if (m_currentTimeCountDown <=0)
        {
            m_currentTimeCountDown = 0;
            Debug.Log("Start");
        }
        m_countdownText.text = m_currentTimeCountDown.ToString("##");
    }
}
