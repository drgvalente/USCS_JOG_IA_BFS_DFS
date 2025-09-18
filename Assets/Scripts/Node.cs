using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

public enum NodeType
{
    Floor,
    Wall,
    Start,
    Goal
}

public class Node : MonoBehaviour
{
    [Header("Materials")]
    public Material actualWay;
    public Material blackListed;
    public Material finish;
    public Material floor;
    public Material possibleWay;
    public Material start;
    public Material wall;

    [Header("Node Properties")]
    public List<Node> neighbors = new List<Node>();
    public NodeType nodeType = NodeType.Floor;

    bool isClicked = false;

    public void OnNodeClicked()
    {
        if (nodeType != NodeType.Wall)
        {
            if (nodeType == NodeType.Start)
            {
                GetComponent<Renderer>().material = floor;
                nodeType = NodeType.Floor;
                GameObject.Find("GameManager").GetComponent<GameManager>().hasStart = false;
                GameObject.Find("GameManager").GetComponent<GameManager>().isSearching = false;
                GameObject.Find("GameManager").GetComponent<GameManager>().stackDFS.Clear();
                GameObject.Find("GameManager").GetComponent<GameManager>().visitedNodes.Clear();
                
            }
            else if (nodeType == NodeType.Goal)
            {
                GetComponent<Renderer>().material = floor;
                nodeType = NodeType.Floor;
                GameObject.Find("GameManager").GetComponent<GameManager>().hasGoal = false;
                GameObject.Find("GameManager").GetComponent<GameManager>().isSearching = false;
                GameObject.Find("GameManager").GetComponent<GameManager>().stackDFS.Clear();
                GameObject.Find("GameManager").GetComponent<GameManager>().visitedNodes.Clear();
            }
            else
            {
                if (!GameObject.Find("GameManager").GetComponent<GameManager>().hasStart)
                {
                    GetComponent<Renderer>().material = start;
                    nodeType = NodeType.Start;
                    GameObject.Find("GameManager").GetComponent<GameManager>().stackDFS.Push(this);
                    GameObject.Find("GameManager").GetComponent<GameManager>().hasStart = true;
                    if (GameObject.Find("GameManager").GetComponent<GameManager>().hasGoal)
                    {
                        GameObject.Find("GameManager").GetComponent<GameManager>().isSearching = true;
                        
                    }
                }
                else if (!GameObject.Find("GameManager").GetComponent<GameManager>().hasGoal)
                {
                    GetComponent<Renderer>().material = finish;
                    nodeType = NodeType.Goal;
                    GameObject.Find("GameManager").GetComponent<GameManager>().hasGoal = true;
                    if (GameObject.Find("GameManager").GetComponent<GameManager>().hasStart)
                    {
                        GameObject.Find("GameManager").GetComponent<GameManager>().isSearching = true;
                        foreach (GameObject n in GameObject.Find("GameManager").GetComponent<GameManager>().nodes)
                        {
                            if(n.GetComponent<Node>().nodeType == NodeType.Start)
                            {
                                GameObject.Find("GameManager").GetComponent<GameManager>().stackDFS.Push(n.GetComponent<Node>());
                                break;
                            }
                        }
                    }
                }
            }
        }

        /*
        if ( !isClicked )
        {
            GetComponent<Renderer>().material = start;
            isClicked = true;
        }
        else
        {
            GetComponent<Renderer>().material = floor;
            isClicked = false;
        }
        */
    }
}



