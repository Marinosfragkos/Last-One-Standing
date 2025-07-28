using UnityEngine;

public class SmoothColorLerp : MonoBehaviour
{
    public Color colorA = Color.blue;
    public Color colorB = Color.red;
    public float lerpDuration = 2f;

    private Renderer rend;
    private Material[] mats;
    private float timer = 0f;
    private bool toColorB = true;

    void Start()
    {
        rend = GetComponent<Renderer>();
        mats = rend.materials; // Κάνει copy των materials
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / lerpDuration;

        // Κάνουμε το lerp για όλα τα materials
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i].color = Color.Lerp(toColorB ? colorA : colorB, toColorB ? colorB : colorA, t);
        }

        if (t >= 1f)
        {
            timer = 0f;
            toColorB = !toColorB; // Εναλλαγή κατεύθυνσης
        }
    }
}
