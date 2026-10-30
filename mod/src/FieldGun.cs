using System.Collections.Generic;
using UnityEngine;

namespace CombatSandbox.Demo;

public sealed class FieldGun : MonoBehaviour
{
	public enum GunKind
	{
		Mortar,
		Howitzer
	}

	public static readonly List<FieldGun> All = new List<FieldGun>();

	private static readonly WeaponDef MortarSound = new WeaponDef
	{
		Id = "mortar",
		Name = "82 mm mortar",
		MuzzleVelocity = 200f,
		BulletMass = 3f,
		Rpm = 10f,
		Magazine = 1,
		Sound = GunSound.Shotgun,
		Pitch = 0.45f,
		Loudness = 2f,
		Flash = 3f
	};

	private static Material steel;

	private static Material sandbag;

	private static Material rubber;

	public GunKind Kind;

	public int Team;

	public bool Destroyed;

	private Transform barrel;

	private float nextShot;

	private float recoil;

	public static bool Battery(int team)
	{
		foreach (FieldGun item in All)
		{
			if ((bool)item && item.Kind == GunKind.Howitzer && item.Team == team && !item.Destroyed)
			{
				return true;
			}
		}
		return false;
	}

	public static bool AnyHowitzers(int team)
	{
		foreach (FieldGun item in All)
		{
			if ((bool)item && item.Kind == GunKind.Howitzer && item.Team == team)
			{
				return true;
			}
		}
		return false;
	}

	public static FieldGun Create(GunKind kind, int team, Vector3 at, Vector3 toward)
	{
		GameObject gameObject = new GameObject((kind == GunKind.Mortar) ? "Mortar pit" : "Howitzer");
		gameObject.transform.SetPositionAndRotation(at, Quaternion.LookRotation(toward));
		FieldGun fieldGun = gameObject.AddComponent<FieldGun>();
		fieldGun.Kind = kind;
		fieldGun.Team = team;
		fieldGun.nextShot = Time.time + Random.Range(15f, 30f);
		fieldGun.Build();
		return fieldGun;
	}

	private void OnEnable()
	{
		All.Add(this);
	}

	private void OnDisable()
	{
		All.Remove(this);
	}

	private void Build()
	{
		if (!steel)
		{
			steel = new Material(Shader.Find("Standard"))
			{
				color = new Color(0.22f, 0.25f, 0.17f)
			};
			steel.SetFloat("_Glossiness", 0.25f);
			sandbag = new Material(Shader.Find("Standard"))
			{
				color = new Color(0.45f, 0.4f, 0.28f)
			};
			sandbag.SetFloat("_Glossiness", 0.05f);
		}
		if (!rubber)
		{
			rubber = new Material(Shader.Find("Standard"))
			{
				color = new Color(0.04f, 0.04f, 0.04f)
			};
			rubber.SetFloat("_Glossiness", 0.1f);
		}
		if (!FieldGunMeshes.Ready)
		{
			FieldGunMeshes.Build();
		}
		if (Kind == GunKind.Mortar)
		{
			Draw(base.transform, FieldGunMeshes.Sandbags, sandbag);
			Draw(base.transform, FieldGunMeshes.Baseplate, steel);
			(Vector3, float)[] array = FieldGunMeshes.Bags();
			for (int i = 0; i < 12; i++)
			{
				Solid(array[i].Item1, new Vector3(0.86f, 0.24f, 0.42f), new Vector3(0f, array[i].Item2, 0f));
			}
			barrel = new GameObject("Tube").transform;
			barrel.SetParent(base.transform, worldPositionStays: false);
			barrel.localPosition = FieldGunMeshes.MortarPivot;
			barrel.localEulerAngles = new Vector3(-50f, 0f, 0f);
			Draw(barrel, FieldGunMeshes.MortarTube, steel);
		}
		else
		{
			Draw(base.transform, FieldGunMeshes.Carriage, steel);
			Draw(base.transform, FieldGunMeshes.Shield, steel);
			Vector3[] wheels = FieldGunMeshes.Wheels;
			foreach (Vector3 vector in wheels)
			{
				Transform transform = new GameObject("Wheel").transform;
				transform.SetParent(base.transform, worldPositionStays: false);
				transform.localPosition = vector;
				if (vector.x < 0f)
				{
					transform.localScale = new Vector3(-1f, 1f, 1f);
				}
				Draw(transform, FieldGunMeshes.Tyre, rubber);
				Draw(transform, FieldGunMeshes.Hub, steel);
			}
			Solid(new Vector3(0f, 1.05f, 0.3f), new Vector3(2f, 1.1f, 0.08f), new Vector3(-8f, 0f, 0f));
			Solid(new Vector3(0f, 0.7f, -0.25f), new Vector3(0.62f, 0.5f, 1.2f), Vector3.zero);
			barrel = new GameObject("Barrel").transform;
			barrel.SetParent(base.transform, worldPositionStays: false);
			barrel.localPosition = FieldGunMeshes.HowitzerPivot;
			barrel.localEulerAngles = new Vector3(-25f, 0f, 0f);
			Draw(barrel, FieldGunMeshes.Barrel, steel);
		}
	}

	private static void Draw(Transform parent, Mesh mesh, Material m)
	{
		GameObject gameObject = new GameObject(mesh.name);
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
		gameObject.AddComponent<MeshRenderer>().sharedMaterial = m;
	}

	private void Solid(Vector3 at, Vector3 size, Vector3 euler)
	{
		GameObject gameObject = new GameObject("Collider");
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
		gameObject.transform.localPosition = at;
		gameObject.transform.localEulerAngles = euler;
		gameObject.AddComponent<BoxCollider>().size = size;
	}

	public Vector3 Muzzle
	{
		get
		{
			if (!barrel)
			{
				return base.transform.position + Vector3.up;
			}
			return barrel.TransformPoint(new Vector3(0f, 0f, (Kind == GunKind.Mortar) ? 1.2f : 4.7f));
		}
	}

	public void Flash()
	{
		if (Destroyed)
		{
			return;
		}
		Vector3 muzzle = Muzzle;
		Vector3 vector = (barrel ? barrel.forward : Vector3.up);
		if ((bool)ShotEffects.Instance)
		{
			ShotEffects.Instance.Flash(muzzle, vector, (Kind == GunKind.Mortar) ? 3f : 10f);
			ShotEffects.Instance.Flare(muzzle, (Kind == GunKind.Mortar) ? 0.5f : 1.5f);
		}
		if (Kind == GunKind.Howitzer)
		{
			if ((bool)Acoustics.Instance)
			{
				Acoustics.Instance.Explosion(muzzle, rocket: true);
			}
		}
		else if ((bool)ReconstructionDemo.Instance)
		{
			ReconstructionDemo.Instance.Report(muzzle, MortarSound, null);
		}
		recoil = 1f;
	}

	private void Update()
	{
		if ((bool)barrel && !Destroyed)
		{
			recoil = Mathf.MoveTowards(recoil, 0f, Time.deltaTime * 2f);
			barrel.localPosition = ((Kind == GunKind.Mortar) ? new Vector3(0f, 0.08f, 0f) : new Vector3(0f, 1.1f, 0f)) - barrel.localRotation * Vector3.forward * recoil * ((Kind == GunKind.Mortar) ? 0.05f : 0.6f);
		}
		if (Kind != GunKind.Mortar || Destroyed || Time.time < nextShot || (CaptureMode.Active && !CaptureMode.Instance.Started))
		{
			return;
		}
		nextShot = Time.time + Random.Range(11f, 18f);
		if (Target(out var at))
		{
			Vector3 muzzle = Muzzle;
			Vector3 vector = at - muzzle;
			Vector3 vector2 = new Vector3(vector.x, 0f, vector.z);
			float magnitude = vector2.magnitude;
			float num = 5f + magnitude / 90f;
			Vector3 vector3 = vector2 / num + Vector3.up * (vector.y / num + 4.905f * num);
			barrel.rotation = Quaternion.LookRotation(vector3);
			TankShell.Fire(null, null, muzzle, vector3, 0, 1.6f, tracer: false, num + 4f);
			Flash();
		}
	}

	private bool Target(out Vector3 at)
	{
		at = Vector3.zero;
		Vector3 position = base.transform.position;
		float num = float.MaxValue;
		foreach (CombatBrain item in CombatBrain.All)
		{
			if (!CaptureMode.Standing(item) || item.Team == Team)
			{
				continue;
			}
			float magnitude = (item.Here - position).magnitude;
			if (magnitude < 50f || magnitude > 400f)
			{
				continue;
			}
			bool flag = false;
			foreach (CombatBrain item2 in CombatBrain.All)
			{
				if (CaptureMode.Standing(item2) && item2.Team == Team && (item2.Here - item.Here).magnitude < 25f)
				{
					flag = true;
					break;
				}
			}
			if (!flag && magnitude < num)
			{
				num = magnitude;
				Vector2 vector = Random.insideUnitCircle * Mathf.Lerp(4f, 14f, magnitude / 400f);
				at = item.Here + new Vector3(vector.x, 0f, vector.y);
			}
		}
		return num < float.MaxValue;
	}

	public static void Blast(Vector3 centre, float power)
	{
		foreach (FieldGun item in All.ToArray())
		{
			if ((bool)item && !item.Destroyed && Vector3.Distance(item.transform.position, centre) < 1.6f * power + 1f)
			{
				item.Wreck();
			}
		}
	}

	private void Wreck()
	{
		Destroyed = true;
		if ((bool)ShotEffects.Instance)
		{
			ShotEffects.Instance.Explosion(base.transform.position + Vector3.up * 0.5f, (Kind == GunKind.Mortar) ? 0.8f : 1.5f);
		}
		Plumes.Smoke(base.transform, 0.25f, new Color(0.1f, 0.095f, 0.09f, 0.7f));
		if ((bool)barrel)
		{
			barrel.localEulerAngles = new Vector3(10f, Random.Range(-30f, 30f), Random.Range(-20f, 20f));
		}
		BattleDirector instance = BattleDirector.Instance;
		if ((bool)instance)
		{
			instance.Event(instance.Teams[Team].Name + ": " + ((Kind == GunKind.Mortar) ? "mortar destroyed" : "howitzer destroyed"), Team);
		}
	}
}
