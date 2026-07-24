

using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

namespace myro.teleporter
{
	public enum ETeleportationMode
	{
		Player,
		Checkpoint
	}

	[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
	public class PlayerTeleporter : UdonSharpBehaviour
	{

		public ETeleportationMode TeleportationMode;

		private DataList _registeredCheckpoints;
		private DataDictionary _playerRows;

		[Header("Event sender when the local player respawns")]
		public UdonBehaviour EventBehaviourRespawn;
		public string EventNameRespawn;

		[Header("Internal stuff, do not modify except if you know what you're doing")]
		public GameObject Content;

		private IPlayerTeleporterModule _playerTeleporterModule;

		void Start()
		{
		}

		public void PLayerTeleporterModuleSetter(IPlayerTeleporterModule playerTeleporterModule)
		{
			_playerTeleporterModule = playerTeleporterModule;
		}

		public override void OnPlayerLeft(VRCPlayerApi player)
		{
			_RemovePlayer(player);
		}

		public override void OnPlayerRespawn(VRCPlayerApi player)
		{
			if (!player.isLocal)
				return;

			PlayerRow row = _GetPlayerObjectOfPlayer(Networking.LocalPlayer);

            if (row != null && row.GetLastKnownPlayerLocationId() != 0) //if the last position was also not "spawn"
            {
				row._SetPlayerLocation(0, true);//0 = spawn
			}
		}

		public PlayerRow _GetPlayerObjectOfPlayer(VRCPlayerApi player)
		{
			if (_playerRows == null)
				return null;

			if (_playerRows.ContainsKey(player.playerId))
				return (PlayerRow) _playerRows[player.playerId].Reference;

			return null;
		}

		public void _RegisterNewPlayer(VRCPlayerApi player, PlayerRow row)
		{
			if (_playerRows == null)
			{
				_playerRows = new DataDictionary();
			}

			_playerRows[player.playerId] = row;
		}

		public void _RemovePlayer(VRCPlayerApi player)
		{
			if (_playerRows == null)
			{
				return;
			}
			_playerRows.Remove(player.playerId);
		}

		public int _RegisterCheckpoint(PlayerCheckpoint playerCheckpoint)
		{
			if (_registeredCheckpoints == null)
				_registeredCheckpoints = new DataList();

			_registeredCheckpoints.Add(playerCheckpoint);

			return _registeredCheckpoints.Count;
		}

		public PlayerCheckpoint _GetCheckpoint(int id)
		{
			//if id == 0 => Respawn
			//else => We get the checkpoint at index id-1
			if (_registeredCheckpoints == null || id <= 0 || id > _registeredCheckpoints.Count)
				return null;

			//id >= 1 and id +inf ] since 0 == respawn
			return (PlayerCheckpoint) _registeredCheckpoints[id - 1].Reference;
		}

		public void TeleportLocalPlayerToPlayer(VRCPlayerApi player)
		{
			PlayerRow playerLocator = _GetPlayerObjectOfPlayer(player);
			PlayerCheckpoint checkpoint = _GetCheckpoint(playerLocator.GetLastKnownPlayerLocationId(false));

			if (!playerLocator.CanBeReached())
				return;

			if (TeleportationMode == ETeleportationMode.Player)
			{
				if (_playerTeleporterModule != null && !_playerTeleporterModule.OnLocalPlayerTeleportToPlayer(player, checkpoint))
					return;

				ExecuteCheckpointEvent(checkpoint);

				Networking.LocalPlayer.TeleportTo(player.GetPosition(), player.GetRotation());
			}
			else
			{
				if (_playerTeleporterModule != null && !_playerTeleporterModule.OnLocalPlayerTeleportToPlayerCheckpoint(player, checkpoint))
					return;

				TeleportLocalPlayerToCheckpoint(checkpoint);
			}
		}

		public void TeleportLocalPlayerToCheckpoint(PlayerCheckpoint checkpoint)
		{
			if (checkpoint != null)
			{
				checkpoint._TeleportLocalPlayer();
			}
			else
			{
				_RespawnLocalPlayer();
			}
		}

		public void ExecuteCheckpointEvent(PlayerCheckpoint checkpoint)
		{
			if (checkpoint != null)
			{
				checkpoint._ExecuteTeleportEvents();
			}
			else
			{
				_ExecuteRespawnEvent();
			}
		}

		public void _TeleportLocalPlayerToLastKnownCheckpoint()
		{
			PlayerRow playerLocator = _GetPlayerObjectOfPlayer(Networking.LocalPlayer);
			if (playerLocator == null)
				return;

			int lastKnownLocation = playerLocator.GetPlayerLocationIdAt(1);

			if (lastKnownLocation <= 0)
				_RespawnLocalPlayer();

			PlayerCheckpoint checkpoint = _GetCheckpoint(lastKnownLocation); //We respawn the player if he didn't reached any checkpoint yet
			if (checkpoint == null)
				return;

			TeleportLocalPlayerToCheckpoint(checkpoint);
		}

		private void _RespawnLocalPlayer()
		{
			Networking.LocalPlayer.Respawn();
			
			_ExecuteRespawnEvent();
		}

		private void _ExecuteRespawnEvent()
		{
			if (EventBehaviourRespawn != null)
				EventBehaviourRespawn.SendCustomEvent(EventNameRespawn);
		}

		public void Layout()
		{
			RectTransform parent = (RectTransform)Content.transform;
			float y = 0f;

			for (int i = 0; i < parent.childCount; i++)
			{
				RectTransform child = (RectTransform)parent.GetChild(i);

				if (!child.gameObject.activeSelf)
					continue;

				float height = child.rect.height; // cache before changing anchors

				child.anchorMin = new Vector2(0f, 1f);
				child.anchorMax = new Vector2(1f, 1f);
				child.pivot = new Vector2(0f, 1f);
				child.localScale = Vector3.one;

				child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, y, height);
				child.offsetMin = new Vector2(0f, child.offsetMin.y);
				child.offsetMax = new Vector2(0f, child.offsetMax.y);
				child.localPosition = new Vector3(child.localPosition.x, child.localPosition.y, 0);
				y += height;
			}

			parent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, y);
		}

		public void OnPlayerEnteredCheckpoint(int playerLocation, int playerID)
		{
			PlayerCheckpoint checkpoint = _GetCheckpoint(playerLocation);
			VRCPlayerApi player = VRCPlayerApi.GetPlayerById(playerID);
			if (checkpoint != null && player != null && _playerTeleporterModule != null)
			{
				_playerTeleporterModule.OnPlayerEnteredCheckpoint(player, checkpoint);
			}
		}
	}
}
