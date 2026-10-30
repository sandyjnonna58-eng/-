using UnityEngine;

namespace CombatSandbox.Demo;

public sealed partial class CombatBrain
{
	private Vehicle armourTarget;

	private float armourCheckAt;

	private float atgmTryAt;

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
		if ((bool)armourTarget && armourTarget.Armoured && v.AtgmReady && Time.time >= atgmTryAt && (!gun.Autocannon || armourTarget.ArmourRating >= gun.ApEnergy))
		{
			atgmTryAt = Time.time + UnityEngine.Random.Range(1.5f, 3f);
			Vehicle aimed = armourTarget;
			Vector3 worldCenterOfMass = aimed.Body.worldCenterOfMass;
			float magnitude = (worldCenterOfMass - gun.SightAt).magnitude;
			if (magnitude > 100f && magnitude < 2200f && ClearOfFriends(gun.SightAt, (worldCenterOfMass - gun.SightAt).normalized, magnitude))
			{
				v.FireAtgm(target, () => ((bool)aimed && !aimed.Wrecked && (bool)aimed.Body) ? new Vector3?(aimed.Body.worldCenterOfMass + Vector3.up * 0.2f) : null, worldCenterOfMass);
			}
		}
		if (gun.Coax != null)
		{
			gun.FireCoax = engage && !flag2 && !flag && (point - gun.SightAt).magnitude < 450f && Mathf.Repeat(Time.time + (float)(GetInstanceID() % 7) * 0.37f, 3f) < 1.3f;
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
			if (gun.Autocannon && item.ArmourRating >= gun.ApEnergy && !v.AtgmReady)
			{
				num2 += 900f;
			}
			if (num2 < num)
			{
				num = num2;
				result = item;
			}
		}
		return result;
	}
}
