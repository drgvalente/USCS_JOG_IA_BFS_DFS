using UnityEngine; // Importa as funcionalidades básicas do Unity
using System.Collections.Generic; // Importa coleções genéricas como Stack e HashSet
using System.Collections; // Importa IEnumerator para corrotinas

// Classe que implementa o algoritmo Depth-First Search (DFS)
public class TraeDFS : MonoBehaviour
{
    [Header("DFS Configuration")] // Cria uma seção no Inspector do Unity
    public Node startNode; // Nó inicial para começar a busca DFS
    
    [Header("Visualization")] // Seção para configurações de visualização
    public float searchDelay = 0.1f; // Delay entre cada passo da busca para visualização
    
    private bool isSearching = false; // Flag para controlar se uma busca está em andamento

    // Método público para iniciar a busca DFS
    public void StartDFS()
    {
        // Verifica se já não está executando uma busca
        if (!isSearching)
        {
            StartCoroutine(DFSAlgorithm());
        }
    }

    // Método principal que executa o algoritmo DFS como corrotina
    private IEnumerator DFSAlgorithm()
    {
        // Marca que a busca está em andamento
        isSearching = true;
        
        // Verifica se o nó inicial foi definido
        if (startNode == null)
        {
            Debug.LogError("Nó inicial não foi definido!");
            isSearching = false;
            yield break;
        }
        
        // Cria uma pilha (Stack) para armazenar os nós a serem visitados
        // DFS usa pilha para implementar o comportamento LIFO (Last In, First Out)
        Stack<Node> stack = new Stack<Node>();
        
        // Cria um conjunto (HashSet) para rastrear os nós já visitados
        // HashSet oferece verificação O(1) para evitar revisitar nós
        HashSet<Node> visited = new HashSet<Node>();

        // Adiciona o nó inicial na pilha para começar a busca
        stack.Push(startNode);
        
        // Marca o nó inicial como visitado
        visited.Add(startNode);

        // Loop principal: continua enquanto houver nós na pilha
        while (stack.Count > 0)
        {
            // Remove e obtém o nó do topo da pilha (comportamento LIFO)
            Node currentNode = stack.Pop();
            
            // Aplica material visual para mostrar o nó sendo processado
            if (currentNode.nodeType != NodeType.Start && currentNode.nodeType != NodeType.Goal)
            {
                currentNode.GetComponent<Renderer>().material = currentNode.actualWay;
            }
            
            // Verifica se encontrou o objetivo
            if (currentNode.nodeType == NodeType.Goal)
            {
                Debug.Log("Objetivo encontrado!");
                isSearching = false;
                yield break;
            }
            
            // Aguarda um tempo para visualização
            yield return new WaitForSeconds(searchDelay);
            
            // Itera através de todos os vizinhos do nó atual
            foreach (Node neighbor in currentNode.neighbors)
            {
                // Verifica se o vizinho existe, não foi visitado e não é uma parede
                if (neighbor != null && !visited.Contains(neighbor) && neighbor.nodeType != NodeType.Wall)
                {
                    // Adiciona o vizinho não visitado na pilha
                    stack.Push(neighbor);
                    
                    // Marca o vizinho como visitado
                    visited.Add(neighbor);
                    
                    // Aplica material visual para mostrar nós possíveis
                    if (neighbor.nodeType != NodeType.Start && neighbor.nodeType != NodeType.Goal)
                    {
                        neighbor.GetComponent<Renderer>().material = neighbor.possibleWay;
                    }
                }
            }
        }
        
        // Se chegou aqui, não encontrou o objetivo
        Debug.Log("Objetivo não encontrado!");
        isSearching = false;
    }
}
