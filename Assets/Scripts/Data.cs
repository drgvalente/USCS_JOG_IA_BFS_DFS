using UnityEngine;
using System.Collections.Generic;

public class Data : MonoBehaviour
{
    public Stack<Node> stackDFS = new Stack<Node>(); // caminho do DFS
    public Queue<Node> queueBFS = new Queue<Node>(); // caminho do BFS alternativa 2
    public List<Node> listBFS = new List<Node>(); // caminho do BFS alternativa 1
    public HashSet<Node> visitedNodes = new HashSet<Node>(); // nós visitados

}
