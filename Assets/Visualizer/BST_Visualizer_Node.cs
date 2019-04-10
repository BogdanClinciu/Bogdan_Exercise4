using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BST_Visualizer_Node : MonoBehaviour
{
    [SerializeField]
    internal Image Background;
    [SerializeField]
    internal Text NodeVal;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.parent.position);
    }
}
