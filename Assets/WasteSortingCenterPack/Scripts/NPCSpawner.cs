using UnityEngine;
using System.Collections;

public class NPCSpawner : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject npcPrefab;
    public float intervalle = 10f;

    [Header("Le Parcours")]
    public Transform pointDeDepart;
    public Transform pointIntermediaire;
    public Transform pointFinal;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnNPC();
            yield return new WaitForSeconds(intervalle);
        }
    }

    void SpawnNPC()
    {
        GameObject nouveauPerso = Instantiate(npcPrefab, pointDeDepart.position, pointDeDepart.rotation);
        NPCSequence script = nouveauPerso.GetComponent<NPCSequence>();
        if (script != null)
        {
            script.destinationB = pointIntermediaire;
            script.destinationC = pointFinal;
            script.StartWalking();
        }
    }
}