using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CombatSandbox.Demo;

public sealed class T72 : Vehicle
{
	public const int Gunner = 1;

	private const float HullHalf = 1.08f;

	private const float Fender = 1.75f;

	private const float Shelf = 1.1f;

	private const float Deck = 1.5f;

	private const float Nose = 3.45f;

	private const float Tail = -3.4f;

	private const float TrackX = 1.4f;

	private const float TrackWidth = 0.58f;

	private const float WheelR = 0.35f;

	private const float FrontR = 0.3f;

	private const float RearR = 0.32f;

	private static readonly float[] Stations = new float[6] { 2.3f, 1.45f, 0.6f, -0.25f, -1.1f, -1.95f };

	private static readonly Vector3 Front = new Vector3(0f, 0.68f, 2.95f);

	private static readonly Vector3 Rear = new Vector3(0f, 0.72f, -2.95f);

	private static readonly Vector3 TurretPivot = new Vector3(0f, 1.5f, 0.2f);

	public static readonly WeaponDef Cannon125 = new WeaponDef
	{
		Id = "2a46m",
		Name = "2A46M 125 mm",
		Caliber = "125 mm",
		Class = WeaponClass.MachineGun,
		MuzzleVelocity = 850f,
		BulletMass = 23f,
		Drag = 0f,
		Rpm = 10f,
		Magazine = 1,
		Reload = 6.5f,
		Auto = false,
		Semi = true,
		Recoil = 300f,
		Dispersion = 1.5f,
		Mass = 2500f,
		Zero = 1000f,
		TracerEvery = 0,
		Sound = GunSound.Sniper,
		Pitch = 0.35f,
		Loudness = 3.5f,
		Flash = 9f,
		Burst = new Vector2Int(1, 1),
		EffectiveRange = 2500f
	};

	private static readonly List<Vector2> loop = new List<Vector2>();

	private static readonly List<float> along = new List<float>();

	private Transform[] wheelModelsCache;

	private Transform sprocketL;

	private Transform sprocketR;

	private Transform idlerL;

	private Transform idlerR;

	private Transform[] shoesL;

	private Transform[] shoesR;

	private float wreckedAt = -1f;

	private bool tossed;

	private static Mesh hull;

	private static Mesh fittings;

	private static Mesh skirts;

	private static Mesh drums;

	private static Mesh log;

	private static Mesh turretMesh;

	private static Mesh turretKit;

	private static Mesh mantlet;

	private static Mesh barrel;

	private static Mesh roadRubber;

	private static Mesh roadHub;

	private static Mesh sprocketMesh;

	private static Mesh idlerMesh;

	private static Mesh shoeMesh;

	private static Mesh glass;

	private static Material[] armour = new Material[2];

	private static Material steel;

	private static Material rubber;

	private static Material dark;

	private static Material wood;

	private static Material trackSteel;

	private static Material gunMetal;

	private static Material lamp;

	private static Material drumPaint;

	protected override Vector3 FlagMount => new Vector3(-1.2f, 1.55f, -3.05f);

	protected override float FlagLength => 1.8f;

	protected override void Describe()
	{
		Wheel[] array = new Wheel[Stations.Length * 2];
		for (int i = 0; i < Stations.Length; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				array[i * 2 + j] = new Wheel
				{
					At = new Vector3((j == 0) ? (-1.4f) : 1.4f, 0.4f, Stations[i]),
					Radius = 0.4f,
					Width = 0.58f,
					Steer = Stations[i] / Stations[0],
					Driven = true
				};
			}
		}
		spec = new Spec
		{
			Name = "T-72B",
			Mass = 44500f,
			CentreOfMass = new Vector3(0f, 0.85f, 0f),
			Wheels = array,
			Tracked = true,
			TrackWidth = 0.58f,
			PivotTorque = 420000f,
			Grip = 0.9f,
			Suspension = 0.25f,
			Spring = 380000f,
			Damper = 28000f,
			WheelMass = 150f,
			MaxTorque = 70000f,
			ReverseTorque = 30000f,
			Power = 560000f,
			TopSpeed = 16f,
			ReverseTop = 4.5f,
			BrakeTorque = 12000f,
			EngineBrake = 1500f,
			MaxSteer = 9f,
			AntiRoll = 220000f,
			Drag = 6f,
			SteerPivot = new Vector3(0f, 0.9f, 2.9f),
			SteerAxis = Vector3.up,
			SteerRatio = 1f,
			SteerInertia = 0.6f,
			SteerArms = 300f,
			EngineCentre = new Vector3(0f, 1f, -2.4f),
			EngineSize = new Vector3(1.9f, 0.8f, 1.6f),
			EngineHealth = 1100f,
			Armoured = true,
			ArmourEnergy = 30f,
			Hatches = new Vector3[3]
			{
				new Vector3(0f, 1.55f, 2.45f),
				TurretPivot + new Vector3(0.45f, 0.72f, -0.15f),
				TurretPivot + new Vector3(-0.45f, 0.72f, -0.1f)
			},
			EnginePitch = 0.42f,
			EngineLoudness = 2.2f,
			CameraOffset = new Vector3(0f, 4.2f, -13f)
		};
		List<Place> list = new List<Place>
		{
			new Place
			{
				Role = SeatRole.Driver,
				Pelvis = new Vector3(0f, 0.78f, 2.55f),
				FootL = new Vector3(-0.13f, 0.55f, 3.1f),
				FootR = new Vector3(0.13f, 0.55f, 3.1f),
				Door = new Vector3(0f, 1.5f, 2.4f),
				Out = Vector3.left,
				Stand = 2.3f,
				Lean = 25f
			},
			new Place
			{
				Role = SeatRole.Gunner,
				Pelvis = new Vector3(-0.42f, 1.05f, 0.25f),
				FootL = new Vector3(-0.55f, 0.55f, 0.6f),
				FootR = new Vector3(-0.3f, 0.55f, 0.6f),
				Door = new Vector3(-0.45f, 1.6f, 0f),
				Out = Vector3.left,
				Stand = 2.3f,
				Lean = 5f
			}
		};
		int[] array2 = new int[2] { -1, 1 };
		foreach (int num in array2)
		{
			float[] array3 = new float[2] { -1.6f, -2.6f };
			foreach (float num2 in array3)
			{
				float num3 = ((num < 0) ? 1 : (-1));
				list.Add(new Place
				{
					Role = SeatRole.Holder,
					Pelvis = new Vector3((float)num * 1f, 1.66f, num2),
					Yaw = num * 90,
					Lean = 6f,
					FootL = new Vector3((float)num * 1.45f, 1.16f, num2 + 0.14f * num3),
					FootR = new Vector3((float)num * 1.45f, 1.16f, num2 - 0.14f * num3),
					HoldL = new Vector3((float)num * 0.8f, 1.55f, num2 + 0.28f * num3),
					HoldR = new Vector3((float)num * 0.8f, 1.55f, num2 - 0.28f * num3),
					Side = num,
					Stand = 1f
				});
			}
		}
		places = list.ToArray();
		gun = new GunMount(this, 1, TurretPivot, Cannon125)
		{
			MaxTraverse = 180f,
			MinPitch = -5f,
			MaxPitch = 14f,
			GunLocal = new Vector3(0f, 0.36f, 0.95f),
			Muzzle = new Vector3(0f, 0f, 4.8f),
			Port = new Vector3(0f, 0f, 0.1f),
			Casings = false,
			GripsElevate = false,
			GripL = new Vector3(-0.6f, -0.25f, 0.35f),
			GripR = new Vector3(-0.3f, -0.25f, 0.35f),
			SeatTurn = 1f,
			Inertia = 30f,
			Laying = 150f,
			Damping = 135f,
			Cannon = true
		};
	}

	protected override void Update()
	{
		base.Update();
		if ((bool)sprocketL)
		{
			sprocketL.localRotation = Quaternion.Euler(base.RolledLeft / 0.34f * 57.29578f, 0f, 0f);
			sprocketR.localRotation = Quaternion.Euler(base.RolledRight / 0.34f * 57.29578f, 0f, 0f);
			idlerL.localRotation = Quaternion.Euler(base.RolledLeft / 0.32f * 57.29578f, 0f, 0f);
			idlerR.localRotation = Quaternion.Euler(base.RolledRight / 0.32f * 57.29578f, 0f, 0f);
			LayTrack(shoesL, -1, base.RolledLeft);
			LayTrack(shoesR, 1, base.RolledRight);
		}
		if (base.Wrecked && !tossed)
		{
			if (wreckedAt < 0f)
			{
				wreckedAt = Time.time + UnityEngine.Random.Range(2.5f, 4.5f);
			}
			else if (Time.time >= wreckedAt)
			{
				TossTurret();
			}
		}
	}

	private void TossTurret()
	{
		tossed = true;
		if (gun == null || !gun.Turret || UnityEngine.Random.value > 0.65f)
		{
			return;
		}
		Transform turret = gun.Turret;
		gun.Turret = null;
		gun.Cradle = null;
		gun.Gun = null;
		Vector3 position = turret.position;
		turret.SetParent(null, worldPositionStays: true);
		Rigidbody rigidbody = turret.gameObject.AddComponent<Rigidbody>();
		rigidbody.mass = 12000f;
		rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		rigidbody.velocity = (base.Body ? base.Body.velocity : Vector3.zero) + Vector3.up * UnityEngine.Random.Range(9f, 14f) + UnityEngine.Random.insideUnitSphere * 2.5f;
		rigidbody.angularVelocity = UnityEngine.Random.insideUnitSphere * 2.5f;
		if ((bool)ShotEffects.Instance)
		{
			ShotEffects.Instance.Explosion(position, 2.6f);
			ShotEffects.Instance.Flare(position, 1.6f);
		}
		if ((bool)Acoustics.Instance)
		{
			Acoustics.Instance.Explosion(position, rocket: true);
		}
		if ((bool)BattleDirector.Instance)
		{
			BattleDirector.Instance.Shake(position, 1.5f);
		}
		Transform obj = new GameObject("Turret ring fire").transform;
		obj.SetParent(base.transform, worldPositionStays: false);
		obj.localPosition = TurretPivot + Vector3.up * 0.1f;
		Plumes.Fire(obj, 0.9f, light: true);
		UnityEngine.Object.Destroy(obj.gameObject, UnityEngine.Random.Range(40, 70));
		UnityEngine.Object.Destroy(turret.gameObject, 300f);
	}

	private void LayTrack(Transform[] shoes, int side, float rolled)
	{
		loop.Clear();
		Vector2 vector = new Vector2(Front.z, Front.y);
		Vector2 vector2 = new Vector2(Rear.z, Rear.y);
		float num = 0.325f;
		float num2 = 0.345f;
		for (int i = 0; i <= 10; i++)
		{
			float num3 = (float)i / 10f;
			loop.Add(Vector2.Lerp(vector + Vector2.up * num, vector2 + Vector2.up * num2, num3) + Vector2.down * Mathf.Sin(num3 * MathF.PI) * 0.04f);
		}
		for (int j = 1; j <= 8; j++)
		{
			float f = (float)j / 8f * MathF.PI;
			loop.Add(vector2 + new Vector2(0f - Mathf.Sin(f), Mathf.Cos(f)) * num2);
		}
		for (int num4 = Stations.Length - 1; num4 >= 0; num4--)
		{
			Transform transform = WheelModel(num4 * 2 + ((side >= 0) ? 1 : 0));
			Vector3 vector3 = (transform ? base.transform.InverseTransformPoint(transform.position) : new Vector3(0f, 0.4f, Stations[num4]));
			float y = vector3.y - 0.35f - 0.01f;
			loop.Add(new Vector2(vector3.z - 0.16f, y));
			loop.Add(new Vector2(vector3.z + 0.16f, y));
		}
		for (int k = 0; k < 8; k++)
		{
			float f2 = (float)k / 8f * MathF.PI;
			loop.Add(vector + new Vector2(Mathf.Sin(f2), 0f - Mathf.Cos(f2)) * num);
		}
		along.Clear();
		float num5 = 0f;
		for (int l = 0; l < loop.Count; l++)
		{
			along.Add(num5);
			num5 += Vector2.Distance(loop[l], loop[(l + 1) % loop.Count]);
		}
		float num6 = num5 / (float)shoes.Length;
		float num7 = Mathf.Repeat(0f - rolled, num5);
		int m = 0;
		float x = (float)side * 1.4f;
		for (int n = 0; n < shoes.Length; n++)
		{
			float num8 = Mathf.Repeat((float)n * num6 + num7, num5);
			while (m > 0 && along[m] > num8)
			{
				m--;
			}
			for (; m + 1 < loop.Count && along[m + 1] <= num8; m++)
			{
			}
			Vector2 vector4 = loop[m];
			Vector2 vector5 = loop[(m + 1) % loop.Count];
			float num9 = Vector2.Distance(vector4, vector5);
			Vector2 vector6 = ((num9 > 0.0001f) ? Vector2.Lerp(vector4, vector5, (num8 - along[m]) / num9) : vector4);
			Vector2 vector7 = ((num9 > 0.0001f) ? ((vector5 - vector4) / num9) : Vector2.right);
			Vector2 vector8 = new Vector2(vector7.y, 0f - vector7.x);
			shoes[n].localPosition = new Vector3(x, vector6.y, vector6.x);
			shoes[n].localRotation = Quaternion.LookRotation(new Vector3(0f, vector7.y, vector7.x), new Vector3(0f, vector8.y, vector8.x));
			if (n + 1 < shoes.Length && Mathf.Repeat((float)(n + 1) * num6 + num7, num5) < num8)
			{
				m = 0;
			}
		}
	}

	private Transform WheelModel(int i)
	{
		if (wheelModelsCache == null || i >= wheelModelsCache.Length)
		{
			return null;
		}
		return wheelModelsCache[i];
	}

	protected override void Colliders()
	{
		Box("Belly", new Vector3(0f, 0.5f, 0f), new Vector3(2.1f, 0.12f, 6.3f));
		Vehicle.Skid(colliders[colliders.Count - 1]);
		int[] array = new int[2] { -1, 1 };
		foreach (int num in array)
		{
			Box("Lower side", new Vector3((float)num * 1.05f, 0.8f, -0.1f), new Vector3(0.06f, 0.6f, 6.2f));
			Box("Fender", new Vector3((float)num * 1.42f, 1.12f, 0f), new Vector3(0.66f, 0.06f, 6.8f));
			Box("Skirt", new Vector3((float)num * 1.75f, 0.94f, 0.1f), new Vector3(0.04f, 0.3f, 6.2f));
		}
		Box("Upper hull", new Vector3(0f, 1.3f, -0.45f), new Vector3(3.5f, 0.4f, 5.9f));
		Box("Glacis", new Vector3(0f, 1.17f, 2.93f), new Vector3(2.2f, 0.08f, 1.24f), Quaternion.Euler(32.2f, 0f, 0f));
		Box("Lower plate", new Vector3(0f, 0.6f, 3.22f), new Vector3(2.1f, 0.08f, 0.55f), Quaternion.Euler(-35f, 0f, 0f));
		Box("Engine", spec.EngineCentre, spec.EngineSize);
		Box("Fuel drums", new Vector3(0f, 1.25f, -3.62f), new Vector3(2.3f, 0.6f, 0.6f));
		if (gun != null && (bool)gun.Turret)
		{
			Box("Turret", new Vector3(0f, 0.3f, 0f), new Vector3(2.3f, 0.6f, 2.3f), null, gun.Turret);
			Box("Turret front", new Vector3(0f, 0.3f, 1.1f), new Vector3(1.9f, 0.5f, 0.5f), null, gun.Turret);
		}
	}

	protected override void Model(out Transform[] wheelModels, out Transform steering)
	{
		if (!hull)
		{
			Meshes();
		}
		Material material = Armour(Team);
		Transform model = new GameObject("Model").transform;
		model.SetParent(base.transform, worldPositionStays: false);
		Vehicle.Draw(model, "Hull", hull, material);
		Vehicle.Draw(model, "Fittings", fittings, steel);
		Vehicle.Draw(model, "Side skirts", skirts, rubber);
		Vehicle.Draw(model, "Fuel drums", drums, drumPaint);
		Vehicle.Draw(model, "Unditching log", log, wood);
		Vehicle.Draw(model, "Lamps", glass, lamp);
		Transform parent = gun.Build(model);
		Vehicle.Draw(gun.Turret, "Turret", turretMesh, material);
		Vehicle.Draw(gun.Turret, "Turret fittings", turretKit, dark);
		Vehicle.Draw(parent, "Mantlet", mantlet, material);
		Vehicle.Draw(parent, "2A46M", barrel, gunMetal);
		wheelModels = new Transform[spec.Wheels.Length];
		for (int i = 0; i < wheelModels.Length; i++)
		{
			Transform transform = new GameObject("Road wheel " + i).transform;
			transform.SetParent(base.transform, worldPositionStays: false);
			Transform transform2 = new GameObject("Wheel").transform;
			transform2.SetParent(transform, worldPositionStays: false);
			if (spec.Wheels[i].At.x < 0f)
			{
				transform2.localScale = new Vector3(-1f, 1f, 1f);
			}
			Vehicle.Draw(transform2, "Tyres", roadRubber, rubber);
			Vehicle.Draw(transform2, "Hub", roadHub, material);
			wheelModels[i] = transform;
		}
		wheelModelsCache = wheelModels;
		sprocketL = Spin("Sprocket", sprocketMesh, Rear, -1);
		sprocketR = Spin("Sprocket", sprocketMesh, Rear, 1);
		idlerL = Spin("Idler", idlerMesh, Front, -1);
		idlerR = Spin("Idler", idlerMesh, Front, 1);
		shoesL = Shoes(model, "Left track");
		shoesR = Shoes(model, "Right track");
		steering = null;
		Transform Spin(string name, Mesh mesh, Vector3 at, int sx)
		{
			Transform transform3 = new GameObject(name).transform;
			transform3.SetParent(model, worldPositionStays: false);
			transform3.localPosition = new Vector3((float)sx * 1.4f, at.y, at.z);
			Transform transform4 = new GameObject("Wheel").transform;
			transform4.SetParent(transform3, worldPositionStays: false);
			if (sx < 0)
			{
				transform4.localScale = new Vector3(-1f, 1f, 1f);
			}
			Vehicle.Draw(transform4, name, mesh, steel);
			return transform3;
		}
	}

	private static Transform[] Shoes(Transform model, string name)
	{
		Transform transform = new GameObject(name).transform;
		transform.SetParent(model, worldPositionStays: false);
		Transform[] array = new Transform[82];
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = new GameObject("Shoe");
			gameObject.transform.SetParent(transform, worldPositionStays: false);
			gameObject.AddComponent<MeshFilter>().sharedMesh = shoeMesh;
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.sharedMaterial = trackSteel;
			meshRenderer.shadowCastingMode = ((i % 2 == 0) ? ShadowCastingMode.On : ShadowCastingMode.Off);
			array[i] = gameObject.transform;
		}
		return array;
	}

	private static Material Armour(int team)
	{
		int num = ((team == 1) ? 1 : 0);
		if (!armour[num])
		{
			armour[num] = ((num == 0) ? Vehicle.Paint("Olive armour (T-72)", new Color(0.24f, 0.27f, 0.16f), 0.2f, 0.1f) : Vehicle.Paint("Khaki armour (T-72)", new Color(0.4f, 0.37f, 0.25f), 0.2f, 0.1f));
		}
		return armour[num];
	}

	private static Vector2[] Section(float bottom, float top)
	{
		float num = Mathf.Min(1.1f, top - 0.03f);
		return new Vector2[8]
		{
			new Vector2(0f, bottom),
			new Vector2(1.02f, bottom),
			new Vector2(1.08f, Mathf.Min(bottom + 0.06f, num - 0.01f)),
			new Vector2(1.08f, num),
			new Vector2(1.75f, num),
			new Vector2(1.75f, top - 0.04f),
			new Vector2(1.71f, top),
			new Vector2(0f, top)
		};
	}

	private static float GlacisAt(float z)
	{
		return 0.84f + (3.45f - z) / 1.05f * 0.66f;
	}

	private static void Meshes()
	{
		steel = Vehicle.Paint("Steel running gear (T-72)", new Color(0.16f, 0.16f, 0.14f), 0.35f, 0.45f);
		rubber = Vehicle.Paint("Rubber skirts and tyres (T-72)", new Color(0.04f, 0.04f, 0.038f), 0.12f);
		dark = Vehicle.Paint("Black steel (T-72)", new Color(0.06f, 0.06f, 0.055f), 0.3f, 0.4f);
		wood = Vehicle.Paint("Unditching log wood (T-72)", new Color(0.33f, 0.22f, 0.12f), 0.15f);
		trackSteel = Vehicle.Paint("Track steel (T-72)", new Color(0.12f, 0.11f, 0.1f), 0.3f, 0.5f);
		trackSteel.enableInstancing = true;
		gunMetal = Vehicle.Paint("Gun steel (2A46M)", new Color(0.2f, 0.22f, 0.15f), 0.25f, 0.3f);
		lamp = Vehicle.Paint("Lamp glass (T-72)", new Color(0.82f, 0.8f, 0.7f), 0.9f);
		drumPaint = Vehicle.Paint("Fuel drum paint (T-72)", new Color(0.2f, 0.23f, 0.14f), 0.3f, 0.2f);
		VehicleMesh vehicleMesh = new VehicleMesh();
		vehicleMesh.Hull(new(float, Vector2[])[5]
		{
			(3.45f, Section(0.72f, 0.84f)),
			(3.1f, Section(0.45f, GlacisAt(3.1f))),
			(2.4f, Section(0.45f, 1.5f)),
			(-3.25f, Section(0.45f, 1.5f)),
			(-3.4f, Section(0.6f, 1.42f))
		});
		int[] array = new int[2] { -1, 1 };
		foreach (int num in array)
		{
			vehicleMesh.Box(new Vector3((float)num * 1.42f, 1.02f, 3.55f), new Vector3(0.62f, 0.03f, 0.42f), Quaternion.Euler(20f, 0f, 0f));
			vehicleMesh.Box(new Vector3((float)num * 1.42f, 1.18f, -3.45f), new Vector3(0.62f, 0.03f, 0.2f), Quaternion.identity);
		}
		vehicleMesh.Box(new Vector3(1.42f, 1.28f, -0.9f), new Vector3(0.5f, 0.34f, 2.8f), Quaternion.identity);
		vehicleMesh.Box(new Vector3(-1.42f, 1.24f, -1.4f), new Vector3(0.5f, 0.26f, 1.2f), Quaternion.identity);
		vehicleMesh.Box(new Vector3(-1.42f, 1.24f, 0.4f), new Vector3(0.5f, 0.26f, 1.2f), Quaternion.identity);
		Vector3 normalized = new Vector3(0f, 1.05f, 0.66f).normalized;
		for (int j = 0; j < 2; j++)
		{
			for (int k = -2; k <= 2; k++)
			{
				float num2 = 2.75f + (float)j * 0.38f;
				vehicleMesh.Box(new Vector3((float)k * 0.38f, GlacisAt(num2), num2) + normalized * 0.07f, new Vector3(0.34f, 0.1f, 0.34f), Quaternion.FromToRotation(Vector3.up, normalized));
			}
		}
		vehicleMesh.Box(new Vector3(0f, 1.51f, -2.3f), new Vector3(1.8f, 0.04f, 1.6f), Quaternion.identity);
		vehicleMesh.Box(new Vector3(0f, 1.53f, 2.4f), new Vector3(0.55f, 0.05f, 0.5f), Quaternion.identity);
		hull = vehicleMesh.Build("T-72 hull");
		VehicleMesh vehicleMesh2 = new VehicleMesh();
		for (float num3 = -3f; num3 < -1.6f; num3 += 0.12f)
		{
			vehicleMesh2.Box(new Vector3(0f, 1.545f, num3), new Vector3(1.7f, 0.025f, 0.05f), Quaternion.Euler(25f, 0f, 0f));
		}
		array = new int[2] { -1, 1 };
		foreach (int num4 in array)
		{
			vehicleMesh2.Tube(new Vector3[4]
			{
				new Vector3((float)num4 * 1.62f, 1.52f, 2.2f),
				new Vector3((float)num4 * 1.66f, 1.52f, 2f),
				new Vector3((float)num4 * 1.66f, 1.52f, -2.8f),
				new Vector3((float)num4 * 1.62f, 1.52f, -3f)
			}, 0.025f, 6);
			vehicleMesh2.Tube(new Vector3[2]
			{
				new Vector3((float)num4 * 0.7f, 0.7f, 3.5f),
				new Vector3((float)num4 * 0.7f, 0.7f, 3.62f)
			}, 0.07f, 10);
			for (float num5 = -1.2f; num5 <= 1.21f; num5 += 1.2f)
			{
				vehicleMesh2.Round(new Vector3((float)num4 * 1.4f, 1.0f, num5), Vector3.right, new Vector2[4]
				{
					new Vector2(-0.12f, 0f),
					new Vector2(-0.12f, 0.1f),
					new Vector2(0.12f, 0.1f),
					new Vector2(0.12f, 0f)
				}, 12);
			}
		}
		fittings = vehicleMesh2.Build("T-72 fittings");
		VehicleMesh vehicleMesh3 = new VehicleMesh();
		array = new int[2] { -1, 1 };
		foreach (int num6 in array)
		{
			for (float num7 = -2.9f; num7 < 3.2f; num7 += 0.62f)
			{
				vehicleMesh3.Box(new Vector3((float)num6 * 1.76f, 0.93f, num7 + 0.3f), new Vector3(0.025f, 0.32f, 0.6f), Quaternion.Euler(0f, 0f, (float)num6 * 3f));
			}
		}
		skirts = vehicleMesh3.Build("T-72 side skirts");
		VehicleMesh vehicleMesh4 = new VehicleMesh();
		array = new int[2] { -1, 1 };
		foreach (int num8 in array)
		{
			vehicleMesh4.Round(new Vector3((float)num8 * 0.6f, 1.25f, -3.66f), Vector3.right, new Vector2[6]
			{
				new Vector2(-0.45f, 0f),
				new Vector2(-0.45f, 0.29f),
				new Vector2(-0.45f, 0.29f),
				new Vector2(0.45f, 0.29f),
				new Vector2(0.45f, 0.29f),
				new Vector2(0.45f, 0f)
			}, 18);
		}
		drums = vehicleMesh4.Build("T-72 fuel drums");
		VehicleMesh vehicleMesh5 = new VehicleMesh();
		vehicleMesh5.Tube(new Vector3[2]
		{
			new Vector3(-1.4f, 1.68f, -3.25f),
			new Vector3(1.4f, 1.68f, -3.25f)
		}, 0.13f, 12);
		log = vehicleMesh5.Build("T-72 unditching log");
		VehicleMesh vehicleMesh6 = new VehicleMesh();
		array = new int[2] { -1, 1 };
		foreach (int num9 in array)
		{
			vehicleMesh6.Round(new Vector3((float)num9 * 0.7f, 0.7f, 3.62f), Vector3.forward, new Vector2[4]
			{
				new Vector2(0f, 0f),
				new Vector2(0f, 0.06f),
				new Vector2(0.01f, 0.06f),
				new Vector2(0.015f, 0f)
			}, 12);
		}
		glass = vehicleMesh6.Build("T-72 lamps");
		VehicleMesh vehicleMesh7 = new VehicleMesh();
		vehicleMesh7.Round(Vector3.zero, Vector3.up, new Vector2[10]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 1.2f),
			new Vector2(0f, 1.2f),
			new Vector2(0.14f, 1.17f),
			new Vector2(0.14f, 1.17f),
			new Vector2(0.42f, 1.0f),
			new Vector2(0.42f, 1.0f),
			new Vector2(0.62f, 0.6f),
			new Vector2(0.62f, 0.6f),
			new Vector2(0.68f, 0f)
		}, 30);
		array = new int[2] { -1, 1 };
		foreach (int num10 in array)
		{
			vehicleMesh7.Box(new Vector3((float)num10 * 0.62f, 0.4f, 0.92f), new Vector3(0.62f, 0.42f, 0.42f), Quaternion.Euler(-12f, (float)num10 * 28f, 0f));
			vehicleMesh7.Box(new Vector3((float)num10 * 0.55f, 0.66f, 0.55f), new Vector3(0.55f, 0.1f, 0.5f), Quaternion.Euler(-8f, (float)num10 * 20f, 0f));
		}
		vehicleMesh7.Box(new Vector3(0f, 0.35f, -1.12f), new Vector3(1.5f, 0.36f, 0.36f), Quaternion.identity);
		vehicleMesh7.Round(new Vector3(0.45f, 0.62f, -0.15f), Vector3.up, new Vector2[6]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 0.33f),
			new Vector2(0f, 0.33f),
			new Vector2(0.16f, 0.33f),
			new Vector2(0.16f, 0.33f),
			new Vector2(0.17f, 0f)
		}, 18);
		vehicleMesh7.Round(new Vector3(-0.45f, 0.62f, -0.1f), Vector3.up, new Vector2[6]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 0.29f),
			new Vector2(0f, 0.29f),
			new Vector2(0.08f, 0.29f),
			new Vector2(0.08f, 0.29f),
			new Vector2(0.09f, 0f)
		}, 18);
		vehicleMesh7.Box(new Vector3(-0.55f, 0.74f, 0.42f), new Vector3(0.28f, 0.28f, 0.38f), Quaternion.identity);
		turretMesh = vehicleMesh7.Build("T-72 turret");
		VehicleMesh vehicleMesh8 = new VehicleMesh();
		array = new int[2] { -1, 1 };
		foreach (int num11 in array)
		{
			for (int l = 0; l < 4; l++)
			{
				Vector3 vector = new Vector3((float)num11 * (0.95f + (float)l * 0.05f), 0.52f + (float)(l % 2) * 0.1f, 0.35f - (float)l * 0.18f);
				vehicleMesh8.Tube(new Vector3[2]
				{
					vector,
					vector + new Vector3((float)num11 * 0.12f, 0.12f, 0.2f)
				}, 0.045f, 8);
			}
		}
		vehicleMesh8.Tube(new Vector3[2]
		{
			new Vector3(0.45f, 0.98f, -0.05f),
			new Vector3(0.45f, 1.0f, 0.95f)
		}, 0.022f, 8);
		vehicleMesh8.Box(new Vector3(0.45f, 0.96f, 0.05f), new Vector3(0.12f, 0.14f, 0.35f), Quaternion.identity);
		vehicleMesh8.Tube(new Vector3[2]
		{
			new Vector3(0.45f, 0.95f, -0.1f),
			new Vector3(0.45f, 0.82f, -0.1f)
		}, 0.03f, 8);
		vehicleMesh8.Tube(new Vector3[3]
		{
			new Vector3(-0.75f, 0.55f, -0.65f),
			new Vector3(-0.76f, 1.8f, -0.7f),
			new Vector3(-0.78f, 3.2f, -0.8f)
		}, 0.008f, 4);
		turretKit = vehicleMesh8.Build("T-72 turret fittings");
		VehicleMesh vehicleMesh9 = new VehicleMesh();
		vehicleMesh9.Box(new Vector3(0f, 0f, 0.05f), new Vector3(0.6f, 0.44f, 0.42f), Quaternion.identity);
		vehicleMesh9.Round(new Vector3(0f, 0f, 0.25f), Vector3.forward, new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 0.2f),
			new Vector2(0.18f, 0.16f),
			new Vector2(0.2f, 0f)
		}, 16);
		mantlet = vehicleMesh9.Build("T-72 mantlet");
		VehicleMesh vehicleMesh10 = new VehicleMesh();
		vehicleMesh10.Tube(new Vector3[2]
		{
			new Vector3(0f, 0f, 0.3f),
			new Vector3(0f, 0f, 4.8f)
		}, 0.085f, 14);
		vehicleMesh10.Tube(new Vector3[2]
		{
			new Vector3(0f, 0f, 0.4f),
			new Vector3(0f, 0f, 2f)
		}, 0.11f, 14);
		vehicleMesh10.Tube(new Vector3[2]
		{
			new Vector3(0f, 0f, 2f),
			new Vector3(0f, 0f, 2.45f)
		}, 0.15f, 16);
		vehicleMesh10.Tube(new Vector3[2]
		{
			new Vector3(0f, 0f, 2.45f),
			new Vector3(0f, 0f, 3.6f)
		}, 0.105f, 14);
		vehicleMesh10.Tube(new Vector3[2]
		{
			new Vector3(0f, 0f, 4.68f),
			new Vector3(0f, 0f, 4.8f)
		}, 0.1f, 14);
		vehicleMesh10.Tube(new Vector3[2]
		{
			new Vector3(0.24f, -0.02f, 0.2f),
			new Vector3(0.24f, -0.02f, 0.62f)
		}, 0.022f, 8);
		barrel = vehicleMesh10.Build("2A46M barrel");
		VehicleMesh vehicleMesh11 = new VehicleMesh();
		float[] array2 = new float[2] { -0.26f, 0.04f };
		foreach (float num12 in array2)
		{
			vehicleMesh11.Round(Vector3.zero, Vector3.right, new Vector2[10]
			{
				new Vector2(num12, 0f),
				new Vector2(num12, 0.33f),
				new Vector2(num12, 0.33f),
				new Vector2(num12 + 0.02f, 0.35f),
				new Vector2(num12 + 0.02f, 0.35f),
				new Vector2(num12 + 0.2f, 0.35f),
				new Vector2(num12 + 0.2f, 0.35f),
				new Vector2(num12 + 0.22f, 0.33f),
				new Vector2(num12 + 0.22f, 0.33f),
				new Vector2(num12 + 0.22f, 0f)
			}, 24);
		}
		roadRubber = vehicleMesh11.Build("T-72 road wheel tyres");
		VehicleMesh vehicleMesh12 = new VehicleMesh();
		vehicleMesh12.Round(Vector3.zero, Vector3.right, new Vector2[8]
		{
			new Vector2(-0.285f, 0f),
			new Vector2(-0.285f, 0.1f),
			new Vector2(-0.285f, 0.1f),
			new Vector2(-0.27f, 0.3f),
			new Vector2(-0.27f, 0.3f),
			new Vector2(0.27f, 0.3f),
			new Vector2(0.27f, 0.3f),
			new Vector2(0.28f, 0f)
		}, 18);
		for (int n = 0; n < 8; n++)
		{
			float f = (float)(n * 45) * (MathF.PI / 180f);
			vehicleMesh12.Box(new Vector3(-0.29f, Mathf.Cos(f) * 0.2f, Mathf.Sin(f) * 0.2f), new Vector3(0.02f, 0.05f, 0.05f), Quaternion.identity);
		}
		roadHub = vehicleMesh12.Build("T-72 road wheel hubs");
		VehicleMesh vehicleMesh13 = new VehicleMesh();
		array2 = new float[2] { -0.24f, 0.14f };
		foreach (float num13 in array2)
		{
			vehicleMesh13.Round(Vector3.zero, Vector3.right, new Vector2[5]
			{
				new Vector2(num13, 0f),
				new Vector2(num13, 0.26f),
				new Vector2(num13, 0.26f),
				new Vector2(num13 + 0.08f, 0.26f),
				new Vector2(num13 + 0.08f, 0f)
			}, 22);
		}
		vehicleMesh13.Round(Vector3.zero, Vector3.right, new Vector2[6]
		{
			new Vector2(-0.3f, 0f),
			new Vector2(-0.3f, 0.12f),
			new Vector2(-0.3f, 0.12f),
			new Vector2(0.3f, 0.12f),
			new Vector2(0.3f, 0.12f),
			new Vector2(0.3f, 0f)
		}, 12);
		for (int num14 = 0; num14 < 12; num14++)
		{
			Quaternion quaternion = Quaternion.Euler(num14 * 30, 0f, 0f);
			array2 = new float[2] { -0.2f, 0.18f };
			foreach (float x in array2)
			{
				vehicleMesh13.Box(quaternion * new Vector3(x, 0.3f, 0f), new Vector3(0.08f, 0.08f, 0.07f), quaternion);
			}
		}
		sprocketMesh = vehicleMesh13.Build("T-72 sprocket");
		VehicleMesh vehicleMesh14 = new VehicleMesh();
		array2 = new float[2] { -0.24f, 0.06f };
		foreach (float num15 in array2)
		{
			vehicleMesh14.Round(Vector3.zero, Vector3.right, new Vector2[6]
			{
				new Vector2(num15, 0f),
				new Vector2(num15, 0.3f),
				new Vector2(num15, 0.3f),
				new Vector2(num15 + 0.18f, 0.3f),
				new Vector2(num15 + 0.18f, 0.3f),
				new Vector2(num15 + 0.18f, 0f)
			}, 22);
		}
		idlerMesh = vehicleMesh14.Build("T-72 idler");
		VehicleMesh vehicleMesh15 = new VehicleMesh();
		vehicleMesh15.Box(Vector3.zero, new Vector3(0.58f, 0.04f, 0.15f), Quaternion.identity);
		array2 = new float[2] { -0.17f, 0.17f };
		foreach (float x2 in array2)
		{
			vehicleMesh15.Box(new Vector3(x2, 0.035f, 0f), new Vector3(0.18f, 0.03f, 0.05f), Quaternion.identity);
		}
		vehicleMesh15.Box(new Vector3(0f, -0.05f, 0f), new Vector3(0.06f, 0.07f, 0.07f), Quaternion.identity);
		shoeMesh = vehicleMesh15.Build("T-72 track shoe");
	}
}
