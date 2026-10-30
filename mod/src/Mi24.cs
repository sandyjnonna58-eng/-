using UnityEngine;

namespace CombatSandbox.Demo;

public sealed class Mi24 : Gunship
{
	public static readonly WeaponDef YakB = new WeaponDef
	{
		Id = "yakb",
		Name = "YakB-12.7",
		Caliber = "12.7×108 mm",
		Class = WeaponClass.MachineGun,
		MuzzleVelocity = 860f,
		BulletMass = 0.048f,
		Drag = 0.0006f,
		Rpm = 1000f,
		Magazine = 200,
		Reload = 8f,
		Auto = true,
		Semi = false,
		Recoil = 2.4f,
		Dispersion = 4f,
		Mass = 45f,
		Zero = 300f,
		TracerEvery = 3,
		Tracer = new Color(0.45f, 1f, 0.3f),
		CaseKit = "ak74m",
		CaseScale = new Vector3(2.2f, 2.2f, 2.4f),
		Sound = GunSound.Sniper,
		Pitch = 0.75f,
		Loudness = 1.6f,
		Flash = 1.8f,
		Eject = new Vector3(0f, -1f, 0f),
		EjectSpeed = 1.5f,
		Burst = new Vector2Int(8, 16),
		EffectiveRange = 1500f
	};

	private static Material paint;

	private static Material glass;

	private static Material dark;

	private static Material stores;

	private static Material rubber;

	private static Material blades;

	protected override Vector3[] Pods => Mi24Meshes.Pods;

	protected override int RocketLoad => 40;

	protected override Vector3 FlagMount => new Vector3(0f, 2.75f, -9.6f);

	protected override void Describe()
	{
		Wheel[] array = new Wheel[4]
		{
			new Wheel
			{
				At = new Vector3(-0.2f, 0.3f, 4.9f),
				Radius = 0.3f,
				Width = 0.2f
			},
			new Wheel
			{
				At = new Vector3(0.2f, 0.3f, 4.9f),
				Radius = 0.3f,
				Width = 0.2f
			},
			new Wheel
			{
				At = new Vector3(-1.5f, 0.36f, -0.9f),
				Radius = 0.36f,
				Width = 0.25f
			},
			new Wheel
			{
				At = new Vector3(1.5f, 0.36f, -0.9f),
				Radius = 0.36f,
				Width = 0.25f
			}
		};
		spec = new Spec
		{
			Name = "Mi-24V",
			Mass = 8500f,
			CentreOfMass = new Vector3(0f, 1.6f, 0.4f),
			Wheels = array,
			Grip = 1f,
			Suspension = 0.2f,
			Spring = 110000f,
			Damper = 11000f,
			WheelMass = 40f,
			MaxTorque = 0f,
			ReverseTorque = 0f,
			Power = 1f,
			TopSpeed = 1f,
			ReverseTop = 1f,
			BrakeTorque = 9000f,
			EngineBrake = 9000f,
			MaxSteer = 1f,
			AntiRoll = 35000f,
			Drag = 3f,
			SteerPivot = new Vector3(0f, 1.2f, 4.9f),
			SteerAxis = Vector3.up,
			SteerRatio = 1f,
			SteerInertia = 0.6f,
			SteerArms = 300f,
			EngineCentre = new Vector3(0f, 3.05f, 0.1f),
			EngineSize = new Vector3(1.7f, 0.6f, 4.2f),
			EngineHealth = 420f,
			Armoured = true,
			ArmourEnergy = 9f,
			Hatches = new Vector3[3]
			{
				new Vector3(0f, 3.1f, 0.4f),
				new Vector3(0.95f, 1.6f, 1.3f),
				new Vector3(-0.95f, 1.6f, 1.3f)
			},
			EnginePitch = 1.05f,
			EngineLoudness = 2.5f,
			CameraOffset = new Vector3(0f, 5f, -18f)
		};
		places = new Place[6]
		{
			new Place
			{
				Role = SeatRole.Driver,
				Pelvis = new Vector3(0f, 1.95f, 3.75f),
				FootL = new Vector3(-0.13f, 1.6f, 4.25f),
				FootR = new Vector3(0.13f, 1.6f, 4.25f),
				Door = new Vector3(0.95f, 1.7f, 3.8f),
				Out = Vector3.right,
				Stand = 2.2f,
				Lean = 8f
			},
			new Place
			{
				Role = SeatRole.Gunner,
				Pelvis = new Vector3(0f, 1.4f, 5.5f),
				FootL = new Vector3(-0.13f, 1.05f, 5.95f),
				FootR = new Vector3(0.13f, 1.05f, 5.95f),
				Door = new Vector3(-0.9f, 1.3f, 5.4f),
				Out = Vector3.left,
				Stand = 1.9f,
				Lean = 6f
			},
			Bench(-1, 1.8f),
			Bench(-1, 0.9f),
			Bench(1, 1.8f),
			Bench(1, 0.9f)
		};
		atgmLoad = 4;
		atgmRails = new Vector3[4]
		{
			new Vector3(-3.36f, 1.2f, 0.7f),
			new Vector3(3.36f, 1.2f, 0.7f),
			new Vector3(-3.36f, 1.44f, 0.7f),
			new Vector3(3.36f, 1.44f, 0.7f)
		};
		gun = new GunMount(this, 1, Mi24Meshes.ChinGun, YakB)
		{
			MaxTraverse = 60f,
			MinPitch = -60f,
			MaxPitch = 15f,
			GunLocal = Vector3.zero,
			Muzzle = new Vector3(0f, 0f, 1.15f),
			Port = new Vector3(0f, -0.05f, 0.2f),
			Casings = false,
			GripsElevate = false,
			GripL = new Vector3(-0.2f, 0.4f, -0.3f),
			GripR = new Vector3(0.2f, 0.4f, -0.3f),
			SeatTurn = 0f,
			Inertia = 4f,
			Laying = 1800f,
			Damping = 120f
		};
		static Place Bench(int side, float z)
		{
			float num = -side;
			return new Place
			{
				Role = SeatRole.Holder,
				Pelvis = new Vector3((float)side * 0.55f, 1.35f, z),
				Yaw = -side * 90,
				Lean = 5f,
				FootL = new Vector3((float)side * 0.2f, 0.98f, z + 0.13f * num),
				FootR = new Vector3((float)side * 0.2f, 0.98f, z - 0.13f * num),
				HoldL = new Vector3((float)side * 0.42f, 1.28f, z + 0.25f * num),
				HoldR = new Vector3((float)side * 0.42f, 1.28f, z - 0.25f * num),
				Door = new Vector3((float)side * 0.95f, 1.2f, 1.3f),
				Out = Vector3.right * side,
				Stand = 1.9f
			};
		}
	}

	protected override void Colliders()
	{
		Box("Belly", new Vector3(0f, 0.95f, 1.5f), new Vector3(1.6f, 0.1f, 7f));
		Vehicle.Skid(colliders[colliders.Count - 1]);
		int[] array = new int[2] { -1, 1 };
		foreach (int num in array)
		{
			Box("Side", new Vector3((float)num * 0.88f, 1.9f, 0.9f), new Vector3(0.06f, 1.8f, 6.4f));
			Box("Wing", new Vector3((float)num * 2.05f, 1.55f, -0.75f), new Vector3(2.45f, 0.14f, 1f), Quaternion.Euler(0f, 0f, (float)num * -10f));
		}
		Box("Roof", new Vector3(0f, 2.88f, 0.5f), new Vector3(1.75f, 0.06f, 6f));
		Box("Nose", new Vector3(0f, 1.55f, 5.6f), new Vector3(1.3f, 1.3f, 2.4f));
		Box("Tail", new Vector3(0f, 2.2f, -3.2f), new Vector3(1.2f, 1f, 2f));
		Box("Boom", new Vector3(0f, 2.5f, -7.1f), new Vector3(0.5f, 0.5f, 6.6f));
		Box("Fin", new Vector3(0f, 3.4f, -10.5f), new Vector3(0.2f, 1.6f, 1.2f));
		Box("Engine", spec.EngineCentre, spec.EngineSize);
	}

	protected override void Model(out Transform[] wheelModels, out Transform steering)
	{
		if (!Mi24Meshes.Ready)
		{
			Mi24Meshes.Build();
		}
		if (!paint)
		{
			paint = Vehicle.Paint("Helicopter camouflage (Mi-24)", new Color(0.36f, 0.38f, 0.26f), 0.25f, 0.1f);
			glass = Vehicle.Paint("Cockpit glass (Mi-24)", new Color(0.12f, 0.17f, 0.2f), 0.92f, 0.3f);
			dark = Vehicle.Paint("Dark fittings (Mi-24)", new Color(0.07f, 0.07f, 0.065f), 0.3f, 0.4f);
			stores = Vehicle.Paint("Weapon pods (Mi-24)", new Color(0.28f, 0.3f, 0.22f), 0.3f, 0.25f);
			rubber = Vehicle.Paint("Tyres (Mi-24)", new Color(0.04f, 0.04f, 0.04f), 0.1f);
			blades = Vehicle.Paint("Rotor blades (Mi-24)", new Color(0.09f, 0.09f, 0.08f), 0.3f, 0.2f);
		}
		Transform transform = new GameObject("Model").transform;
		transform.SetParent(base.transform, worldPositionStays: false);
		Vehicle.Draw(transform, "Fuselage", Mi24Meshes.Fuselage, paint);
		Vehicle.Draw(transform, "Glazing", Mi24Meshes.Glass, glass);
		Vehicle.Draw(transform, "Fittings", Mi24Meshes.Fittings, dark);
		Vehicle.Draw(transform, "Weapons", Mi24Meshes.Stores, stores);
		rotor = new GameObject("Main rotor").transform;
		rotor.SetParent(transform, worldPositionStays: false);
		rotor.localPosition = Mi24Meshes.Mast;
		Vehicle.Draw(rotor, "Blades", Mi24Meshes.Rotor, blades);
		tailRotor = new GameObject("Tail rotor").transform;
		tailRotor.SetParent(transform, worldPositionStays: false);
		tailRotor.localPosition = Mi24Meshes.TailHub;
		Vehicle.Draw(tailRotor, "Blades", Mi24Meshes.TailRotor, blades);
		Vehicle.Draw(gun.Build(transform), "Chin gun", Mi24Meshes.Gun, dark);
		wheelModels = new Transform[spec.Wheels.Length];
		for (int i = 0; i < wheelModels.Length; i++)
		{
			Transform transform2 = new GameObject("Wheel " + i).transform;
			transform2.SetParent(base.transform, worldPositionStays: false);
			Vehicle.Draw(transform2, "Tyre", Mi24Meshes.Tyre, rubber);
			wheelModels[i] = transform2;
		}
		steering = null;
	}
}
