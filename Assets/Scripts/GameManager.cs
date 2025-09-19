using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

// Enumeração que define os tipos de algoritmos de busca disponíveis
public enum searchType
{
    BFS, // Busca em Largura (Breadth-First Search)
    DFS  // Busca em Profundidade (Depth-First Search)
}

// Classe principal que gerencia o jogo e coordena as buscas
public class GameManager : MonoBehaviour
{
    // Seção de configurações do mapa
    [Header("Map Size")]
    public int height = 20;        // Altura do grid em nós
    public int width = 20;         // Largura do grid em nós
    public int numberOfWalls = 50; // Número de paredes a serem geradas aleatoriamente

    // Seção de configurações dos nós
    [Header("Node")]
    public GameObject node;           // Prefab do nó a ser instanciado
    public List<GameObject> nodes;    // Lista de todos os nós no grid

    public List<GameObject> walls;    // Lista específica de nós que são paredes
    
    // Variáveis de controle de estado do jogo (ocultas no inspector)
    [HideInInspector]
    public bool hasStart = false;     // Indica se há um nó inicial definido
    [HideInInspector]
    public Node start;                // Referência ao nó inicial
    [HideInInspector]
    public bool hasGoal = false;      // Indica se há um nó objetivo definido
    [HideInInspector]
    public Node goal;                 // Referência ao nó objetivo

    // Seção de configurações da busca
    [Header("Search")]
    public searchType searchType = searchType.DFS; // Tipo de algoritmo de busca selecionado
    public Data data;                               // Componente que armazena estruturas de dados da busca
    public float searchDelay = 1f;                  // Atraso em segundos entre cada passo da busca
    float searchTimer = 0f;                         // Timer interno para controlar o atraso

    // Variáveis de controle do estado da busca (ocultas no inspector)
    [HideInInspector]
    public bool isSearching = false;  // Indica se uma busca está em andamento
    [HideInInspector]
    public bool searchDone = false;   // Indica se a busca foi concluída
    
    // Seção de configurações da animação do caminho
    [Header("Path Animation")]
    public float animationSpeed = 0.1f;  // Velocidade da animação (tempo entre mudanças)
    public int lightTrailLength = 3;     // Quantos nós ficam "acesos" simultaneamente
    
    // Variáveis privadas para controle da animação
    private List<Node> finalPath = new List<Node>(); // Caminho final encontrado pela busca
    private float animationTimer = 0f;               // Timer para controlar a velocidade da animação
    private int currentAnimationIndex = 0;           // Índice atual na animação do caminho

    // Método chamado uma vez antes da primeira execução do Update
    void Start()
    {
        // Gera o grid inicial com as dimensões e número de paredes especificados
        GridGenerator.GenerateGrid(height, width, numberOfWalls, node);
    }

    // Método chamado a cada frame
    void Update()
    {
        // Verifica se o botão esquerdo do mouse foi pressionado
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick(); // Processa o clique do mouse
        }

        // Se uma busca está em andamento, controla os passos da busca
        if (isSearching)
        {
            searchTimer += Time.deltaTime; // Incrementa o timer com o tempo decorrido
            
            // Se o tempo de atraso foi atingido, executa o próximo passo
            if (searchTimer >= searchDelay)
            {
                searchTimer = 0f; // Reseta o timer
                
                // Executa o passo apropriado baseado no tipo de busca selecionado
                if (searchType == searchType.DFS)
                {
                    DFS.PerformSearchStep(); // Executa um passo do DFS
                }
                else if (searchType == searchType.BFS)
                {
                    BFS.PerformSearchStep(); // Executa um passo do BFS
                }
            }
        }
        
        // Chama a animação do caminho se a busca foi concluída
        AnimatePath();
    }  

    // Método privado para processar cliques do mouse no grid
    private void HandleMouseClick()
    {
        // Cria um ray da câmera principal para a posição atual do mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit; // Variável para armazenar informações do objeto atingido

        // Verifica se o ray atingiu algum objeto com collider
        if (Physics.Raycast(ray, out hit))
        {
            // Tenta obter o componente Node do objeto atingido
            Node nodeComponent = hit.collider.GetComponent<Node>();
            
            // Se o objeto tem um componente Node, processa o clique
            if (nodeComponent != null)
            {
                nodeComponent.OnNodeClicked(); // Chama o método de clique do nó
            }
        }
    }

    // Método privado para animar o caminho encontrado
    private void AnimatePath()
    {
        // Só executa a animação se a busca foi concluída e há um caminho válido
        if (!searchDone || finalPath.Count == 0)
            return;

        // Controla o tempo da animação baseado na velocidade configurada
        animationTimer += Time.deltaTime;
        
        // Se o tempo da animação foi atingido, atualiza a visualização
        if (animationTimer >= animationSpeed)
        {
            animationTimer = 0f; // Reseta o timer da animação
            
            // Remove o efeito de "luz" de todos os nós do caminho
            for (int i = 0; i < finalPath.Count; i++)
            {
                if (finalPath[i] != null)
                {
                    finalPath[i].SetPathAnimation(false); // Desativa animação do nó
                }
            }
            
            // Aplica o efeito de "luz" nos nós atuais (baseado no comprimento da trilha)
            for (int i = 0; i < lightTrailLength; i++)
            {
                // Calcula o índice do nó considerando o movimento circular
                int nodeIndex = (currentAnimationIndex + i) % finalPath.Count;
                
                // Se o nó existe, ativa sua animação
                if (finalPath[nodeIndex] != null)
                {
                    finalPath[nodeIndex].SetPathAnimation(true); // Ativa animação do nó
                }
            }
            
            // Avança para o próximo nó na sequência (movimento circular)
            currentAnimationIndex = (currentAnimationIndex + 1) % finalPath.Count;
        }
    }
    
    // Método público para definir o caminho final e iniciar a animação
    public void SetFinalPath(List<Node> path)
    {
        finalPath = new List<Node>(path); // Cria uma cópia da lista do caminho
        currentAnimationIndex = 0;        // Reseta o índice da animação
        animationTimer = 0f;              // Reseta o timer da animação
        searchDone = true;                // Marca a busca como concluída
    }
}
