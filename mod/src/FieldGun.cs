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
		if (Kind == GunKind.Mortar)
		{
			for (int i = 0; i < 10; i++)
			{
				float f = (float)i * 36f * (Mathf.PI / 180f);
				Part(PrimitiveType.Cube, new Vector3(Mathf.Sin(f) * 1.6f, 0.25f, Mathf.Cos(f) * 1.6f), new Vector3(0.9f, 0.5f, 0.45f), sandbag, new Vector3(0f, (float)i * 36f, 0f), collide: true);
			}
			Part(PrimitiveType.Cylinder, new Vector3(0f, 0.04f, 0f), new Vector3(0.5f, 0.03f, 0.5f), steel, Vector3.zero, collide: false);
			barrel = new GameObject("Tube").transform;
			barrel.SetParent(base.transform, worldPositionStays: false);
			barrel.localPosition = new Vector3(0f, 0.08f, 0f);
			barrel.localEulerAngles = new Vector3(-50f, 0f, 0f);
			Part(PrimitiveType.Cylinder, new Vector3(0f, 0f, 0.6f), new Vector3(0.09f, 0.6f, 0.09f), steel, new Vector3(90f, 0f, 0f), collide: false, barrel);
			Part(PrimitiveType.Cube, new Vector3(0f, 0.45f, 0.5f), new Vector3(0.04f, 0.9f, 0.04f), steel, new Vector3(-30f, 0f, 0f), collide: false);
		}
		else
		{
			Part(PrimitiveType.Cube, new Vector3(0f, 1.05f, 0.2f), new Vector3(2f, 1.1f, 0.06f), steel, new Vector3(-8f, 0f, 0f), collide: true);
			Part(PrimitiveType.Cube, new Vector3(0f, 0.75f, -0.3f), new Vector3(0.6f, 0.5f, 1.2f), steel, Vector3.zero, collide: true);
			int[] array = new int[2] { -1, 1 };
			foreach (int num in array)
			{
				Part(PrimitiveType.Cylinder, new Vector3((float)num * 0.85f, 0.45f, -0.2f), new Vector3(0.9f, 0.12f, 0.9f), sandbag, new Vector3(0f, 0f, 90f), collide: false);
				Part(PrimitiveType.Cube, new Vector3((float)num * 0.6f, 0.2f, -1.8f), new Vector3(0.14f, 0.14f, 2.6f), steel, new Vector3(0f, (float)num * 20f, 0f), collide: false);
			}
			barrel = new GameObject("Barrel").transform;
			barrel.SetParent(base.transform, worldPositionStays: false);
			barrel.localPosition = new Vector3(0f, 1.1f, 0f);
			barrel.localEulerAngles = new Vector3(-25f, 0f, 0f);
			Part(PrimitiveType.Cylinder, new Vector3(0f, 0f, 2.2f), new Vector3(0.16f, 2.4f, 0.16f), steel, new Vector3(90f, 0f, 0f), collide: false, barrel);
			Part(PrimitiveType.Cube, new Vector3(0f, 0f, 4.55f), new Vector3(0.3f, 0.2f, 0.4f), steel, Vector3.zero, collide: false, barrel);
		}
	}

	private void Part(PrimitiveType type, Vector3 at, Vector3 size, Material m, Vector3 euler, bool collide, Transform parent = null)
	{
		GameObject gameObject = GameObject.CreatePrimitive(type);
		if (!collide)
		{
			Object.Destroy(gameObject.GetComponent<Collider>());
		}
		gameObject.transform.SetParent(parent ? parent : base.transform, worldPositionStays: false);
		gameObject.transform.localPosition = at;
		gameObject.transform.localEulerAngles = euler;
		gameObject.transform.localScale = size;
		gameObject.GetComponent<Renderer>().sharedMaterial = m;
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
