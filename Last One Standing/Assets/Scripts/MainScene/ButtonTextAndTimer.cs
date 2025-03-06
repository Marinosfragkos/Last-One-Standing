using UnityEngine;
using TMPro; // Χρειαζόμαστε την βιβλιοθήκη TextMesh Pro
using UnityEngine.UI;
using System.Collections;

public class ButtonTextAndTimer : MonoBehaviour
{
    public Button button; // Το κουμπί
    public TextMeshProUGUI buttonText; // Το κείμενο του κουμπιού, τύπου TextMeshProUGUI
    private bool isMatchmaking = false; // Έλεγχος αν είναι στο στάδιο του matchmaking
    private float timer = 0f; // Χρονόμετρο
    private bool isCounting = false; // Αν μετράει τα δευτερόλεπτα
    public float startFontSize = 70f; // Μέγεθος γραμματοσειράς για το "Start"
    public float matchmakingFontSize = 30f; // Μέγεθος γραμματοσειράς για το "Matchmaking"
    public Color startColor = new Color(0.752941f, 0.258823f, 0.1176471f,1); // Χρώμα C0421E

    



    void Start()
    {
        if (button == null) button = GetComponent<Button>(); // Αν δεν υπάρχει reference στο button, το παίρνουμε από το gameObject
        if (buttonText == null) buttonText = button.GetComponentInChildren<TextMeshProUGUI>(); // Αν δεν υπάρχει reference στο TextMeshPro, το παίρνουμε από το κουμπί
        button.onClick.AddListener(OnButtonClick); // Προσθέτουμε την συνάρτηση στο click event
        buttonText.text = "Start"; // Αρχικό κείμενο του κουμπιού
        buttonText.fontSize = startFontSize; // Αρχικό μέγεθος γραμματοσειράς
    }
    void Update()
    {
        if (isCounting)
        {
            timer += Time.deltaTime; // Αν μετράει, προσθέτουμε τον χρόνο

            // Υπολογισμός λεπτών και δευτερολέπτων
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);

            // Υπολογισμός και μορφοποίηση σε 00:00
            buttonText.text = "Matchmaking \n " + string.Format("{0:D2}:{1:D2}", minutes, seconds);

        }
    }

    // Συνάρτηση που καλείται όταν πατιέται το κουμπί
    void OnButtonClick()
    {
        if (!isMatchmaking)
        {
            // Αν δεν είναι στο matchmaking, ξεκινάμε την μέτρηση
            isMatchmaking = true;
            isCounting = true; // Ξεκινάμε την μέτρηση του χρόνου
            buttonText.text = "Matchmaking"; // Αλλάζουμε το κείμενο σε "Matchmaking"
            buttonText.fontSize = matchmakingFontSize; // Αλλάζουμε το μέγεθος γραμματοσειράς για "Matchmaking"
            button.image.color = new Color(0.416f, 0.416f, 0.416f, 0.3f); // Αλλάζουμε το χρώμα του κουμπιού σεrgba(106, 106, 106, 0.95) κατά τη διάρκεια του matchmaking
        }
        else
        {
            // Αν είναι ήδη στο matchmaking, μηδενίζουμε τον χρόνο και επιστρέφουμε στο "Start"
            isMatchmaking = false;
            isCounting = false; // Σταματάμε την μέτρηση του χρόνου
            timer = 0f; // Μηδενίζουμε τον χρόνο
             button.image.color = startColor; // Επαναφέρουμε το αρχικό χρώμα του κουμπιού
            buttonText.text = "Start"; // Επιστρέφουμε στο "Start"
            buttonText.fontSize = startFontSize; // Επαναφέρουμε το αρχικό μέγεθος γραμματοσειράς για "Start"
           
        }
    }
}
