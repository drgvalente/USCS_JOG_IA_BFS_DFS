using UnityEngine; // Importa as funcionalidades básicas do Unity
using System.Collections.Generic; // Importa coleções genéricas como Stack e HashSet
using System.Collections; // Importa IEnumerator para corrotinas

// Classe que implementa o algoritmo Depth-First Search (DFS) com visualização
public class TraeDFS : MonoBehaviour
{
    // Seção de configuração do DFS no Inspector do Unity
    [Header("DFS Configuration")]
    public Node startNode; // Nó inicial para começar a busca DFS
    
    // Seção de configurações de visualização no Inspector
    [Header("Visualization")]
    public float searchDelay = 0.1f; // Delay em segundos entre cada passo da busca para visualização
    
    // Variável privada para controlar se uma busca está em andamento
    private bool isSearching = false;

    // Método público para iniciar a busca DFS
    public void StartDFS()
    {
        // Verifica se já não está executando uma busca para evitar múltiplas execuções simultâneas
        if (!isSearching)
        {
            StartCoroutine(DFSAlgorithm()); // Inicia a corrotina do algoritmo DFS
        }
    }

    // Método principal que executa o algoritmo DFS como corrotina para permitir visualização
    private IEnumerator DFSAlgorithm()
    {
        // Marca que a busca está em andamento
        isSearching = true;
        
        // Verifica se o nó inicial foi definido antes de começar
        if (startNode == null)
        {
            isSearching = false; // Reseta o flag de busca
            yield break; // Sai da corrotina se não há nó inicial
        }
        
        // Cria uma pilha (Stack) para armazenar os nós a serem visitados
        // DFS usa pilha para implementar o comportamento LIFO (Last In, First Out)
        Stack<Node> stack = new Stack<Node>();
        
        // Cria um conjunto (HashSet) para rastrear os nós já visitados
        // HashSet oferece verificação O(1) para evitar revisitar nós
        HashSet<Node> visited = new HashSet<Node>();

        // Adiciona o nó inicial na pilha para começar a busca
        stack.Push(startNode);
        
        // Marca o nó inicial como visitado para evitar processá-lo novamente
        visited.Add(startNode);

        // Loop principal: continua enquanto houver nós na pilha para processar
        while (stack.Count > 0)
        {
            // Remove e obtém o nó do topo da pilha (comportamento LIFO do DFS)
            Node currentNode = stack.Pop();
            
            // Aplica material visual para mostrar o nó sendo processado atualmente
            // Não altera a aparência dos nós especiais (Start e Goal)
            if (currentNode.nodeType != NodeType.Start && currentNode.nodeType != NodeType.Goal)
            {
                currentNode.GetComponent<Renderer>().material = currentNode.actualWay;
            }
            
            // Verifica se o nó atual é o objetivo da busca
            if (currentNode.nodeType == NodeType.Goal)
            {
                isSearching = false; // Marca que a busca foi concluída
                yield break; // Sai da corrotina pois encontrou o objetivo
            }
            
            // Aguarda um tempo configurado para permitir visualização do processo
            yield return new WaitForSeconds(searchDelay);
            
            // Itera através de todos os vizinhos do nó atual
            foreach (Node neighbor in currentNode.neighbors)
            {
                // Verifica se o vizinho é válido: existe, não foi visitado e não é uma parede
                if (neighbor != null && !visited.Contains(neighbor) && neighbor.nodeType != NodeType.Wall)
                {
                    // Adiciona o vizinho não visitado na pilha para processamento futuro
                    stack.Push(neighbor);
                    
                    // Marca o vizinho como visitado para evitar processá-lo novamente
                    visited.Add(neighbor);
                    
                    // Aplica material visual para mostrar nós que podem ser explorados
                    // Não altera a aparência dos nós especiais (Start e Goal)
                    if (neighbor.nodeType != NodeType.Start && neighbor.nodeType != NodeType.Goal)
                    {
                        neighbor.GetComponent<Renderer>().material = neighbor.possibleWay;
                    }
                }
            }
        }
        
        // Se chegou aqui, a pilha está vazia e não encontrou o objetivo
        isSearching = false; // Marca que a busca foi concluída sem sucesso
    }
}
