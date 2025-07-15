using UnityEngine;

public class FullScreenSwitch : MonoBehaviour
{
    public GameObject switchOn;
    public GameObject switchOff;

    private bool isOn = true;

    public void Toggle()
    {
        isOn = !isOn;
        switchOn.SetActive(isOn);
        switchOff.SetActive(!isOn);
    }
}
