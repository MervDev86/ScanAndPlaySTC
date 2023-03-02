using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentBehaviour : MonoBehaviour
{
    [Header("Objs")]
    [SerializeField] SegmentManager m_spawnParent;
    [SerializeField] Vector3 m_respawnPoint;
    [Header("Block Settings")]
    [SerializeField] GameObject m_initialBlock;
    [SerializeField] Transform m_blockParent;
    [SerializeField] GameObject m_blockPrefab;
    [SerializeField] int m_totalBlockCount = 17;

    [Header("Debugger")]
    [SerializeField] bool m_showEndSegment = false;
    [SerializeField] float m_segmentPosLimit = -200;
    [SerializeField] GameObject endOfSegmentIndicator;
    [SerializeField] Vector3 d_despawnPoint = new Vector3(14, 4, 10);

    public Action onRespawn;

    #region LifeCycle

    private void Start()
    {
#if !UNITY_EDITOR
    m_showEndSegment = false;
#endif
        endOfSegmentIndicator.SetActive(m_showEndSegment);
        SetRespawnPoint(new Vector3(transform.position.x, transform.position.y, m_spawnParent.GetLastSpawnPosition()));
        SpawnBlocks();

        Debug.Log($"{gameObject.name} spawned at {transform.position}");
    }

    private void LateUpdate()
    {
        if (transform.position.z <= m_segmentPosLimit)
        {
            Respawn();
        }
    }
    #endregion

    void SpawnBlocks()
    {
        Vector3[] blockPositionArr = Block.GetBlockSpawnPoints(m_totalBlockCount, m_initialBlock.transform.position);
        for (int spawnIndex = 1; spawnIndex < blockPositionArr.Length; spawnIndex++)
        {
            var tempBlock = Instantiate(m_blockPrefab, m_blockParent);
            tempBlock.GetComponent<Block>().SetSegmentParent(this);
            tempBlock.transform.position = blockPositionArr[spawnIndex];
        }
    }

    #region Respawn Functions

    void Respawn()
    {
        Debug.Log($"{gameObject.name} spawned at {transform.position}");
        transform.position = m_respawnPoint;
        onRespawn?.Invoke();
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
        //Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y, m_segmentPosLimit), 100f);
        Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y, m_segmentPosLimit), d_despawnPoint);
        Gizmos.color = Color.green;

        Gizmos.DrawSphere(m_respawnPoint, 40f);

    }

    [ContextMenu("GetEndPositionFromSize")]
    public void GetEndPositionFromSize()
    {
        m_segmentPosLimit = transform.position.z - GetComponent<BoxCollider>().bounds.size.z;
    }
    #endregion

}