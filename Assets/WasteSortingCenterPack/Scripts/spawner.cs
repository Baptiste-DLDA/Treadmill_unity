using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Les crochets [] signifient que c'est une liste d'objets
    public GameObject[] objectsToSpawn;

    public float timeBetweenSpawns = 1.0f;
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timeBetweenSpawns)
        {
            SpawnRandomObject();
            timer = 0f;
        }
    }

    void SpawnRandomObject()
    {
        // Sécurité : si la liste est vide, on ne fait rien
        if (objectsToSpawn.Length == 0) return;

        // On choisit un nombre au hasard entre 0 et le nombre total d'objets dans ta liste
        int randomIndex = Random.Range(0, objectsToSpawn.Length);

        // On fait apparaître l'objet correspondant au numéro tiré au sort
        Instantiate(objectsToSpawn[randomIndex], transform.position, transform.rotation);
    }
}