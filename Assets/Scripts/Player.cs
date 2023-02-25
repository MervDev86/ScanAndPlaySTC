using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class Player : MonoBehaviour
{
    [SerializeField] int playerIndex = 1;

    [SerializeField] float m_minX = -3;
    [SerializeField] float m_maxX = 3;
    [SerializeField] int m_life = 3;
    bool alive = true;

    public float speed = 5;
    [SerializeField] Rigidbody rb;

    float horizontalInput;
    [SerializeField] float horizontalMultiplier = 2;

    public float speedIncreasePerPoint = 0.1f;

    //Michon Changes
    [Header("Movement Positions")]
    int positionIndex = 0;

    [SerializeField] Vector3 initPosition;
    [SerializeField] Vector3[] movementPositions;

    [SerializeField] float m_offset;
    [SerializeField] Vector3 targetPosition;


    //EVENTS

    public Action<int> onPlayerDeath;
    public Action<int> onPlayerHit;

    private void Start()
    {
        positionIndex = 1;

        m_offset = GameManager.instance.GetMovementDistance;
        initPosition = transform.position;

        movementPositions = new[] {
            new Vector3(initPosition.x - GameManager.instance.GetMovementDistance, initPosition.y),
            initPosition,
            new Vector3(initPosition.x + GameManager.instance.GetMovementDistance, initPosition.y),
            };

    }

    private void FixedUpdate()
    {
        if (!alive) return;

        Vector3 forwardMove = transform.forward * speed * Time.fixedDeltaTime;
        Vector3 horizontalMove = transform.right * horizontalInput * speed * Time.fixedDeltaTime * horizontalMultiplier;
        rb.MovePosition(rb.position + forwardMove + horizontalMove);
    }

    private void Update()
    {
        //horizontalInput = Input.GetAxis("Horizontal");
        if (!alive)
            return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            //transform.position = new Vector3(Mathf.Clamp(transform.position.x - 2, m_minX, m_maxX), transform.position.y, transform.position.z);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            //transform.position = new Vector3(Mathf.Clamp(transform.position.x + 2, m_minX, m_maxX), transform.position.y, transform.position.z);
        }


        if (transform.position.y < -5)
        {
            Die();
        }
    }

    public void Hit()
    {
        if (!alive)
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

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}