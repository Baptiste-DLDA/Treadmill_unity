using UnityEngine;
using System.Collections;

public class NPCSpawner : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject npcPrefab; // Le modèle du personnage à faire apparaître
    public float intervalle = 10f; // Temps en secondes entre chaque apparition

    [Header("Le Parcours")]
    public Transform pointDeDepart; // Point A
    public Transform pointIntermediaire; // Point B (Rayon)
    public Transform pointFinal; // Point C (Sortie)

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true) // Boucle infinie
        {
            SpawnNPC();
            yield return new WaitForSeconds(intervalle);
        }
    }

    void SpawnNPC()
    {
        // 1. On crée le personnage au point de départ
        GameObject nouveauPerso = Instantiate(npcPrefab, pointDeDepart.position, pointDeDepart.rotation);

        // 2. On récupère son script pour lui donner les destinations
        NPCSequence script = nouveauPerso.GetComponent<NPCSequence>();

        if (script != null)
        {
            script.destinationB = pointIntermediaire;
            script.destinationC = pointFinal;
        }
    }
}