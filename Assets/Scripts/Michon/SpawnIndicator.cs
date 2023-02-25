using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnIndicator : MonoBehaviour
{
    public Color color = Color.yellow;
    public float sphereSize = 1f;
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, sphereSize);
    }
}
