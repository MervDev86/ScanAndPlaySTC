using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class Player : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] CapsuleCollider m_capCollider;

    [Header("Player")]
    [SerializeField] int playerIndex = 1;
    [SerializeField] int m_life = 3;
    bool alive = true;

    [Header("Player Speed")]
    //public float speed = 5;
    //public float speedIncreasePerPoint = 0.1f;
    //[SerializeField] float horizontalMultiplier = 2;

    //Michon Changes
    [Header("Movement Positions")]
    [SerializeField] int positionIndex = 0;
    //float horizontalInput;

    [SerializeField] float m_movementMultiplier = 1;
    [SerializeField] float m_minX = -3;
    [SerializeField] float m_maxX = 3;
    [Space]
    [SerializeField] Vector3 initPosition;
    [SerializeField] Vector3 targetPosition;
    [SerializeField] float[] movementXPositions;

    [SerializeField] float m_offset;

    [Header("Debug ")]
    public bool enableCollider = false;
    //EVENTS

    public Action<int> onPlayerDeath;
    public Action<int> onPlayerHit;

    #region Lifecycles

    private void Start()
    {
        //m_capCollider = GetComponent<CapsuleCollider>();

#if !UNITY_EDITOR
    enableCollider = true;
#endif
        m_capCollider.enabled = enableCollider;
        positionIndex = 1;

        m_offset = GameManager.instance.GetMovementDistance;
        initPosition = transform.position;

        //movementXPositions = new[] {
        //    new Vector2(initPosition.x - GameManager.instance.GetMovementDistance, initPosition.y),
        //    (Vector2)initPosition,
        //    new Vector2(initPosition.x + GameManager.instance.GetMovementDistance, initPosition.y),
        //    };
        movementXPositions = new[] {
             initPosition.x - GameManager.instance.GetMovementDistance,
            (float)initPosition.x,
            initPosition.x + GameManager.instance.GetMovementDistance,
            };
    }

    private void FixedUpdate()
    {
        //if (!alive) return;

        //Vector3 forwardMove = transform.forward * speed * Time.fixedDeltaTime;
        //Vector3 horizontalMove = transform.right * horizontalInput * speed * Time.fixedDeltaTime * horizontalMultiplier;
        //rb.MovePosition(rb.position + forwardMove + horizontalMove);

        //transform.position = Vector3.Lerp(transform.position, new Vector3(targetPosition.x, transform.position.y, transform.position.z), Time.fixedDeltaTime * m_movementMultiplier);
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, targetPosition.x, Time.fixedDeltaTime * m_movementMultiplier), transform.position.y, transform.position.z);
    }

    private void Update()
    {
        //horizontalInput = Input.GetAxis("Horizontal");
        if (!alive)
            return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            positionIndex--;
            if (positionIndex <= 0)
                positionIndex = 0;
            //transform.position = new Vector3(Mathf.Clamp(transform.position.x - 2, m_minX, m_maxX), transform.position.y, transform.position.z);
            //targetPosition = new Vector3(movementXPositions[positionIndex].x, movementXPositions[positionIndex].y, transform.position.z);
            targetPosition.x = movementXPositions[positionIndex];

        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            positionIndex++;
            if (positionIndex >= movementXPositions.Length)
                positionIndex = movementXPositions.Length - 1;

            targetPosition.x = movementXPositions[positionIndex];
            //targetPosition = new Vector3(movementXPositions[positionIndex].x, movementXPositions[positionIndex].y, transform.position.z);
            //transform.position = new Vector3(Mathf.Clamp(transform.position.x + 2, m_minX, m_maxX), transform.position.y, transform.position.z);
        }

        if (transform.position.y < -5)
        {
            Die();
        }
    }
    #endregion

    #region Action
    public void Hit()
    {
        if (!alive || !enableCollider)
            return;

        m_life--;
        if (m_life <= 0)
        {
            Die();
        }
        Debug.Log("Life: " + m_life);
    }

    public void Die()
    {
        alive = false;
        // Restart the game
        Debug.Log("Player Died!");

        //Invoke("Restart", 2);
    }

    #endregion

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}