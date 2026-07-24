
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace myro.teleporter
{
	[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
	public class PlayerReachableSetter : UdonSharpBehaviour
	{
		public PlayerTeleporter PlayerTeleporterReference;
		public bool PlayerReachable = true;

		public override void OnPlayerTriggerEnter(VRCPlayerApi player)
		{
			if (!player.isLocal)
			{
				return;
			}

			PlayerRow po = PlayerTeleporterReference._GetPlayerObjectOfPlayer(player);
			if (po != null)
			{
				po.SetCanBeReached(PlayerReachable);
			}
		}

		public override void OnPlayerRespawn(VRCPlayerApi player)
		{
			if (!player.isLocal)
			{
				return;
			}

			PlayerRow po = PlayerTeleporterReference._GetPlayerObjectOfPlayer(player);
			if (po != null)
			{
				po.SetCanBeReached(true);
			}
		}
	}
}