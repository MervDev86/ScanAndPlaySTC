using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentManager : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float m_envMovementSpeed = 0.5f; [Header("Spawn Values")]

    [Header("Spawn Values")]
    [Tooltip("If theres already a preset spawn enabled")]
    [SerializeField] GameObject m_initSpawn;
    [SerializeField] int m_segmentSpawnCount = 15;
    [Space]
    [Tooltip("Gameobject that handled the Spawning of this Object")]
    [SerializeField] GameObject m_segmentParent;
    [SerializeField] bool m_useInitSpawn = false;
    [SerializeField] GameObject m_initSegmentPrefab;
    [SerializeField] GameObject m_segmentPrefab;
    [SerializeField] Vector3 m_nextSpawnPoint;
    [Space]
    [Tooltip("Found in Prefab as 'Next Point'. Sets where the next spawn object will be.")]
    [SerializeField] float m_spawnPositionDiff;

    [Header("Debugger")]
    [SerializeField] float m_segmentSpawnPointSphereSize = 15f;
    [SerializeField] Vector3 m_segmentSpawnPoint;
    [SerializeField] Color m_segmentSpawnPointColor;

    public Action<float> OnEnvValueChanged;
    bool environtmentInitialized = false;

    public float EnvMovementSpeed
    {
        get { return m_envMovementSpeed; }
        set
        {
            if (value != m_envMovementSpeed)
            {
                m_envMovementSpeed = value;
                OnEnvValueChanged?.Invoke(value);
            }
        }
    }

    private void OnValidate()
    {
        if (m_segmentPrefab != null)
        {
            m_spawnPositionDiff = m_segmentPrefab.transform.GetChild(1).transform.position.z;
        }

        try
        {
            m_nextSpawnPoint.x = transform.parent.position.x;//ensures the location of the parent is inline with the spawn
        }
        catch (Exception)
        {
            //Debug.LogError($"{gameObject.name} error cant find Parent");
        }
    }

    private void Start()
    {
        environtmentInitialized = false;
    }

    public void InitEnvironment()
    {
        if (environtmentInitialized)
            return;

        m_envMovementSpeed = 0;
        for (int spawnIndex = 0; spawnIndex <= m_segmentSpawnCount; spawnIndex++)
        {
            if (m_initSpawn != null && spawnIndex < 1)
            {
                m_initSpawn.SetActive(false);
                SpawnSegment(m_useInitSpawn, spawnIndex, spawnIndex == m_segmentSpawnCount);
            }
            SpawnSegment(spawnIndex, spawnIndex == m_segmentSpawnCount);
        }
        environtmentInitialized = true;
    }

    public void SpawnSegment(int? p_index = null, bool p_isLast = false)
    {
        GameObject temp = Instantiate(m_segmentPrefab, m_nextSpawnPoint, Quaternion.identity);
        if (p_isLast)
        {
            m_segmentSpawnPoint = temp.transform.position;
        }

        SegmentBehaviour sb = temp.GetComponent<SegmentBehaviour>();
        sb.SetSpawnParent(this);
        OnEnvValueChanged += sb.UpdateSpeed;
        temp.transform.parent = m_segmentParent.transform;
        temp.name = p_index != null ? $"Segment {p_index} " : "Segment";
        m_nextSpawnPoint = temp.transform.GetChild(1).transform.position;
    }

    public void SpawnSegment(bool p_useInitModel, int? p_index = null, bool p_isLast = false)
    {
        GameObject temp;
        if (!p_useInitModel)
            temp = Instantiate(m_segmentPrefab, m_nextSpawnPoint, Quaternion.identity);
        else
        {
            temp = m_initSpawn;
            m_initSpawn.SetActive(true);
        }

        if (p_isLast)
        {
            m_segmentSpawnPoint = temp.transform.position;
        }

        SegmentBehaviour sb = temp.GetComponent<SegmentBehaviour>();
        sb.SetSpawnParent(this);
        OnEnvValueChanged += sb.UpdateSpeed;
        temp.transform.parent = m_segmentParent.transform;
        temp.name = p_index != null ? $"Segment {p_index} " : "Segment";
        m_nextSpawnPoint = temp.transform.GetChild(1).transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = m_segmentSpawnPointColor;
        m_segmentSpawnPoint = new Vector3(m_nextSpawnPoint.x, m_nextSpawnPoint.y, GetLastSpawnPosition());
        Gizmos.DrawSphere(m_segmentSpawnPoint, m_segmentSpawnPointSphereSize);
    }

    public float GetLastSpawnPosition() => (m_segmentSpawnCount) * m_spawnPositionDiff; //Returns z position of the last Segment
    public int GetTotalSpawnCount() => m_segmentSpawnCount;
    public float GetSpawnPositionDifference() => m_spawnPositionDiff;

}