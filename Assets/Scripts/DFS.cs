using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;

// Classe que implementa o algoritmo de Busca em Profundidade (Depth-First Search)
public class DFS : MonoBehaviour
{
    // Método estático que executa um passo da busca DFS
    public static void PerformSearchStep()
    {
        // Obtém referências aos componentes necessários do GameManager
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Data data = GameObject.Find("GameManager").GetComponent<Data>();
        
        // Pega o nó atual do topo da pilha (sem removê-lo)
        Node currentNode = data.stackDFS.Peek();

        // Chama o método para encontrar vizinhos do nó atual
        FindNeighbor(currentNode);
    }

    // Método estático que procura vizinhos válidos do nó atual
    static void FindNeighbor(Node currentNode)
    {
        // Obtém o conjunto de nós já visitados
        HashSet<Node> visitedNodes = GameObject.Find("GameManager").GetComponent<Data>().visitedNodes;
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        // Se o nó atual não é o nó inicial, pinta-o com material de "caminho possível"
        if(currentNode.nodeType != NodeType.Start)
            currentNode.GetComponent<Renderer>().material = currentNode.possibleWay;
            
        bool goalFound = false; // Flag para indicar se o objetivo foi encontrado
        
        // Primeiro, verifica se algum vizinho é o nó objetivo
        foreach (Node neighbor in currentNode.neighbors)
        {
            // Se encontrou o objetivo
            if (neighbor == gameManager.goal)
            {
                goalFound = true;                    // Marca que o objetivo foi encontrado
                gameManager.isSearching = false;     // Para a busca
                visitedNodes.Add(neighbor);          // Adiciona o objetivo aos visitados
                GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Push(neighbor); // Adiciona à pilha
                
                // Cria o caminho final para animação
                List<Node> finalPath = new List<Node>();
                
                // Converte a pilha para array e depois para lista mantendo ordem correta
                Node[] stackArray = GameObject.Find("GameManager").GetComponent<Data>().stackDFS.ToArray();
                
                // Adiciona na ordem inversa (do início para o fim do caminho)
                for (int i = stackArray.Length - 1; i >= 0; i--)
                {
                    finalPath.Add(stackArray[i]);
                }
                
                // Inicia a animação do caminho encontrado
                gameManager.SetFinalPath(finalPath);
                break; // Sai do loop pois encontrou o objetivo
            }
        }
        
        // Se o objetivo não foi encontrado, procura por vizinhos não visitados
        if (!goalFound)
        {
            bool neighborFound = false; // Flag para indicar se encontrou vizinho válido
            
            // Procura por vizinhos não visitados
            foreach (Node neighbor in currentNode.neighbors)
            {
                // Se o vizinho ainda não foi visitado
                if (!visitedNodes.Contains(neighbor))
                {
                    visitedNodes.Add(neighbor);     // Marca como visitado
                    GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Push(neighbor); // Adiciona à pilha
                    neighborFound = true;           // Marca que encontrou um vizinho
                    break; // Sai do loop (DFS explora apenas um caminho por vez)
                }
            }
            
            // Se não encontrou nenhum vizinho válido, faz backtrack
            if (!neighborFound)
            {
                // Remove o nó atual da pilha (backtrack)
                GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Pop();
                
                // Pinta o nó como "blacklisted" (caminho sem saída)
                currentNode.GetComponent<Renderer>().material = currentNode.blackListed;
                
                // Se a pilha ficou vazia, não há caminho possível
                if (GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Count == 0)
                {
                    gameManager.isSearching = false; // Para a busca
                }
            }
        }
    }
}