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

public class PlayerTeam : MonoBehaviourPunCallbacks
{
    public enum Team { Blue, Red }
    public Team team;

    private void Start()
    {
        UpdateTeamFromProperties();
    }

    void UpdateTeamFromProperties()
    {
        PhotonView pv = GetComponent<PhotonView>();
        if (pv != null && pv.Owner != null && pv.Owner.CustomProperties.ContainsKey("Team"))
        {
            team = (Team)(int)pv.Owner.CustomProperties["Team"];
        }

        Debug.Log($"PlayerTeam Start/Update: {pv.Owner.NickName} | Actor #{pv.Owner.ActorNumber} | Team {team}");
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        PhotonView pv = GetComponent<PhotonView>();
        if (pv != null && pv.Owner != null && pv.Owner == targetPlayer && changedProps.ContainsKey("Team"))
        {
            team = (Team)(int)changedProps["Team"];
            Debug.Log($"PlayerTeam Updated: {pv.Owner.NickName} | Actor #{pv.Owner.ActorNumber} | Team {team}");
        }
    }
}

