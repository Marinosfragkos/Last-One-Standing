using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class RandomObjectLooper : MonoBehaviourPunCallbacks
{
    public List<GameObject> objects = new List<GameObject>();
    public float displayTime = 20f;

    private List<GameObject> availableObjects = new List<GameObject>();
    private GameObject currentActive;

    void Start()
    {
        foreach (GameObject obj in objects)
            if (obj != null) obj.SetActive(false);

        availableObjects = new List<GameObject>(objects);

        if (PhotonNetwork.IsMasterClient) // ✅ μόνο ο master ξεκινάει τον κύκλο
            StartCoroutine(ObjectCycle());
    }

    private IEnumerator ObjectCycle()
    {
        while (true)
        {
            if (availableObjects.Count == 0)
                availableObjects = new List<GameObject>(objects);

            int index = Random.Range(0, availableObjects.Count);
            GameObject selected = availableObjects[index];
            availableObjects.RemoveAt(index);

            int selectedIndex = objects.IndexOf(selected);

            // Στέλνουμε RPC σε όλους να εμφανίσουν αυτό το αντικείμενο
            photonView.RPC("ShowObjectRPC", RpcTarget.All, selectedIndex);

            yield return new WaitForSeconds(displayTime);

            // Στέλνουμε RPC να κλείσουν το αντικείμενο
            photonView.RPC("HideObjectRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    void ShowObjectRPC(int index)
    {
        if (currentActive != null)
            currentActive.SetActive(false);

        if (index >= 0 && index < objects.Count && objects[index] != null)
        {
            currentActive = objects[index];
            currentActive.SetActive(true);
        }
    }

    [PunRPC]
    void HideObjectRPC()
    {
        if (currentActive != null)
        {
            currentActive.SetActive(false);
            currentActive = null;
        }
    }
}

