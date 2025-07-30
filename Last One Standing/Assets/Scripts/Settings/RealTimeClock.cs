using UnityEngine;
using TMPro;
using System;

public class RealTimeClock : MonoBehaviour
{
    public TextMeshProUGUI timeText;

    void Update()
    {
        DateTime now = DateTime.Now;
        string formattedTime = now.ToString("dd/MM/yyyy - HH:mm:ss");
        timeText.text = formattedTime;
    }
}
