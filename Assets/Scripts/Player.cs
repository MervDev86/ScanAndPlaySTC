using UnityEngine;
using UnityEngine.SceneManagement;
using System;

using NetworkClientHandler;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms.Impl;

public class Player : MonoBehaviour
{
    public int playerID = 1;
    public int score = 0;
    public string playerName = "PLAYER";
    [Header("HUD")]
    [SerializeField] PlayerHud m_playerHUD;
    
    [Header("Movement")]
    [SerializeField] int m_positionIndex = 0;
    [SerializeField] float m_movementSpeed = 1;

    [SerializeField] private KeyCode m_moveLeftKey;
    [SerializeField] private KeyCode m_moveRightKey;
    
    Vector3 m_initPosition;
    Vector3 m_targetPosition;
    float[] m_movementXPositions;
    
    int m_life = 3;
    bool m_isActive = false;
    private bool m_isSingle = true;

    #region Lifecycles

    public void InitPlayer(bool p_isSingle)
    {
        score = 0;
        m_isSingle = p_isSingle;
        m_positionIndex = 1;
        m_initPosition = transform.localPosition;

        m_movementXPositions = new[] {
            m_initPosition.x - GameManager.instance.GetMovementDistance,
            (float)m_initPosition.x,
            m_initPosition.x + GameManager.instance.GetMovementDistance,
        };
        m_playerHUD.gameObject.SetActive(true);
        m_isActive = false;
        StopAllCoroutines();
        StartCoroutine(StartGameSeq());
    }

    IEnumerator StartGameSeq()
    {
        m_playerHUD.SetCountDown("3");
        yield return new WaitForSeconds(1);
        m_playerHUD.SetCountDown("2");
        yield return new WaitForSeconds(1);
        m_playerHUD.SetCountDown("1");
        yield return new WaitForSeconds(1);
        m_playerHUD.SetCountDown("GO!");
        yield return new WaitForSeconds(1);
        m_playerHUD.SetCountDown("");
        m_isActive = true;
    }

    public void SetPlayerName(string p_name)
    {
        m_playerHUD.InitHud(p_name,m_isSingle);
    }

    private void FixedUpdate()
    {
        if (!m_isActive) return;
            transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, m_targetPosition.x, Time.fixedDeltaTime * m_movementSpeed), transform.localPosition.y, transform.localPosition.z);
    }

    private void Update()
    {
        if (!m_isActive)
            return;

        if (Input.GetKeyDown(m_moveLeftKey))
        {
            MoveLeft();
        }

        if (Input.GetKeyDown(m_moveRightKey))
        {
            MoveRight();
        }

        if (transform.position.y < -5)
        {
            Die();
        }
    }

    private void MoveLeft()
    {
        m_positionIndex--;
        if (m_positionIndex <= 0)
            m_positionIndex = 0;
        m_targetPosition.x = m_movementXPositions[m_positionIndex];
    }

    private void MoveRight()
    {
        m_positionIndex++;
        if (m_positionIndex >= m_movementXPositions.Length)
            m_positionIndex = m_movementXPositions.Length - 1;

        m_targetPosition.x = m_movementXPositions[m_positionIndex];
    }

    public void MoveToPositionIndex(int p_index)
    {
        if(m_isActive)
            m_targetPosition.x = m_movementXPositions[p_index];
    }
    #endregion
    
    public void Hit()
    {
        if (m_isActive)
        {
            m_life--;
            if (m_life <= 0)
                Die();
            Debug.Log("Life: " + m_life);
        }
    }

    public void AddScore(int p_score)
    {
        score += p_score;
        m_playerHUD.SetScore(score);
    }
    
    public void ResetPlayer()
    {
        m_isActive = false;
        m_playerHUD.DeactivateHud();
        m_playerHUD.gameObject.SetActive(false);
    }

    public void Die()
    {
        m_isActive = false;
        Debug.Log("Player Died!");
    }
}