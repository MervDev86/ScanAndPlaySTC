using UnityEngine;
using UnityEngine.SceneManagement;
using System;

using NetworkClientHandler;
using System.Collections;

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

    #region Lifecycles

    private void Start()
    {
#if !UNITY_EDITOR
    enableCollider = true;
#endif
        m_capCollider.enabled = enableCollider;
        positionIndex = 1;

        m_offset = GameManager.instance.GetMovementDistance;
        initPosition = transform.position;

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
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, targetPosition.x, Time.fixedDeltaTime * m_movementMultiplier), transform.position.y, transform.position.z);
    }

    private void Update()
    {
        if (!alive)
            return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveLeft();

            //positionIndex--;
            //if (positionIndex <= 0)
            //    positionIndex = 0;
            ////transform.position = new Vector3(Mathf.Clamp(transform.position.x - 2, m_minX, m_maxX), transform.position.y, transform.position.z);
            ////targetPosition = new Vector3(movementXPositions[positionIndex].x, movementXPositions[positionIndex].y, transform.position.z);
            //targetPosition.x = movementXPositions[positionIndex];
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            MoveRight();

            //positionIndex++;
            //if (positionIndex >= movementXPositions.Length)
            //    positionIndex = movementXPositions.Length - 1;
            //targetPosition.x = movementXPositions[positionIndex];
            ////targetPosition = new Vector3(movementXPositions[positionIndex].x, movementXPositions[positionIndex].y, transform.position.z);
            ////transform.position = new Vector3(Mathf.Clamp(transform.position.x + 2, m_minX, m_maxX), transform.position.y, transform.position.z);
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
        //transform.position = new Vector3(Mathf.Clamp(transform.position.x - 2, m_minX, m_maxX), transform.position.y, transform.position.z);
        //targetPosition = new Vector3(movementXPositions[positionIndex].x, movementXPositions[positionIndex].y, transform.position.z);
        targetPosition.x = movementXPositions[positionIndex];
    }

    private void MoveRight()
    {
        positionIndex++;
        if (positionIndex >= movementXPositions.Length)
            positionIndex = movementXPositions.Length - 1;

        targetPosition.x = movementXPositions[positionIndex];
        //targetPosition = new Vector3(movementXPositions[positionIndex].x, movementXPositions[positionIndex].y, transform.position.z);
        //transform.position = new Vector3(Mathf.Clamp(transform.position.x + 2, m_minX, m_maxX), transform.position.y, transform.position.z);
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

            UIManager.Instance.UpdateLife();
        }
        else { return; }

       
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