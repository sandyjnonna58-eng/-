using System.Collections.Generic;
using UnityEngine;

namespace CombatSandbox.Demo;

public abstract partial class Vehicle
{
	public enum ModuleKind
	{
		Engine,
		Transmission,
		FuelTank,
		AmmoRack,
		TurretRing,
		Gun,
		TrackLeft,
		TrackRight,
		Wheel
	}

	public sealed class Module
	{
		public ModuleKind Kind;

		public string Name;

		public Vector3 Centre;

		public Vector3 Size;

		public float Health;

		public float Max;

		public float MinEnergy;

		public bool Outside;

		public int Wheel = -1;

		public float LastHit = -100f;

		public bool Destroyed => Health <= 0f;

		public bool Damaged => Health < Max * 0.65f;

		public float Fraction => Mathf.Clamp01(Health / Mathf.Max(0.01f, Max));
	}

	private List<Module> modules;

	private float fuelFireUntil;

	private float lastModuleHit = -100f;

	private bool wheelsFlattened;

	public List<Module> Modules
	{
		get
		{
			if (modules == null)
			{
				BuildModules();
			}
			return modules;
		}
	}

	public float DriveFactor
	{
		get
		{
			float num = 1f;
			foreach (Module module in Modules)
			{
				if (module.Kind == ModuleKind.Transmission)
				{
					num = (module.Destroyed ? 0f : (module.Damaged ? 0.5f : 1f));
				}
			}
			return num;
		}
	}

	private void BuildModules()
	{
		modules = new List<Module>();
		Vector3 engineCentre = spec.EngineCentre;
		Vector3 engineSize = spec.EngineSize;
		float num = ((engineCentre.z >= 0f) ? 1f : (-1f));
		modules.Add(new Module
		{
			Kind = ModuleKind.Engine,
			Name = "Engine",
			Centre = engineCentre,
			Size = engineSize,
			Max = 1f,
			Health = 1f
		});
		modules.Add(new Module
		{
			Kind = ModuleKind.Transmission,
			Name = "Transmission",
			Centre = engineCentre + new Vector3(0f, (0f - engineSize.y) * 0.3f, num * (engineSize.z * 0.5f + 0.2f)),
			Size = new Vector3(engineSize.x * 0.8f, 0.45f, 0.4f),
			Max = (spec.Armoured ? 30f : 10f),
			MinEnergy = 1f
		});
		modules.Add(new Module
		{
			Kind = ModuleKind.FuelTank,
			Name = "Fuel tank",
			Centre = engineCentre + new Vector3(engineSize.x * 0.25f, 0f, (0f - num) * (engineSize.z * 0.5f + 0.3f)),
			Size = new Vector3(engineSize.x * 0.5f, engineSize.y * 0.8f, 0.5f),
			Max = (spec.Armoured ? 20f : 8f),
			MinEnergy = 1f
		});
		if (gun != null)
		{
			if (spec.Armoured)
			{
				modules.Add(new Module
				{
					Kind = ModuleKind.AmmoRack,
					Name = "Ammo rack",
					Centre = new Vector3(gun.Pivot.x, Mathf.Max(0.7f, gun.Pivot.y - 0.75f), gun.Pivot.z),
					Size = new Vector3(1.3f, 0.5f, 1.1f),
					Max = 20f,
					MinEnergy = 2f
				});
			}
			modules.Add(new Module
			{
				Kind = ModuleKind.TurretRing,
				Name = "Turret ring",
				Centre = gun.Pivot,
				Size = new Vector3(1.4f, 0.35f, 1.4f),
				Max = (spec.Armoured ? 25f : 10f),
				MinEnergy = 2f
			});
			modules.Add(new Module
			{
				Kind = ModuleKind.Gun,
				Name = "Gun",
				Max = (gun.Cannon ? 30f : 10f),
				MinEnergy = (gun.Cannon ? 8f : 2f),
				Outside = true
			});
		}
		if (spec.Tracked)
		{
			for (int i = 0; i < 2; i++)
			{
				float num2 = float.MaxValue;
				float num3 = float.MinValue;
				float num4 = 0f;
				float num5 = 0.4f;
				Wheel[] wheels = spec.Wheels;
				for (int j = 0; j < wheels.Length; j++)
				{
					Wheel wheel = wheels[j];
					if ((wheel.At.x < 0f) == (i == 0))
					{
						num2 = Mathf.Min(num2, wheel.At.z);
						num3 = Mathf.Max(num3, wheel.At.z);
						num4 = wheel.At.x;
						num5 = wheel.Radius;
					}
				}
				modules.Add(new Module
				{
					Kind = ((i == 0) ? ModuleKind.TrackLeft : ModuleKind.TrackRight),
					Name = ((i == 0) ? "Left track" : "Right track"),
					Centre = new Vector3(num4, num5, (num2 + num3) * 0.5f),
					Size = new Vector3(spec.TrackWidth + 0.2f, num5 * 2f + 0.3f, num3 - num2 + 1.2f),
					Max = 40f,
					MinEnergy = 15f,
					Outside = true
				});
			}
		}
		else
		{
			for (int k = 0; k < spec.Wheels.Length; k++)
			{
				Wheel wheel2 = spec.Wheels[k];
				modules.Add(new Module
				{
					Kind = ModuleKind.Wheel,
					Name = WheelName(k),
					Centre = wheel2.At,
					Size = new Vector3(wheel2.Width + 0.15f, wheel2.Radius * 2f, wheel2.Radius * 2f),
					Max = (spec.Armoured ? 20f : 6f),
					MinEnergy = 1f,
					Outside = true,
					Wheel = k
				});
			}
		}
		foreach (Module module in modules)
		{
			if (module.Kind != ModuleKind.Engine)
			{
				module.Health = module.Max;
			}
		}
	}

	private string WheelName(int i)
	{
		Wheel wheel = spec.Wheels[i];
		bool flag = wheel.At.x < 0f;
		int num = 0;
		int num2 = 0;
		for (int j = 0; j < spec.Wheels.Length; j++)
		{
			if ((spec.Wheels[j].At.x < 0f) == flag)
			{
				num2++;
				if (spec.Wheels[j].At.z > wheel.At.z)
				{
					num++;
				}
			}
		}
		if (num2 == 2 || spec.Wheels.Length <= 4)
		{
			return (num == 0) ? (flag ? "Front left wheel" : "Front right wheel") : (flag ? "Rear left wheel" : "Rear right wheel");
		}
		return (flag ? "Left wheel " : "Right wheel ") + (num + 1);
	}

	public Module ModuleOf(ModuleKind kind)
	{
		foreach (Module module in Modules)
		{
			if (module.Kind == kind)
			{
				return module;
			}
		}
		return null;
	}

	public float ModuleState(Module m)
	{
		if (m.Kind == ModuleKind.Engine)
		{
			if (!Wrecked)
			{
				return Condition;
			}
			return 0f;
		}
		return m.Fraction;
	}

	private bool WheelLost(int j, out float drag)
	{
		drag = 0f;
		if (modules == null)
		{
			return false;
		}
		Wheel wheel = spec.Wheels[j];
		foreach (Module module in modules)
		{
			if (module.Destroyed && ((module.Kind == ModuleKind.Wheel && module.Wheel == j) || (module.Kind == ModuleKind.TrackLeft && wheel.At.x < 0f) || (module.Kind == ModuleKind.TrackRight && wheel.At.x >= 0f)))
			{
				drag = ((module.Kind == ModuleKind.Wheel) ? (spec.BrakeTorque * 0.3f) : (spec.BrakeTorque * 1.5f));
				return true;
			}
		}
		return false;
	}

	private void ModuleStep(float dt)
	{
		if (modules == null || Wrecked)
		{
			return;
		}
		if (gun != null)
		{
			Module module = ModuleOf(ModuleKind.TurretRing);
			Module module2 = ModuleOf(ModuleKind.Gun);
			gun.Traverse = ((module == null) ? 1f : (module.Destroyed ? 0f : (module.Damaged ? 0.4f : 1f)));
			gun.Disabled = module2 != null && module2.Destroyed;
		}
		if (Time.time < fuelFireUntil)
		{
			Damage(spec.EngineHealth * 0.03f * dt);
		}
		if (!wheelsFlattened)
		{
			foreach (Module module3 in modules)
			{
				if (module3.Kind == ModuleKind.Wheel && module3.Destroyed && module3.Wheel >= 0 && module3.Wheel < wheels.Length && (bool)wheels[module3.Wheel])
				{
					WheelFrictionCurve sidewaysFriction = wheels[module3.Wheel].sidewaysFriction;
					sidewaysFriction.stiffness *= 0.35f;
					wheels[module3.Wheel].sidewaysFriction = sidewaysFriction;
					wheels[module3.Wheel].radius *= 0.85f;
					wheelsFlattened = true;
				}
			}
		}
		bool flag = (bool)Riders[0] && !Riders[0].Body.Dead && Mathf.Abs(Speed) < 0.5f;
		if (!flag || !(Time.time - lastModuleHit > 20f))
		{
			return;
		}
		foreach (Module module4 in modules)
		{
			if (module4.Destroyed && (module4.Kind == ModuleKind.TrackLeft || module4.Kind == ModuleKind.TrackRight || module4.Kind == ModuleKind.Wheel) && Time.time - module4.LastHit > 25f)
			{
				module4.Health = module4.Max * 0.5f;
				if (module4.Kind == ModuleKind.Wheel && module4.Wheel >= 0 && (bool)wheels[module4.Wheel])
				{
					WheelFrictionCurve sidewaysFriction2 = wheels[module4.Wheel].sidewaysFriction;
					sidewaysFriction2.stiffness = ((spec.Grip > 0f) ? spec.Grip : 1.1f);
					wheels[module4.Wheel].sidewaysFriction = sidewaysFriction2;
					wheels[module4.Wheel].radius = spec.Wheels[module4.Wheel].Radius;
				}
				Tell("Module repaired: " + module4.Name);
			}
		}
	}

	private void Tell(string text)
	{
		BattleDirector instance = BattleDirector.Instance;
		if (!instance || instance.Notice == null || !instance.Possessed)
		{
			return;
		}
		ArenaTarget[] riders = Riders;
		foreach (ArenaTarget arenaTarget in riders)
		{
			if ((bool)arenaTarget && arenaTarget == instance.Possessed.Body)
			{
				instance.Notice(Kind + ": " + text);
				break;
			}
		}
	}

	private void HitModule(Module m, float amount)
	{
		if (m.Kind == ModuleKind.Engine || m.Destroyed || Wrecked || amount < m.MinEnergy)
		{
			return;
		}
		m.LastHit = Time.time;
		lastModuleHit = Time.time;
		bool damaged = m.Damaged;
		m.Health -= amount;
		if (m.Destroyed)
		{
			m.Health = 0f;
			Broken(m);
		}
		else if (m.Damaged && !damaged)
		{
			Tell("Module damaged: " + m.Name);
			if (m.Kind == ModuleKind.AmmoRack && Random.value < 0.3f)
			{
				m.Health = 0f;
				Broken(m);
			}
		}
	}

	private void Broken(Module m)
	{
		Tell("Module destroyed: " + m.Name);
		switch (m.Kind)
		{
		case ModuleKind.FuelTank:
		{
			fuelFireUntil = Time.time + 25f;
			Transform obj = new GameObject("Fuel fire").transform;
			obj.SetParent(base.transform, worldPositionStays: false);
			obj.localPosition = m.Centre + Vector3.up * 0.4f;
			Plumes.Fire(obj, 0.35f, light: true);
			Object.Destroy(obj.gameObject, 25f);
			break;
		}
		case ModuleKind.AmmoRack:
		{
			Vector3 vector = World(m.Centre);
			if ((bool)ShotEffects.Instance)
			{
				ShotEffects.Instance.Explosion(vector, 2.2f);
			}
			if ((bool)Acoustics.Instance)
			{
				Acoustics.Instance.Explosion(vector, rocket: true);
			}
			Detonated();
			Wreck();
			break;
		}
		}
	}

	protected virtual void Detonated()
	{
	}

	private bool PathHits(Module m, Vector3 p0, Vector3 dir, float depth)
	{
		if (m.Kind == ModuleKind.Gun)
		{
			if (gun == null || !gun.Gun)
			{
				return false;
			}
			Vector3 a = Local(gun.Gun.position);
			Vector3 b = Local(gun.Gun.TransformPoint(gun.Muzzle));
			return SegmentDistance(p0, p0 + dir * depth, a, b) < 0.2f;
		}
		Vector3 vector = m.Size * 0.5f + Vector3.one * 0.05f;
		Vector3 vector2 = p0 - m.Centre;
		float num = 0f;
		float num2 = depth;
		for (int i = 0; i < 3; i++)
		{
			float num3 = vector2[i];
			float num4 = dir[i];
			if (Mathf.Abs(num4) < 1E-05f)
			{
				if (Mathf.Abs(num3) > vector[i])
				{
					return false;
				}
				continue;
			}
			float num5 = (0f - vector[i] - num3) / num4;
			float num6 = (vector[i] - num3) / num4;
			if (num5 > num6)
			{
				float num7 = num5;
				num5 = num6;
				num6 = num7;
			}
			num = Mathf.Max(num, num5);
			num2 = Mathf.Min(num2, num6);
			if (num > num2)
			{
				return false;
			}
		}
		return true;
	}

	private static float SegmentDistance(Vector3 p1, Vector3 q1, Vector3 p2, Vector3 q2)
	{
		Vector3 vector = q1 - p1;
		Vector3 vector2 = q2 - p2;
		Vector3 vector3 = p1 - p2;
		float num = Vector3.Dot(vector, vector);
		float num2 = Vector3.Dot(vector2, vector2);
		float num3 = Vector3.Dot(vector2, vector3);
		float num4 = 0f;
		float num5 = 0f;
		if (num > 1E-06f && num2 > 1E-06f)
		{
			float num6 = Vector3.Dot(vector, vector3);
			float num7 = Vector3.Dot(vector, vector2);
			float num8 = num * num2 - num7 * num7;
			num4 = ((num8 > 1E-06f) ? Mathf.Clamp01((num7 * num3 - num6 * num2) / num8) : 0f);
			num5 = Mathf.Clamp01((num7 * num4 + num3) / num2);
		}
		return Vector3.Distance(p1 + vector * num4, p2 + vector2 * num5);
	}

	private void ModulesAlongPath(Vector3 point, Vector3 direction, float outside, float inside, float depth)
	{
		Vector3 p = Local(point - direction.normalized * 0.4f);
		Vector3 dir = Quaternion.Inverse(Rotation) * direction.normalized;
		foreach (Module module in Modules)
		{
			float num = (module.Outside ? outside : inside);
			if (num > 0f && PathHits(module, p, dir, depth + 0.4f))
			{
				HitModule(module, num);
			}
		}
	}

	private void ModulesInBlast(Vector3 centre, float power)
	{
		Vector3 vector = Local(centre);
		float num = 1.3f * power;
		foreach (Module module in Modules)
		{
			if (module.Kind == ModuleKind.Engine)
			{
				continue;
			}
			float num2;
			if (module.Kind == ModuleKind.Gun)
			{
				if (gun == null || !gun.Gun)
				{
					continue;
				}
				num2 = SegmentDistance(vector, vector, Local(gun.Gun.position), Local(gun.Gun.TransformPoint(gun.Muzzle)));
			}
			else
			{
				Vector3 vector2 = vector - module.Centre;
				Vector3 vector3 = module.Size * 0.5f;
				num2 = new Vector3(Mathf.Max(0f, Mathf.Abs(vector2.x) - vector3.x), Mathf.Max(0f, Mathf.Abs(vector2.y) - vector3.y), Mathf.Max(0f, Mathf.Abs(vector2.z) - vector3.z)).magnitude;
			}
			if (!(num2 > num))
			{
				float num3 = 32f * power * (1f - num2 / num) * ((spec.Armoured && !module.Outside) ? 0.3f : 1f);
				HitModule(module, num3);
			}
		}
	}

	public float ArmourRating => spec.ArmourEnergy;

	public static bool Pierce(Collider c, Vector3 point, Vector3 direction, float energy)
	{
		if (!c || !byCollider.TryGetValue(c, out var value) || !value)
		{
			return false;
		}
		if (value.Wrecked)
		{
			return true;
		}
		Vector3 normalized = direction.normalized;
		value.Body.AddForceAtPosition(normalized * 60f, point, ForceMode.Impulse);
		if (value.spec.Armoured && energy < value.spec.ArmourEnergy)
		{
			value.ModulesAlongPath(point, normalized, energy * 0.7f, 0f, 0.5f);
			if ((bool)ShotEffects.Instance)
			{
				ShotEffects.Instance.Sparks(point, -normalized, 12, 9f);
			}
			if ((bool)Acoustics.Instance)
			{
				Acoustics.Instance.Impact(point, ImpactSound.Metal, 1.2f);
			}
			return false;
		}
		Vector3 vector = value.Local(point + normalized * 0.3f) - value.spec.EngineCentre;
		Vector3 vector2 = value.spec.EngineSize * 0.5f + Vector3.one * 0.15f;
		bool flag = Mathf.Abs(vector.x) < vector2.x && Mathf.Abs(vector.y) < vector2.y && Mathf.Abs(vector.z) < vector2.z;
		if ((bool)ShotEffects.Instance)
		{
			ShotEffects.Instance.Sparks(point, -normalized, 18, 12f);
			ShotEffects.Instance.Flare(point, 0.5f);
		}
		if ((bool)Acoustics.Instance)
		{
			Acoustics.Instance.Impact(point, ImpactSound.Metal, 1.6f);
		}
		value.ModulesAlongPath(point, normalized, 18f, 14f, 2.5f);
		if ((bool)Ballistics.Instance)
		{
			for (int i = 0; i < 4; i++)
			{
				Ballistics.Instance.Fragment(null, point + normalized * 0.15f, (normalized + Random.insideUnitSphere * 0.35f).normalized, Random.Range(300, 500), Random.Range(0.002f, 0.005f));
			}
		}
		value.Damage(value.spec.EngineHealth * (flag ? 0.25f : 0.07f));
		return true;
	}
}
