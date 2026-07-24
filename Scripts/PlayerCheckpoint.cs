
using Cysharp.Threading.Tasks.Triggers;
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace myro.teleporter
{
	[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
	public class PlayerCheckpoint : UdonSharpBehaviour
	{
		public PlayerTeleporter PlayerTeleporterReference;
		public string Name;
		[SerializeField]
		private Transform TeleportLocation;

		public bool CanTeleportHere = true;

		[Header("Event sender when the local player enters the checkpoint")]
		public UdonBehaviour EventBehaviourCheckpointEntered;
		public string EventNameCheckpointEntered;

		[Header("Event sender when the local player teleports to that location")]
		public UdonBehaviour[] EventBehaviours;
		public string[] EventNames;

		[Header("Event sender when the local player leaves the checkpoint")]
		public UdonBehaviour EventBehaviourCheckpointExited;
		public string EventNameCheckpointExited;

		private int _id;

		void Start()
		{
			_id = PlayerTeleporterReference._RegisterCheckpoint(this);
		}

		public override void OnPlayerTriggerEnter(VRCPlayerApi player)
		{
			if (!player.isLocal)
			{
				return;
			}

			if (EventBehaviourCheckpointEntered != null)
			{
				EventBehaviourCheckpointEntered.SendCustomEvent(EventNameCheckpointEntered);
			}

			PlayerRow playerRow = PlayerTeleporterReference._GetPlayerObjectOfPlayer(player);
			playerRow._SetPlayerLocation(_id, CanTeleportHere);
		}

		public override void OnPlayerTriggerExit(VRCPlayerApi player)
		{
			if (!player.isLocal)
			{
				return;
			}

			if (EventBehaviourCheckpointExited != null)
			{
				EventBehaviourCheckpointExited.SendCustomEvent(EventNameCheckpointExited);
			}
		}

		public Transform GetSpawn()
		{
			if (TeleportLocation == null)
				return transform;

			return TeleportLocation;
		}

		public void _TeleportLocalPlayer()
		{
			_ExecuteTeleportEvents();

			if (!CanTeleportHere)
				return;

			Networking.LocalPlayer.TeleportTo(GetSpawn().position, GetSpawn().rotation);
		}

		public void _TeleportLocalPlayerForce()
		{
			_ExecuteTeleportEvents();

			Networking.LocalPlayer.TeleportTo(GetSpawn().position, GetSpawn().rotation);
		}

		public void _ExecuteTeleportEvents()
		{
			if (EventBehaviourCheckpointEntered != null)
			{
				EventBehaviourCheckpointEntered.SendCustomEvent(EventNameCheckpointEntered);
			}
			for (int i = 0; i < EventBehaviours.Length; i++)
			{
				if (EventBehaviours[i] != null && i < EventNames.Length)
				{
					EventBehaviours[i].SendCustomEvent(EventNames[i]);
				}
			}
		}
	}
}
