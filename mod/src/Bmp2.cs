using System.Collections.Generic;
using UnityEngine;

namespace CombatSandbox.Demo;

public sealed class Bmp2 : Ifv
{
	private static Material paint;

	private static Material trim;

	private static Material steel;

	private static Material rubber;

	private static Material dark;

	private static Material trackSteel;

	private static Material gunMetal;

	protected override Vector3 TurretPivot => Bmp2Meshes.Pivot;

	protected override Vector3 FlagMount => new Vector3(-1.2f, 1.55f, -3f);

	protected override float FlagLength => 1.5f;

	protected override void Describe()
	{
		stations = Bmp2Meshes.Stations;
		idlerAt = Bmp2Meshes.Front;
		sprocketAt = Bmp2Meshes.Rear;
		idlerR = 0.28f;
		sprocketR = 0.27f;
		wheelR = 0.36f;
		trackX = 1.27f;
		shoeCount = 100;
		Vector3 pivot = Bmp2Meshes.Pivot;
		spec = new Spec
		{
			Name = "BMP-2",
			Mass = 14300f,
			CentreOfMass = new Vector3(0f, 0.8f, 0.2f),
			Wheels = Tank.TrackWheels(Bmp2Meshes.Stations, 1.27f, 0.4f, 0.4f, 0.3f),
			Tracked = true,
			TrackWidth = 0.3f,
			PivotTorque = 95000f,
			Grip = 0.85f,
			Suspension = 0.24f,
			Spring = 150000f,
			Damper = 11000f,
			WheelMass = 60f,
			MaxTorque = 36000f,
			ReverseTorque = 16000f,
			Power = 175000f,
			TopSpeed = 18f,
			ReverseTop = 5f,
			BrakeTorque = 6000f,
			EngineBrake = 700f,
			MaxSteer = 9f,
			AntiRoll = 70000f,
			Drag = 3.5f,
			SteerPivot = new Vector3(0f, 0.9f, 2.6f),
			SteerAxis = Vector3.up,
			SteerRatio = 1f,
			SteerInertia = 0.6f,
			SteerArms = 300f,
			EngineCentre = new Vector3(0.5f, 1f, 2.2f),
			EngineSize = new Vector3(1f, 0.8f, 1.3f),
			EngineHealth = 450f,
			Armoured = true,
			ArmourEnergy = 13f,
			Hatches = new Vector3[3]
			{
				new Vector3(-0.62f, 1.6f, 1.15f),
				pivot + new Vector3(-0.4f, 0.55f, -0.38f),
				pivot + new Vector3(0.42f, 0.6f, -0.3f)
			},
			EnginePitch = 0.6f,
			EngineLoudness = 1.8f,
			CameraOffset = new Vector3(0f, 3.2f, -9.5f)
		};
		List<Place> list = new List<Place>
		{
			Tank.Inside(SeatRole.Driver, new Vector3(-0.62f, 0.78f, 1.55f), new Vector3(-0.62f, 1.55f, 1.15f), Vector3.left, 20f),
			Tank.Inside(SeatRole.Gunner, new Vector3(-0.38f, 1.1f, 0.25f), new Vector3(-0.4f, 1.6f, 0f), Vector3.left)
		};
		Ifv.Troops(list, new float[3] { -1f, -1.7f, -2.4f }, 0.3f, 0.72f, 0.72f, 0.48f, outward: true, new Vector3(0f, 0.95f, -3.4f));
		places = list.ToArray();
		atgmLoad = 4;
		atgmOnTurret = true;
		atgmRails = new Vector3[1]
		{
			new Vector3(0.02f, 0.86f, 0.75f)
		};
		gun = new GunMount(this, 1, pivot, Ifv.Gun2A42)
		{
			MaxTraverse = 180f,
			MinPitch = -5f,
			MaxPitch = 60f,
			GunLocal = Bmp2Meshes.GunLocal,
			Muzzle = new Vector3(0f, 0f, 3.05f),
			Port = new Vector3(0f, 0f, 0.1f),
			Casings = false,
			Sight = new Vector3(-0.42f, 0.29f, -0.35f),
			GripsElevate = false,
			GripL = new Vector3(-0.55f, -0.25f, 0.3f),
			GripR = new Vector3(-0.25f, -0.25f, 0.3f),
			SeatTurn = 1f,
			Inertia = 10f,
			Laying = 160f,
			Damping = 80f,
			Cannon = true,
			Autocannon = true,
			ApEnergy = 24f,
			Coax = WeaponCatalog.Get("pkm"),
			CoaxMuzzle = new Vector3(0.18f, 0.02f, 0.22f)
		};
	}

	protected override void Colliders()
	{
		Box("Belly", new Vector3(0f, 0.48f, 0f), new Vector3(2.1f, 0.12f, 6.2f));
		Vehicle.Skid(colliders[colliders.Count - 1]);
		int[] array = new int[2] { -1, 1 };
		foreach (int num in array)
		{
			Box("Lower side", new Vector3((float)num * 1.06f, 0.75f, -0.1f), new Vector3(0.06f, 0.55f, 6.2f));
			Box("Sponson", new Vector3((float)num * 1.27f, 1.24f, -0.6f), new Vector3(0.4f, 0.58f, 5.4f));
		}
		Box("Upper hull", new Vector3(0f, 1.25f, -0.85f), new Vector3(2.2f, 0.56f, 4.7f));
		Box("Glacis", new Vector3(0f, 1.13f, 2.41f), new Vector3(2.9f, 0.25f, 1.95f), Quaternion.Euler(16.2f, 0f, 0f));
		Box("Bow", new Vector3(0f, 0.75f, 2.9f), new Vector3(2.1f, 0.5f, 0.8f));
		Box("Engine", spec.EngineCentre, spec.EngineSize);
		if (gun != null && (bool)gun.Turret)
		{
			Box("Turret", new Vector3(0f, 0.25f, 0f), new Vector3(1.9f, 0.5f, 1.9f), null, gun.Turret);
		}
	}

	protected override void Model(out Transform[] wheelModels, out Transform steering)
	{
		if (!Bmp2Meshes.Ready)
		{
			Bmp2Meshes.Build();
		}
		if (!paint)
		{
			paint = Vehicle.Paint("Green armour (BMP-2)", new Color(0.23f, 0.27f, 0.16f), 0.22f, 0.1f);
			trim = Vehicle.Paint("Green fittings (BMP-2)", new Color(0.18f, 0.21f, 0.13f), 0.25f, 0.15f);
			steel = Vehicle.Paint("Steel running gear (BMP-2)", new Color(0.15f, 0.15f, 0.13f), 0.35f, 0.45f);
			rubber = Vehicle.Paint("Rubber (BMP-2)", new Color(0.04f, 0.04f, 0.038f), 0.12f);
			dark = Vehicle.Paint("Dark fittings (BMP-2)", new Color(0.08f, 0.09f, 0.07f), 0.3f, 0.35f);
			trackSteel = Vehicle.Paint("Track steel (BMP-2)", new Color(0.12f, 0.11f, 0.1f), 0.3f, 0.5f);
			gunMetal = Vehicle.Paint("Gun tube (BMP-2)", new Color(0.2f, 0.23f, 0.15f), 0.25f, 0.25f);
		}
		Transform model = new GameObject("Model").transform;
		model.SetParent(base.transform, worldPositionStays: false);
		Vehicle.Draw(model, "Hull", Bmp2Meshes.Hull, paint);
		Vehicle.Draw(model, "Deck", Bmp2Meshes.Deck, trim);
		Transform parent = gun.Build(model);
		Vehicle.Draw(gun.Turret, "Turret", Bmp2Meshes.Turret, paint);
		Vehicle.Draw(gun.Turret, "Turret fittings", Bmp2Meshes.TurretKit, dark);
		Vehicle.Draw(parent, "Mantlet", Bmp2Meshes.Mantlet, paint);
		Vehicle.Draw(parent, "Gun", Bmp2Meshes.Barrel, gunMetal);
		BuildRunningGear(model, Bmp2Meshes.Tyres, Bmp2Meshes.Hubs, paint, Bmp2Meshes.Idler, Bmp2Meshes.Sprocket, steel, rubber, Bmp2Meshes.Shoe, trackSteel, out wheelModels);
		steering = null;
	}
}
