using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

public class BombZone : MonoBehaviourPun
{
    public float blinkInterval = 0.5f;   // Αναβοσβήνει κάθε μισό δευτερόλεπτο
    public float activeTime = 6f;        // Πόσο μένει ενεργή πριν "σκάσει"
    public float soundLifeTime = 8f;     // Πόσο θα συνεχίσει ο ήχος
    public float damage = 30f;           // Damage
    public AudioClip bombSound;
    public AudioSource audioSource;

    private Renderer rend;
    private readonly HashSet<GameObject> playersInside = new HashSet<GameObject>();

    void Start()
{
    rend = GetComponent<Renderer>();

    // Ελέγχουμε ποιοι παίκτες είναι ήδη μέσα στην ζώνη
    Collider[] hits = Physics.OverlapBox(transform.position, transform.localScale / 2);
    foreach (Collider hit in hits)
    {
        if (hit.CompareTag("Player"))
        {
            GameObject playerRoot = hit.transform.root.gameObject;
            playersInside.Add(playerRoot);
            Debug.Log($"Player {playerRoot.name} was already inside bomb zone at start!");
        }
    }

    if (audioSource != null && bombSound != null)
    {
        audioSource.clip = bombSound;
        audioSource.Play();
    }

    if (rend != null)
        StartCoroutine(BombRoutine());
}


    private IEnumerator BombRoutine()
    {
        float elapsed = 0f;

        while (elapsed < activeTime)
        {
            elapsed += blinkInterval;
            rend.enabled = !rend.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }

        // Σκάει
        rend.enabled = true;

        if (PhotonNetwork.IsMasterClient)
        {
            foreach (GameObject player in playersInside)
            {
                if (player == null) continue;

                // Παίρνουμε το TargetHealth από το player root
                TargetHealth th = player.GetComponent<TargetHealth>();
                if (th != null)
                {
                    th.photonView.RPC("TakeDamageRPC", RpcTarget.All, damage);
                }
            }
        }

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Παίρνουμε το root του player
            GameObject playerRoot = other.transform.root.gameObject;
            playersInside.Add(playerRoot);
            Debug.Log($"Player {playerRoot.name} entered bomb zone!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject playerRoot = other.transform.root.gameObject;
            if (playersInside.Contains(playerRoot))
                playersInside.Remove(playerRoot);
        }
    }
}
