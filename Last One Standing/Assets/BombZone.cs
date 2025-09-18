using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

public class BombZone : MonoBehaviourPun
{
    public float blinkInterval = 0.5f;   // Κάθε πόσο αναβοσβήνει
    public float activeTime = 6f;        // Πόσο μένει πριν σκάσει
    public float damage = 40f;           // Damage
    public AudioClip bombSound;
    public AudioSource audioSource;

    private Renderer rend;
    private readonly HashSet<GameObject> playersInside = new HashSet<GameObject>();

    void Start()
    {
        rend = GetComponent<Renderer>();

        if (rend == null)
        {
            Debug.LogError("⚠ Bomb has no Renderer!");
        }
        else
        {
            Debug.Log("✅ Bomb renderer found, starting routine...");
        }

        // Βλέπουμε ποιοι ήταν ήδη μέσα από την αρχή
        Collider[] hits = Physics.OverlapBox(transform.position, transform.localScale / 2);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                GameObject playerRoot = hit.transform.root.gameObject;
                playersInside.Add(playerRoot);
                Debug.Log($"👤 Player {playerRoot.name} already inside bomb zone at start!");
            }
        }

        if (audioSource != null && bombSound != null)
        {
            audioSource.clip = bombSound;
            audioSource.Play();
        }

        StartCoroutine(BombRoutine());
    }
[PunRPC]
private void DeactivateBomb()
{
    gameObject.SetActive(false);
}
    private IEnumerator BombRoutine()
    {
        float elapsed = 0f;

        while (elapsed < activeTime)
        {
            elapsed += blinkInterval;
            if (rend != null) rend.enabled = !rend.enabled;
           // Debug.Log($"⏳ Bomb ticking... elapsed={elapsed:F1}/{activeTime}");
            yield return new WaitForSeconds(blinkInterval);
        }

        // Σκάει η βόμβα
        if (rend != null) rend.enabled = true;
        Debug.Log($"💣 Bomb exploding! Players inside: {playersInside.Count}");

        if (PhotonNetwork.IsMasterClient)
        {
            foreach (GameObject player in playersInside)
            {
                if (player == null) continue;

                TargetHealth th = player.GetComponent<TargetHealth>();
                if (th != null)
                {
                    Debug.Log($"🔥 Damaging {player.name} for {damage} HP");
                    th.photonView.RPC("TakeDamageRPC", th.photonView.Owner, damage);
                }
                else
                {
                    Debug.LogWarning($"⚠ No TargetHealth on {player.name}");
                }
            }
        }

        // Περιμένει λίγο πριν εξαφανιστεί (για να ακουστεί ήχος/εφέ)
        yield return new WaitForSeconds(1f);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("🗑 Destroying bomb object...");
            //  PhotonNetwork.Destroy(gameObject);
          photonView.RPC("DeactivateBomb", RpcTarget.All);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject playerRoot = other.transform.root.gameObject;
            playersInside.Add(playerRoot);
            Debug.Log($"➡ Player {playerRoot.name} entered bomb zone!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject playerRoot = other.transform.root.gameObject;
            if (playersInside.Contains(playerRoot))
            {
                playersInside.Remove(playerRoot);
                Debug.Log($"⬅ Player {playerRoot.name} left bomb zone!");
            }
        }
    }

    //private void OnDestroy()
   // {
      //  Debug.LogError("💥 Bomb object destroyed!");
   // }
}
