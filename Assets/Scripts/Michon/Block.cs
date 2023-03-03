using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] BoxCollider m_collider;
    [SerializeField] SegmentBehaviour m_segmentBehaviour;

    [Header("Spawner")]
    [SerializeField] GameObject m_coinPrefab;
    [SerializeField] GameObject m_ObstaclePrefab;
    [SerializeField] bool m_spawnObstacle = false;
    [Space]
    [Range(0, 1)]
    [SerializeField] float m_spawnChance = 1;
    [Header("Grid")]
    //xPoints is supposed to be based off the movement points of the player  | l | m = 0 | r |   ++ current: -2 0 2
    [SerializeField] float[] xPoints = new float[3];
    [SerializeField] int m_gridColumn = 3; //DEFAULT = 3  => FOR NOW KEEP THE SAME VAL UNLESS CHANGED IN THE FUTURE
    [Range(0, 100)]
    [SerializeField] int m_gridRow;
    [Space]
    [SerializeField] float zOffset;
    [SerializeField] float m_itemHeight = 1;
    Vector3[,] spawnPoints;
    [Space]

    [Header("Item Spawn Visualizer")]
    [SerializeField] float d_sphereSize = 0.3f;
    [SerializeField] Color d_color = Color.white;
    [Space]

    [Header("Block Visualizer")]
    [SerializeField] bool ShowDebugTools = true;
    [SerializeField] bool showNextBlockPreview = true;
    [SerializeField] int previewLimit = 2;
    [SerializeField] Color previewColor = Color.white;

    public float maxXBounds;
    public float maxYBounds;
    public static float m_boundaryLength = 0;

    #region LifeCycle
    private void OnValidate()
    {
        if (m_collider == null)
            m_collider = GetComponent<BoxCollider>();

        InitGridLayout(); // To keep track of the spawn Positions
    }

    private void Start()
    {
        m_collider = this.gameObject.GetComponent<BoxCollider>();
        m_segmentBehaviour.onRespawn += OnSegmentRespawn;

        InitGridLayout();
        DestroySpawnedItems();
        SpawnItems();
    }
    #endregion

    private void OnSegmentRespawn()
    {
        DestroySpawnedItems();
        SpawnItems();
    }

    [ExecuteInEditMode]
    private void Update()
    {
        InitGridLayout();
    }

    void InitGridLayout()
    {
        spawnPoints = new Vector3[m_gridColumn, m_gridRow];

        if (m_boundaryLength <= 0)
        {
            m_boundaryLength = m_collider.size.z;
            Debug.Log($"zSize has been set to {m_boundaryLength}");
        }

        m_boundaryLength = m_collider.bounds.size.z;
        var spawnPointDelta = m_boundaryLength / m_gridRow;

        //Debug.LogWarning($"{gameObject.transform.parent.transform.parent}  using Max Z Bounds: {m_boundaryLength} \n" +
        //    $"calculated spawn distance = {spawnPointDelta}");

        for (int columnIndex = 0; columnIndex < m_gridColumn; columnIndex++)
        {
            for (int rowIndex = 0; rowIndex < m_gridRow; rowIndex++)
            {
                var yVal = (rowIndex * spawnPointDelta) + zOffset;
                try
                {
                    spawnPoints[columnIndex, rowIndex] = new Vector3(xPoints[columnIndex], m_itemHeight, yVal);
                }
                catch (System.Exception e)
                {

                }
            }
        }
    }

    void GetXpoints()
    {
        var playerMovementPointDelta = GameManager.instance.GetMovementDistance;
        for (int xPointIndex = 0; xPointIndex < m_gridRow; xPointIndex++)
        {
            xPoints[xPointIndex] = xPointIndex * playerMovementPointDelta;
        }
    }

    [ContextMenu("Spawnables/Spawn Collections")]
    void SpawnItems()
    {
        bool obstacleSpawned = false;
        for (int columnIndex = 0; columnIndex < m_gridColumn; columnIndex++)
        {
            bool spawnOnThisColumn = Random.Range(0, 2) >= m_spawnChance;
            if (m_spawnObstacle && Random.Range(0, 2) >= 1 && !obstacleSpawned)
            {
                var obj = Instantiate(m_ObstaclePrefab, transform);
                obj.transform.localPosition = spawnPoints[columnIndex, 0];
                obstacleSpawned = true;
                continue;
            }

            for (int rowIndex = 0; rowIndex < m_gridRow; rowIndex++)
            {
                if (spawnOnThisColumn)
                {
                    var obj = Instantiate(m_coinPrefab, transform);
                    obj.transform.localPosition = spawnPoints[columnIndex, rowIndex];
                }
            }
        }
    }


    //void SpawnItems()//Spawn on All Points
    //{
    //    foreach (var spawnPosition in spawnPoints)
    //    {
    //        var obj = Instantiate(m_coinPrefab, transform);
    //        obj.transform.localPosition = spawnPosition;
    //    }
    //}

    #region Debugger

    private void OnDrawGizmos()
    {
        if (!ShowDebugTools)
            return;
        ShowSpawnPointVisualizer();
        ShowNextBlockVisualizer();
    }

    void ShowSpawnPointVisualizer()
    {
        Gizmos.color = d_color;
        foreach (var item in spawnPoints)
        {
            Gizmos.DrawSphere(transform.position + item, d_sphereSize);
        }
    }

    void ShowNextBlockVisualizer()
    {
        if (showNextBlockPreview)
        {
            Gizmos.color = previewColor;
            for (int i = 0; i < previewLimit; i++)
            {
                //Gizmos.DrawCube(transform.position , m_collider.size);
                Gizmos.DrawCube(transform.position + new Vector3(transform.position.x, transform.position.y, m_collider.size.z * i), m_collider.size);
            }
        }
    }

    [ContextMenu("Debug/Print Spawn Point Values")]
    public void PrintSpawnValues()
    {
        string info = "";
        for (int xIndex = 0; xIndex < m_gridRow; xIndex++)
        {
            for (int yIndex = 0; yIndex < m_gridColumn; yIndex++)
            {

                info += $" {spawnPoints[xIndex, yIndex]}";
            }
            info += "\n";
        }
        Debug.Log(info);
    }

    #endregion

    public static Vector3[] GetBlockSpawnPoints(int p_count, Vector3 p_initialBlockPosition)
    {
        Vector3[] spawnPoints = new Vector3[p_count];
        for (int spawnPointIndex = 0; spawnPointIndex < spawnPoints.Length; spawnPointIndex++)
        {
            spawnPoints[spawnPointIndex] = p_initialBlockPosition + new Vector3(p_initialBlockPosition.x, p_initialBlockPosition.y, m_boundaryLength * spawnPointIndex);
        }

        return spawnPoints;
    }

    [ContextMenu("Block/Check Collider Length")]
    public void GetColliderLength()
    {
        m_boundaryLength = GetComponent<BoxCollider>().bounds.size.z;
    }

    [ContextMenu("Kill Children")]
    public void DestroySpawnedItems()
    {
        if (transform.childCount == 0)
            return;
        for (int childIndex = 0; childIndex < transform.childCount; childIndex++)
        {
            Destroy(transform.GetChild(childIndex).gameObject);
        }
    }

    public void SetSegmentParent(SegmentBehaviour p_segmentBehaviour)
    {
        m_segmentBehaviour = p_segmentBehaviour;
    }

}