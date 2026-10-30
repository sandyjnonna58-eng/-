using System.Collections.Generic;
using UnityEngine;

namespace CombatSandbox.Demo;

public sealed class Bradley : Ifv
{
	private static Material paint;

	private static Material trim;

	private static Material steel;

	private static Material rubber;

	private static Material dark;

	private static Material trackSteel;

	private static Material gunMetal;

	protected override Vector3 TurretPivot => BradleyMeshes.Pivot;

	protected override Vector3 FlagMount => new Vector3(-1.4f, 2f, -3f);

	protected override float FlagLength => 1.5f;

	protected override void Describe()
	{
		stations = BradleyMeshes.Stations;
		idlerAt = BradleyMeshes.Front;
		sprocketAt = BradleyMeshes.Rear;
		idlerR = 0.3f;
		sprocketR = 0.28f;
		wheelR = 0.32f;
		trackX = 1.36f;
		shoeCount = 95;
		Vector3 pivot = BradleyMeshes.Pivot;
		spec = new Spec
		{
			Name = "M2A2 Bradley",
			Mass = 27000f,
			CentreOfMass = new Vector3(0f, 0.95f, 0.2f),
			Wheels = Tank.TrackWheels(BradleyMeshes.Stations, 1.36f, 0.37f, 0.37f, 0.53f),
			Tracked = true,
			TrackWidth = 0.53f,
			PivotTorque = 220000f,
			Grip = 0.9f,
			Suspension = 0.25f,
			Spring = 260000f,
			Damper = 19000f,
			WheelMass = 100f,
			MaxTorque = 55000f,
			ReverseTorque = 24000f,
			Power = 330000f,
			TopSpeed = 17f,
			ReverseTop = 6f,
			BrakeTorque = 9000f,
			EngineBrake = 1000f,
			MaxSteer = 9f,
			AntiRoll = 140000f,
			Drag = 4.5f,
			SteerPivot = new Vector3(0f, 0.9f, 2.6f),
			SteerAxis = Vector3.up,
			SteerRatio = 1f,
			SteerInertia = 0.6f,
			SteerArms = 300f,
			EngineCentre = new Vector3(0.55f, 1.3f, 2f),
			EngineSize = new Vector3(1f, 0.9f, 1.4f),
			EngineHealth = 600f,
			Armoured = true,
			ArmourEnergy = 16f,
			Hatches = new Vector3[3]
			{
				new Vector3(-0.68f, 2.15f, 1.25f),
				pivot + new Vector3(0.45f, 0.82f, -0.45f),
				pivot + new Vector3(-0.4f, 0.79f, -0.25f)
			},
			EnginePitch = 0.6f,
			EngineLoudness = 1.9f,
			CameraOffset = new Vector3(0f, 3.8f, -11f)
		};
		List<Place> list = new List<Place>
		{
			Tank.Inside(SeatRole.Driver, new Vector3(-0.68f, 1.2f, 1.5f), new Vector3(-0.68f, 2.1f, 1.25f), Vector3.left, 20f),
			Tank.Inside(SeatRole.Gunner, new Vector3(-0.18f, 1.15f, 0.25f), new Vector3(-0.2f, 2.1f, 0.1f), Vector3.left)
		};
		Ifv.Troops(list, new float[3] { -1f, -1.75f, -2.5f }, 0.8f, 1f, 0.38f, 0.5f, outward: false, new Vector3(0f, 1f, -3.35f));
		places = list.ToArray();
		gun = new GunMount(this, 1, pivot, Ifv.M242)
		{
			MaxTraverse = 180f,
			MinPitch = -9f,
			MaxPitch = 59f,
			GunLocal = BradleyMeshes.GunLocal,
			Muzzle = new Vector3(0f, 0f, 2.45f),
			Port = new Vector3(0f, 0f, 0.1f),
			Casings = false,
			Sight = new Vector3(0.52f, 0.5f, -0.32f),
			GripsElevate = false,
			GripL = new Vector3(0.2f, -0.25f, 0.3f),
			GripR = new Vector3(0.5f, -0.25f, 0.3f),
			SeatTurn = 1f,
			Inertia = 12f,
			Laying = 160f,
			Damping = 88f,
			Cannon = true,
			Autocannon = true,
			ApEnergy = 24f,
			Coax = WeaponCatalog.Get("pkm"),
			CoaxMuzzle = new Vector3(0.22f, 0f, 0.3f)
		};
	}

	protected override void Colliders()
	{
		Box("Belly", new Vector3(0f, 0.5f, 0f), new Vector3(2.1f, 0.12f, 6.3f));
		Vehicle.Skid(colliders[colliders.Count - 1]);
		int[] array = new int[2] { -1, 1 };
		foreach (int num in array)
		{
			Box("Lower side", new Vector3((float)num * 1.06f, 0.78f, -0.1f), new Vector3(0.06f, 0.6f, 6.2f));
			Box("Sponson", new Vector3((float)num * 1.37f, 1.55f, -0.81f), new Vector3(0.6f, 0.95f, 4.82f));
			Box("Skirt", new Vector3((float)num * 1.69f, 0.88f, 0f), new Vector3(0.08f, 0.46f, 5.8f));
		}
		Box("Upper hull", new Vector3(0f, 1.5f, -0.8f), new Vector3(2.2f, 1f, 4.8f));
		Box("Glacis", new Vector3(0f, 1.48f, 2.35f), new Vector3(3.3f, 0.3f, 1.9f), Quaternion.Euler(25.2f, 0f, 0f));
		Box("Bow", new Vector3(0f, 0.85f, 2.9f), new Vector3(2.1f, 0.7f, 0.8f));
		Box("Engine", spec.EngineCentre, spec.EngineSize);
		if (gun != null && (bool)gun.Turret)
		{
			Box("Turret", new Vector3(0f, 0.37f, -0.1f), new Vector3(2f, 0.74f, 2.3f), null, gun.Turret);
			Box("TOW launcher", new Vector3(-1.22f, 0.42f, -0.25f), new Vector3(0.36f, 0.38f, 1.25f), null, gun.Turret);
		}
	}

	protected override void Model(out Transform[] wheelModels, out Transform steering)
	{
		if (!BradleyMeshes.Ready)
		{
			BradleyMeshes.Build();
		}
		if (!paint)
		{
			paint = Vehicle.Paint("Tan armour (Bradley)", new Color(0.42f, 0.38f, 0.27f), 0.2f, 0.1f);
			trim = Vehicle.Paint("Tan fittings (Bradley)", new Color(0.36f, 0.33f, 0.23f), 0.22f, 0.15f);
			steel = Vehicle.Paint("Steel running gear (Bradley)", new Color(0.15f, 0.15f, 0.13f), 0.35f, 0.45f);
			rubber = Vehicle.Paint("Rubber (Bradley)", new Color(0.04f, 0.04f, 0.038f), 0.12f);
			dark = Vehicle.Paint("Dark fittings (Bradley)", new Color(0.1f, 0.1f, 0.09f), 0.3f, 0.35f);
			trackSteel = Vehicle.Paint("Track steel (Bradley)", new Color(0.12f, 0.11f, 0.1f), 0.3f, 0.5f);
			gunMetal = Vehicle.Paint("Gun tube (Bradley)", new Color(0.3f, 0.28f, 0.21f), 0.25f, 0.25f);
		}
		Transform model = new GameObject("Model").transform;
		model.SetParent(base.transform, worldPositionStays: false);
		Vehicle.Draw(model, "Hull", BradleyMeshes.Hull, paint);
		Vehicle.Draw(model, "Side armour", BradleyMeshes.Armour, paint);
		Vehicle.Draw(model, "Deck", BradleyMeshes.Deck, trim);
		Transform parent = gun.Build(model);
		Vehicle.Draw(gun.Turret, "Turret", BradleyMeshes.Turret, paint);
		Vehicle.Draw(gun.Turret, "Turret fittings", BradleyMeshes.TurretKit, dark);
		Vehicle.Draw(gun.Turret, "TOW launcher", BradleyMeshes.Launcher, trim);
		Vehicle.Draw(parent, "Mantlet", BradleyMeshes.Mantlet, paint);
		Vehicle.Draw(parent, "Gun", BradleyMeshes.Barrel, gunMetal);
		BuildRunningGear(model, BradleyMeshes.Tyres, BradleyMeshes.Hubs, paint, BradleyMeshes.Idler, BradleyMeshes.Sprocket, steel, rubber, BradleyMeshes.Shoe, trackSteel, out wheelModels);
		steering = null;
	}
}
