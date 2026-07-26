
using System;
using System.Linq;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Data;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using static UnityEngine.Rendering.DebugUI;

namespace myro.teleporter
{
	[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
	public class PlayerRow : UdonSharpBehaviour
	{
		[UdonSynced]
		private int[] _playerLocationHistory = new int[0];

		[UdonSynced]
		private int[] _playerLocationHistoryAll = new int[0];

		[UdonSynced]
		private bool _canBeReached = true;

		public PlayerTeleporter PlayerTeleporterReference;
		public bool ShowPlayerLocation = false;

		[Header("Internal stuff, do not modify except if you know what you're doing")]
		public TextMeshProUGUI PlayerText;
		public TextMeshProUGUI PlayerLocation;
		public UnityEngine.UI.Button TeleportButton;

		public GameObject PlayerRowRoot;

		private VRCPlayerApi _owner;

		private const int HISTORY_LENGTH = 8;

		void Start()
		{
			_owner = Networking.GetOwner(gameObject);
			PlayerText.text = _owner.displayName;
			PlayerLocation.text = "";
			PlayerTeleporterReference._RegisterNewPlayer(_owner, this);

			//Reparenting
			PlayerRowRoot.transform.parent = PlayerTeleporterReference.Content.transform;

			PlayerTeleporterReference.Layout();

			SetButtonEnableState();
			UpdatePlayerLocation();
		}

		private void OnDestroy()
		{
			DestroyImmediate(PlayerRowRoot);
			PlayerTeleporterReference.Layout();
		}

		private void SetButtonEnableState()
		{
			TeleportButton.interactable = !_owner.isLocal
				&& _canBeReached;
		}

		private void UpdatePlayerLocation()
		{
			if (!ShowPlayerLocation)
				return;

			int id = GetLastKnownPlayerLocationId(false);

			if (id == 0)
			{
				PlayerLocation.text = PlayerTeleporterReference.SpawnName;
			}
			else
			{
				PlayerCheckpoint pc = PlayerTeleporterReference._GetCheckpoint(id);
				if (pc != null)
				{
					PlayerLocation.text = pc.Name;
				}
			}
		}


		public void OnClick()
		{
			PlayerTeleporterReference.TeleportLocalPlayerToPlayer(_owner);
		}

		

		public int GetLastKnownPlayerLocationId(bool onlyValidTeleportLocation = true)
		{
			int[] arrayToUse = onlyValidTeleportLocation ? _playerLocationHistory : _playerLocationHistoryAll;

			if (arrayToUse == null || arrayToUse.Length == 0)
				return -1;
			return arrayToUse[0];
		}

		public PlayerCheckpoint GetLastKnownPlayerLocation(bool onlyValidTeleportLocation = true)
		{
			int id = GetLastKnownPlayerLocationId(onlyValidTeleportLocation);

			return PlayerTeleporterReference._GetCheckpoint(id);
		}

		public int GetPlayerLocationIdAt(int index, bool onlyValidTeleportLocation = true)
		{
			int[] arrayToUse = onlyValidTeleportLocation ? _playerLocationHistory : _playerLocationHistoryAll;

			if (arrayToUse == null || arrayToUse.Length == 0)
				return -1;

			index = Mathf.Clamp(index, 0, arrayToUse.Length - 1);
			return arrayToUse[index];
		}

		private static int[] PrependToArray(int[] array, int item, int maxLength)
		{
			int newLength = Math.Min(array.Length + 1, maxLength);
			int[] result = new int[newLength];

			result[0] = item;
			Array.Copy(array, 0, result, 1, newLength - 1);

			return result;
		}

		public void _SetPlayerLocation(int playerLocation, bool canTeleportToLocation)
		{
			if (!Networking.IsOwner(gameObject))
				return;

			if (canTeleportToLocation)
			{
				_playerLocationHistory = PrependToArray(_playerLocationHistory, playerLocation, HISTORY_LENGTH);
			}
			_playerLocationHistoryAll = PrependToArray(_playerLocationHistoryAll, playerLocation, HISTORY_LENGTH);

			RequestSerialization();
			OnDeserialization();

			SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(OnPlayerEnteredCheckpoint), playerLocation, Networking.LocalPlayer.playerId);

		}

		[NetworkCallable]
		public void OnPlayerEnteredCheckpoint(int playerLocation, int playerID)
		{
			PlayerTeleporterReference.OnPlayerEnteredCheckpoint(playerLocation, playerID);
		}

		public override void OnDeserialization()
		{
			SetButtonEnableState();
			UpdatePlayerLocation();
		}

		public void SetCanBeReached(bool newReachState)
		{
			_canBeReached = newReachState;
			RequestSerialization();
		}

		public bool CanBeReached()
		{
			return _canBeReached;
		}
	}
}
