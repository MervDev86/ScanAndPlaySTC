using UnityEngine;
using UnityEngine.SceneManagement;
using System;

using NetworkClientHandler;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms.Impl;

public class Player : MonoBehaviour
{
    public PlayerStatus playerStat = PlayerStatus.Waiting;
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

    public void InitPlayer(bool p_isSingle)
    {
        m_isSingle = p_isSingle;

        score = 0;
        m_playerHUD.SetScore(score);
 
        m_positionIndex = 1;
        m_initPosition = transform.localPosition;

        m_movementXPositions = new[] {
            m_initPosition.x - GameManager.instance.GetMovementDistance,
            m_initPosition.x,
            m_initPosition.x + GameManager.instance.GetMovementDistance,
        };

        m_playerHUD.gameObject.SetActive(true);
        m_isActive = false;
        m_playerHUD.InitHud(p_isSingle);
        playerStat = PlayerStatus.Waiting;
    }

    IEnumerator StartGameSeq()
    {
        Debug.Log("StartGameSeq");
        m_playerHUD.SetIntroPanel("3");
        yield return new WaitForSeconds(1);
        m_playerHUD.SetIntroPanel("2");
        yield return new WaitForSeconds(1);
        m_playerHUD.SetIntroPanel("1");
        yield return new WaitForSeconds(1);
        m_playerHUD.SetIntroPanel("GO!",180);
        yield return new WaitForSeconds(1);
        m_playerHUD.SetIntroPanel("");
        m_isActive = true;
    }

    public void SetPlayerReady(string p_name)
    {
        m_playerHUD.SetIntroPanel("Ready!",120);
        playerName = p_name;
        playerStat = PlayerStatus.Ready;
        m_playerHUD.SetName(p_name);
    }

    public void StartGame()
    {
        playerStat = PlayerStatus.Playing;
        StartCoroutine(StartGameSeq());
    }

    public void GameOver()
    {
        m_isActive = false;
        playerStat = PlayerStatus.Gameover;
        m_playerHUD.ShowFinalScore(playerName,score);
    }

    #region Control

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
    #endregion Control
    
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

public enum PlayerStatus
{
    Idle,
    Waiting,
    Ready,
    Playing,
    Gameover,
}