using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

// Enumeração que define os tipos de nós possíveis no grid
public enum NodeType
{
    Floor,  // Nó de chão (caminhável)
    Wall,   // Nó de parede (obstáculo)
    Start,  // Nó inicial da busca
    Goal    // Nó objetivo da busca
}

// Classe que representa um nó individual no grid de busca
public class Node : MonoBehaviour
{
    // Seção de materiais visuais para diferentes estados do nó
    [Header("Materials")]
    public Material actualWay;    // Material para nós que fazem parte do caminho final
    public Material blackListed;  // Material para nós visitados que não fazem parte do caminho
    public Material finish;       // Material para o nó objetivo
    public Material floor;        // Material padrão para nós de chão
    public Material possibleWay;  // Material para nós sendo explorados
    public Material start;        // Material para o nó inicial
    public Material wall;         // Material para nós de parede

    // Seção de propriedades do nó
    [Header("Node Properties")]
    public List<Node> neighbors = new List<Node>(); // Lista de nós vizinhos conectados
    public NodeType nodeType = NodeType.Floor;      // Tipo atual do nó

    // Variáveis privadas para controle de animação
    private bool isAnimating = false;     // Controla se o nó está sendo animado
    private Material originalMaterial;    // Armazena o material original antes da animação

    // Método chamado quando o nó é clicado pelo usuário
    public void OnNodeClicked()
    {
        // Verifica se o nó não é uma parede (paredes não podem ser modificadas)
        if (nodeType != NodeType.Wall)
        {
            // Se o nó clicado é o nó inicial, remove-o
            if (nodeType == NodeType.Start)
            {
                GetComponent<Renderer>().material = floor; // Volta ao material de chão
                GameObject.Find("GameManager").GetComponent<GameManager>().start = null; // Remove referência do start
                nodeType = NodeType.Floor; // Muda tipo para chão
                GameObject.Find("GameManager").GetComponent<GameManager>().hasStart = false; // Marca que não há start
                GameObject.Find("GameManager").GetComponent<GameManager>().isSearching = false; // Para a busca
                GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Clear(); // Limpa pilha do DFS
                GameObject.Find("GameManager").GetComponent<Data>().visitedNodes.Clear(); // Limpa nós visitados
            }
            // Se o nó clicado é o nó objetivo, remove-o
            else if (nodeType == NodeType.Goal)
            {
                GetComponent<Renderer>().material = floor; // Volta ao material de chão
                GameObject.Find("GameManager").GetComponent<GameManager>().goal = null; // Remove referência do goal
                nodeType = NodeType.Floor; // Muda tipo para chão
                GameObject.Find("GameManager").GetComponent<GameManager>().hasGoal = false; // Marca que não há goal
                GameObject.Find("GameManager").GetComponent<GameManager>().isSearching = false; // Para a busca
                GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Clear(); // Limpa pilha do DFS
                GameObject.Find("GameManager").GetComponent<Data>().visitedNodes.Clear(); // Limpa nós visitados
            }
            // Se o nó é um chão normal, define como start ou goal conforme necessário
            else
            {
                // Se ainda não há nó inicial, define este como start
                if (!GameObject.Find("GameManager").GetComponent<GameManager>().hasStart)
                {
                    GetComponent<Renderer>().material = start; // Aplica material de start
                    GameObject.Find("GameManager").GetComponent<GameManager>().start = this; // Define referência do start
                    nodeType = NodeType.Start; // Muda tipo para start
                    
                    // Limpa estruturas de dados de buscas anteriores
                    GameObject.Find("GameManager").GetComponent<Data>().visitedNodes.Clear();
                    GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Clear();
                    GameObject.Find("GameManager").GetComponent<Data>().queueBFS.Clear();
                    BFS.ClearParentMap(); // Limpa mapa de pais do BFS
                    
                    // Adiciona o nó inicial na estrutura de dados correta baseada no tipo de busca
                    if (GameObject.Find("GameManager").GetComponent<GameManager>().searchType == searchType.DFS)
                    {
                        GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Push(this); // Adiciona na pilha do DFS
                    }
                    else if (GameObject.Find("GameManager").GetComponent<GameManager>().searchType == searchType.BFS)
                    {
                        GameObject.Find("GameManager").GetComponent<Data>().queueBFS.Enqueue(this); // Adiciona na fila do BFS
                    }
                    
                    GameObject.Find("GameManager").GetComponent<Data>().visitedNodes.Add(this); // Marca como visitado
                    GameObject.Find("GameManager").GetComponent<GameManager>().hasStart = true; // Marca que há start
                    
                    // Se já há goal definido, inicia a busca
                    if (GameObject.Find("GameManager").GetComponent<GameManager>().hasGoal)
                    {
                        GameObject.Find("GameManager").GetComponent<GameManager>().isSearching = true;
                    }
                }
                // Se já há start mas não há goal, define este como goal
                else if (!GameObject.Find("GameManager").GetComponent<GameManager>().hasGoal)
                {
                    GetComponent<Renderer>().material = finish; // Aplica material de goal
                    GameObject.Find("GameManager").GetComponent<GameManager>().goal = this; // Define referência do goal
                    nodeType = NodeType.Goal; // Muda tipo para goal
                    GameObject.Find("GameManager").GetComponent<GameManager>().hasGoal = true; // Marca que há goal
                    
                    // Se já há start definido, prepara para iniciar a busca
                    if (GameObject.Find("GameManager").GetComponent<GameManager>().hasStart)
                    {
                        GameObject.Find("GameManager").GetComponent<GameManager>().isSearching = true; // Inicia busca
                        
                        // Limpa estruturas de dados de buscas anteriores
                        GameObject.Find("GameManager").GetComponent<Data>().visitedNodes.Clear();
                        GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Clear();
                        GameObject.Find("GameManager").GetComponent<Data>().queueBFS.Clear();
                        BFS.ClearParentMap(); // Limpa mapa de pais do BFS
                        
                        // Encontra o nó start e o adiciona na estrutura de dados correta
                        foreach (GameObject n in GameObject.Find("GameManager").GetComponent<GameManager>().nodes)
                        {
                            if(n.GetComponent<Node>().nodeType == NodeType.Start)
                            {
                                // Adiciona o nó inicial na estrutura correta baseada no tipo de busca
                                if (GameObject.Find("GameManager").GetComponent<GameManager>().searchType == searchType.DFS)
                                {
                                    GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Push(n.GetComponent<Node>());
                                }
                                else if (GameObject.Find("GameManager").GetComponent<GameManager>().searchType == searchType.BFS)
                                {
                                    GameObject.Find("GameManager").GetComponent<Data>().queueBFS.Enqueue(n.GetComponent<Node>());
                                }
                                break; // Para o loop após encontrar o start
                            }
                        }
                    }
                }
            }
        }
    }
    
    // Método para controlar a animação visual do caminho encontrado
    public void SetPathAnimation(bool animate)
    {
        // Se deve animar e ainda não está animando
        if (animate && !isAnimating)
        {
            // Salva o material original antes de aplicar a animação
            originalMaterial = GetComponent<Renderer>().material;
            GetComponent<Renderer>().material = actualWay; // Aplica material de caminho atual
            isAnimating = true; // Marca como animando
        }
        // Se deve parar de animar e está animando
        else if (!animate && isAnimating)
        {
            // Restaura o material original se existir
            if (originalMaterial != null)
            {
                GetComponent<Renderer>().material = originalMaterial;
            }
            isAnimating = false; // Marca como não animando
        }
    }
}



