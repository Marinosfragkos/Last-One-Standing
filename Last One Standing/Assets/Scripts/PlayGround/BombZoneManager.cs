using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

public class BombZoneManager : MonoBehaviourPun
{
    [Header("Bomb Settings")]
    public GameObject bombPrefab;       // Prefab της βόμβας (πρέπει να έχει Renderer & PhotonView)
    public Transform[] spawnPoints;     // 4 spawn points στο inspector
    public float spawnInterval = 30f;   // Κάθε 30 δευτερόλεπτα
    public float bombLifeTime = 6f;     // Η βόμβα μένει ενεργή 8 δευτερόλεπτα

    private List<Transform> availablePoints;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient) // Μόνο ο Master κάνει spawn
        {
            availablePoints = new List<Transform>(spawnPoints);
            StartCoroutine(SpawnRoutine());
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (availablePoints.Count > 0)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Διάλεξε τυχαίο spawn point από τη λίστα
            int randomIndex = Random.Range(0, availablePoints.Count);
            Transform spawnPoint = availablePoints[randomIndex];

            // Κάνε instantiate τη βόμβα για όλους τους παίκτες
            GameObject bomb = PhotonNetwork.InstantiateRoomObject(
                bombPrefab.name,
                spawnPoint.position,
                Quaternion.identity
            );

            // Αφαίρεσε το spawn point από τη λίστα για να μην ξαναχρησιμοποιηθεί
            availablePoints.RemoveAt(randomIndex);

            // Περίμενε bombLifeTime και μετά καταστρέψε τη βόμβα
            yield return new WaitForSeconds(bombLifeTime);

            if (bomb != null && bomb.GetComponent<PhotonView>() != null)
            {
                PhotonNetwork.Destroy(bomb);
            }
        }

        Debug.Log("✅ Όλα τα spawn points χρησιμοποιήθηκαν!");
    }
}
