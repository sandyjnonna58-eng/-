using UnityEngine;

namespace CombatSandbox.Demo;

public sealed class Apache : Gunship
{
	public static readonly WeaponDef M230 = Ifv.Autocannon("m230", "M230 30 mm", "30×113 mm", 805f, 0.23f, 625f, 60, 15f, 0.5f, 2.4f, 2.6f);

	private static Material paint;

	private static Material glass;

	private static Material dark;

	private static Material stores;

	private static Material rubber;

	private static Material blades;

	protected override Vector3[] Pods => ApacheMeshes.Pods;

	protected override int RocketLoad => 38;

	protected override Vector3 FlagMount => new Vector3(0f, 2.5f, -8.2f);

	protected override void Describe()
	{
		Wheel[] array = new Wheel[4]
		{
			new Wheel
			{
				At = new Vector3(-1.05f, 0.32f, 3.3f),
				Radius = 0.32f,
				Width = 0.22f
			},
			new Wheel
			{
				At = new Vector3(1.05f, 0.32f, 3.3f),
				Radius = 0.32f,
				Width = 0.22f
			},
			new Wheel
			{
				At = new Vector3(-0.1f, 0.22f, -8.75f),
				Radius = 0.22f,
				Width = 0.12f
			},
			new Wheel
			{
				At = new Vector3(0.1f, 0.22f, -8.75f),
				Radius = 0.22f,
				Width = 0.12f
			}
		};
		spec = new Spec
		{
			Name = "AH-64D Apache",
			Mass = 7000f,
			CentreOfMass = new Vector3(0f, 1.7f, 0.6f),
			Wheels = array,
			Grip = 1f,
			Suspension = 0.2f,
			Spring = 95000f,
			Damper = 9500f,
			WheelMass = 35f,
			MaxTorque = 0f,
			ReverseTorque = 0f,
			Power = 1f,
			TopSpeed = 1f,
			ReverseTop = 1f,
			BrakeTorque = 9000f,
			EngineBrake = 9000f,
			MaxSteer = 1f,
			AntiRoll = 30000f,
			Drag = 3f,
			SteerPivot = new Vector3(0f, 1.2f, 3.3f),
			SteerAxis = Vector3.up,
			SteerRatio = 1f,
			SteerInertia = 0.6f,
			SteerArms = 300f,
			EngineCentre = new Vector3(0f, 2.45f, -0.8f),
			EngineSize = new Vector3(2.2f, 0.7f, 2.9f),
			EngineHealth = 360f,
			Armoured = true,
			ArmourEnergy = 6f,
			Hatches = new Vector3[2]
			{
				new Vector3(0f, 2.9f, 0.4f),
				new Vector3(0f, 2.6f, 4f)
			},
			EnginePitch = 1.15f,
			EngineLoudness = 2.3f,
			CameraOffset = new Vector3(0f, 4.6f, -16f)
		};
		places = new Place[2]
		{
			new Place
			{
				Role = SeatRole.Driver,
				Pelvis = new Vector3(0f, 1.95f, 3.6f),
				FootL = new Vector3(-0.13f, 1.6f, 4.1f),
				FootR = new Vector3(0.13f, 1.6f, 4.1f),
				Door = new Vector3(0.75f, 1.9f, 3.6f),
				Out = Vector3.right,
				Stand = 2.2f,
				Lean = 8f
			},
			new Place
			{
				Role = SeatRole.Gunner,
				Pelvis = new Vector3(0f, 1.45f, 5.3f),
				FootL = new Vector3(-0.13f, 1.1f, 5.8f),
				FootR = new Vector3(0.13f, 1.1f, 5.8f),
				Door = new Vector3(-0.75f, 1.4f, 5.3f),
				Out = Vector3.left,
				Stand = 2f,
				Lean = 6f
			}
		};
		atgmLoad = 8;
		atgmRails = new Vector3[8]
		{
			new Vector3(-2.3f, 1.45f, 1.3f),
			new Vector3(2.3f, 1.45f, 1.3f),
			new Vector3(-2f, 1.45f, 1.3f),
			new Vector3(2f, 1.45f, 1.3f),
			new Vector3(-2.3f, 1.22f, 1.3f),
			new Vector3(2.3f, 1.22f, 1.3f),
			new Vector3(-2f, 1.22f, 1.3f),
			new Vector3(2f, 1.22f, 1.3f)
		};
		gun = new GunMount(this, 1, ApacheMeshes.ChinGun, M230)
		{
			MaxTraverse = 100f,
			MinPitch = -60f,
			MaxPitch = 11f,
			GunLocal = Vector3.zero,
			Muzzle = new Vector3(0f, 0f, 1.75f),
			Port = new Vector3(0f, -0.05f, 0.2f),
			Casings = false,
			Sight = new Vector3(0f, 0.3f, 0.3f),
			GripsElevate = false,
			GripL = new Vector3(-0.2f, 0.4f, -0.3f),
			GripR = new Vector3(0.2f, 0.4f, -0.3f),
			SeatTurn = 0f,
			Inertia = 4f,
			Laying = 1800f,
			Damping = 120f,
			Cannon = true,
			Autocannon = true,
			ApEnergy = 18f
		};
	}

	protected override void Colliders()
	{
		Box("Belly", new Vector3(0f, 0.95f, 2.5f), new Vector3(1.1f, 0.1f, 6f));
		Vehicle.Skid(colliders[colliders.Count - 1]);
		int[] array = new int[2] { -1, 1 };
		foreach (int num in array)
		{
			Box("Sponson", new Vector3((float)num * 0.7f, 1.3f, 2.9f), new Vector3(0.45f, 0.65f, 4.6f));
			Box("Engine nacelle", new Vector3((float)num * 0.92f, 2.45f, -0.75f), new Vector3(0.6f, 0.66f, 2.9f));
		}
		Box("Fuselage", new Vector3(0f, 1.8f, 1.8f), new Vector3(1.15f, 1.6f, 6.4f));
		Box("Nose", new Vector3(0f, 1.3f, 6.6f), new Vector3(0.8f, 0.9f, 1.2f));
		Box("Tail", new Vector3(0f, 2.15f, -2.8f), new Vector3(0.85f, 1f, 2.4f));
		Box("Boom", new Vector3(0f, 2.3f, -6.4f), new Vector3(0.45f, 0.5f, 5.2f));
		Box("Fin", new Vector3(0f, 3.5f, -9.2f), new Vector3(0.2f, 1.9f, 1.2f));
		Box("Wing", new Vector3(0f, 1.95f, 0.35f), new Vector3(5.1f, 0.12f, 1f));
		Box("Engine", spec.EngineCentre, spec.EngineSize);
	}

	protected override void Model(out Transform[] wheelModels, out Transform steering)
	{
		if (!ApacheMeshes.Ready)
		{
			ApacheMeshes.Build();
		}
		if (!paint)
		{
			paint = Vehicle.Paint("Helicopter paint (AH-64)", new Color(0.17f, 0.19f, 0.14f), 0.25f, 0.1f);
			glass = Vehicle.Paint("Cockpit glass (AH-64)", new Color(0.12f, 0.16f, 0.18f), 0.92f, 0.3f);
			dark = Vehicle.Paint("Dark fittings (AH-64)", new Color(0.06f, 0.06f, 0.06f), 0.3f, 0.4f);
			stores = Vehicle.Paint("Weapon racks (AH-64)", new Color(0.22f, 0.23f, 0.19f), 0.3f, 0.25f);
			rubber = Vehicle.Paint("Tyres (AH-64)", new Color(0.04f, 0.04f, 0.04f), 0.1f);
			blades = Vehicle.Paint("Rotor blades (AH-64)", new Color(0.09f, 0.09f, 0.08f), 0.3f, 0.2f);
		}
		Transform transform = new GameObject("Model").transform;
		transform.SetParent(base.transform, worldPositionStays: false);
		Vehicle.Draw(transform, "Fuselage", ApacheMeshes.Fuselage, paint);
		Vehicle.Draw(transform, "Canopy", ApacheMeshes.Glass, glass);
		Vehicle.Draw(transform, "Fittings", ApacheMeshes.Fittings, dark);
		Vehicle.Draw(transform, "Weapons", ApacheMeshes.Stores, stores);
		rotor = new GameObject("Main rotor").transform;
		rotor.SetParent(transform, worldPositionStays: false);
		rotor.localPosition = ApacheMeshes.Mast;
		Vehicle.Draw(rotor, "Blades", ApacheMeshes.Rotor, blades);
		tailRotor = new GameObject("Tail rotor").transform;
		tailRotor.SetParent(transform, worldPositionStays: false);
		tailRotor.localPosition = ApacheMeshes.TailHub;
		Vehicle.Draw(tailRotor, "Blades", ApacheMeshes.TailRotor, blades);
		Vehicle.Draw(gun.Build(transform), "Chin gun", ApacheMeshes.Gun, dark);
		wheelModels = new Transform[spec.Wheels.Length];
		for (int i = 0; i < wheelModels.Length; i++)
		{
			Transform transform2 = new GameObject("Wheel " + i).transform;
			transform2.SetParent(base.transform, worldPositionStays: false);
			Vehicle.Draw(transform2, "Tyre", (i < 2) ? ApacheMeshes.Tyre : ApacheMeshes.TailTyre, rubber);
			wheelModels[i] = transform2;
		}
		steering = null;
	}
}
