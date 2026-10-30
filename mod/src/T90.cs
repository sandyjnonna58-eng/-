using UnityEngine;

namespace CombatSandbox.Demo;

public sealed class T90 : Tank
{
	public static readonly WeaponDef Cannon125 = Tank.Cannon("2a46m5", "2A46M-5 125 mm", "125 mm");

	private static Material paint;

	private static Material steel;

	private static Material rubber;

	private static Material dark;

	private static Material trackSteel;

	private static Material gunMetal;

	protected override Vector3 TurretPivot => T90Meshes.Pivot;

	protected override Vector3 FlagMount => new Vector3(-1.2f, 1.55f, -3.05f);

	protected override float FlagLength => 1.8f;

	protected override void Describe()
	{
		stations = T90Meshes.Stations;
		idlerAt = T90Meshes.Front;
		sprocketAt = T90Meshes.Rear;
		idlerR = 0.3f;
		sprocketR = 0.32f;
		wheelR = 0.35f;
		trackX = 1.4f;
		shoeCount = 84;
		Vector3 pivot = T90Meshes.Pivot;
		spec = new Spec
		{
			Name = "T-90M",
			Mass = 46500f,
			CentreOfMass = new Vector3(0f, 0.85f, 0f),
			Wheels = Tank.TrackWheels(T90Meshes.Stations, 1.4f, 0.4f, 0.4f, 0.58f),
			Tracked = true,
			TrackWidth = 0.58f,
			PivotTorque = 440000f,
			Grip = 0.9f,
			Suspension = 0.25f,
			Spring = 395000f,
			Damper = 29000f,
			WheelMass = 150f,
			MaxTorque = 76000f,
			ReverseTorque = 32000f,
			Power = 620000f,
			TopSpeed = 17f,
			ReverseTop = 4.5f,
			BrakeTorque = 12500f,
			EngineBrake = 1500f,
			MaxSteer = 9f,
			AntiRoll = 230000f,
			Drag = 6f,
			SteerPivot = new Vector3(0f, 0.9f, 2.9f),
			SteerAxis = Vector3.up,
			SteerRatio = 1f,
			SteerInertia = 0.6f,
			SteerArms = 300f,
			EngineCentre = new Vector3(0f, 1f, -2.4f),
			EngineSize = new Vector3(1.9f, 0.8f, 1.6f),
			EngineHealth = 1200f,
			Armoured = true,
			ArmourEnergy = 33f,
			Hatches = new Vector3[3]
			{
				new Vector3(0f, 1.55f, 2.45f),
				pivot + new Vector3(0.45f, 0.75f, -0.5f),
				pivot + new Vector3(-0.42f, 0.75f, -0.35f)
			},
			EnginePitch = 0.45f,
			EngineLoudness = 2.2f,
			CameraOffset = new Vector3(0f, 4.2f, -13f)
		};
		places = CrewAndRiders(Tank.Inside(SeatRole.Driver, new Vector3(0f, 0.78f, 2.55f), new Vector3(0f, 1.5f, 2.4f), Vector3.left, 25f), Tank.Inside(SeatRole.Gunner, new Vector3(-0.42f, 1.05f, 0.25f), new Vector3(-0.45f, 1.6f, 0f), Vector3.left), 1f, 1.66f, 1.45f, 1.16f, new float[2] { -1.7f, -2.6f });
		gun = new GunMount(this, 1, pivot, Cannon125)
		{
			MaxTraverse = 180f,
			MinPitch = -6f,
			MaxPitch = 14f,
			GunLocal = T90Meshes.GunLocal,
			Muzzle = new Vector3(0f, 0f, 5f),
			Port = new Vector3(0f, 0f, 0.1f),
			Casings = false,
			Sight = new Vector3(-0.55f, 0.6f, -0.5f),
			GripsElevate = false,
			GripL = new Vector3(-0.6f, -0.25f, 0.35f),
			GripR = new Vector3(-0.3f, -0.25f, 0.35f),
			SeatTurn = 1f,
			Inertia = 30f,
			Laying = 160f,
			Damping = 140f,
			Cannon = true,
			Coax = WeaponCatalog.Get("pkm"),
			CoaxMuzzle = new Vector3(0.26f, -0.03f, 0.52f)
		};
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
			Box("Skirt", new Vector3((float)num * 1.78f, 0.92f, 0.1f), new Vector3(0.06f, 0.36f, 6.2f));
		}
		Box("Upper hull", new Vector3(0f, 1.3f, -0.45f), new Vector3(3.5f, 0.4f, 5.9f));
		Box("Glacis", new Vector3(0f, 1.2f, 2.93f), new Vector3(2.2f, 0.16f, 1.24f), Quaternion.Euler(32.2f, 0f, 0f));
		Box("Engine", spec.EngineCentre, spec.EngineSize);
		if (gun != null && (bool)gun.Turret)
		{
			Box("Turret", new Vector3(0f, 0.36f, -0.35f), new Vector3(2.5f, 0.72f, 2.7f), null, gun.Turret);
			Box("Turret front", new Vector3(0f, 0.36f, 1f), new Vector3(2.2f, 0.62f, 0.6f), null, gun.Turret);
		}
	}

	protected override void Model(out Transform[] wheelModels, out Transform steering)
	{
		if (!T90Meshes.Ready)
		{
			T90Meshes.Build();
		}
		if (!paint)
		{
			paint = Vehicle.Paint("Green armour (T-90M)", new Color(0.21f, 0.25f, 0.15f), 0.22f, 0.12f);
			steel = Vehicle.Paint("Steel running gear (T-90M)", new Color(0.15f, 0.15f, 0.13f), 0.35f, 0.45f);
			rubber = Vehicle.Paint("Rubber (T-90M)", new Color(0.04f, 0.04f, 0.038f), 0.12f);
			dark = Vehicle.Paint("Dark fittings (T-90M)", new Color(0.08f, 0.09f, 0.07f), 0.3f, 0.35f);
			trackSteel = Vehicle.Paint("Track steel (T-90M)", new Color(0.12f, 0.11f, 0.1f), 0.3f, 0.5f);
			gunMetal = Vehicle.Paint("Gun tube (T-90M)", new Color(0.19f, 0.22f, 0.14f), 0.25f, 0.25f);
		}
		Transform model = new GameObject("Model").transform;
		model.SetParent(base.transform, worldPositionStays: false);
		Vehicle.Draw(model, "Hull", T90Meshes.Hull, paint);
		Vehicle.Draw(model, "Reactive armour", T90Meshes.Era, paint);
		Vehicle.Draw(model, "Skirts", T90Meshes.Skirts, rubber);
		Vehicle.Draw(model, "Deck", T90Meshes.Deck, dark);
		Transform parent = gun.Build(model);
		Vehicle.Draw(gun.Turret, "Turret", T90Meshes.Turret, paint);
		Vehicle.Draw(gun.Turret, "Turret fittings", T90Meshes.TurretKit, dark);
		Vehicle.Draw(parent, "Mantlet", T90Meshes.Mantlet, paint);
		Vehicle.Draw(parent, "Gun", T90Meshes.Barrel, gunMetal);
		BuildRunningGear(model, T90Meshes.Tyres, T90Meshes.Hubs, paint, T90Meshes.Sprocket, T90Meshes.Idler, steel, rubber, T90Meshes.Shoe, trackSteel, out wheelModels);
		steering = null;
	}
}
