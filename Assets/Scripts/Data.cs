using UnityEngine;
using System.Collections.Generic;

public class Data : MonoBehaviour
{
    public Stack<Node> stackDFS = new Stack<Node>(); // caminho do DFS
    public Queue<Node> queueBFS = new Queue<Node>(); // caminho do BFS
    public HashSet<Node> visitedNodes = new HashSet<Node>(); // nós visitados

}
