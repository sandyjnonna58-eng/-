using UnityEngine;

namespace CombatSandbox.Demo;

internal static class Leopard2Meshes
{
	public static readonly float[] Stations = new float[7] { 2.85f, 1.95f, 1.05f, 0.15f, -0.75f, -1.65f, -2.55f };

	public static readonly Vector3 Front = new Vector3(0f, 0.62f, 3.45f);

	public static readonly Vector3 Rear = new Vector3(0f, 0.7f, -3.35f);

	public static readonly Vector3 Pivot = new Vector3(0f, 1.42f, 0.05f);

	public static readonly Vector3 GunLocal = new Vector3(0f, 0.42f, 1.42f);

	public static Mesh Hull;

	public static Mesh Skirts;

	public static Mesh Deck;

	public static Mesh Turret;

	public static Mesh TurretKit;

	public static Mesh Mantlet;

	public static Mesh Barrel;

	public static Mesh Tyres;

	public static Mesh Hubs;

	public static Mesh Sprocket;

	public static Mesh Idler;

	public static Mesh Shoe;

	public static bool Ready => Hull != null;

	private static Vector2[] Section(float bottom, float top)
	{
		float num = Mathf.Min(1f, top - 0.03f);
		return new Vector2[8]
		{
			new Vector2(0f, bottom),
			new Vector2(1.06f, bottom),
			new Vector2(1.12f, Mathf.Min(bottom + 0.06f, num - 0.01f)),
			new Vector2(1.12f, num),
			new Vector2(1.84f, num),
			new Vector2(1.84f, top - 0.04f),
			new Vector2(1.8f, top),
			new Vector2(0f, top)
		};
	}

	public static void Build()
	{
		VehicleMesh vehicleMesh = new VehicleMesh();
		vehicleMesh.Hull(new(float, Vector2[])[6]
		{
			(3.98f, Section(0.6f, 0.98f)),
			(3.82f, Section(0.42f, 1.05f)),
			(2.35f, Section(0.42f, 1.42f)),
			(-3.5f, Section(0.42f, 1.42f)),
			(-3.72f, Section(0.5f, 1.4f)),
			(-3.82f, Section(0.62f, 1.32f))
		});
		int[] array = new int[2] { -1, 1 };
		foreach (int num in array)
		{
			vehicleMesh.Box(new Vector3((float)num * 1.35f, 1.2f, 3.9f), new Vector3(0.24f, 0.14f, 0.12f), Quaternion.identity);
			vehicleMesh.Box(new Vector3((float)num * 1.5f, 1.45f, -3.2f), new Vector3(0.5f, 0.06f, 0.8f), Quaternion.identity);
		}
		Hull = vehicleMesh.Build("Leopard 2A6 hull");
		PolyMesh polyMesh = new PolyMesh();
		array = new int[2] { -1, 1 };
		foreach (int num2 in array)
		{
			for (int j = 0; j < 3; j++)
			{
				polyMesh.Box(new Vector3((float)num2 * 1.88f, 0.78f, 3.2f - (float)j * 0.72f), new Vector3(0.16f, 0.56f, 0.7f), Quaternion.identity, 0.025f);
			}
			polyMesh.Box(new Vector3((float)num2 * 1.86f, 0.8f, -1.0f), new Vector3(0.06f, 0.5f, 4.6f), Quaternion.identity, 0.015f);
		}
		Skirts = polyMesh.Build("Leopard 2A6 skirts");
		VehicleMesh vehicleMesh2 = new VehicleMesh();
		for (float num3 = -3.45f; num3 < -1.9f; num3 += 0.1f)
		{
			vehicleMesh2.Box(new Vector3(0f, 1.43f, num3), new Vector3(2f, 0.02f, 0.045f), Quaternion.Euler(30f, 0f, 0f));
		}
		vehicleMesh2.Box(new Vector3(0f, 0.95f, -3.84f), new Vector3(2.2f, 0.55f, 0.04f), Quaternion.identity);
		array = new int[2] { -1, 1 };
		foreach (int num4 in array)
		{
			vehicleMesh2.Tube(new Vector3[2]
			{
				new Vector3((float)num4 * 0.6f, 0.62f, 3.96f),
				new Vector3((float)num4 * 0.6f, 0.62f, 4.1f)
			}, 0.07f, 10);
			vehicleMesh2.Tube(new Vector3[4]
			{
				new Vector3((float)num4 * 1.7f, 1.44f, 2.2f),
				new Vector3((float)num4 * 1.74f, 1.44f, 2f),
				new Vector3((float)num4 * 1.74f, 1.44f, -3f),
				new Vector3((float)num4 * 1.7f, 1.44f, -3.2f)
			}, 0.025f, 6);
		}
		Deck = vehicleMesh2.Build("Leopard 2A6 deck");
		PolyMesh polyMesh2 = new PolyMesh();
		Vector2[] half = new Vector2[8]
		{
			new Vector2(0f, 1.3f),
			new Vector2(0.44f, 1.38f),
			new Vector2(0.95f, 2.08f),
			new Vector2(1.8f, 1.2f),
			new Vector2(1.82f, 0.1f),
			new Vector2(1.8f, -1.35f),
			new Vector2(1.6f, -2.7f),
			new Vector2(0f, -2.7f)
		};
		polyMesh2.Loft(new Vector3[5][]
		{
			PolyMesh.Mirror(0f, 0.96f, Vector2.zero, half),
			PolyMesh.Mirror(0.08f, 1f, Vector2.zero, half),
			PolyMesh.Mirror(0.7f, 1f, Vector2.zero, half),
			PolyMesh.Mirror(0.8f, 0.96f, new Vector2(0f, -0.03f), half),
			PolyMesh.Mirror(0.82f, 0.92f, new Vector2(0f, -0.05f), half)
		});
		Turret = polyMesh2.Build("Leopard 2A6 turret");
		PolyMesh polyMesh3 = new PolyMesh();
		polyMesh3.Box(new Vector3(0.62f, 1.02f, 1.05f), new Vector3(0.5f, 0.42f, 0.6f), Quaternion.identity, 0.04f);
		polyMesh3.Tube(new Vector3(-0.85f, 0.82f, -0.85f), new Vector3(-0.85f, 1.15f, -0.85f), 0.13f, 0.13f, 16);
		polyMesh3.Box(new Vector3(-0.85f, 1.22f, -0.8f), new Vector3(0.32f, 0.22f, 0.36f), Quaternion.identity, 0.03f);
		polyMesh3.Tube(new Vector3(0.6f, 0.82f, -0.75f), new Vector3(0.6f, 0.88f, -0.75f), 0.32f, 0.32f, 16);
		polyMesh3.Tube(new Vector3(-0.2f, 0.82f, -0.2f), new Vector3(-0.2f, 0.88f, -0.2f), 0.28f, 0.28f, 16);
		polyMesh3.Tube(new Vector3(0.6f, 0.98f, -0.6f), new Vector3(0.6f, 1f, 0.4f), 0.018f, 0.018f, 8);
		array = new int[2] { -1, 1 };
		foreach (int num5 in array)
		{
			for (int k = 0; k < 4; k++)
			{
				Vector3 vector = new Vector3((float)num5 * 1.86f, 0.55f + (float)(k % 2) * 0.13f, -1.2f - (float)(k / 2) * 0.16f);
				polyMesh3.Tube(vector, vector + new Vector3((float)num5 * 0.15f, 0.12f, 0.1f), 0.045f, 0.045f, 10);
			}
			polyMesh3.Tube(new Vector3((float)num5 * 1.62f, 0.4f, -2.72f), new Vector3((float)num5 * 1.62f, 0.4f, -3.05f), 0.02f, 0.02f, 6);
		}
		polyMesh3.Tube(new Vector3(-1.62f, 0.62f, -3.05f), new Vector3(1.62f, 0.62f, -3.05f), 0.02f, 0.02f, 6);
		polyMesh3.Tube(new Vector3(-1.62f, 0.2f, -3.05f), new Vector3(1.62f, 0.2f, -3.05f), 0.02f, 0.02f, 6);
		polyMesh3.Tube(new Vector3(-1.2f, 0.82f, -2.2f), new Vector3(-1.22f, 3.3f, -2.35f), 0.009f, 0.006f, 5);
		TurretKit = polyMesh3.Build("Leopard 2A6 turret fittings");
		PolyMesh polyMesh4 = new PolyMesh();
		polyMesh4.Box(new Vector3(0f, 0f, -0.05f), new Vector3(0.82f, 0.6f, 0.3f), Quaternion.identity, 0.05f);
		polyMesh4.Tube(new Vector3(0f, 0f, 0.05f), new Vector3(0f, 0f, 0.35f), 0.17f, 0.15f, 16);
		polyMesh4.Tube(new Vector3(0.28f, 0.02f, 0f), new Vector3(0.28f, 0.02f, 0.25f), 0.03f, 0.03f, 8);
		Mantlet = polyMesh4.Build("Leopard 2A6 mantlet");
		PolyMesh polyMesh5 = new PolyMesh();
		Kit.Barrel(polyMesh5, 6.6f, 0.078f, 3.05f, 0.13f, 0f);
		polyMesh5.Box(new Vector3(0f, 0.11f, 6.45f), new Vector3(0.08f, 0.06f, 0.16f), Quaternion.identity, 0.01f);
		Barrel = polyMesh5.Build("Rh-120 L55 barrel");
		Tyres = Kit.WheelTyres(0.33f, 0.63f, "Leopard 2A6 road wheel tyres");
		Hubs = Kit.WheelHub(0.33f, 0.63f, 8, "Leopard 2A6 road wheel hubs");
		Sprocket = Kit.Sprocket(0.33f, 0.63f, 12, "Leopard 2A6 sprocket");
		Idler = Kit.WheelHub(0.3f, 0.55f, 6, "Leopard 2A6 idler");
		Shoe = Kit.Shoe(0.63f, 0.16f, "Leopard 2A6 track shoe");
	}
}
