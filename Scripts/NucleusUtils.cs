using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDKBase;

public static class NucleusUtils
{
	public static Donut Find(VRCPlayerApi player)
	{
		var objects = Networking.GetPlayerObjects(player);
		for (int i = 0; i < objects.Length; i++)
		{
			if (!Utilities.IsValid(objects[i])) continue;
			Donut foundScript = objects[i].GetComponent<Donut>();
			if (Utilities.IsValid(foundScript)) return foundScript;
		}
		return null;
	}

	public static MetroChair FindChair(VRCPlayerApi player)
	{
		var objects = Networking.GetPlayerObjects(player);
		for (int i = 0; i < objects.Length; i++)
		{
			if (!Utilities.IsValid(objects[i])) continue;
			MetroChair foundScript = objects[i].GetComponent<MetroChair>();
			if (Utilities.IsValid(foundScript)) return foundScript;
		}
		return null;
	}
}
