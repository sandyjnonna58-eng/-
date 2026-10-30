using UnityEngine;

namespace CombatSandbox.Demo;

internal static class Bmp2Meshes
{
	public static readonly float[] Stations = new float[6] { 2.2f, 1.36f, 0.52f, -0.32f, -1.16f, -2f };

	public static readonly Vector3 Front = new Vector3(0f, 0.66f, 2.92f);

	public static readonly Vector3 Rear = new Vector3(0f, 0.62f, -2.8f);

	public static readonly Vector3 Pivot = new Vector3(0f, 1.53f, 0.3f);

	public static readonly Vector3 GunLocal = new Vector3(0f, 0.33f, 0.85f);

	public static Mesh Hull;

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
		float num = Mathf.Min(0.95f, top - 0.03f);
		return new Vector2[8]
		{
			new Vector2(0f, bottom),
			new Vector2(1.04f, bottom),
			new Vector2(1.08f, Mathf.Min(bottom + 0.05f, num - 0.01f)),
			new Vector2(1.08f, num),
			new Vector2(1.46f, num),
			new Vector2(1.46f, top - 0.04f),
			new Vector2(1.41f, top),
			new Vector2(0f, top)
		};
	}

	private static float GlacisAt(float z)
	{
		return 1f + (3.32f - z) * 0.279f;
	}

	public static void Build()
	{
		VehicleMesh vehicleMesh = new VehicleMesh();
		vehicleMesh.Hull(new(float, Vector2[])[7]
		{
			(3.42f, Section(0.86f, 0.95f)),
			(3.32f, Section(0.76f, 1f)),
			(2.72f, Section(0.42f, GlacisAt(2.72f))),
			(1.6f, Section(0.42f, 1.48f)),
			(1.45f, Section(0.42f, 1.53f)),
			(-3.12f, Section(0.42f, 1.53f)),
			(-3.3f, Section(0.5f, 1.48f))
		});
		int[] array = new int[2] { -1, 1 };
		foreach (int num in array)
		{
			vehicleMesh.Box(new Vector3((float)num * 1.27f, 0.97f, 3.15f), new Vector3(0.34f, 0.03f, 0.42f), Quaternion.Euler(18f, 0f, 0f));
			vehicleMesh.Box(new Vector3((float)num * 0.45f, 0.98f, -3.33f), new Vector3(0.72f, 0.8f, 0.1f), Quaternion.identity);
			vehicleMesh.Box(new Vector3((float)num * 0.45f, 1.12f, -3.39f), new Vector3(0.06f, 0.06f, 0.04f), Quaternion.identity);
		}
		Hull = vehicleMesh.Build("BMP-2 hull");
		PolyMesh polyMesh = new PolyMesh();
		Quaternion quaternion = Quaternion.Euler(15.6f, 0f, 0f);
		for (int j = 0; j < 4; j++)
		{
			float num2 = 3.15f - (float)j * 0.17f;
			polyMesh.Box(new Vector3(0f, GlacisAt(num2) + 0.02f, num2), new Vector3(2.5f, 0.04f, 0.05f), quaternion, 0.01f);
		}
		polyMesh.Box(new Vector3(0f, GlacisAt(2.25f) + 0.04f, 2.25f), new Vector3(2.3f, 0.03f, 0.5f), quaternion, 0.01f);
		for (int k = 0; k < 5; k++)
		{
			float num3 = 2.15f - (float)k * 0.1f;
			polyMesh.Box(new Vector3(0.6f, GlacisAt(num3) + 0.02f, num3), new Vector3(0.75f, 0.03f, 0.05f), quaternion, 0.008f);
		}
		polyMesh.Tube(new Vector3(-0.62f, 1.53f, 1.15f), new Vector3(-0.62f, 1.6f, 1.15f), 0.3f, 0.3f, 16);
		array = new int[2] { -1, 1 };
		foreach (int num4 in array)
		{
			for (int l = 0; l < 2; l++)
			{
				polyMesh.Box(new Vector3((float)num4 * 0.52f, 1.55f, -1.25f - (float)l * 1f), new Vector3(0.62f, 0.04f, 0.78f), Quaternion.identity, 0.015f);
			}
			for (int m = 0; m < 3; m++)
			{
				float num5 = -0.9f - (float)m * 0.62f;
				polyMesh.Tube(new Vector3((float)num4 * 1.44f, 1.28f, num5), new Vector3((float)num4 * 1.5f, 1.28f, num5), 0.05f, 0.05f, 10);
			}
			polyMesh.Box(new Vector3((float)num4 * 1.02f, 1.08f, 3.05f), new Vector3(0.16f, 0.1f, 0.06f), Quaternion.identity, 0.015f);
			polyMesh.Tube(new Vector3((float)num4 * 0.55f, 0.78f, 3.36f), new Vector3((float)num4 * 0.55f, 0.78f, 3.5f), 0.05f, 0.05f, 10);
		}
		polyMesh.Box(new Vector3(1.2f, 1.6f, -2.6f), new Vector3(0.1f, 0.12f, 0.9f), Quaternion.identity, 0.02f);
		Deck = polyMesh.Build("BMP-2 deck fittings");
		PolyMesh polyMesh2 = new PolyMesh();
		Vector2[] half = new Vector2[7]
		{
			new Vector2(0f, 0.95f),
			new Vector2(0.38f, 0.98f),
			new Vector2(0.82f, 0.62f),
			new Vector2(0.98f, 0.05f),
			new Vector2(0.92f, -0.62f),
			new Vector2(0.6f, -0.98f),
			new Vector2(0f, -1.02f)
		};
		polyMesh2.Loft(new Vector3[4][]
		{
			PolyMesh.Mirror(0f, 1f, Vector2.zero, half),
			PolyMesh.Mirror(0.06f, 1.02f, Vector2.zero, half),
			PolyMesh.Mirror(0.42f, 0.86f, new Vector2(0f, -0.02f), half),
			PolyMesh.Mirror(0.5f, 0.78f, new Vector2(0f, -0.05f), half)
		});
		Turret = polyMesh2.Build("BMP-2 turret");
		PolyMesh polyMesh3 = new PolyMesh();
		polyMesh3.Tube(new Vector3(0.42f, 0.46f, -0.3f), new Vector3(0.42f, 0.6f, -0.3f), 0.3f, 0.28f, 16);
		polyMesh3.Tube(new Vector3(-0.4f, 0.46f, -0.38f), new Vector3(-0.4f, 0.55f, -0.38f), 0.27f, 0.27f, 16);
		polyMesh3.Box(new Vector3(-0.42f, 0.56f, 0.38f), new Vector3(0.26f, 0.2f, 0.3f), Quaternion.identity, 0.03f);
		polyMesh3.Box(new Vector3(0.42f, 0.66f, -0.06f), new Vector3(0.2f, 0.1f, 0.12f), Quaternion.identity, 0.02f);
		polyMesh3.Tube(new Vector3(0.02f, 0.45f, -0.05f), new Vector3(0.02f, 0.74f, -0.05f), 0.045f, 0.045f, 10);
		polyMesh3.Box(new Vector3(0.02f, 0.76f, -0.05f), new Vector3(0.14f, 0.08f, 0.45f), Quaternion.identity, 0.02f);
		polyMesh3.Tube(new Vector3(0.02f, 0.86f, -0.62f), new Vector3(0.02f, 0.86f, 0.64f), 0.075f, 0.075f, 14);
		polyMesh3.Tube(new Vector3(0.02f, 0.86f, 0.62f), new Vector3(0.02f, 0.86f, 0.7f), 0.085f, 0.085f, 14);
		polyMesh3.Tube(new Vector3(0.02f, 0.86f, -0.68f), new Vector3(0.02f, 0.86f, -0.6f), 0.085f, 0.085f, 14);
		array = new int[2] { -1, 1 };
		foreach (int num6 in array)
		{
			for (int n = 0; n < 3; n++)
			{
				Vector3 vector = new Vector3((float)num6 * 0.86f, 0.28f, -0.45f - (float)n * 0.16f);
				polyMesh3.Tube(vector, vector + new Vector3((float)num6 * 0.14f, 0.1f, 0.06f), 0.04f, 0.04f, 10);
			}
		}
		polyMesh3.Tube(new Vector3(0.3f, 0.46f, -0.85f), new Vector3(0.32f, 2.9f, -0.95f), 0.008f, 0.005f, 5);
		polyMesh3.Tube(new Vector3(0.62f, 0.62f, -0.12f), new Vector3(0.62f, 0.62f, 0.12f), 0.07f, 0.075f, 12);
		TurretKit = polyMesh3.Build("BMP-2 turret fittings");
		PolyMesh polyMesh4 = new PolyMesh();
		polyMesh4.Box(new Vector3(0f, 0f, -0.05f), new Vector3(0.48f, 0.3f, 0.28f), Quaternion.identity, 0.04f);
		polyMesh4.Tube(new Vector3(0f, 0f, 0.08f), new Vector3(0f, 0f, 0.3f), 0.09f, 0.08f, 14);
		polyMesh4.Tube(new Vector3(0.18f, 0.02f, 0.05f), new Vector3(0.18f, 0.02f, 0.22f), 0.025f, 0.022f, 8);
		Mantlet = polyMesh4.Build("BMP-2 mantlet");
		PolyMesh polyMesh5 = new PolyMesh();
		polyMesh5.Tube(new Vector3(0f, 0f, 0.25f), new Vector3(0f, 0f, 0.95f), 0.065f, 0.06f, 14);
		polyMesh5.Tube(new Vector3(0f, 0f, 0.9f), new Vector3(0f, 0f, 2.88f), 0.036f, 0.032f, 12);
		polyMesh5.Tube(new Vector3(0f, 0f, 2.85f), new Vector3(0f, 0f, 3.05f), 0.048f, 0.046f, 12);
		polyMesh5.Tube(new Vector3(0f, 0f, 2.6f), new Vector3(0f, 0f, 2.66f), 0.042f, 0.042f, 12);
		Barrel = polyMesh5.Build("2A42 barrel");
		Tyres = Kit.WheelTyres(0.36f, 0.3f, "BMP-2 road wheel tyres");
		Hubs = Kit.WheelHub(0.36f, 0.3f, 6, "BMP-2 road wheel hubs");
		Sprocket = Kit.Sprocket(0.28f, 0.3f, 14, "BMP-2 sprocket");
		Idler = Kit.WheelHub(0.27f, 0.28f, 6, "BMP-2 idler");
		Shoe = Kit.Shoe(0.3f, 0.13f, "BMP-2 track shoe");
	}
}

internal static class BradleyMeshes
{
	public static readonly float[] Stations = new float[6] { 2.2f, 1.36f, 0.52f, -0.32f, -1.16f, -2f };

	public static readonly Vector3 Front = new Vector3(0f, 0.78f, 2.92f);

	public static readonly Vector3 Rear = new Vector3(0f, 0.66f, -2.82f);

	public static readonly Vector3 Pivot = new Vector3(0.22f, 2f, 0.35f);

	public static readonly Vector3 GunLocal = new Vector3(0f, 0.36f, 1.08f);

	public static Mesh Hull;

	public static Mesh Armour;

	public static Mesh Deck;

	public static Mesh Turret;

	public static Mesh TurretKit;

	public static Mesh Launcher;

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
		float num = Mathf.Min(1.08f, top - 0.03f);
		return new Vector2[8]
		{
			new Vector2(0f, bottom),
			new Vector2(1.04f, bottom),
			new Vector2(1.08f, Mathf.Min(bottom + 0.05f, num - 0.01f)),
			new Vector2(1.08f, num),
			new Vector2(1.66f, num),
			new Vector2(1.66f, top - 0.05f),
			new Vector2(1.6f, top),
			new Vector2(0f, top)
		};
	}

	private static float GlacisAt(float z)
	{
		return 1.2f + (3.2f - z) * 0.47f;
	}

	public static void Build()
	{
		VehicleMesh vehicleMesh = new VehicleMesh();
		vehicleMesh.Hull(new(float, Vector2[])[6]
		{
			(3.3f, Section(0.86f, 1.1f)),
			(3.2f, Section(0.74f, 1.2f)),
			(2.62f, Section(0.45f, GlacisAt(2.62f))),
			(1.5f, Section(0.45f, 2f)),
			(-3.02f, Section(0.45f, 2f)),
			(-3.22f, Section(0.52f, 1.94f))
		});
		vehicleMesh.Box(new Vector3(0f, 1.15f, -3.25f), new Vector3(1.6f, 1.25f, 0.06f), Quaternion.identity);
		int[] array = new int[2] { -1, 1 };
		foreach (int num in array)
		{
			vehicleMesh.Box(new Vector3((float)num * 1.36f, 1.1f, 3.12f), new Vector3(0.56f, 0.03f, 0.42f), Quaternion.Euler(16f, 0f, 0f));
			vehicleMesh.Box(new Vector3((float)num * 1.35f, 1.75f, -3.27f), new Vector3(0.18f, 0.1f, 0.04f), Quaternion.identity);
		}
		Hull = vehicleMesh.Build("M2A2 Bradley hull");
		PolyMesh polyMesh = new PolyMesh();
		array = new int[2] { -1, 1 };
		foreach (int num2 in array)
		{
			for (int j = 0; j < 5; j++)
			{
				float num3 = 2.25f - (float)j * 1.12f;
				polyMesh.Box(new Vector3((float)num2 * 1.69f, 0.88f, num3), new Vector3(0.06f, 0.46f, 1.08f), Quaternion.identity, 0.012f);
				for (int k = 0; k < 4; k++)
				{
					polyMesh.Box(new Vector3((float)num2 * 1.725f, 1.03f, num3 - 0.4f + (float)k * 0.27f), new Vector3(0.02f, 0.04f, 0.04f), Quaternion.identity);
				}
			}
			for (int l = 0; l < 3; l++)
			{
				float num4 = 0.3f - (float)l * 1.35f;
				polyMesh.Box(new Vector3((float)num2 * 1.69f, 1.55f, num4), new Vector3(0.05f, 0.72f, 1.3f), Quaternion.identity, 0.012f);
				for (int m = 0; m < 2; m++)
				{
					for (int n = 0; n < 3; n++)
					{
						polyMesh.Box(new Vector3((float)num2 * 1.72f, 1.3f + (float)m * 0.5f, num4 - 0.5f + (float)n * 0.5f), new Vector3(0.02f, 0.04f, 0.04f), Quaternion.identity);
					}
				}
			}
		}
		Armour = polyMesh.Build("M2A2 Bradley side armour");
		PolyMesh polyMesh2 = new PolyMesh();
		Quaternion quaternion = Quaternion.Euler(25f, 0f, 0f);
		polyMesh2.Box(new Vector3(0f, GlacisAt(2.75f) + 0.04f, 2.75f), new Vector3(2.6f, 0.03f, 0.55f), quaternion, 0.012f);
		for (int num5 = 0; num5 < 6; num5++)
		{
			float num6 = 2.3f - (float)num5 * 0.11f;
			polyMesh2.Box(new Vector3(0.75f, GlacisAt(num6) + 0.02f, num6), new Vector3(0.9f, 0.03f, 0.05f), quaternion, 0.008f);
		}
		polyMesh2.Tube(new Vector3(-0.68f, 2f, 1.25f), new Vector3(-0.68f, 2.12f, 1.25f), 0.3f, 0.28f, 16);
		for (int num7 = 0; num7 < 4; num7++)
		{
			Quaternion quaternion2 = Quaternion.Euler(0f, -60f + (float)num7 * 40f, 0f);
			polyMesh2.Box(new Vector3(-0.68f, 2.16f, 1.25f) + quaternion2 * new Vector3(0f, 0f, 0.24f), new Vector3(0.12f, 0.08f, 0.06f), quaternion2, 0.01f);
		}
		polyMesh2.Box(new Vector3(-0.55f, 2.03f, -1.8f), new Vector3(0.9f, 0.05f, 0.9f), Quaternion.identity, 0.02f);
		array = new int[2] { -1, 1 };
		foreach (int num8 in array)
		{
			polyMesh2.Box(new Vector3((float)num8 * 1.25f, 1.25f, 3.22f), new Vector3(0.2f, 0.12f, 0.08f), Quaternion.identity, 0.015f);
			polyMesh2.Tube(new Vector3((float)num8 * 1.25f, 1.12f, 3.2f), new Vector3((float)num8 * 1.25f, 1.42f, 3.2f), 0.012f, 0.012f, 6);
			polyMesh2.Tube(new Vector3((float)num8 * 0.6f, 0.82f, 3.28f), new Vector3((float)num8 * 0.6f, 0.82f, 3.42f), 0.05f, 0.05f, 10);
		}
		Deck = polyMesh2.Build("M2A2 Bradley deck fittings");
		PolyMesh polyMesh3 = new PolyMesh();
		Vector2[] half = new Vector2[6]
		{
			new Vector2(0f, 1.05f),
			new Vector2(0.5f, 1.1f),
			new Vector2(0.95f, 0.78f),
			new Vector2(1.02f, -0.85f),
			new Vector2(0.85f, -1.3f),
			new Vector2(0f, -1.3f)
		};
		polyMesh3.Loft(new Vector3[4][]
		{
			PolyMesh.Mirror(0f, 1f, Vector2.zero, half),
			PolyMesh.Mirror(0.08f, 1f, Vector2.zero, half),
			PolyMesh.Mirror(0.62f, 0.95f, new Vector2(0f, -0.03f), half),
			PolyMesh.Mirror(0.74f, 0.86f, new Vector2(0f, -0.08f), half)
		});
		polyMesh3.Box(new Vector3(0f, 0.36f, -1.42f), new Vector3(1.7f, 0.5f, 0.3f), Quaternion.identity, 0.03f);
		Turret = polyMesh3.Build("M2A2 Bradley turret");
		PolyMesh polyMesh4 = new PolyMesh();
		polyMesh4.Box(new Vector3(0.52f, 0.86f, 0.55f), new Vector3(0.4f, 0.26f, 0.42f), Quaternion.identity, 0.03f);
		polyMesh4.Tube(new Vector3(0.45f, 0.72f, -0.45f), new Vector3(0.45f, 0.82f, -0.45f), 0.32f, 0.3f, 16);
		for (int num9 = 0; num9 < 5; num9++)
		{
			Quaternion quaternion3 = Quaternion.Euler(0f, -80f + (float)num9 * 40f, 0f);
			polyMesh4.Box(new Vector3(0.45f, 0.86f, -0.45f) + quaternion3 * new Vector3(0f, 0f, 0.27f), new Vector3(0.12f, 0.08f, 0.06f), quaternion3, 0.01f);
		}
		polyMesh4.Tube(new Vector3(-0.4f, 0.72f, -0.25f), new Vector3(-0.4f, 0.79f, -0.25f), 0.28f, 0.28f, 16);
		array = new int[2] { -1, 1 };
		foreach (int num10 in array)
		{
			for (int num11 = 0; num11 < 4; num11++)
			{
				Vector3 vector = new Vector3((float)num10 * 0.92f, 0.5f + (float)(num11 % 2) * 0.1f, 0.62f - (float)(num11 / 2) * 0.12f);
				polyMesh4.Tube(vector, vector + new Vector3((float)num10 * 0.12f, 0.1f, 0.18f), 0.04f, 0.04f, 10);
			}
		}
		polyMesh4.Tube(new Vector3(-0.75f, 0.74f, -1.2f), new Vector3(-0.78f, 3.2f, -1.3f), 0.008f, 0.005f, 5);
		polyMesh4.Tube(new Vector3(0.75f, 0.74f, -1.2f), new Vector3(0.78f, 2.8f, -1.3f), 0.008f, 0.005f, 5);
		polyMesh4.Tube(new Vector3(-0.85f, 0.62f, -1.6f), new Vector3(0.85f, 0.62f, -1.6f), 0.02f, 0.02f, 6);
		polyMesh4.Tube(new Vector3(-0.85f, 0.2f, -1.6f), new Vector3(-0.85f, 0.62f, -1.6f), 0.02f, 0.02f, 6);
		polyMesh4.Tube(new Vector3(0.85f, 0.2f, -1.6f), new Vector3(0.85f, 0.62f, -1.6f), 0.02f, 0.02f, 6);
		TurretKit = polyMesh4.Build("M2A2 Bradley turret fittings");
		PolyMesh polyMesh5 = new PolyMesh();
		polyMesh5.Box(new Vector3(-1.22f, 0.42f, -0.25f), new Vector3(0.36f, 0.38f, 1.25f), Quaternion.identity, 0.03f);
		polyMesh5.Box(new Vector3(-1.02f, 0.42f, -0.25f), new Vector3(0.1f, 0.12f, 0.3f), Quaternion.identity, 0.015f);
		for (int num12 = 0; num12 < 2; num12++)
		{
			polyMesh5.Tube(new Vector3(-1.22f, 0.33f + (float)num12 * 0.18f, 0.36f), new Vector3(-1.22f, 0.33f + (float)num12 * 0.18f, 0.41f), 0.075f, 0.075f, 12);
		}
		Launcher = polyMesh5.Build("TOW launcher");
		PolyMesh polyMesh6 = new PolyMesh();
		polyMesh6.Box(new Vector3(0f, 0f, -0.05f), new Vector3(0.62f, 0.36f, 0.3f), Quaternion.identity, 0.04f);
		polyMesh6.Tube(new Vector3(0f, 0f, 0.1f), new Vector3(0f, 0f, 0.35f), 0.085f, 0.08f, 14);
		polyMesh6.Tube(new Vector3(0.22f, 0f, 0.05f), new Vector3(0.22f, 0f, 0.3f), 0.022f, 0.02f, 8);
		Mantlet = polyMesh6.Build("M2A2 Bradley mantlet");
		PolyMesh polyMesh7 = new PolyMesh();
		polyMesh7.Tube(new Vector3(0f, 0f, 0.25f), new Vector3(0f, 0f, 0.8f), 0.06f, 0.058f, 14);
		polyMesh7.Tube(new Vector3(0f, 0f, 0.75f), new Vector3(0f, 0f, 2.3f), 0.032f, 0.03f, 12);
		polyMesh7.Tube(new Vector3(0f, 0f, 2.25f), new Vector3(0f, 0f, 2.45f), 0.046f, 0.044f, 12);
		Barrel = polyMesh7.Build("M242 barrel");
		Tyres = Kit.WheelTyres(0.32f, 0.53f, "M2A2 Bradley road wheel tyres");
		Hubs = Kit.WheelHub(0.32f, 0.53f, 8, "M2A2 Bradley road wheel hubs");
		Sprocket = Kit.Sprocket(0.3f, 0.53f, 11, "M2A2 Bradley sprocket");
		Idler = Kit.WheelHub(0.28f, 0.5f, 6, "M2A2 Bradley idler");
		Shoe = Kit.Shoe(0.53f, 0.14f, "M2A2 Bradley track shoe");
	}
}
