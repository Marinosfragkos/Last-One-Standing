using System.Collections;
using UnityEngine;

public class ZoneSequenceManager : MonoBehaviour
{
    public ZoneTrigger[] zones;

    [Header("Audio Settings")]
    public AudioClip countdownTickSound9;
    public AudioClip countdownTickSound5;
    public AudioClip countdownEndSoundBase1;
    public AudioClip countdownEndSoundBase2;
    public AudioClip countdownEndSoundBase3;
    public AudioSource audioSource;

    private void Start()
    {
        foreach (var zone in zones)
            zone.SetActive(false); // Απενεργοποίηση όλων στην αρχή

        StartCoroutine(ActivateZonesSequentially());
    }

    private IEnumerator ActivateZonesSequentially()
    {
        ZoneTrigger firstZone = zones[0];

        // Αντίστροφη μέτρηση 10 δευτερόλεπτα για την πρώτη βάση
        for (int i = 10; i > 0; i--)
        {
            if (firstZone != null)
                firstZone.UpdateBaseNameUI(i.ToString());

            if (audioSource != null)
            {
                if (i == 9 && countdownTickSound9 != null)
                    audioSource.PlayOneShot(countdownTickSound9);
                else if (i == 4 && countdownTickSound5 != null)
                    audioSource.PlayOneShot(countdownTickSound5);
            }

            yield return new WaitForSeconds(1f);
        }

        // Παίξε ήχο τέλους αντίστροφης μέτρησης για Βάση 1
        if (audioSource != null && countdownEndSoundBase1 != null)
            audioSource.PlayOneShot(countdownEndSoundBase1);

        // Ενεργοποίηση βάσεων διαδοχικά
        for (int i = 0; i < zones.Length; i++)
        {
            ZoneTrigger currentZone = zones[i];

            currentZone.ResetZone();
            currentZone.SetActive(true);

            yield return new WaitUntil(() => currentZone.IsComplete);

            currentZone.SetActive(false);

            // Μετά από 10 δευτερόλεπτα πριν την επόμενη βάση
            if (i + 1 < zones.Length)
            {
                yield return new WaitForSeconds(10f);

                if (audioSource != null)
                {
                    if (i == 0 && countdownEndSoundBase2 != null)
                        audioSource.PlayOneShot(countdownEndSoundBase2);
                    else if (i == 1 && countdownEndSoundBase3 != null)
                        audioSource.PlayOneShot(countdownEndSoundBase3);
                }
            }
        }
    }
}

