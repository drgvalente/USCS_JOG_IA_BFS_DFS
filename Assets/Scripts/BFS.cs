using UnityEngine; // Importa funcionalidades básicas do Unity
using System.Collections; // Importa coleções básicas do C#
using System.Collections.Generic; // Importa coleções genéricas como Dictionary, List, HashSet
using System.Diagnostics; // Importa funcionalidades de diagnóstico
using Unity.Properties; // Importa sistema de propriedades do Unity

// Classe responsável pela implementação do algoritmo BFS (Breadth-First Search)
public class BFS : MonoBehaviour
{
    // Método estático que executa um passo da busca BFS
    public static void PerformSearchStep()
    {
        // Obtém referência ao GameManager através da busca por GameObject
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // Obtém referência ao componente Data que contém as estruturas de dados
        Data data = GameObject.Find("GameManager").GetComponent<Data>();
        
        // Proteção contra loops infinitos: limita o número máximo de nós na queue
        if (data.queueBFS.Count > 10000)
        {
            // Para a busca se a queue ficar muito grande
            gameManager.isSearching = false;
            // Retorna sem continuar a execução
            return;
        }
        
        // Verifica se a queue está vazia (não há mais nós para processar)
        if (data.queueBFS.Count == 0)
        {
            // Para a busca pois não há caminho possível
            gameManager.isSearching = false;
            // Informa ao usuário que não foi possível encontrar um caminho
            UnityEngine.Debug.Log("BFS: Nenhum caminho encontrado para o objetivo!");
            // Pinta todos os nós visitados como blackListed quando não há caminho
            PaintVisitedNodesWhenNoPath(data.visitedNodes);
            // Retorna sem continuar a execução
            return;
        }
        
        // Remove o primeiro nó da queue (FIFO - First In, First Out)
        Node currentNode = data.queueBFS.Dequeue();
        // Chama o método para encontrar vizinhos do nó atual
        FindNeighbor(currentNode);
    }

    // Método estático que processa os vizinhos do nó atual
    static void FindNeighbor(Node currentNode)
    {
        // Obtém referência ao HashSet de nós visitados
        HashSet<Node> visitedNodes = GameObject.Find("GameManager").GetComponent<Data>().visitedNodes;
        // Obtém referência ao GameManager
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // Obtém referência ao componente Data
        Data data = GameObject.Find("GameManager").GetComponent<Data>();
        
        // Marca o nó atual como visitado ANTES de processar vizinhos (correção importante)
        if (!visitedNodes.Contains(currentNode))
        {
            // Adiciona o nó atual ao conjunto de visitados
            visitedNodes.Add(currentNode);
        }
        
        // Se o nó atual não é o Start, muda sua cor para "possibleWay"
        if(currentNode.nodeType != NodeType.Start)
            // Aplica o material que indica caminho possível
            currentNode.GetComponent<Renderer>().material = currentNode.possibleWay;
        
        // Flag para indicar se o objetivo foi encontrado
        bool goalFound = false;
        // Percorre todos os vizinhos do nó atual
        foreach (Node neighbor in currentNode.neighbors)
        {
            // Verifica se o vizinho é o nó objetivo
            if (neighbor == gameManager.goal)
            {
                // Marca que o objetivo foi encontrado
                goalFound = true;
                // Para a busca
                gameManager.isSearching = false;
                // Adiciona o nó objetivo aos visitados
                visitedNodes.Add(neighbor);
                
                // Reconstrói o caminho do start até o goal
                List<Node> finalPath = ReconstructPath(neighbor, currentNode);
                
                // Pinta todos os nós visitados que não fazem parte do caminho final
                PaintVisitedNodesNotInPath(finalPath, visitedNodes);
                
                // Define o caminho final no GameManager para iniciar a animação
                gameManager.SetFinalPath(finalPath);
                // Retorna pois o objetivo foi encontrado
                return;
            }
        }
        
        // Se o objetivo não foi encontrado, processa os vizinhos normalmente
        if (!goalFound)
        {
            // Percorre todos os vizinhos do nó atual
            foreach (Node neighbor in currentNode.neighbors)
            {
                // Verifica se o vizinho não foi visitado E não é uma parede
                if (!visitedNodes.Contains(neighbor) && neighbor.nodeType != NodeType.Wall)
                {
                    // Marca o vizinho como visitado
                    visitedNodes.Add(neighbor);
                    // Adiciona o vizinho à queue para processamento futuro
                    data.queueBFS.Enqueue(neighbor);
                    
                    // Armazena o pai do vizinho para reconstrução do caminho
                    if (!parentMap.ContainsKey(neighbor))
                    {
                        // Define o nó atual como pai do vizinho
                        parentMap[neighbor] = currentNode;
                    }
                }
            }
        }
    }
    
    // Dicionário estático para rastrear os pais dos nós (necessário para reconstrução do caminho)
    private static Dictionary<Node, Node> parentMap = new Dictionary<Node, Node>();
    
    // Método estático para reconstruir o caminho do goal até o start
    static List<Node> ReconstructPath(Node goal, Node lastParent)
    {
        // Cria uma nova lista para armazenar o caminho
        List<Node> path = new List<Node>();
        
        // Adiciona o nó objetivo ao caminho
        path.Add(goal);
        
        // Define o último pai como nó atual para reconstrução
        Node current = lastParent;
        // Adiciona o último pai ao caminho
        path.Add(current);
        
        // Reconstrói o caminho seguindo os pais até chegar ao start
        while (parentMap.ContainsKey(current))
        {
            // Obtém o pai do nó atual
            current = parentMap[current];
            // Adiciona o pai ao caminho
            path.Add(current);
        }
        
        // Inverte a lista para ter o caminho do start para o goal
        path.Reverse();
        
        // Retorna o caminho completo
        return path;
    }
    
    // Método público estático para limpar o mapa de pais quando uma nova busca começar
    public static void ClearParentMap()
    {
        // Remove todos os elementos do dicionário de pais
        parentMap.Clear();
    }
    
    // Método estático para pintar nós visitados quando não há caminho disponível
    static void PaintVisitedNodesWhenNoPath(HashSet<Node> visitedNodes)
    {
        // Percorre todos os nós visitados durante a busca
        foreach (Node visitedNode in visitedNodes)
        {
            // Verifica se o nó visitado não é Start nem Goal
            if (visitedNode.nodeType != NodeType.Start && 
                visitedNode.nodeType != NodeType.Goal)
            {
                // Aplica o material blackListed para indicar nó explorado sem caminho
                visitedNode.GetComponent<Renderer>().material = visitedNode.blackListed;
            }
        }
    }
    
    // Método estático para pintar nós visitados que não fazem parte do caminho final
    static void PaintVisitedNodesNotInPath(List<Node> finalPath, HashSet<Node> visitedNodes)
    {
        // Converte o caminho final para HashSet para busca mais eficiente O(1)
        HashSet<Node> pathNodes = new HashSet<Node>(finalPath);
        
        // Percorre todos os nós visitados durante a busca
        foreach (Node visitedNode in visitedNodes)
        {
            // Verifica se o nó visitado não está no caminho final e não é Start nem Goal
            if (!pathNodes.Contains(visitedNode) && 
                visitedNode.nodeType != NodeType.Start && 
                visitedNode.nodeType != NodeType.Goal)
            {
                // Aplica o material blackListed para indicar nó explorado mas não usado
                visitedNode.GetComponent<Renderer>().material = visitedNode.blackListed;
            }
        }
    }
}