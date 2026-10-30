using UnityEngine;

namespace CombatSandbox.Demo;

internal static class T90Meshes
{
	public static readonly float[] Stations = new float[6] { 2.3f, 1.45f, 0.6f, -0.25f, -1.1f, -1.95f };

	public static readonly Vector3 Front = new Vector3(0f, 0.68f, 2.95f);

	public static readonly Vector3 Rear = new Vector3(0f, 0.72f, -2.95f);

	public static readonly Vector3 Pivot = new Vector3(0f, 1.5f, 0.15f);

	public static readonly Vector3 GunLocal = new Vector3(0f, 0.38f, 1.05f);

	public static Mesh Hull;

	public static Mesh Era;

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
		float num = Mathf.Min(1.1f, top - 0.03f);
		return new Vector2[8]
		{
			new Vector2(0f, bottom),
			new Vector2(1.02f, bottom),
			new Vector2(1.08f, Mathf.Min(bottom + 0.06f, num - 0.01f)),
			new Vector2(1.08f, num),
			new Vector2(1.76f, num),
			new Vector2(1.76f, top - 0.05f),
			new Vector2(1.7f, top),
			new Vector2(0f, top)
		};
	}

	private static float GlacisAt(float z)
	{
		return 0.84f + (3.45f - z) / 1.05f * 0.66f;
	}

	public static void Build()
	{
		VehicleMesh vehicleMesh = new VehicleMesh();
		vehicleMesh.Hull(new(float, Vector2[])[6]
		{
			(3.45f, Section(0.72f, 0.84f)),
			(3.1f, Section(0.45f, GlacisAt(3.1f))),
			(2.4f, Section(0.45f, 1.5f)),
			(-2.9f, Section(0.45f, 1.5f)),
			(-3.3f, Section(0.5f, 1.46f)),
			(-3.42f, Section(0.62f, 1.38f))
		});
		int[] array = new int[2] { -1, 1 };
		foreach (int num in array)
		{
			vehicleMesh.Box(new Vector3((float)num * 1.42f, 1.02f, 3.55f), new Vector3(0.64f, 0.03f, 0.42f), Quaternion.Euler(20f, 0f, 0f));
			vehicleMesh.Box(new Vector3((float)num * 1.42f, 1.28f, -0.9f), new Vector3(0.5f, 0.3f, 2.6f), Quaternion.identity);
			vehicleMesh.Box(new Vector3((float)num * 0.7f, 0.7f, 3.52f), new Vector3(0.18f, 0.1f, 0.12f), Quaternion.identity);
		}
		Hull = vehicleMesh.Build("T-90M hull");
		PolyMesh polyMesh = new PolyMesh();
		Vector3 normalized = new Vector3(0f, 1.05f, 0.66f).normalized;
		Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, normalized);
		for (int j = 0; j < 2; j++)
		{
			for (int k = -3; k <= 3; k++)
			{
				float num2 = 2.7f + (float)j * 0.42f;
				polyMesh.Box(new Vector3((float)k * 0.3f, GlacisAt(num2), num2) + normalized * 0.08f, new Vector3(0.27f, 0.12f, 0.38f), quaternion, 0.02f);
			}
		}
		array = new int[2] { -1, 1 };
		foreach (int num3 in array)
		{
			for (int l = 0; l < 4; l++)
			{
				polyMesh.Box(new Vector3((float)num3 * 1.8f, 0.88f, 2.65f - (float)l * 0.62f), new Vector3(0.1f, 0.42f, 0.58f), Quaternion.identity, 0.02f);
			}
		}
		Era = polyMesh.Build("T-90M ERA");
		VehicleMesh vehicleMesh2 = new VehicleMesh();
		array = new int[2] { -1, 1 };
		foreach (int num4 in array)
		{
			for (float num5 = -2.9f; num5 < 0.4f; num5 += 0.62f)
			{
				vehicleMesh2.Box(new Vector3((float)num4 * 1.77f, 0.92f, num5 + 0.3f), new Vector3(0.025f, 0.34f, 0.6f), Quaternion.Euler(0f, 0f, (float)num4 * 3f));
			}
		}
		Skirts = vehicleMesh2.Build("T-90M skirts");
		VehicleMesh vehicleMesh3 = new VehicleMesh();
		for (float num6 = -3.1f; num6 < -1.6f; num6 += 0.11f)
		{
			vehicleMesh3.Box(new Vector3(0f, 1.515f, num6), new Vector3(1.8f, 0.025f, 0.05f), Quaternion.Euler(28f, 0f, 0f));
		}
		vehicleMesh3.Tube(new Vector3[2]
		{
			new Vector3(-1.4f, 1.66f, -3.28f),
			new Vector3(1.4f, 1.66f, -3.28f)
		}, 0.12f, 12);
		array = new int[2] { -1, 1 };
		foreach (int num7 in array)
		{
			vehicleMesh3.Tube(new Vector3[4]
			{
				new Vector3((float)num7 * 1.62f, 1.52f, 2.2f),
				new Vector3((float)num7 * 1.66f, 1.52f, 2f),
				new Vector3((float)num7 * 1.66f, 1.52f, -2.8f),
				new Vector3((float)num7 * 1.62f, 1.52f, -3f)
			}, 0.025f, 6);
			for (float num8 = -1.2f; num8 <= 1.21f; num8 += 1.2f)
			{
				vehicleMesh3.Round(new Vector3((float)num7 * 1.4f, 1f, num8), Vector3.right, new Vector2[4]
				{
					new Vector2(-0.12f, 0f),
					new Vector2(-0.12f, 0.1f),
					new Vector2(0.12f, 0.1f),
					new Vector2(0.12f, 0f)
				}, 12);
			}
		}
		Deck = vehicleMesh3.Build("T-90M deck and fittings");
		PolyMesh polyMesh2 = new PolyMesh();
		Vector2[] half = new Vector2[8]
		{
			new Vector2(0f, 0.95f),
			new Vector2(0.42f, 1f),
			new Vector2(1.08f, 0.62f),
			new Vector2(1.26f, 0.1f),
			new Vector2(1.26f, -0.75f),
			new Vector2(1.02f, -1.1f),
			new Vector2(0.92f, -1.8f),
			new Vector2(0f, -1.8f)
		};
		polyMesh2.Loft(new Vector3[5][]
		{
			PolyMesh.Mirror(0f, 0.95f, Vector2.zero, half),
			PolyMesh.Mirror(0.08f, 1f, Vector2.zero, half),
			PolyMesh.Mirror(0.48f, 1f, Vector2.zero, half),
			PolyMesh.Mirror(0.66f, 0.9f, new Vector2(0f, -0.04f), half),
			PolyMesh.Mirror(0.72f, 0.8f, new Vector2(0f, -0.08f), half)
		});
		array = new int[2] { -1, 1 };
		foreach (int num9 in array)
		{
			Vector3 vector = new Vector3((float)num9 * 0.8f, 0.36f, 1.02f);
			polyMesh2.Wedge(PolyMesh.Place(PolyMesh.Ring(-0.28f, new Vector2(-0.5f, 0.3f), new Vector2(0.52f, 0.25f), new Vector2(0.45f, -0.25f), new Vector2(-0.5f, -0.25f)), vector, Quaternion.Euler(0f, (float)(-num9) * 32f, 0f)), PolyMesh.Place(PolyMesh.Ring(0.3f, new Vector2(-0.48f, 0.18f), new Vector2(0.48f, 0.14f), new Vector2(0.43f, -0.25f), new Vector2(-0.48f, -0.25f)), vector, Quaternion.Euler(0f, (float)(-num9) * 32f, 0f)));
			for (int m = 0; m < 3; m++)
			{
				polyMesh2.Box(new Vector3((float)num9 * (0.35f + (float)m * 0.3f), 0.76f, 0.35f + (float)m * 0.08f), new Vector3(0.26f, 0.08f, 0.5f), Quaternion.Euler(-6f, (float)num9 * 12f, 0f), 0.015f);
			}
		}
		polyMesh2.Box(new Vector3(0f, 0.36f, -1.98f), new Vector3(1.8f, 0.5f, 0.38f), Quaternion.identity, 0.04f);
		polyMesh2.Box(new Vector3(-0.55f, 0.86f, 0.5f), new Vector3(0.3f, 0.28f, 0.42f), Quaternion.identity, 0.03f);
		Turret = polyMesh2.Build("T-90M turret");
		PolyMesh polyMesh3 = new PolyMesh();
		polyMesh3.Tube(new Vector3(0.52f, 0.72f, 0.15f), new Vector3(0.52f, 1.05f, 0.15f), 0.11f, 0.11f, 14);
		polyMesh3.Box(new Vector3(0.52f, 1.12f, 0.2f), new Vector3(0.3f, 0.2f, 0.3f), Quaternion.identity, 0.03f);
		polyMesh3.Box(new Vector3(0.45f, 0.82f, -0.5f), new Vector3(0.45f, 0.18f, 0.45f), Quaternion.identity, 0.03f);
		polyMesh3.Box(new Vector3(0.45f, 0.98f, -0.45f), new Vector3(0.22f, 0.16f, 0.32f), Quaternion.identity, 0.02f);
		polyMesh3.Tube(new Vector3(0.45f, 0.99f, -0.3f), new Vector3(0.45f, 1f, 0.55f), 0.02f, 0.02f, 8);
		polyMesh3.Tube(new Vector3(-0.42f, 0.72f, -0.35f), new Vector3(-0.42f, 0.78f, -0.35f), 0.3f, 0.3f, 16);
		array = new int[2] { -1, 1 };
		foreach (int num10 in array)
		{
			for (int n = 0; n < 6; n++)
			{
				Vector3 vector2 = new Vector3((float)num10 * (1.18f + (float)(n % 2) * 0.06f), 0.45f + (float)(n % 2) * 0.12f, 0.15f - (float)(n / 2) * 0.16f);
				polyMesh3.Tube(vector2, vector2 + new Vector3((float)num10 * 0.08f, 0.1f, 0.2f), 0.042f, 0.042f, 10);
			}
		}
		polyMesh3.Tube(new Vector3(-0.85f, 0.7f, -1.4f), new Vector3(-0.88f, 3.2f, -1.55f), 0.009f, 0.006f, 5);
		TurretKit = polyMesh3.Build("T-90M turret fittings");
		PolyMesh polyMesh4 = new PolyMesh();
		polyMesh4.Box(new Vector3(0f, 0f, 0.02f), new Vector3(0.62f, 0.46f, 0.4f), Quaternion.identity, 0.05f);
		polyMesh4.Tube(new Vector3(0f, 0f, 0.2f), new Vector3(0f, 0f, 0.45f), 0.2f, 0.14f, 16);
		polyMesh4.Tube(new Vector3(0.26f, -0.03f, 0.1f), new Vector3(0.26f, -0.03f, 0.5f), 0.025f, 0.022f, 8);
		Mantlet = polyMesh4.Build("T-90M mantlet");
		PolyMesh polyMesh5 = new PolyMesh();
		Kit.Barrel(polyMesh5, 5f, 0.075f, 2.35f, 0.13f, 0.095f);
		Barrel = polyMesh5.Build("2A46M-5 barrel");
		Tyres = Kit.WheelTyres(0.35f, 0.58f, "T-90M road wheel tyres");
		Hubs = Kit.WheelHub(0.35f, 0.58f, 8, "T-90M road wheel hubs");
		Sprocket = Kit.Sprocket(0.32f, 0.58f, 13, "T-90M sprocket");
		Idler = Kit.WheelHub(0.3f, 0.5f, 6, "T-90M idler");
		Shoe = Kit.Shoe(0.58f, 0.155f, "T-90M track shoe");
	}
}
