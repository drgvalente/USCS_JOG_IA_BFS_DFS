using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    static List<GameObject> nodes = new List<GameObject>();

    public static void GenerateGrid(int h, int w, int wall, GameObject node)
    {        
        for (int i = 0; i < w; i++)
        {
            for(int j = 0; j < h; j++)
            {
                // Encontrar o GameManager na cena
                GameManager gameManager = FindFirstObjectByType<GameManager>();
                GameObject n = Instantiate(node, new Vector3(i - w/2, 0, j - h/2), Quaternion.identity);
                gameManager.nodes.Add(n);
                nodes.Add(n);
            }            
        }
        RandomizeWalls(wall);
        SetupNeighbors(); // Configura os vizinhos após criar todos os nós
    }

    // Método para configurar os vizinhos de cada nó
    public static void SetupNeighbors()
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        
        foreach (GameObject nodeObj in gameManager.nodes)
        {
            Node currentNode = nodeObj.GetComponent<Node>();
            Vector3 currentPos = nodeObj.transform.position;
            
            // Limpa a lista de vizinhos antes de popular
            currentNode.neighbors.Clear();
            
            // Verifica as 4 direções (cima, baixo, esquerda, direita)
            Vector3[] directions = {
                Vector3.forward,  // Cima (z+1)
                Vector3.back,     // Baixo (z-1)
                Vector3.left,     // Esquerda (x-1)
                Vector3.right     // Direita (x+1)
            };
            
            foreach (Vector3 direction in directions)
            {
                Vector3 neighborPos = currentPos + direction;
                
                // Procura por um nó na posição do vizinho
                foreach (GameObject otherNodeObj in gameManager.nodes)
                {
                    if (Vector3.Distance(otherNodeObj.transform.position, neighborPos) < 0.1f)
                    {
                        Node neighborNode = otherNodeObj.GetComponent<Node>();
                        currentNode.neighbors.Add(neighborNode);
                        break; // Encontrou o vizinho, não precisa continuar procurando
                    }
                }
            }
        }
    }

    public static void RandomizeWalls(int n)
    {
        for (int i = 0; i < n; i++)
        {
            int index = Random.Range(0, nodes.Count);
            nodes[index].transform.Translate(Vector3.up);
            nodes[index].GetComponent<Renderer>().material = 
                nodes[index].GetComponent<Node>().wall;
            nodes[index].GetComponent<Node>().nodeType = NodeType.Wall;
            nodes.Remove(nodes[index]);
        }
    }
}
