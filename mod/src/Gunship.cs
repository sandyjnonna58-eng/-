using UnityEngine;

namespace CombatSandbox.Demo;

public abstract class Gunship : Helicopter
{
	private int rockets = -1;

	private int pod;

	private int salvo;

	private float nextRocket;

	private float salvoPause;

	private float rearmAt;

	private float lookAt;

	private bool hasStrike;

	private Vector3 strike;

	private Vehicle strikeVehicle;

	protected abstract Vector3[] Pods { get; }

	protected abstract int RocketLoad { get; }

	protected override float CruiseSpeed => 30f;

	protected override string PilotHelp => "Gunship: W / S - forward / back, A / D - turn, Space - up, Ctrl - down, LMB - rockets, M - ATGM";

	public int Rockets => Mathf.Max(0, rockets);

	public Vector3 NosePoint()
	{
		Vector3 position = base.transform.TransformPoint(new Vector3(0f, 1.2f, 8f));
		Vector3 vector = base.Rotation * Quaternion.Euler(4f, 0f, 0f) * Vector3.forward;
		if (Physics.Raycast(position, vector, out var hitInfo, 2500f, -261, QueryTriggerInteraction.Ignore) && !Holds(hitInfo.collider))
		{
			return hitInfo.point;
		}
		return position + vector * 2500f;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (rockets < 0)
		{
			rockets = RocketLoad;
		}
		if (rockets < RocketLoad && Time.time >= rearmAt && (bool)base.Body && !base.Wrecked)
		{
			rearmAt = Time.time + 1.2f;
			Vector3 position = base.Body.position;
			if (base.Body.velocity.sqrMagnitude < 1f && position.y - Helicopter.Ground(position) < 2.5f)
			{
				rockets = Mathf.Min(RocketLoad, rockets + 2);
			}
		}
	}

	protected override void PilotWeapons(ArenaTarget pilot, PlayerInput input)
	{
		if (!input.Fire || Time.time < nextRocket || base.Wrecked)
		{
			return;
		}
		if (rockets <= 0)
		{
			nextRocket = Time.time + 1.5f;
			if ((bool)BattleDirector.Instance && BattleDirector.Instance.Notice != null)
			{
				BattleDirector.Instance.Notice("Out of rockets - land to rearm");
			}
			return;
		}
		Vector3 direction = base.Rotation * Quaternion.Euler(1.5f, 0f, 0f) * Vector3.forward;
		Launch(pilot, direction);
		nextRocket = Time.time + 0.18f;
	}

	private void Launch(ArenaTarget pilot, Vector3 direction)
	{
		Vector3[] pods = Pods;
		Vector3 vector = base.transform.TransformPoint(pods[pod % pods.Length]) + direction * 0.3f;
		pod++;
		rockets--;
		Rocket.Fire(pilot, null, vector, direction, base.Body ? base.Body.velocity : Vector3.zero);
		if ((bool)ShotEffects.Instance)
		{
			ShotEffects.Instance.Flash(vector, direction, 2.5f);
		}
	}

	protected override void Autopilot(Vector3 here, Vector3 velocity, float ground, float height, out Vector3 accel, out float yawRate, out float climb, out bool land)
	{
		base.Autopilot(here, velocity, ground, height, out accel, out yawRate, out climb, out land);
		Motorpool.Task task = Motorpool.TaskOf(this);
		if (task == null || task.Stage == Motorpool.Stage.Loading || task.Stage == Motorpool.Stage.Unloading || task.Stage == Motorpool.Stage.Returning || Carrying())
		{
			hasStrike = false;
			return;
		}
		if (task.Stage == Motorpool.Stage.Outbound && Helicopter.Flat(task.Goal - here).magnitude < 25f)
		{
			climb = Mathf.Clamp((ground + 35f - here.y) * 0.4f, -5f, 6f);
			land = false;
		}
		if (Time.time >= lookAt)
		{
			lookAt = Time.time + 1f;
			hasStrike = FindStrike(task, here, out strike, out strikeVehicle);
		}
		if ((bool)strikeVehicle && (strikeVehicle.Wrecked || !strikeVehicle.Body))
		{
			strikeVehicle = null;
			hasStrike = false;
		}
		if (!hasStrike)
		{
			return;
		}
		Vector3 vector = (strikeVehicle ? strikeVehicle.Body.worldCenterOfMass : strike);
		Vector3 vector2 = Helicopter.Flat(here - vector);
		float magnitude = vector2.magnitude;
		if (magnitude < 1f)
		{
			vector2 = -Helicopter.Flat(base.Rotation * Vector3.forward);
		}
		Vector3 vector3 = Helicopter.Flat(vector + vector2.normalized * 380f - here);
		float magnitude2 = vector3.magnitude;
		float num = Mathf.Clamp(magnitude2 * 0.3f, 0f, CruiseSpeed);
		accel = (((magnitude2 > 0.5f) ? (vector3 / magnitude2 * num) : Vector3.zero) - Helicopter.Flat(velocity)) * 0.8f;
		float num2 = Mathf.Max(ground, Helicopter.Ground(here + Helicopter.Flat(velocity) * 3f), Helicopter.Ground(here + Helicopter.Flat(velocity) * 6f)) + 30f;
		climb = Mathf.Clamp((num2 - here.y) * 0.4f, -5f, 6f);
		land = false;
		float num3 = Vector3.SignedAngle(Helicopter.Flat(base.Rotation * Vector3.forward), Helicopter.Flat(vector - here), Vector3.up);
		yawRate = Mathf.Clamp(num3 * 1.5f, -45f, 45f);
		ArenaTarget arenaTarget = base.Riders[0];
		if ((bool)strikeVehicle && strikeVehicle.Armoured && (bool)arenaTarget && AtgmReady && Mathf.Abs(num3) < 20f && magnitude > 200f && magnitude < 2200f)
		{
			Vehicle aimed = strikeVehicle;
			FireAtgm(arenaTarget, () => ((bool)aimed && !aimed.Wrecked && (bool)aimed.Body) ? new Vector3?(aimed.Body.worldCenterOfMass + Vector3.up * 0.2f) : null, vector);
			if (aimed.ArmourRating >= 20f)
			{
				return;
			}
		}
		if (rockets <= 0 || !arenaTarget || Mathf.Abs(num3) > 7f || magnitude < 140f || magnitude > 950f || height < 6f || Time.time < nextRocket)
		{
			return;
		}
		if (salvo <= 0 && Time.time >= salvoPause)
		{
			salvo = Random.Range(4, 9);
		}
		if (salvo > 0)
		{
			Vector3 vector4 = base.transform.TransformPoint(Pods[pod % Pods.Length]);
			Vector3 to = vector + Random.insideUnitSphere * (magnitude * 0.012f);
			if (!Rocket.Solve(vector4, to, out var direction, out var _))
			{
				direction = (to - vector4).normalized;
			}
			direction = Vector3.RotateTowards(base.Rotation * Vector3.forward, direction, 0.2f, 0f);
			Launch(arenaTarget, direction);
			salvo--;
			nextRocket = Time.time + 0.2f;
			if (salvo <= 0)
			{
				salvoPause = Time.time + Random.Range(5f, 9f);
			}
		}
	}

	private bool Carrying()
	{
		ArenaTarget[] riders = base.Riders;
		for (int i = 2; i < riders.Length; i++)
		{
			if ((bool)riders[i] && riders[i].Seated && !riders[i].Body.Dead)
			{
				return true;
			}
		}
		return false;
	}

	private bool FindStrike(Motorpool.Task task, Vector3 here, out Vector3 point, out Vehicle vehicle)
	{
		point = Vector3.zero;
		vehicle = null;
		float num = float.MaxValue;
		foreach (Vehicle item in Vehicle.All)
		{
			if (!item || item == this || item.Wrecked || !item.Body || item.Team < 0 || item.Team == base.Team || item is Helicopter)
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
			Vector3 vector = worldCenterOfMass - here;
			float magnitude = vector.magnitude;
			if (!(magnitude > 1300f) && (!Physics.Raycast(here, vector / magnitude, out var hitInfo, magnitude, -261, QueryTriggerInteraction.Ignore) || item.Holds(hitInfo.collider) || Holds(hitInfo.collider)))
			{
				float num2 = magnitude - (item.Armoured ? 300f : 0f) - ((item.Gun != null && item.Gun.Cannon) ? 200f : 0f);
				if (num2 < num)
				{
					num = num2;
					vehicle = item;
				}
			}
		}
		if ((bool)vehicle)
		{
			point = vehicle.Body.worldCenterOfMass;
			return true;
		}
		if (task.HasSuppress)
		{
			point = task.Suppress;
			return Helicopter.Flat(point - here).magnitude < 1500f;
		}
		return false;
	}
}
