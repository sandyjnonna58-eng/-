using UnityEngine;

namespace CombatSandbox.Demo;

public sealed partial class CombatBrain
{
	private Vehicle armourTarget;

	private float armourCheckAt;

	private void CannonTarget(Vehicle v, GunMount gun, ref Vector3 point, ref bool engage)
	{
		if (Time.time >= armourCheckAt)
		{
			armourCheckAt = Time.time + UnityEngine.Random.Range(0.8f, 1.3f);
			armourTarget = FindArmour(v, gun);
		}
		if ((bool)armourTarget && (armourTarget.Wrecked || !armourTarget.Body))
		{
			armourTarget = null;
		}
		bool flag = engage && (bool)enemy && (bool)enemy.target.Vehicle && enemy.target.Vehicle.Armoured;
		bool flag2 = false;
		if ((bool)armourTarget && (!engage || armourTarget.Armoured))
		{
			point = armourTarget.Body.worldCenterOfMass;
			engage = true;
			flag2 = armourTarget.Armoured;
		}
		int num = ((flag2 || flag) ? 1 : 0);
		if (gun.Shell != num && !gun.Reloading)
		{
			gun.PendingShell = num;
		}
	}

	private static Vehicle FindArmour(Vehicle v, GunMount gun)
	{
		Vector3 sightAt = gun.SightAt;
		Vehicle result = null;
		float num = float.MaxValue;
		foreach (Vehicle item in Vehicle.All)
		{
			if (!item || item == v || item.Wrecked || !item.Body || item.Team < 0 || item.Team == v.Team)
			{
				continue;
			}
			bool flag = false;
			ArenaTarget[] riders = item.Riders;
			foreach (ArenaTarget arenaTarget in riders)
			{
				if ((bool)arenaTarget && !arenaTarget.Body.Dead)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				continue;
			}
			Vector3 worldCenterOfMass = item.Body.worldCenterOfMass;
			Vector3 vector = worldCenterOfMass - sightAt;
			float magnitude = vector.magnitude;
			if (magnitude > 1400f || magnitude < 8f)
			{
				continue;
			}
			gun.Off(worldCenterOfMass, out var inArc);
			if (!inArc)
			{
				continue;
			}
			if (Physics.Raycast(sightAt, vector / magnitude, out var hitInfo, magnitude, -261, QueryTriggerInteraction.Ignore) && !item.Holds(hitInfo.collider) && !v.Holds(hitInfo.collider))
			{
				continue;
			}
			float num2 = magnitude - ((item.Gun != null && item.Gun.Cannon) ? 600f : 0f) - (item.Armoured ? 200f : 0f);
			if (num2 < num)
			{
				num = num2;
				result = item;
			}
		}
		return result;
	}
}
