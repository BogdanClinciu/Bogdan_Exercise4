using System.Collections.Generic;
using System.Linq;

public class BinaryST<T>
{
    public Node<T> topNode;

    public BinaryST()
    {
        topNode = null;
    }

    public BinaryST(T topNodeValue)
    {
        topNode = new Node<T>(topNodeValue);
    }

    #region BinaryST Operations

        // TODO: add sorting by a value of the node

        // TODO: add remove node(node) method

        // TODO: add ContainsId, or custom action

        public void Add(Node<T> node)
        {
            AddRecursive(ref topNode, node);
        }

        public bool ContainsId(Node<T> node)
        {
            return ContainsIdRecursive(ref topNode, node.id);
        }

        public bool ContainsId(int nodeId)
        {
            return ContainsIdRecursive(ref topNode, nodeId);
        }

        public bool Any(System.Predicate<Node<T>> predicate)
        {
            return AnyRecursive(ref topNode, predicate);
        }

    #endregion

    #region Private recursives


        private bool AnyRecursive(ref Node<T> node, System.Predicate<Node<T>> predicate)
        {
            if(!node.LeftNode.Equals(null))
            {
                if(predicate(node.LeftNode))
                {
                    return true;
                }
                else
                {
                    AnyRecursive(ref node.LeftNode, predicate);
                }
            }

            if(!node.RightNode.Equals(null))
            {

                if(predicate(node.RightNode))
                {
                    return true;
                }
                else
                {
                    AnyRecursive(ref node.RightNode, predicate);
                }
            }

            return false;
        }

        private void AddRecursive(ref Node<T> refNode, Node<T> node)
        {
            //Check if the topnode is null
            if(refNode.Equals(null))
            {
                //Whenever we find a node reference that is null (going either left or right) we set that node
                refNode = new Node<T>(node);
                return;
            }
            else if (refNode.id > node.id)
            {
                AddRecursive(ref refNode.LeftNode, node);
                return;
            }
            else if (refNode.id <= node.id)
            {
                AddRecursive(ref refNode.RightNode, node);
                return;
            }
        }

        private bool ContainsIdRecursive(ref Node<T> refNode, int idToLookFor)
        {
            //Check if the topnode is null
            if(!refNode.Equals(null))
            {
                if(refNode.id.Equals(idToLookFor))
                {
                    return true;
                }
                else if (refNode.id > idToLookFor)
                {
                    return ContainsIdRecursive(ref refNode.LeftNode, idToLookFor);
                }
                else if (refNode.id <= idToLookFor)
                {
                    return ContainsIdRecursive(ref refNode.RightNode, idToLookFor);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

    #endregion
}

public class Node<T>
{
    public int id;
    public T NodeValue;

    public Node<T> LeftNode;
    public Node<T> RightNode;


    public Node()
    {

    }

    public Node(T nodeValue)
    {
        NodeValue = nodeValue;
    }

    public Node(Node<T> nodeToDuplicate)
    {
        id = nodeToDuplicate.id;
        NodeValue = nodeToDuplicate.NodeValue;
        LeftNode = null;
        RightNode = null;
    }
}
