using System.Collections.Generic;
using UnityEngine;

public class BinaryST<T>
{
    public Node<T> topNode;

    public BinaryST()
    {
        topNode = null;
    }

    public BinaryST(T topNodeValue, string topNodeId)
    {
        topNode = new Node<T>(topNodeValue);
        topNode.id = topNodeId;
    }

    ///<summary>
    /// Constructs a balanced binary search tree from <paramref name="dataList"/> (used for node values), and <paramref name="nameList"/> (used for node ids).
    ///</summary>
    public BinaryST(List<T> dataList, List<string> nameList)
    {
        RecursiveFromList(dataList, nameList, ref topNode, 0, dataList.Count - 1);
    }

    #region BinaryST Operations

        ///<summary>
        /// Populates the binary search tree with node values from <paramref name="dataList"/>, and ids from <paramref name="nameList"/>, this also balances the binary search tree.
        ///</summary>
        private void RecursiveFromList(List<T> dataList, List<string> nameList, ref Node<T> refNode, int start, int end)
        {
            if(end - start < 0)
            {
                return;
            }

            int mid = (end + start)/2;

            refNode = new Node<T>(dataList[mid], nameList[mid]);

            RecursiveFromList(dataList, nameList, ref refNode.LeftNode, start, mid - 1);
            RecursiveFromList(dataList, nameList, ref refNode.RightNode, mid + 1, end);
        }

        ///<summary>
        /// Returns an in order list of all the node values.
        ///</summary>
        public List<T> ToOrderedList()
        {
            List<T> returnList = new List<T>();

            ToListAscending(ref topNode, ref returnList);

            return returnList;
        }

        ///<summary>
        /// Returns the node at the given <paramref name="id"/>.
        ///</summary>
        public Node<T> GetNodeAt(string id)
        {
            return NodeAtID(ref topNode, id);
        }

        ///<summary>
        /// Adds the given <paramref name="node"/> to the first matching position within the tree.
        ///</summary>
        public void Add(Node<T> node)
        {
            AddRecursive(ref topNode, node);
            //topNode = BalanceTree(topNode);
        }

        ///<summary>
        /// Removes a note at <paramref name="nodeId"/> exists from the tree if it exists.
        ///</summary>
        public void Remove(string nodeId)
        {
            if(ContainsId(nodeId))
            {
                RemoveRecursive(ref topNode, nodeId);
                //topNode = BalanceTree(topNode);
            }
            else
            {
                Debug.Log("Remove failed, no item with " + nodeId + "id found.");
            }
        }

        ///<summary>
        /// Returns true if a node with <paramref name="node"/>'s id exists within the tree.
        ///</summary>
        public bool ContainsId(Node<T> node)
        {
            return ContainsIdRecursive(ref topNode, node.id);
        }

        ///<summary>
        /// Returns true if a node with <paramref name="nodeId"/> exists within the tree.
        ///</summary>
        public bool ContainsId(string nodeId)
        {
            return ContainsIdRecursive(ref topNode, nodeId);
        }

        ///<summary>
        /// Returns true if there are any matches for <paramref name="predicate"/>.
        ///</summary>
        public bool Any(System.Predicate<Node<T>> predicate)
        {
            bool retunBool = false;
            AnyRecursive(ref topNode, predicate, ref retunBool);
            return retunBool;
        }

        ///<summary>
        /// Returns all node values matching the <paramref name="predicate"/>.
        ///</summary>
        public List<T> All(System.Predicate<Node<T>> predicate)
        {
            List<T> returnList = new List<T>();
            AllRecursive(ref topNode, predicate, returnList);
            return returnList;
        }

        ///<summary>
        /// Applies <paramref name="doAction"/> to all nodes of the binary search tree.
        ///</summary>
        public void DoForeach(System.Action<Node<T>> doAction)
        {
            DoForeachRecursive(ref topNode, doAction);
        }

    #endregion

    #region Private recursives

        ///<summary>
        /// Travels the entire tree and applies the <paramref name="doAction"/> to all nodes.
        ///</summary>
        private void DoForeachRecursive(ref Node<T> node, System.Action<Node<T>> doAction)
        {
            if(node == null)
            {
                return;
            }

            doAction(node);

            DoForeachRecursive(ref node.LeftNode, doAction);
            DoForeachRecursive(ref node.RightNode, doAction);
        }

        ///<summary>
        /// Travels the entire tree looking for a predicate match. Adds all matches to <paramref name="returnList"/>.
        ///</summary>
        private void AllRecursive(ref Node<T> node, System.Predicate<Node<T>> predicate, List<T> returnList)
        {
            if(node == null)
            {
                return;
            }

            if(predicate(node))
            {
                returnList.Add(node.NodeValue);
            }

            AllRecursive(ref node.LeftNode, predicate, returnList);
            AllRecursive(ref node.RightNode, predicate, returnList);
        }

        ///<summary>
        /// Revursively travels the tree in order adding items to the <paramref name="returnList"/>.
        ///</summary>
        private void ToListAscending(ref Node<T> refNode, ref List<T> returnList)
        {
            if(refNode != (null))
            {
                ToListAscending(ref refNode.LeftNode, ref returnList);
                returnList.Add(refNode.NodeValue);
                ToListAscending(ref refNode.RightNode, ref returnList);
            }
            else
            {
                return;
            }
        }

        ///<summary>
        /// Travels the entire tree looking for a predicate match, sets the <paramref name="found"/> parameter true on the first match.
        ///</summary>
        private void AnyRecursive(ref Node<T> node, System.Predicate<Node<T>> predicate, ref bool found)
        {
            if(node == null)
            {
                return;
            }

            if(predicate(node))
            {
                found = true;
                return;
            }

            AnyRecursive(ref node.LeftNode, predicate, ref found);
            AnyRecursive(ref node.RightNode, predicate, ref found);
        }

        ///<summary>
        /// Travels the tree on the apropriate branch and adds the <paramref name="nodeToAdd"/> to the first null value down the right branch.
        ///</summary>
        private void AddRecursive(ref Node<T> refNode, Node<T> nodeToAdd)
        {
            //Check if the topnode is null
            if(refNode == null)
            {
                //Whenever we find a node reference that is null (going either left or right) we set that node
                refNode = new Node<T>(nodeToAdd);
                return;
            }
            else if (refNode.id.CompareTo(nodeToAdd.id) > 0)
            {
                AddRecursive(ref refNode.LeftNode, nodeToAdd);
                return;
            }
            else if (refNode.id.CompareTo(nodeToAdd.id) <= 0)
            {
                AddRecursive(ref refNode.RightNode, nodeToAdd);
                return;
            }
        }

        ///<summary>
        /// Recursively travels the tree and returns true if <paramref name="idToLookFor"/> exists.
        ///</summary>
        private bool ContainsIdRecursive(ref Node<T> refNode, string idToLookFor)
        {
            //Check if the topnode is null
            if(refNode != null)
            {
                if(refNode.id.Equals(idToLookFor))
                {
                    return true;
                }
                else if (refNode.id.CompareTo(idToLookFor) > 0)
                {
                    return ContainsIdRecursive(ref refNode.LeftNode, idToLookFor);
                }
                else if (refNode.id.CompareTo(idToLookFor) <= 0)
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

        ///<summary>
        /// Recursively travels the tree until the node with <paramref name="removeId"/> is found, and then checks what operation is needed to remove that node.
        ///</summary>
        private void RemoveRecursive(ref Node<T> refNode, string removeId)
        {
            // find the right node
            if(refNode.id.Equals(removeId))
            {
                // proper node found begin remove checks
                RemoveRecursiveCheck(ref refNode);
            }
            else if (refNode.id.CompareTo(removeId) > 0)
            {
                //recurse left
                RemoveRecursive(ref refNode.LeftNode, removeId);
            }
            else if (refNode.id.CompareTo(removeId) <= 0)
            {
                //recurse right
                RemoveRecursive(ref refNode.RightNode, removeId);
            }


        }

        ///<summary>
        /// Returns the apropriate node to remove/replace.
        ///</summary>
        private void RemoveRecursiveCheck(ref Node<T> nodeToRemove)
        {
            if(nodeToRemove.LeftNode.Equals(null) && nodeToRemove.RightNode.Equals(null))
            {
                //Leaf case
                // remove the link from the root of this node
                nodeToRemove = null;
            }
            else if(nodeToRemove.LeftNode.Equals(null) || nodeToRemove.RightNode.Equals(null))
            {
                //Single child case
                // replace node to remove with it's single child
                nodeToRemove = (nodeToRemove.LeftNode.Equals(null)) ? nodeToRemove.RightNode : nodeToRemove.LeftNode;
            }
            else
            {
                //find the minimum value of the right subtree(go left down) (should have no child) and set that as the node to delete, then null the minimum
                Node<T> nodeCache = MinInRightBranch(ref nodeToRemove.RightNode);
                nodeToRemove.NodeValue = nodeCache.NodeValue;
                nodeCache = null;
            }
        }

        ///<summary>
        /// Returns the lowest node at the botom right from the first given <paramref name="refNode"/>.
        ///</summary>
        private Node<T> MinInRightBranch(ref Node<T> refNode)
        {
            if(refNode.LeftNode.Equals(null))
            {
                return refNode;
            }
            else
            {
                return MinInRightBranch(ref refNode.LeftNode);
            }
        }

        ///<summary>
        /// Recirsively travels the tree only in the direction of the given <paramref name="id"/>.
        ///</summary>
        private Node<T> NodeAtID(ref Node<T> refNode, string id)
        {
            if(refNode.id == id.ToLower())
            {
                return refNode;
            }
            else if(refNode.id.CompareTo(id) > 0)
            {
                return NodeAtID(ref refNode.LeftNode, id);
            }
            else if (refNode.id.CompareTo(id) <= 0)
            {
                return NodeAtID(ref refNode.RightNode, id);
            }
            else
            {
                return null;
            }
        }


    #endregion
}

public class Node<T>
{
    public string id;
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

    public Node(T nodeValue, string nodeId)
    {
        NodeValue = nodeValue;
        id = nodeId;
    }

    public Node(Node<T> nodeToDuplicate)
    {
        id = nodeToDuplicate.id;
        NodeValue = nodeToDuplicate.NodeValue;
        LeftNode = null;
        RightNode = null;
    }
}
