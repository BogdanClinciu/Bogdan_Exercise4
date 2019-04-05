using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BST_Tester : MonoBehaviour
{
    [SerializeField]
    private GameObject nodePrefab;

    private List<string> stockStrings;

    [SerializeField]
    private float verticalOffset;

    [SerializeField]
    private float horizontalOffset;

    private Vector3 lastnodePos;
    private float counter = 6;

    private void Start()
    {
        stockStrings = new List<string>{"first", "second", "third", "fourth", "fifth", "sixth", "seventh", "eight", "nineth", "tenth", "eleventh", "twelveth"};

        BinaryST<string> bst = new BinaryST<string>(stockStrings,stockStrings);


        List<string> bstList = bst.ToOrderedList();

        string debugString = string.Empty;

        for (int i = 0; i < bstList.Count; i++)
        {
            debugString += bstList[i] + ", ";
        }

        Debug.Log(debugString);
        Debug.Log(bst.Any(a => a.NodeValue.Contains("x")));
        Debug.Log(bst.GetNodeAt("sec.ond").NodeValue);

        bst.DoForeach((urnode) => urnode.NodeValue = urnode.NodeValue.ToUpper());

        RecursiveCreateSpheres(ref bst.topNode, Vector3.zero, transform);
    }

    private void RecursiveCreateSpheres(ref Node<string> node, Vector3 position, Transform parent)
    {
        if(node == null)
        {
            return;
        }

        GameObject n = Instantiate(nodePrefab, position, Quaternion.identity, parent);
        n.GetComponentInChildren<Text>().text = node.NodeValue;


        counter --;
        RecursiveCreateSpheres(ref node.LeftNode, position + Vector3.down * verticalOffset + Vector3.left * horizontalOffset, n.transform );
        RecursiveCreateSpheres(ref node.RightNode, position + Vector3.down * verticalOffset + Vector3.right * horizontalOffset,n.transform );
    }
}
