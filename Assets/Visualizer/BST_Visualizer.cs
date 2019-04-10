using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BST_Visualizer : MonoBehaviour
{
    [SerializeField]
    private GameObject nodePrefab;
    [SerializeField]
    private RectTransform nodeOrigin;
    [SerializeField]
    private InputField nodeInputField;

    private BinaryST<BST_Visualizer_Node> tree;

    [SerializeField]
    private float horizontalOffset = 1.0f;
    [SerializeField]
    private float verticalOffset = 1.0f;



    private int levelsDown = 1;

    private void Start()
    {
        tree = new BinaryST<BST_Visualizer_Node>();
    }

    public void AddNew()
    {
        if (!string.IsNullOrEmpty(nodeInputField.text))
        {
            if(!tree.ContainsId(nodeInputField.text.ToLower()))
            {
                levelsDown = 1;
                BST_Visualizer_Node newNode = Instantiate(nodePrefab, nodeOrigin.position, Quaternion.identity, nodeOrigin).GetComponent<BST_Visualizer_Node>();
                newNode.NodeVal.text = nodeInputField.text.ToLower();
                AddRecursive(ref tree.topNode, new Node<BST_Visualizer_Node>(newNode, nodeInputField.text.ToLower()), nodeOrigin.position);
            }
        }
    }


    private void AddRecursive(ref Node<BST_Visualizer_Node> refNode, Node<BST_Visualizer_Node> nodeToAdd, Vector3 parentPos)
    {
        //Check if the topnode is null
        if(refNode == null)
        {
            //Whenever we find a node reference that is null (going either left or right) we set that node
            refNode = nodeToAdd;
            return;
        }
        else if (refNode.id.CompareTo(nodeToAdd.id) > 0)
        {
            nodeToAdd.NodeValue.transform.SetParent(refNode.NodeValue.transform);
            nodeToAdd.NodeValue.transform.localPosition = Vector3.zero;
            nodeToAdd.NodeValue.transform.localPosition += new Vector3(- (horizontalOffset / levelsDown) -50, - verticalOffset, 0);
            levelsDown ++;
            AddRecursive(ref refNode.LeftNode, nodeToAdd, refNode.NodeValue.transform.position);
            return;
        }
        else if (refNode.id.CompareTo(nodeToAdd.id) <= 0)
        {
            nodeToAdd.NodeValue.transform.SetParent(refNode.NodeValue.transform);
            nodeToAdd.NodeValue.transform.localPosition = Vector3.zero;
            nodeToAdd.NodeValue.transform.localPosition = new Vector3(+ (horizontalOffset / levelsDown)+50, - verticalOffset, 0);
            levelsDown ++;
            AddRecursive(ref refNode.RightNode, nodeToAdd, refNode.NodeValue.transform.position);
            return;
        }
    }
}
