using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;

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
        currentNode.GetComponent<Renderer>().material = currentNode.possibleWay;
        bool goalFound = false;
        foreach (Node neighbor in currentNode.neighbors)
        {
            if (neighbor == GameObject.Find("GameManager").GetComponent<GameManager>().goal)
            {
                goalFound = true;
                GameObject.Find("GameManager").GetComponent<GameManager>().isSearching = false;
                visitedNodes.Add(neighbor);
                GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Push(neighbor);
                break;
            }
        }
        if (!goalFound)
        {
            bool neighborFound = false;
            foreach (Node neighbor in currentNode.neighbors)
            {

                if (!visitedNodes.Contains(neighbor))
                {
                    visitedNodes.Add(neighbor);
                    GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Push(neighbor);
                    neighborFound = true;
                }
            }
            if (!neighborFound)
            {
                GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Pop();
                currentNode.GetComponent<Renderer>().material = currentNode.blackListed;
                if (GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Count == 0)
                {
                    GameObject.Find("GameManager").GetComponent<GameManager>().isSearching = false;
                    UnityEngine.Debug.Log("Não existe caminho possível");
                }
            }
        }
    }
}