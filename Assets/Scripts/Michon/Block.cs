using System.Collections;
using System.Collections.Generic;
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
    [Tooltip("Chance Percentage To Spawn Item per Column (1 = 100% chance)")]
    [SerializeField] float m_spawnChance = 1;

    [Header("Item Grid")]
    //xPoints is based off the movement points of the player  | l | m = 0 | r |   ++ current: -2 0 2
    [SerializeField] float[] xPoints = new float[3];
    [Range(0, 100)]
    [SerializeField] int m_gridRow;
    int m_gridColumn = 3; //DEFAULT = 3  => FOR NOW KEEP THE SAME VAL UNLESS CHANGED IN THE FUTURE
    [Space]
    [SerializeField] float m_itemHeight = 1;
    [Tooltip("Coin Spawn Offset in Z Axis")]
    [SerializeField] float m_itemSpawnzOffset;
    [Tooltip("Maximum Columns that spawn coins. Default : 3")]
    int m_coinSpawnLimit = 1;
    Vector3[,] spawnPoints;
    [Space]

    #region Debug Properties
    [Header("Item Spawn Visualizer")]
    [SerializeField] float d_sphereSize = 0.3f;
    [SerializeField] Color d_color = Color.white;
    [Space]

    [Header("Block Visualizer")]

    [SerializeField] Vector3 blockSpawnOffset;
    [SerializeField] bool showDebugTools = true;
    [SerializeField] bool showNextBlockPreview = true;
    [SerializeField] int previewLimit = 2;
    [SerializeField] Color previewColor = Color.white;

    public float maxXBounds;
    public float maxYBounds;
    public static float m_boundaryLength = 0;
    #endregion



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
        SpawnItems(true);
        Debug.Log($"{gameObject.name} spawned at index {transform.GetSiblingIndex()}");
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
            // Debug.Log($"zSize has been set to {m_boundaryLength}");
        }

        m_boundaryLength = m_collider.bounds.size.z;
        var spawnPointDelta = m_boundaryLength / m_gridRow;

        //Debug.LogWarning($"{gameObject.transform.parent.transform.parent}  using Max Z Bounds: {m_boundaryLength} \n" +
        //    $"calculated spawn distance = {spawnPointDelta}");

        for (int columnIndex = 0; columnIndex < m_gridColumn; columnIndex++)
        {
            for (int rowIndex = 0; rowIndex < m_gridRow; rowIndex++)
            {
                var yVal = (rowIndex * spawnPointDelta) + m_itemSpawnzOffset;
                try
                {
                    spawnPoints[columnIndex, rowIndex] = new Vector3(xPoints[columnIndex], m_itemHeight, yVal);
                }
                catch (System.Exception e)
                {
                    //getting error to capacity
                }
            }
        }
    }


    [ContextMenu("Spawnables/Spawn Collections")]
    void SpawnItems(bool p_initSpawn = false)
    {
        if (p_initSpawn && transform.GetSiblingIndex() <= m_coinSpawnLimit)//Skip Spawn on Init
        {
            Debug.Log($"{gameObject.name} skipped spawn at index {transform.GetSiblingIndex()}");
            return;
        }

        bool spawnedColumn = false;
        bool obstacleSpawned = false;

        for (int columnIndex = 0; columnIndex < m_gridColumn; columnIndex++)
        {
            bool spawnOnThisColumn = Random.Range(0, 2) >= 1 - m_spawnChance;
            //SPAWN OBSTACLE
            if (SpawnObstacle(obstacleSpawned, columnIndex))
            {
                continue;
            }
            else
            {
                //SPAWN COIN
                for (int rowIndex = 0; rowIndex < m_gridRow; rowIndex++)
                {
                    if (spawnOnThisColumn)
                    {
                        var obj = Instantiate(m_coinPrefab, transform);
                        obj.transform.localPosition = spawnPoints[columnIndex, rowIndex];
                    }
                }
                spawnedColumn = spawnOnThisColumn;
            }
            if (spawnedColumn)// Spawn only on 1 column per block 
            {
                break;
            }
        }
    }

    bool SpawnObstacle(bool p_obstacleSpawned, int p_columnIndex)
    {
        if (m_spawnObstacle && Random.Range(0, 2) >= 1 && !p_obstacleSpawned)
        {
            var obj = Instantiate(m_ObstaclePrefab, transform);
            obj.transform.localPosition = spawnPoints[p_columnIndex, 0];
            p_obstacleSpawned = true;
            return true;
        }
        return false;
    }

    void GetXpoints()
    {
        var playerMovementPointDelta = GameManager.instance.GetMovementDistance;
        for (int xPointIndex = 0; xPointIndex < m_gridRow; xPointIndex++)
        {
            xPoints[xPointIndex] = xPointIndex * playerMovementPointDelta;
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


    public static Vector3[] GetBlockSpawnPoints(int p_count, Vector3 p_initialBlockPosition, float p_zPosOffset = 0)
    {
        Vector3[] spawnPoints = new Vector3[p_count];
        for (int spawnPointIndex = 0; spawnPointIndex < spawnPoints.Length; spawnPointIndex++)
        {
            //spawnPoints[spawnPointIndex] = p_initialBlockPosition + new Vector3(p_initialBlockPosition.x, p_initialBlockPosition.y, m_boundaryLength * spawnPointIndex);
            spawnPoints[spawnPointIndex] = new Vector3(0, 0, (m_boundaryLength + p_zPosOffset) * spawnPointIndex) + p_initialBlockPosition;
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

    #region Debugger

    private void OnDrawGizmos()
    {
        if (!showDebugTools)
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
                Gizmos.DrawCube(transform.position + new Vector3(transform.position.x, transform.position.y, (m_segmentBehaviour.GetBlockOffset() + m_collider.size.z) * i), m_collider.size);
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

}