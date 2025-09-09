using UnityEngine;
using Photon.Pun;
using System.Collections;

public class BombZoneManager : MonoBehaviourPun
{
    public GameObject[] bombZonePrefabs; // τα 4 prefabs σου
    public float spawnInterval = 150;   // κάθε πόσα δευτερόλεπτα

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient) // μόνο ο MasterClient κάνει spawn
        {
            StartCoroutine(SpawnZonesRoutine());
        }
    }

    private IEnumerator SpawnZonesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            SpawnRandomZone();
        }
    }

    private void SpawnRandomZone()
    {
        if (bombZonePrefabs.Length == 0) return;

        // Διάλεξε τυχαίο prefab
        int index = Random.Range(0, bombZonePrefabs.Length);

        // Θέση spawn – μπορείς να βάλεις δικές σου συντεταγμένες ή random
        Vector3 spawnPos = GetRandomPosition();

        // Spawn μέσω Photon
        PhotonNetwork.InstantiateRoomObject(bombZonePrefabs[index].name, spawnPos, Quaternion.identity);
    }

    private Vector3 GetRandomPosition()
    {
        // Βάλε τα δικά σου όρια χάρτη εδώ
        float x = Random.Range(-20f, 20f);
        float z = Random.Range(-20f, 20f);
        float y = 0f; // έδαφος

        return new Vector3(x, y, z);
    }
}
