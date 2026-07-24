
using myro.teleporter;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TeleportToLastCheckpointOnJoin : UdonSharpBehaviour
{
	public PlayerTeleporter Teleporter;

    void Start()
    {
        
    }

	public override void OnPlayerRestored(VRCPlayerApi player)
	{
		if (!player.isLocal)
		{
			return;
		}

		Teleporter._TeleportLocalPlayerToLastKnownCheckpoint();
	}
}
