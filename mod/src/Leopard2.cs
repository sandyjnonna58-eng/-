using UnityEngine;

namespace CombatSandbox.Demo;

public sealed class Leopard2 : Tank
{
	public static readonly WeaponDef Cannon120 = Tank.Cannon("rh120l55", "Rh-120 L55 120 mm", "120 mm");

	private static Material paint;

	private static Material steel;

	private static Material rubber;

	private static Material dark;

	private static Material trackSteel;

	private static Material gunMetal;

	protected override Vector3 TurretPivot => Leopard2Meshes.Pivot;

	protected override Vector3 FlagMount => new Vector3(-1.3f, 1.44f, -3.55f);

	protected override float FlagLength => 1.8f;

	protected override void Describe()
	{
		stations = Leopard2Meshes.Stations;
		idlerAt = Leopard2Meshes.Front;
		sprocketAt = Leopard2Meshes.Rear;
		idlerR = 0.3f;
		sprocketR = 0.33f;
		wheelR = 0.33f;
		trackX = 1.47f;
		shoeCount = 96;
		Vector3 pivot = Leopard2Meshes.Pivot;
		spec = new Spec
		{
			Name = "Leopard 2A6",
			Mass = 62000f,
			CentreOfMass = new Vector3(0f, 0.85f, 0f),
			Wheels = Tank.TrackWheels(Leopard2Meshes.Stations, 1.47f, 0.37f, 0.37f, 0.63f),
			Tracked = true,
			TrackWidth = 0.63f,
			PivotTorque = 590000f,
			Grip = 0.9f,
			Suspension = 0.27f,
			Spring = 525000f,
			Damper = 39000f,
			WheelMass = 160f,
			MaxTorque = 100000f,
			ReverseTorque = 46000f,
			Power = 810000f,
			TopSpeed = 19f,
			ReverseTop = 8f,
			BrakeTorque = 15000f,
			EngineBrake = 1800f,
			MaxSteer = 9f,
			AntiRoll = 300000f,
			Drag = 6f,
			SteerPivot = new Vector3(0f, 0.9f, 3.3f),
			SteerAxis = Vector3.up,
			SteerRatio = 1f,
			SteerInertia = 0.6f,
			SteerArms = 300f,
			EngineCentre = new Vector3(0f, 1f, -2.6f),
			EngineSize = new Vector3(2f, 0.8f, 1.6f),
			EngineHealth = 1300f,
			Armoured = true,
			ArmourEnergy = 36f,
			Hatches = new Vector3[3]
			{
				new Vector3(0.5f, 1.45f, 2.7f),
				pivot + new Vector3(0.6f, 0.88f, -0.75f),
				pivot + new Vector3(-0.2f, 0.88f, -0.2f)
			},
			EnginePitch = 0.5f,
			EngineLoudness = 2.3f,
			CameraOffset = new Vector3(0f, 4.4f, -14f)
		};
		places = CrewAndRiders(Tank.Inside(SeatRole.Driver, new Vector3(0.5f, 0.75f, 2.9f), new Vector3(0.5f, 1.5f, 2.75f), Vector3.right, 25f), Tank.Inside(SeatRole.Gunner, new Vector3(0.5f, 1f, 0.3f), new Vector3(0.6f, 1.7f, -0.2f), Vector3.right), 1.55f, 1.58f, 1.98f, 1.12f, new float[2] { -2.95f, -3.35f });
		gun = new GunMount(this, 1, pivot, Cannon120)
		{
			MaxTraverse = 180f,
			MinPitch = -9f,
			MaxPitch = 20f,
			GunLocal = Leopard2Meshes.GunLocal,
			Muzzle = new Vector3(0f, 0f, 6.6f),
			Port = new Vector3(0f, 0f, 0.1f),
			Casings = false,
			Sight = new Vector3(0.62f, 0.62f, -0.1f),
			GripsElevate = false,
			GripL = new Vector3(0.3f, -0.25f, 0.35f),
			GripR = new Vector3(0.6f, -0.25f, 0.35f),
			SeatTurn = 1f,
			Inertia = 32f,
			Laying = 170f,
			Damping = 150f,
			Cannon = true,
			Coax = WeaponCatalog.Get("pkm"),
			CoaxMuzzle = new Vector3(0.28f, 0.02f, 0.3f)
		};
	}

	protected override void Colliders()
	{
		Box("Belly", new Vector3(0f, 0.5f, 0f), new Vector3(2.2f, 0.12f, 7.2f));
		Vehicle.Skid(colliders[colliders.Count - 1]);
		int[] array = new int[2] { -1, 1 };
		foreach (int num in array)
		{
			Box("Lower side", new Vector3((float)num * 1.1f, 0.8f, -0.1f), new Vector3(0.06f, 0.6f, 7.2f));
			Box("Sponson", new Vector3((float)num * 1.48f, 1.21f, -0.1f), new Vector3(0.72f, 0.42f, 7.3f));
			Box("Skirt", new Vector3((float)num * 1.88f, 0.78f, 0.1f), new Vector3(0.1f, 0.56f, 7f));
		}
		Box("Upper hull", new Vector3(0f, 1.2f, -0.55f), new Vector3(2.24f, 0.44f, 5.9f));
		Box("Glacis", new Vector3(0f, 1.2f, 3.1f), new Vector3(2.24f, 0.2f, 1.6f), Quaternion.Euler(14f, 0f, 0f));
		Box("Engine", spec.EngineCentre, spec.EngineSize);
		if (gun != null && (bool)gun.Turret)
		{
			Box("Turret", new Vector3(0f, 0.4f, -0.6f), new Vector3(3.6f, 0.8f, 4.2f), null, gun.Turret);
			Box("Turret front", new Vector3(0f, 0.38f, 1.65f), new Vector3(1.9f, 0.7f, 0.8f), null, gun.Turret);
		}
	}

	protected override void Model(out Transform[] wheelModels, out Transform steering)
	{
		if (!Leopard2Meshes.Ready)
		{
			Leopard2Meshes.Build();
		}
		if (!paint)
		{
			paint = Vehicle.Paint("Green armour (Leopard 2A6)", new Color(0.24f, 0.27f, 0.19f), 0.22f, 0.12f);
			steel = Vehicle.Paint("Steel running gear (Leopard 2A6)", new Color(0.15f, 0.15f, 0.13f), 0.35f, 0.45f);
			rubber = Vehicle.Paint("Rubber (Leopard 2A6)", new Color(0.04f, 0.04f, 0.038f), 0.12f);
			dark = Vehicle.Paint("Dark fittings (Leopard 2A6)", new Color(0.09f, 0.1f, 0.08f), 0.3f, 0.35f);
			trackSteel = Vehicle.Paint("Track steel (Leopard 2A6)", new Color(0.12f, 0.11f, 0.1f), 0.3f, 0.5f);
			gunMetal = Vehicle.Paint("Gun tube (Leopard 2A6)", new Color(0.22f, 0.25f, 0.18f), 0.25f, 0.25f);
		}
		Transform model = new GameObject("Model").transform;
		model.SetParent(base.transform, worldPositionStays: false);
		Vehicle.Draw(model, "Hull", Leopard2Meshes.Hull, paint);
		Vehicle.Draw(model, "Skirts", Leopard2Meshes.Skirts, paint);
		Vehicle.Draw(model, "Deck", Leopard2Meshes.Deck, dark);
		Transform parent = gun.Build(model);
		Vehicle.Draw(gun.Turret, "Turret", Leopard2Meshes.Turret, paint);
		Vehicle.Draw(gun.Turret, "Turret fittings", Leopard2Meshes.TurretKit, dark);
		Vehicle.Draw(parent, "Mantlet", Leopard2Meshes.Mantlet, paint);
		Vehicle.Draw(parent, "Gun", Leopard2Meshes.Barrel, gunMetal);
		BuildRunningGear(model, Leopard2Meshes.Tyres, Leopard2Meshes.Hubs, paint, Leopard2Meshes.Sprocket, Leopard2Meshes.Idler, steel, rubber, Leopard2Meshes.Shoe, trackSteel, out wheelModels);
		steering = null;
	}
}
