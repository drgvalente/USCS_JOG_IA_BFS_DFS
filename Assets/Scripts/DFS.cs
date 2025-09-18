using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class DFS : MonoBehaviour
{
    public static void PerformSearchStep()
    {
        UnityEngine.Debug.Log("Buscando DFS");

        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Data data = GameObject.Find("GameManager").GetComponent<Data>();
        Node currentNode = data.stackDFS.Peek();

        FindNeighbor(currentNode);
    }

    static void FindNeighbor(Node currentNode)
    {
        HashSet<Node> visitedNodes = GameObject.Find("GameManager").GetComponent<Data>().visitedNodes;
        foreach (Node neighbor in currentNode.neighbors)
        {
            if(neighbor == GameObject.Find("GameManager").GetComponent<GameManager>().goa)
            {

            }
            if (!visitedNodes.Contains(neighbor))
            {

            }
        }
    }
}