
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace myro.teleporter
{
	[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
	public class TeteportToCheckpoint : UdonSharpBehaviour
	{
		public PlayerCheckpoint Checkpoint;

		void Start()
		{

		}

		public override void Interact()
		{
			if (Checkpoint != null)
			{
				Checkpoint._TeleportLocalPlayerForce();
			}
		}
	}
}
