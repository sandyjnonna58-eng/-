using System;
using UnityEngine;

namespace CombatSandbox.Demo;

public sealed class Abrams : Tank
{
	public const int Gunner = 1;

	private static readonly float[] Stations = new float[7] { 2.75f, 1.9f, 1.05f, 0.2f, -0.65f, -1.5f, -2.35f };

	private static readonly Vector3 Front = new Vector3(0f, 0.62f, 3.35f);

	private static readonly Vector3 Rear = new Vector3(0f, 0.7f, -3.3f);

	private static readonly Vector3 Pivot = new Vector3(0f, 1.38f, 0.15f);

	public static readonly WeaponDef Cannon120 = Tank.Cannon("m256", "M256 120 mm", "120 mm");

	private static Mesh hull;

	private static Mesh fittings;

	private static Mesh skirts;

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

	private static Material sand;

	private static Material steel;

	private static Material rubber;

	private static Material dark;

	private static Material trackSteel;

	private static Material gunMetal;

	private static Material lamp;

	protected override Vector3 TurretPivot => Pivot;

	protected override Vector3 FlagMount => new Vector3(-1.3f, 1.42f, -3.6f);

	protected override float FlagLength => 1.8f;

	protected override void Describe()
	{
		stations = Stations;
		idlerAt = Front;
		sprocketAt = Rear;
		idlerR = 0.3f;
		sprocketR = 0.33f;
		wheelR = 0.32f;
		trackX = 1.47f;
		shoeCount = 92;
		Wheel[] array = new Wheel[Stations.Length * 2];
		for (int i = 0; i < Stations.Length; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				array[i * 2 + j] = new Wheel
				{
					At = new Vector3((j == 0) ? (-1.47f) : 1.47f, 0.37f, Stations[i]),
					Radius = 0.37f,
					Width = 0.63f,
					Steer = Stations[i] / Stations[0],
					Driven = true
				};
			}
		}
		spec = new Spec
		{
			Name = "M1A2 Abrams",
			Mass = 62000f,
			CentreOfMass = new Vector3(0f, 0.8f, -0.1f),
			Wheels = array,
			Tracked = true,
			TrackWidth = 0.63f,
			PivotTorque = 600000f,
			Grip = 0.9f,
			Suspension = 0.27f,
			Spring = 450000f,
			Damper = 34000f,
			WheelMass = 160f,
			MaxTorque = 85000f,
			ReverseTorque = 36000f,
			Power = 840000f,
			TopSpeed = 18f,
			ReverseTop = 5f,
			BrakeTorque = 14000f,
			EngineBrake = 1800f,
			MaxSteer = 9f,
			AntiRoll = 280000f,
			Drag = 7f,
			SteerPivot = new Vector3(0f, 0.9f, 3.1f),
			SteerAxis = Vector3.up,
			SteerRatio = 1f,
			SteerInertia = 0.6f,
			SteerArms = 300f,
			EngineCentre = new Vector3(0f, 0.95f, -2.9f),
			EngineSize = new Vector3(2f, 0.8f, 1.6f),
			EngineHealth = 1300f,
			Armoured = true,
			ArmourEnergy = 35f,
			Hatches = new Vector3[3]
			{
				new Vector3(0f, 1.42f, 2.6f),
				Pivot + new Vector3(0.62f, 0.84f, -0.5f),
				Pivot + new Vector3(-0.62f, 0.84f, -0.4f)
			},
			EnginePitch = 0.95f,
			EngineLoudness = 1.9f,
			CameraOffset = new Vector3(0f, 4.3f, -14f)
		};
		places = CrewAndRiders(new Place
		{
			Role = SeatRole.Driver,
			Pelvis = new Vector3(0f, 0.72f, 2.75f),
			FootL = new Vector3(-0.13f, 0.52f, 3.35f),
			FootR = new Vector3(0.13f, 0.52f, 3.35f),
			Door = new Vector3(0f, 1.4f, 2.6f),
			Out = Vector3.left,
			Stand = 2.4f,
			Lean = 28f
		}, new Place
		{
			Role = SeatRole.Gunner,
			Pelvis = new Vector3(0.45f, 1f, 0.35f),
			FootL = new Vector3(0.3f, 0.5f, 0.7f),
			FootR = new Vector3(0.55f, 0.5f, 0.7f),
			Door = new Vector3(0.6f, 1.5f, -0.2f),
			Out = Vector3.right,
			Stand = 2.4f,
			Lean = 5f
		}, 1.05f, 1.52f, 1.5f, 1.04f, new float[2] { -2.1f, -3f });
		gun = new GunMount(this, 1, Pivot, Cannon120)
		{
			MaxTraverse = 180f,
			MinPitch = -9f,
			MaxPitch = 20f,
			GunLocal = new Vector3(0f, 0.42f, 1.8f),
			Muzzle = new Vector3(0f, 0f, 5.3f),
			Port = new Vector3(0f, 0f, 0.1f),
			Casings = false,
			Sight = new Vector3(-0.55f, 0.75f, -1f),
			GripsElevate = false,
			GripL = new Vector3(0.3f, -0.3f, 0.45f),
			GripR = new Vector3(0.6f, -0.3f, 0.45f),
			SeatTurn = 1f,
			Inertia = 30f,
			Laying = 170f,
			Damping = 145f,
			Cannon = true
		};
	}

	protected override void Colliders()
	{
		Box("Belly", new Vector3(0f, 0.48f, 0f), new Vector3(2.2f, 0.12f, 7.4f));
		Vehicle.Skid(colliders[colliders.Count - 1]);
		int[] array = new int[2] { -1, 1 };
		foreach (int num in array)
		{
			Box("Lower side", new Vector3((float)num * 1.1f, 0.75f, -0.1f), new Vector3(0.06f, 0.55f, 7.2f));
			Box("Skirt", new Vector3((float)num * 1.84f, 0.8f, 0.1f), new Vector3(0.1f, 0.5f, 7f));
		}
		Box("Upper hull", new Vector3(0f, 1.2f, -0.75f), new Vector3(3.66f, 0.36f, 6.4f));
		Box("Glacis", new Vector3(0f, 1.1f, 3.03f), new Vector3(2.3f, 0.08f, 1.9f), Quaternion.Euler(16.8f, 0f, 0f));
		Box("Engine", spec.EngineCentre, spec.EngineSize);
		if (gun != null && (bool)gun.Turret)
		{
			Box("Turret", new Vector3(0f, 0.39f, -0.3f), new Vector3(3.4f, 0.78f, 3.4f), null, gun.Turret);
			Box("Turret front", new Vector3(0f, 0.36f, 1.6f), new Vector3(2.4f, 0.7f, 0.7f), null, gun.Turret);
		}
	}

	protected override void Model(out Transform[] wheelModels, out Transform steering)
	{
		if (!hull)
		{
			Meshes();
		}
		Transform model = new GameObject("Model").transform;
		model.SetParent(base.transform, worldPositionStays: false);
		Vehicle.Draw(model, "Hull", hull, sand);
		Vehicle.Draw(model, "Skirts", skirts, sand);
		Vehicle.Draw(model, "Fittings", fittings, dark);
		Vehicle.Draw(model, "Lamps", glass, lamp);
		Transform parent = gun.Build(model);
		Vehicle.Draw(gun.Turret, "Turret", turretMesh, sand);
		Vehicle.Draw(gun.Turret, "Turret fittings", turretKit, dark);
		Vehicle.Draw(parent, "Mantlet", mantlet, sand);
		Vehicle.Draw(parent, "M256", barrel, gunMetal);
		BuildRunningGear(model, roadRubber, roadHub, sand, sprocketMesh, idlerMesh, steel, rubber, shoeMesh, trackSteel, out wheelModels);
		steering = null;
	}

	private static Vector2[] Section(float bottom, float top)
	{
		float num = Mathf.Min(1f, top - 0.03f);
		return new Vector2[8]
		{
			new Vector2(0f, bottom),
			new Vector2(1.04f, bottom),
			new Vector2(1.1f, Mathf.Min(bottom + 0.06f, num - 0.01f)),
			new Vector2(1.1f, num),
			new Vector2(1.83f, num),
			new Vector2(1.83f, top - 0.04f),
			new Vector2(1.79f, top),
			new Vector2(0f, top)
		};
	}

	private static Vector2[] TurretSection(float half, float height)
	{
		return new Vector2[6]
		{
			new Vector2(0f, 0f),
			new Vector2(half - 0.12f, 0f),
			new Vector2(half, 0.12f),
			new Vector2(half, height - 0.06f),
			new Vector2(half - 0.06f, height),
			new Vector2(0f, height)
		};
	}

	private static float GlacisAt(float z)
	{
		return 0.82f + (3.95f - z) / 1.85f * 0.56f;
	}

	private static void Meshes()
	{
		sand = Vehicle.Paint("Desert tan armour (Abrams)", new Color(0.56f, 0.5f, 0.36f), 0.18f, 0.08f);
		steel = Vehicle.Paint("Steel running gear (Abrams)", new Color(0.18f, 0.17f, 0.15f), 0.35f, 0.45f);
		rubber = Vehicle.Paint("Rubber road wheel tyres (Abrams)", new Color(0.04f, 0.04f, 0.038f), 0.12f);
		dark = Vehicle.Paint("Black fittings (Abrams)", new Color(0.07f, 0.07f, 0.065f), 0.3f, 0.4f);
		trackSteel = Vehicle.Paint("Track steel (Abrams)", new Color(0.13f, 0.12f, 0.11f), 0.3f, 0.5f);
		trackSteel.enableInstancing = true;
		gunMetal = Vehicle.Paint("Gun tube (M256)", new Color(0.5f, 0.45f, 0.33f), 0.22f, 0.2f);
		lamp = Vehicle.Paint("Lamp glass (Abrams)", new Color(0.82f, 0.8f, 0.7f), 0.9f);
		VehicleMesh vehicleMesh = new VehicleMesh();
		vehicleMesh.Hull(new(float, Vector2[])[5]
		{
			(3.95f, Section(0.62f, 0.82f)),
			(3.6f, Section(0.42f, GlacisAt(3.6f))),
			(2.1f, Section(0.42f, 1.38f)),
			(-3.75f, Section(0.42f, 1.38f)),
			(-3.95f, Section(0.6f, 1.32f))
		});
		vehicleMesh.Box(new Vector3(0f, 1.4f, -2.9f), new Vector3(2f, 0.04f, 1.6f), Quaternion.identity);
		vehicleMesh.Box(new Vector3(0f, 1.41f, 2.6f), new Vector3(0.7f, 0.05f, 0.55f), Quaternion.identity);
		hull = vehicleMesh.Build("Abrams hull");
		VehicleMesh vehicleMesh2 = new VehicleMesh();
		int[] array = new int[2] { -1, 1 };
		foreach (int num in array)
		{
			vehicleMesh2.Box(new Vector3((float)num * 1.84f, 0.78f, 0.25f), new Vector3(0.08f, 0.52f, 6.4f), Quaternion.identity);
			vehicleMesh2.Box(new Vector3((float)num * 1.86f, 0.74f, 2.85f), new Vector3(0.12f, 0.6f, 1.3f), Quaternion.identity);
			for (float num2 = -2.6f; num2 < 3.3f; num2 += 0.82f)
			{
				vehicleMesh2.Box(new Vector3((float)num * 1.885f, 0.78f, num2), new Vector3(0.02f, 0.46f, 0.03f), Quaternion.identity);
			}
		}
		skirts = vehicleMesh2.Build("Abrams skirts");
		VehicleMesh vehicleMesh3 = new VehicleMesh();
		for (float num3 = -3.6f; num3 < -2.2f; num3 += 0.1f)
		{
			vehicleMesh3.Box(new Vector3(0f, 1.425f, num3), new Vector3(1.8f, 0.02f, 0.04f), Quaternion.Euler(30f, 0f, 0f));
		}
		vehicleMesh3.Box(new Vector3(0f, 0.95f, -3.97f), new Vector3(1.6f, 0.5f, 0.04f), Quaternion.identity);
		for (float num4 = 0.75f; num4 < 1.2f; num4 += 0.06f)
		{
			vehicleMesh3.Box(new Vector3(0f, num4, -3.99f), new Vector3(1.5f, 0.015f, 0.02f), Quaternion.identity);
		}
		array = new int[2] { -1, 1 };
		foreach (int num5 in array)
		{
			vehicleMesh3.Box(new Vector3((float)num5 * 1.3f, 1.25f, 3.65f), new Vector3(0.22f, 0.12f, 0.12f), Quaternion.identity);
			vehicleMesh3.Tube(new Vector3[2]
			{
				new Vector3((float)num5 * 0.55f, 0.66f, 3.94f),
				new Vector3((float)num5 * 0.55f, 0.66f, 4.08f)
			}, 0.06f, 10);
		}
		fittings = vehicleMesh3.Build("Abrams fittings");
		VehicleMesh vehicleMesh4 = new VehicleMesh();
		array = new int[2] { -1, 1 };
		foreach (int num6 in array)
		{
			vehicleMesh4.Box(new Vector3((float)num6 * 1.3f, 1.25f, 3.72f), new Vector3(0.16f, 0.08f, 0.02f), Quaternion.identity);
		}
		glass = vehicleMesh4.Build("Abrams lamps");
		VehicleMesh vehicleMesh5 = new VehicleMesh();
		vehicleMesh5.Hull(new(float, Vector2[])[6]
		{
			(2.05f, TurretSection(0.6f, 0.62f)),
			(1.75f, TurretSection(1.45f, 0.74f)),
			(1f, TurretSection(1.72f, 0.8f)),
			(-1.15f, TurretSection(1.72f, 0.8f)),
			(-1.3f, TurretSection(1.5f, 0.8f)),
			(-2.45f, TurretSection(1.45f, 0.74f))
		});
		vehicleMesh5.Round(new Vector3(0.62f, 0.8f, -0.5f), Vector3.up, new Vector2[6]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 0.36f),
			new Vector2(0f, 0.36f),
			new Vector2(0.2f, 0.36f),
			new Vector2(0.2f, 0.36f),
			new Vector2(0.22f, 0f)
		}, 18);
		vehicleMesh5.Round(new Vector3(-0.62f, 0.8f, -0.4f), Vector3.up, new Vector2[6]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 0.32f),
			new Vector2(0f, 0.32f),
			new Vector2(0.06f, 0.32f),
			new Vector2(0.06f, 0.32f),
			new Vector2(0.07f, 0f)
		}, 18);
		vehicleMesh5.Box(new Vector3(-0.55f, 0.95f, 0.75f), new Vector3(0.45f, 0.32f, 0.5f), Quaternion.identity);
		vehicleMesh5.Box(new Vector3(0.75f, 0.92f, 0.55f), new Vector3(0.35f, 0.25f, 0.4f), Quaternion.identity);
		turretMesh = vehicleMesh5.Build("Abrams turret");
		VehicleMesh vehicleMesh6 = new VehicleMesh();
		for (float num7 = -2.4f; num7 < -1.25f; num7 += 0.38f)
		{
			array = new int[2] { -1, 1 };
			foreach (int num8 in array)
			{
				vehicleMesh6.Tube(new Vector3[2]
				{
					new Vector3((float)num8 * 1.5f, 0.8f, num7),
					new Vector3((float)num8 * 1.5f, 1.05f, num7)
				}, 0.015f, 6);
			}
		}
		vehicleMesh6.Tube(new Vector3[4]
		{
			new Vector3(-1.5f, 1.05f, -1.3f),
			new Vector3(-1.5f, 1.05f, -2.5f),
			new Vector3(1.5f, 1.05f, -2.5f),
			new Vector3(1.5f, 1.05f, -1.3f)
		}, 0.02f, 6);
		array = new int[2] { -1, 1 };
		foreach (int num9 in array)
		{
			for (int i = 0; i < 6; i++)
			{
				Vector3 vector = new Vector3((float)num9 * 1.55f, 0.55f + (float)(i % 2) * 0.12f, 1.05f - (float)(i / 2) * 0.12f);
				vehicleMesh6.Tube(new Vector3[2]
				{
					vector,
					vector + new Vector3((float)num9 * 0.1f, 0.06f, 0.18f)
				}, 0.04f, 8);
			}
		}
		vehicleMesh6.Tube(new Vector3[2]
		{
			new Vector3(0.62f, 1.25f, -0.4f),
			new Vector3(0.62f, 1.27f, 0.85f)
		}, 0.024f, 8);
		vehicleMesh6.Box(new Vector3(0.62f, 1.22f, -0.3f), new Vector3(0.14f, 0.16f, 0.4f), Quaternion.identity);
		vehicleMesh6.Tube(new Vector3[2]
		{
			new Vector3(0.62f, 1.18f, -0.5f),
			new Vector3(0.62f, 1.05f, -0.5f)
		}, 0.035f, 8);
		vehicleMesh6.Tube(new Vector3[2]
		{
			new Vector3(-0.62f, 1.18f, -0.2f),
			new Vector3(-0.62f, 1.2f, 0.55f)
		}, 0.016f, 8);
		array = new int[2] { -1, 1 };
		foreach (int num10 in array)
		{
			vehicleMesh6.Tube(new Vector3[3]
			{
				new Vector3((float)num10 * 1.1f, 0.8f, -1.9f),
				new Vector3((float)num10 * 1.11f, 2.1f, -1.95f),
				new Vector3((float)num10 * 1.13f, 3.3f, -2.05f)
			}, 0.008f, 4);
		}
		turretKit = vehicleMesh6.Build("Abrams turret fittings");
		VehicleMesh vehicleMesh7 = new VehicleMesh();
		vehicleMesh7.Box(new Vector3(0f, 0f, 0.05f), new Vector3(0.7f, 0.5f, 0.45f), Quaternion.identity);
		vehicleMesh7.Box(new Vector3(0.26f, 0.02f, 0.3f), new Vector3(0.12f, 0.12f, 0.2f), Quaternion.identity);
		mantlet = vehicleMesh7.Build("Abrams mantlet");
		VehicleMesh vehicleMesh8 = new VehicleMesh();
		vehicleMesh8.Tube(new Vector3[2]
		{
			new Vector3(0f, 0f, 0.25f),
			new Vector3(0f, 0f, 5.3f)
		}, 0.08f, 14);
		vehicleMesh8.Tube(new Vector3[2]
		{
			new Vector3(0f, 0f, 0.3f),
			new Vector3(0f, 0f, 2.4f)
		}, 0.115f, 14);
		vehicleMesh8.Tube(new Vector3[2]
		{
			new Vector3(0f, 0f, 2.4f),
			new Vector3(0f, 0f, 3.2f)
		}, 0.14f, 16);
		vehicleMesh8.Tube(new Vector3[2]
		{
			new Vector3(0f, 0f, 3.2f),
			new Vector3(0f, 0f, 4.6f)
		}, 0.1f, 14);
		vehicleMesh8.Box(new Vector3(0f, 0.12f, 5.15f), new Vector3(0.08f, 0.06f, 0.14f), Quaternion.identity);
		barrel = vehicleMesh8.Build("M256 barrel");
		VehicleMesh vehicleMesh9 = new VehicleMesh();
		float[] array2 = new float[2] { -0.29f, 0.05f };
		foreach (float num11 in array2)
		{
			vehicleMesh9.Round(Vector3.zero, Vector3.right, new Vector2[10]
			{
				new Vector2(num11, 0f),
				new Vector2(num11, 0.3f),
				new Vector2(num11, 0.3f),
				new Vector2(num11 + 0.02f, 0.32f),
				new Vector2(num11 + 0.02f, 0.32f),
				new Vector2(num11 + 0.22f, 0.32f),
				new Vector2(num11 + 0.22f, 0.32f),
				new Vector2(num11 + 0.24f, 0.3f),
				new Vector2(num11 + 0.24f, 0.3f),
				new Vector2(num11 + 0.24f, 0f)
			}, 22);
		}
		roadRubber = vehicleMesh9.Build("Abrams road wheel tyres");
		VehicleMesh vehicleMesh10 = new VehicleMesh();
		vehicleMesh10.Round(Vector3.zero, Vector3.right, new Vector2[8]
		{
			new Vector2(-0.305f, 0f),
			new Vector2(-0.305f, 0.1f),
			new Vector2(-0.305f, 0.1f),
			new Vector2(-0.29f, 0.27f),
			new Vector2(-0.29f, 0.27f),
			new Vector2(0.29f, 0.27f),
			new Vector2(0.29f, 0.27f),
			new Vector2(0.3f, 0f)
		}, 18);
		for (int j = 0; j < 6; j++)
		{
			float f = (float)(j * 60) * (MathF.PI / 180f);
			vehicleMesh10.Box(new Vector3(-0.31f, Mathf.Cos(f) * 0.17f, Mathf.Sin(f) * 0.17f), new Vector3(0.02f, 0.05f, 0.05f), Quaternion.identity);
		}
		roadHub = vehicleMesh10.Build("Abrams road wheel hubs");
		VehicleMesh vehicleMesh11 = new VehicleMesh();
		array2 = new float[2] { -0.26f, 0.16f };
		foreach (float num12 in array2)
		{
			vehicleMesh11.Round(Vector3.zero, Vector3.right, new Vector2[5]
			{
				new Vector2(num12, 0f),
				new Vector2(num12, 0.27f),
				new Vector2(num12, 0.27f),
				new Vector2(num12 + 0.08f, 0.27f),
				new Vector2(num12 + 0.08f, 0f)
			}, 22);
		}
		vehicleMesh11.Round(Vector3.zero, Vector3.right, new Vector2[6]
		{
			new Vector2(-0.32f, 0f),
			new Vector2(-0.32f, 0.13f),
			new Vector2(-0.32f, 0.13f),
			new Vector2(0.32f, 0.13f),
			new Vector2(0.32f, 0.13f),
			new Vector2(0.32f, 0f)
		}, 12);
		for (int k = 0; k < 11; k++)
		{
			Quaternion quaternion = Quaternion.Euler((float)k * 32.72727f, 0f, 0f);
			array2 = new float[2] { -0.22f, 0.2f };
			foreach (float x in array2)
			{
				vehicleMesh11.Box(quaternion * new Vector3(x, 0.31f, 0f), new Vector3(0.08f, 0.08f, 0.07f), quaternion);
			}
		}
		sprocketMesh = vehicleMesh11.Build("Abrams sprocket");
		VehicleMesh vehicleMesh12 = new VehicleMesh();
		array2 = new float[2] { -0.27f, 0.07f };
		foreach (float num13 in array2)
		{
			vehicleMesh12.Round(Vector3.zero, Vector3.right, new Vector2[6]
			{
				new Vector2(num13, 0f),
				new Vector2(num13, 0.3f),
				new Vector2(num13, 0.3f),
				new Vector2(num13 + 0.2f, 0.3f),
				new Vector2(num13 + 0.2f, 0.3f),
				new Vector2(num13 + 0.2f, 0f)
			}, 22);
		}
		idlerMesh = vehicleMesh12.Build("Abrams idler");
		VehicleMesh vehicleMesh13 = new VehicleMesh();
		vehicleMesh13.Box(Vector3.zero, new Vector3(0.63f, 0.045f, 0.16f), Quaternion.identity);
		array2 = new float[2] { -0.18f, 0.18f };
		foreach (float x2 in array2)
		{
			vehicleMesh13.Box(new Vector3(x2, 0.035f, 0f), new Vector3(0.22f, 0.03f, 0.1f), Quaternion.identity);
		}
		vehicleMesh13.Box(new Vector3(0f, -0.05f, 0f), new Vector3(0.06f, 0.07f, 0.07f), Quaternion.identity);
		shoeMesh = vehicleMesh13.Build("Abrams track shoe");
	}
}
