/*using UnityEngine;

public enum Team
{
    Red,
    Blue
}

public class PlayerTeam : MonoBehaviour
{
    public Team team;
}
*/
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerTeam : MonoBehaviour
{
    public enum Team { Blue, Red }
    public Team team;

    private void Start()
    {
        PhotonView pv = GetComponent<PhotonView>();
        if (pv != null && pv.Owner != null)
        {
            if (pv.Owner.CustomProperties.ContainsKey("Team"))
            {
                team = (Team)(int)pv.Owner.CustomProperties["Team"];
            }
        }

        Debug.Log($"PlayerTeam Start: {pv.Owner.NickName} | Actor #{pv.Owner.ActorNumber} | Team {team}");
    }
}
