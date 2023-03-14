using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentBehaviour : MonoBehaviour
{
    [Header("Segment Setting")]
    public bool m_spawnOnce = false;
    public float speedDelta = 0;
    [Header("Objs")]
    [SerializeField] SegmentManager m_spawnParent;
    [SerializeField] Vector3 m_respawnPoint;
    [Space]

    [Header("Block Spawn Settings")]
    [SerializeField] GameObject m_initialBlock;
    [SerializeField] Transform m_blockParent;
    [SerializeField] GameObject m_blockPrefab;
    [SerializeField] float m_blockZOffset = 0.3f;
    [SerializeField] int m_totalBlockCount = 17;

    [Header("Debugger")]
    [SerializeField] bool m_showEndSegment = false;
    [SerializeField] GameObject endOfSegmentIndicator;
    [SerializeField] float m_segmentPosLimit = -200;
    [Space]
    [SerializeField] Vector3 d_despawnPoint = new Vector3(14, 4, 10);
    [Header("Respawn Debug")]
    [SerializeField] float respawnPointDebugSize = 20;

    //MERVIN ADDED
    [SerializeField] bool m_isInitSegment = false;
    [SerializeField] GameObject m_startingLine;

    public Action onRespawn;

    #region LifeCycle

    private void Start()
    {
#if !UNITY_EDITOR
    m_showEndSegment = false;
#endif
        HideSegmenEndIndicator();
        SetRespawnPoint(new Vector3(transform.position.x, transform.position.y, m_spawnParent.GetLastSpawnPosition()));
        SpawnBlocks();
        // Debug.Log($"{gameObject.name} spawned at {transform.position}");
    }


    private void Update()
    {
        if (transform.position.z <= m_segmentPosLimit)
        {
            Respawn();

            if (m_isInitSegment && m_startingLine != null)
                m_startingLine.SetActive(false);
        }
    }
    #endregion

    void SpawnBlocks()
    {
        Vector3[] blockPositionArr = Block.GetBlockSpawnPoints(m_totalBlockCount, m_initialBlock.transform.localPosition, m_blockZOffset);
        for (int spawnIndex = 1; spawnIndex < blockPositionArr.Length; spawnIndex++)
        {
            var tempBlock = Instantiate(m_blockPrefab, m_blockParent);
            tempBlock.GetComponent<Block>().SetSegmentParent(this);
            tempBlock.transform.localPosition = blockPositionArr[spawnIndex];
        }
    }

    private void HideSegmenEndIndicator()
    {
        if (endOfSegmentIndicator != null)
        {
            endOfSegmentIndicator.SetActive(m_showEndSegment);
        }
    }
    #region Respawn Functions

    void Respawn()
    {
        if (m_spawnOnce)
            return;
        //Debug.Log($"{gameObject.name} despawned at {transform.position}");
        transform.position = m_respawnPoint;
        //Debug.Log($"{gameObject.name} respawned at {transform.position}");
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

    public void UpdateSpeed(float p_value)
    {
        speedDelta = p_value;
    }


    void FixedUpdate()
    {
        transform.Translate(new Vector3(0, 0, -speedDelta));
    }


    [ContextMenu("GetEndPositionFromSize")]
    public void GetEndPositionFromSize()
    {
        m_segmentPosLimit = transform.position.z - GetComponent<BoxCollider>().bounds.size.z;
    }
    #endregion

    #region Debugger

    private void OnDrawGizmosSelected()
    {
        //Despawn Viz
        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y, m_segmentPosLimit), d_despawnPoint);

        //Respawn Viz
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(m_respawnPoint, respawnPointDebugSize);
    }

    #endregion

    public float GetBlockOffset() => m_blockZOffset;
}