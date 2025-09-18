using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public enum searchType
{
    BFS,
    DFS
}

public class GameManager : MonoBehaviour
{
    [Header("Map Size")]
    public int height = 20;
    public int width = 20;
    public int numberOfWalls = 50;

    [Header("Node")]
    public GameObject node;
    public List<GameObject> nodes;

    public List<GameObject> walls;
    [HideInInspector]
    public bool hasStart = false;
    [HideInInspector]
    public bool hasGoal = false;

    [Header("Search")]
    public searchType searchType = searchType.DFS;
    public Stack<Node> stackDFS = new Stack<Node>(); // caminho do DFS
    public Queue<Node> queueBFS = new Queue<Node>(); // caminho do BFS
    public HashSet<Node> visitedNodes = new HashSet<Node>(); // nós visitados
    public float searchDelay = 1f; // atraso entre cada passo da busca
    float searchTimer = 0f;


    [HideInInspector]
    public bool isSearching = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GridGenerator.GenerateGrid(height, width, numberOfWalls, node);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }

        if (isSearching)
        {
            searchTimer += Time.deltaTime;
            if (searchTimer >= searchDelay)
            {
                searchTimer = 0f;
                if (searchType == searchType.DFS)
                {
                    DFS.PerformSearchStep();
                    UnityEngine.Debug.Log(stackDFS.Count);
                }
                else if (searchType == searchType.BFS)
                {
                    //BFS.PerformSearchStep();
                }
            }
        }
    }

    
     

    private void HandleMouseClick()
    {
        // Cria um ray da câmera para a posição do mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Verifica se o ray atingiu algum objeto
        if (Physics.Raycast(ray, out hit))
        {
            // Verifica se o objeto atingido tem o componente Node
            Node nodeComponent = hit.collider.GetComponent<Node>();
            if (nodeComponent != null)
            {
                nodeComponent.OnNodeClicked();     
            }
        }
    }
}
