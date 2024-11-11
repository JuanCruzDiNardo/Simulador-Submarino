using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public GameObject fishPrefab; // Prefab del pez que quieres instanciar
    public int fishCount = 10; // Cantidad de peces a spawnear
    public Vector3 spawnAreaSize = new Vector3(20f, 10f, 20f); // Tamaño del área de spawn

    void Start()
    {
        SpawnFish();
    }

    void SpawnFish()
    {
        for (int i = 0; i < fishCount; i++)
        {
            // Genera una posición aleatoria dentro del área de spawn
            Vector3 randomPosition = new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
                Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            );

            // Instancia el pez en la posición aleatoria
            GameObject newFish = Instantiate(fishPrefab, transform.position + randomPosition, Quaternion.identity);
            newFish.transform.parent = transform; // Asigna el pez como hijo del spawner para organización
        }
    }

    // Dibuja el área de spawn en el editor para visualizar
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}
