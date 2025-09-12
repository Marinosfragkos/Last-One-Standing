using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomObjectLooper : MonoBehaviour
{
    public List<GameObject> objects = new List<GameObject>(); // Τα 5 αντικείμενα
    public float displayTime = 20f; // Χρόνος σε δευτερόλεπτα

    private List<GameObject> availableObjects = new List<GameObject>();

    void Start()
    {
        // Σιγουρευόμαστε ότι όλα ξεκινούν απενεργοποιημένα
        foreach (GameObject obj in objects)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        // Αντικείμενα που είναι διαθέσιμα για εμφάνιση
        availableObjects = new List<GameObject>(objects);

        // Ξεκινάμε τον κύκλο
        StartCoroutine(ObjectCycle());
    }

    private IEnumerator ObjectCycle()
    {
        while (true)
        {
            // Αν δεν υπάρχουν διαθέσιμα αντικείμενα, επαναφέρουμε τη λίστα
            if (availableObjects.Count == 0)
                availableObjects = new List<GameObject>(objects);

            // Επιλέγουμε τυχαίο αντικείμενο από τα διαθέσιμα
            int index = Random.Range(0, availableObjects.Count);
            GameObject selected = availableObjects[index];
            availableObjects.RemoveAt(index);

            // Ενεργοποιούμε το αντικείμενο
            if (selected != null)
                selected.SetActive(true);

            // Περιμένουμε
            yield return new WaitForSeconds(displayTime);

            // Απενεργοποιούμε το αντικείμενο
            if (selected != null)
                selected.SetActive(false);
        }
    }
}
