using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    public Action<int> OnTriggerPoints;
    [Header("Movement")]
    [SerializeField] int m_positionIndex = 0;
    [SerializeField] float m_movementSpeed = 1;

    [SerializeField] private KeyCode m_moveLeftKey;
    [SerializeField] private KeyCode m_moveRightKey;
    
    Vector3 m_initPosition;
    Vector3 m_targetPosition;
    float[] m_movementXPositions;
    
    bool m_isActive = false;
    
    void Update()
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
    }
    
    private void FixedUpdate()
    {
        if (!m_isActive) return;
        transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, m_targetPosition.x, Time.fixedDeltaTime * m_movementSpeed), transform.localPosition.y, transform.localPosition.z);
    }
    
    public void ActivateController(bool p_isActive)
    {
        m_isActive = p_isActive;
        if (m_isActive)
        {
            m_positionIndex = 1;
            m_initPosition = transform.localPosition;

            m_movementXPositions = new[] {
                m_initPosition.x - GameManager.instance.GetMovementDistance,
                m_initPosition.x,
                m_initPosition.x + GameManager.instance.GetMovementDistance,
            };
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

    void OnTriggerEnter(Collider other)
    {
        if (!m_isActive) return;

        if (other.tag == "Coins")
        {
            OnTriggerPoints.Invoke(other.GetComponent<Coin>().points);
        }
    }
}
