using UnityEngine;
using UnityEngine.SceneManagement;
using System;

using NetworkClientHandler;
using System.Collections;
using Unity.VisualScripting;

public class Player : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] CapsuleCollider m_capCollider;

    [Header("Player")]
    [SerializeField] int playerIndex = 1;
    [SerializeField] int m_life = 3;
    bool alive = true;
    [SerializeField] bool hittable = true;

    [Header("Player Speed")]
    //Michon Changes
    [Header("Movement Positions")]
    [SerializeField] int positionIndex = 0;

    [SerializeField] float m_movementMultiplier = 1;
    [SerializeField] float m_minX = -3;
    [SerializeField] float m_maxX = 3;
    [Space]
    [SerializeField] Vector3 initPosition;
    [SerializeField] Vector3 targetPosition;
    [SerializeField] float[] movementXPositions;

    [SerializeField] float m_offset;
    [SerializeField] float hitRestartDelay = 0.5f;
    [Header("Debug ")]
    public bool enableCollider = false;
    //EVENTS

    public Action<int> onPlayerDeath;
    public Action<int> onPlayerHit;

    public static Action onPlayerMoveRight;
    public static Action onPlayerMoveLeft;
    
    [SerializeField] private KeyCode m_moveLeftKey;
    [SerializeField] private KeyCode m_moveRightKey;

    #region Lifecycles

    private void Start()
    {
#if !UNITY_EDITOR
    enableCollider = true;
#endif
        m_capCollider.enabled = enableCollider;
        positionIndex = 1;

        m_offset = GameManager.instance.GetMovementDistance;
        initPosition = transform.localPosition;

        movementXPositions = new[] {
             initPosition.x - GameManager.instance.GetMovementDistance,
            (float)initPosition.x,
            initPosition.x + GameManager.instance.GetMovementDistance,
            };

        SessionsHandler.OnMovePlayer1 += MoveToPositionIndex;
    }

    private void FixedUpdate()
    {
        //if (!alive) return;
        transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, targetPosition.x, Time.fixedDeltaTime * m_movementMultiplier), transform.localPosition.y, transform.localPosition.z);
    }

    private void Update()
    {
        if (!alive)
            return;

        if (Input.GetKeyDown(m_moveLeftKey))
        {
            MoveLeft();
            Debug.Log(this.name + " : left");
        }

        if (Input.GetKeyDown(m_moveRightKey))
        {
            MoveRight();
            Debug.Log(this.name + " : right");
        }

        if (transform.position.y < -5)
        {
            Die();
        }
    }

    private void MoveLeft()
    {
        positionIndex--;
        if (positionIndex <= 0)
            positionIndex = 0;
        targetPosition.x = movementXPositions[positionIndex];
    }

    private void MoveRight()
    {
        positionIndex++;
        if (positionIndex >= movementXPositions.Length)
            positionIndex = movementXPositions.Length - 1;

        targetPosition.x = movementXPositions[positionIndex];
    }

    private void MoveToPositionIndex(int p_index)
    {
        targetPosition.x = movementXPositions[p_index];
    }
    #endregion

    #region Action
    public void Hit()
    {
       
        if (!alive || !enableCollider)
            return;

        if (hittable)
        {
            StartCoroutine(HitRestartDelay());
            m_life--;
            if (m_life <= 0)
            {
                Die();
            }
            Debug.Log("Life: " + m_life);

            // UIManager.Instance.UpdateLife();
        }
   
    }

    private IEnumerator HitRestartDelay()
    {
        hittable = false;
        yield return new WaitForSeconds(hitRestartDelay);
        hittable = true;
    }
    public void Die()
    {
        alive = false;
        // Restart the game
        Debug.Log("Player Died!");
        GameManager.instance.ChangeGameState(GameState.GAME_END);
        //Invoke("Restart", 2);
    }
   
    #endregion

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        SessionsHandler.OnMovePlayer1 -= MoveToPositionIndex;
    }
}