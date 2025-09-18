using UnityEngine;
using TMPro;
using Photon.Pun;
using System.Collections;

public class ReviveInteractor : MonoBehaviourPun
{
    public KeyCode reviveKey = KeyCode.B;
    public float reviveDuration = 4f;
    public float maxAllowedMove = 0.05f;

    [Header("UI")]
    public UnityEngine.UI.RawImage reviveCross;
    public TMP_Text reviveCountdownText;

    TargetHealth myHealth;
    PlayerTeam myTeam;

    TargetHealth currentTarget;       // downed στόχος μέσα στο panel
    Coroutine reviveCoroutine;

    void Start()
    {
        myHealth = GetComponent<TargetHealth>();
        myTeam = GetComponent<PlayerTeam>();

        if (reviveCross != null) reviveCross.enabled = false;
        if (reviveCountdownText != null) reviveCountdownText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        bool shouldShow = currentTarget != null;
        if (reviveCross != null) reviveCross.enabled = shouldShow;

        if (shouldShow && Input.GetKeyDown(reviveKey) && reviveCoroutine == null)
        {
            reviveCoroutine = StartCoroutine(DoReviveCoroutine(currentTarget));
        }

        if (!shouldShow && reviveCoroutine != null)
        {
            StopCoroutine(reviveCoroutine);
            reviveCoroutine = null;
            if (reviveCountdownText != null) reviveCountdownText.gameObject.SetActive(false);
            Debug.Log($"[{photonView.OwnerActorNr}] Revive cancelled: target lost.");
        }
    }

    public void SetCurrentTarget(TargetHealth target)
    {
        if (target != null && target.isDown)
        {
            currentTarget = target;
            Debug.Log($"[{photonView.OwnerActorNr}] Can revive player {target.photonView.OwnerActorNr}");
        }
    }

    public void ClearCurrentTarget(TargetHealth target)
    {
        if (currentTarget == target)
            currentTarget = null;
    }

    IEnumerator DoReviveCoroutine(TargetHealth target)
    {
        Vector3 startPos = transform.position;
        if (reviveCountdownText != null) reviveCountdownText.gameObject.SetActive(true);

        float remaining = reviveDuration;
        Debug.Log($"[{photonView.OwnerActorNr}] Starting revive on {target.photonView.OwnerActorNr}");

        while (remaining > 0f)
        {
            if (target == null || !target.isDown)
            {
                Debug.Log($"[{photonView.OwnerActorNr}] Revive cancelled: target not down.");
                break;
            }

            if (Vector3.Distance(startPos, transform.position) > maxAllowedMove)
            {
                Debug.Log($"[{photonView.OwnerActorNr}] Revive cancelled: moved.");
                break;
            }

            remaining -= Time.deltaTime;

            if (reviveCountdownText != null)
                reviveCountdownText.text = $"{remaining:F1}s";

            yield return null;
        }

        if (remaining <= 0f && target != null && target.isDown)
        {
            Debug.Log($"[{photonView.OwnerActorNr}] Revive COMPLETE -> calling ReviveRPC on {target.photonView.OwnerActorNr}");
            target.Revive(false); // ή true για full health
        }

        if (reviveCountdownText != null) reviveCountdownText.gameObject.SetActive(false);
        reviveCoroutine = null;
    }
}
