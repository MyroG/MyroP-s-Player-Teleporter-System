using System;
using System.Collections;
using System.Collections.Generic;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace myro.teleporter
{
	public abstract class IPlayerTeleporterModule : UdonSharpBehaviour
	{
		public PlayerTeleporter PlayerTeleporter;

		private void Start()
		{
			PlayerTeleporter.PLayerTeleporterModuleSetter(this);
			OnStart();
		}

		public virtual void OnStart()
		{

		}

		public virtual bool OnLocalPlayerTeleportToPlayer(VRCPlayerApi playerToTeleportTo, PlayerCheckpoint checkpoint)
		{
			return true;
		}

		public virtual bool OnLocalPlayerTeleportToPlayerCheckpoint(VRCPlayerApi playerToTeleportTo, PlayerCheckpoint checkpointToTeleportTo)
		{
			return true;
		}

		public virtual void OnPlayerEnteredCheckpoint(VRCPlayerApi player, PlayerCheckpoint checkpoint)
		{
		}
	}
}
