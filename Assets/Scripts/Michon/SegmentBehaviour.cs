using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentBehaviour : MonoBehaviour
{
    [Header("Objs")]
    [SerializeField] SegmentManager m_spawnParent;
    [SerializeField] Vector3 m_respawnPoint;
    
    [Header("Debugger")]
    [SerializeField] bool m_showEndSegment = false;
    [SerializeField] float m_segmentPosLimit = -200;
    [SerializeField] GameObject endOfSegmentIndicator;

    private void Start()
    {
#if !UNITY_EDITOR
    m_showEndSegment = false;
#endif
        endOfSegmentIndicator.SetActive(m_showEndSegment);
        m_respawnPoint = new Vector3(transform.position.x, transform.position.y, m_spawnParent.GetLastSpawnPosition());
    }

    private void LateUpdate()
    {
        if (transform.position.z <= m_segmentPosLimit)
        {
            Respawn();
        }
    }

    #region Respawn Functions

    void Respawn()
    {
        transform.position = m_respawnPoint;
    }

    public void SetRespawnPoint(Vector3 p_position)
    {
        m_respawnPoint = p_position;
    }

    public void SetSpawnParent(SegmentManager p_parent)
    {
        m_spawnParent = p_parent;
    }
    #endregion

    #region Debugger

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y, m_segmentPosLimit), 100f);
    }

    [ContextMenu("GetEndPositionFromSize")]
    public void GetEndPositionFromSize()
    {
        m_segmentPosLimit = transform.position.z - GetComponent<BoxCollider>().bounds.size.z;
    }
    #endregion

}