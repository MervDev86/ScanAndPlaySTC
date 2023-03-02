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
    [Space]
    [Header("Grid")]
    //xPoints is supposed to be based off the movement points of the player  | l | m = 0 | r |   ++ current: -2 0 2
    [SerializeField] float[] xPoints = new float[3];
    [SerializeField] int m_gridColumn = 3; //DEFAULT = 3  => FOR NOW KEEP THE SAME VAL UNLESS CHANGED IN THE FUTURE
    [Range(0,100)]
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
    public static float zSize = 0;

    private void OnValidate()
    {
        if (m_collider == null)
            m_collider = GetComponent<BoxCollider>();

        if (m_gridRow<0)
        {
            m_gridRow = 0;
        }
        InitGridLayout(); // To Keep track of the spawn Positions
    }

    private void Start()
    {
        m_collider = this.gameObject.GetComponent<BoxCollider>();
        m_segmentBehaviour.onRespawn += OnSegmentRespawn;

        InitGridLayout();
        DestroySpawnedItems();
        SpawnItems();
    }

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
        //GetXpoints();
        spawnPoints = new Vector3[m_gridColumn, m_gridRow];

        //var pointDiffX = m_collider.bounds.max.x / row;

        if (zSize <= 0)
        {
            zSize = m_collider.size.z;
            Debug.Log($"zSize has been set to {zSize}");
        }

        //maxXBounds = m_collider.bounds.max.x;
        zSize = m_collider.bounds.size.z;
        var spawnPointDelta = zSize / m_gridRow;

        Debug.LogWarning($"{gameObject.transform.parent.transform.parent}  using Max Z Bounds: {zSize} \n" +
            $"calculated spawn distance = {spawnPointDelta}");

        for (int columnIndex = 0; columnIndex < m_gridColumn; columnIndex++)//ROW
        {
            for (int rowIndex = 0; rowIndex < m_gridRow; rowIndex++)//Index
            {
                //var xVal = offset - pointXIndex * pointDiffX;
                var yVal = (rowIndex * spawnPointDelta) + zOffset;
                //spawnPoints[pointXIndex, pointYIndex] = new Vector3(xPoints[pointXIndex], transform.position.y, yVal);
                try
                {
                    spawnPoints[columnIndex, rowIndex] = new Vector3(xPoints[columnIndex], m_itemHeight, yVal);
                }
                catch (System.Exception e)
                {
                    //Debug.LogError($"[{pointXIndex},{pointYIndex}] :  Value not Added");
                    //Debug.Log(e.Message);
                }
            }
        }
        //Debug.Log($"total Values loaded: {points.Length}");
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
        foreach (var spawnPosition in spawnPoints)
        {
            var obj = Instantiate(m_coinPrefab, transform);
            obj.transform.localPosition = spawnPosition;
        }
    }

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
            spawnPoints[spawnPointIndex] = p_initialBlockPosition + new Vector3(p_initialBlockPosition.x, p_initialBlockPosition.y, zSize * spawnPointIndex);
        }

        return spawnPoints;
    }

    [ContextMenu("Block/Check Collider Length")]
    public void GetColliderLength()
    {
        zSize = GetComponent<BoxCollider>().bounds.size.z;
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

//        foreach (Transform child in transform)
//        {
//#if UNITY_EDITOR
//            DestroyImmediate(child.gameObject);
//#else
//            Destroy(child.gameObject);
//#endif
//        }
    }

    public void SetSegmentParent(SegmentBehaviour p_segmentBehaviour)
    {
        m_segmentBehaviour = p_segmentBehaviour;
    }

}