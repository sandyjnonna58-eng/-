using UnityEngine;

namespace CombatSandbox.Demo;

internal static class HeliKit
{
	public static Vector3[] Circle(Vector3 centre, float r, int n = 20)
	{
		Vector3[] array = new Vector3[n];
		for (int i = 0; i < n; i++)
		{
			float f = (float)i / (float)n * Mathf.PI * 2f;
			array[i] = centre + new Vector3(Mathf.Cos(f) * r, 0f, Mathf.Sin(f) * r);
		}
		return array;
	}

	public static void Wing(PolyMesh m, float x0, float x1, float y0, float y1, float lead0, float chord0, float lead1, float chord1, float thick)
	{
		m.Loft(new Vector3[2][]
		{
			Airfoil(x0, y0, lead0, chord0, thick),
			Airfoil(x1, y1, lead1, chord1, thick)
		});
	}

	private static Vector3[] Airfoil(float x, float y, float lead, float chord, float thick)
	{
		float num = chord * thick;
		return new Vector3[6]
		{
			new Vector3(x, y, lead),
			new Vector3(x, y + num * 0.55f, lead - chord * 0.25f),
			new Vector3(x, y + num * 0.3f, lead - chord * 0.7f),
			new Vector3(x, y, lead - chord),
			new Vector3(x, y - num * 0.2f, lead - chord * 0.7f),
			new Vector3(x, y - num * 0.45f, lead - chord * 0.25f)
		};
	}

	public static void Fin(PolyMesh m, float x, float thick, params Vector2[] zy)
	{
		Vector3[] array = new Vector3[zy.Length];
		Vector3[] array2 = new Vector3[zy.Length];
		for (int i = 0; i < zy.Length; i++)
		{
			array[i] = new Vector3(x - thick * 0.5f, zy[i].y, zy[i].x);
			array2[i] = new Vector3(x + thick * 0.5f, zy[i].y, zy[i].x);
		}
		m.Loft(new Vector3[2][] { array, array2 });
	}

	public static void Pod(PolyMesh m, Vector3 front, float length, float r)
	{
		m.Loft(new Vector3[5][]
		{
			PolyMesh.Oval(front.z, front.y, r * 0.86f, r * 0.86f, 16, 2f, front.x),
			PolyMesh.Oval(front.z - 0.06f, front.y, r, r, 16, 2f, front.x),
			PolyMesh.Oval(front.z - length * 0.8f, front.y, r, r, 16, 2f, front.x),
			PolyMesh.Oval(front.z - length * 0.95f, front.y, r * 0.75f, r * 0.75f, 16, 2f, front.x),
			PolyMesh.Oval(front.z - length, front.y, r * 0.35f, r * 0.35f, 16, 2f, front.x)
		}, smooth: true);
	}

	public static void Missile(PolyMesh m, Vector3 front, float length, float r)
	{
		m.Loft(new Vector3[4][]
		{
			PolyMesh.Oval(front.z, front.y, r * 0.2f, r * 0.2f, 12, 2f, front.x),
			PolyMesh.Oval(front.z - r * 1.6f, front.y, r, r, 12, 2f, front.x),
			PolyMesh.Oval(front.z - length, front.y, r, r, 12, 2f, front.x),
			PolyMesh.Oval(front.z - length - 0.02f, front.y, r * 0.6f, r * 0.6f, 12, 2f, front.x)
		}, smooth: true);
	}

	public static Mesh Rotor(int blades, float length, float chord, float hubR, string name)
	{
		PolyMesh polyMesh = new PolyMesh();
		polyMesh.Tube(new Vector3(0f, -0.12f, 0f), new Vector3(0f, 0.18f, 0f), hubR, hubR * 0.8f, 16);
		polyMesh.Tube(new Vector3(0f, 0.18f, 0f), new Vector3(0f, 0.3f, 0f), hubR * 0.5f, hubR * 0.2f, 12);
		for (int i = 0; i < blades; i++)
		{
			Quaternion quaternion = Quaternion.Euler(0f, (float)i * 360f / (float)blades, 0f);
			polyMesh.Box(quaternion * new Vector3(0f, 0.04f, hubR + 0.18f), new Vector3(0.18f, 0.12f, 0.42f), quaternion, 0.02f);
			polyMesh.Box(quaternion * new Vector3(0f, 0.05f, hubR + 0.35f + length * 0.5f), new Vector3(chord, 0.045f, length), quaternion * Quaternion.Euler(0f, 0f, 4f), 0.012f);
		}
		return polyMesh.Build(name);
	}

	public static Mesh TailRotor(int blades, float r, float chord, string name)
	{
		PolyMesh polyMesh = new PolyMesh();
		polyMesh.Tube(new Vector3(-0.08f, 0f, 0f), new Vector3(0.12f, 0f, 0f), 0.12f, 0.08f, 12);
		for (int i = 0; i < blades; i++)
		{
			Quaternion quaternion = Quaternion.Euler((float)i * 360f / (float)blades, 0f, 0f);
			polyMesh.Box(quaternion * new Vector3(-0.04f, r * 0.5f + 0.05f, 0f), new Vector3(0.03f, r, chord), quaternion * Quaternion.Euler(0f, 6f, 0f), 0.008f);
		}
		return polyMesh.Build(name);
	}
}

internal static class Mi24Meshes
{
	public static readonly Vector3 Mast = new Vector3(0f, 3.95f, 0.1f);

	public static readonly Vector3 TailHub = new Vector3(-0.24f, 3.95f, -10.9f);

	public static readonly Vector3 ChinGun = new Vector3(0f, 0.95f, 6.15f);

	public static readonly Vector3[] Pods = new Vector3[4]
	{
		new Vector3(-1.75f, 1.21f, 0.8f),
		new Vector3(1.75f, 1.21f, 0.8f),
		new Vector3(-2.65f, 1.05f, 0.8f),
		new Vector3(2.65f, 1.05f, 0.8f)
	};

	public static Mesh Fuselage;

	public static Mesh Glass;

	public static Mesh Fittings;

	public static Mesh Stores;

	public static Mesh Rotor;

	public static Mesh TailRotor;

	public static Mesh Gun;

	public static Mesh Tyre;

	public static bool Ready => Fuselage != null;

	private static float WingY(float x)
	{
		return 1.75f - (Mathf.Abs(x) - 0.85f) / 2.45f * 0.43f;
	}

	public static void Build()
	{
		PolyMesh polyMesh = new PolyMesh();
		polyMesh.Loft(new Vector3[12][]
		{
			PolyMesh.Oval(6.95f, 1.45f, 0.12f, 0.14f, 20, 2.2f),
			PolyMesh.Oval(6.65f, 1.42f, 0.42f, 0.42f, 20, 2.2f),
			PolyMesh.Oval(6f, 1.5f, 0.6f, 0.6f, 20, 2.3f),
			PolyMesh.Oval(5.2f, 1.6f, 0.72f, 0.72f, 20, 2.4f),
			PolyMesh.Oval(4.6f, 1.75f, 0.8f, 0.86f, 20, 2.5f),
			PolyMesh.Oval(3.6f, 1.85f, 0.86f, 0.95f, 20, 2.6f),
			PolyMesh.Oval(2.6f, 1.9f, 0.9f, 1f, 20, 2.7f),
			PolyMesh.Oval(-0.8f, 1.9f, 0.9f, 1f, 20, 2.7f),
			PolyMesh.Oval(-2.6f, 2.15f, 0.65f, 0.72f, 20, 2.5f),
			PolyMesh.Oval(-4f, 2.35f, 0.38f, 0.44f, 20, 2.3f),
			PolyMesh.Oval(-9.5f, 2.6f, 0.2f, 0.24f, 20, 2.2f),
			PolyMesh.Oval(-10.5f, 2.7f, 0.13f, 0.17f, 20, 2.2f)
		}, smooth: true);
		polyMesh.Loft(new Vector3[4][]
		{
			PolyMesh.Oval(2.4f, 2.92f, 0.45f, 0.16f, 20, 3f),
			PolyMesh.Oval(2.05f, 3f, 0.86f, 0.38f, 20, 3f),
			PolyMesh.Oval(-1.8f, 3f, 0.86f, 0.38f, 20, 3f),
			PolyMesh.Oval(-2.5f, 2.95f, 0.5f, 0.24f, 20, 3f)
		}, smooth: true);
		int[] array = new int[2] { -1, 1 };
		foreach (int num in array)
		{
			polyMesh.Loft(new Vector3[4][]
			{
				PolyMesh.Oval(2.95f, 3.02f, 0.18f, 0.18f, 16, 2f, (float)num * 0.48f),
				PolyMesh.Oval(2.75f, 3.02f, 0.3f, 0.3f, 16, 2.2f, (float)num * 0.48f),
				PolyMesh.Oval(2.2f, 3.02f, 0.3f, 0.3f, 16, 2.2f, (float)num * 0.48f),
				PolyMesh.Oval(1.95f, 3.02f, 0.26f, 0.26f, 16, 2.2f, (float)num * 0.48f)
			}, smooth: true);
			HeliKit.Wing(polyMesh, (float)num * 0.85f, (float)num * 3.3f, 1.75f, 1.32f, -0.15f, 1.15f, -0.25f, 0.85f, 0.12f);
			HeliKit.Wing(polyMesh, (float)num * 0.15f, (float)num * 1.7f, 2.65f, 2.65f, -9f, 0.8f, -9.15f, 0.6f, 0.1f);
		}
		HeliKit.Fin(polyMesh, 0f, 0.2f, new Vector2(-9.4f, 2.55f), new Vector2(-10.6f, 4.3f), new Vector2(-11.25f, 4.3f), new Vector2(-10.65f, 2.6f));
		Fuselage = polyMesh.Build("Mi-24 fuselage");
		PolyMesh polyMesh2 = new PolyMesh();
		polyMesh2.Loft(new Vector3[5][]
		{
			PolyMesh.Oval(6.55f, 1.68f, 0.28f, 0.26f, 20, 2f),
			PolyMesh.Oval(6.25f, 1.8f, 0.52f, 0.46f, 20, 2f),
			PolyMesh.Oval(5.6f, 1.9f, 0.64f, 0.58f, 20, 2f),
			PolyMesh.Oval(5.05f, 1.9f, 0.6f, 0.54f, 20, 2f),
			PolyMesh.Oval(4.85f, 1.85f, 0.45f, 0.4f, 20, 2f)
		}, smooth: true);
		polyMesh2.Loft(new Vector3[5][]
		{
			PolyMesh.Oval(4.75f, 2.3f, 0.36f, 0.3f, 20, 2f),
			PolyMesh.Oval(4.4f, 2.45f, 0.6f, 0.48f, 20, 2f),
			PolyMesh.Oval(3.7f, 2.55f, 0.66f, 0.55f, 20, 2f),
			PolyMesh.Oval(3.1f, 2.45f, 0.54f, 0.42f, 20, 2f),
			PolyMesh.Oval(2.85f, 2.4f, 0.36f, 0.3f, 20, 2f)
		}, smooth: true);
		array = new int[2] { -1, 1 };
		foreach (int num2 in array)
		{
			for (int j = 0; j < 3; j++)
			{
				polyMesh2.Box(new Vector3((float)num2 * 0.88f, 2.05f, 1.6f - (float)j * 0.75f), new Vector3(0.04f, 0.38f, 0.45f), Quaternion.Euler(0f, 0f, (float)num2 * -6f), 0.04f);
			}
		}
		Glass = polyMesh2.Build("Mi-24 glazing");
		PolyMesh polyMesh3 = new PolyMesh();
		polyMesh3.Tube(new Vector3(0f, 3.3f, 0.1f), Mast, 0.16f, 0.12f, 14);
		array = new int[2] { -1, 1 };
		foreach (int num3 in array)
		{
			polyMesh3.Tube(new Vector3((float)num3 * 0.55f, 3.05f, -2f), new Vector3((float)num3 * 0.98f, 2.95f, -2.9f), 0.22f, 0.28f, 14);
			polyMesh3.Tube(new Vector3((float)num3 * 0.48f, 3.02f, 2.97f), new Vector3((float)num3 * 0.48f, 3.02f, 3.02f), 0.2f, 0.2f, 14);
			polyMesh3.Tube(new Vector3((float)num3 * 0.2f, 1f, 4.9f), new Vector3((float)num3 * 0.18f, 0.3f, 4.9f), 0.05f, 0.05f, 8);
			polyMesh3.Tube(new Vector3((float)num3 * 0.85f, 1.05f, -0.9f), new Vector3((float)num3 * 1.5f, 0.36f, -0.9f), 0.07f, 0.06f, 8);
			polyMesh3.Tube(new Vector3((float)num3 * 1.3f, WingY(1.3f) - 0.05f, -0.6f), new Vector3((float)num3 * 1.5f, 0.45f, -0.9f), 0.05f, 0.05f, 8);
			polyMesh3.Box(new Vector3((float)num3 * 0.9f, 1.25f, 4.3f), new Vector3(0.04f, 0.5f, 0.7f), Quaternion.identity, 0.02f);
		}
		polyMesh3.Box(new Vector3(0f, 0.86f, 2f), new Vector3(0.5f, 0.12f, 0.9f), Quaternion.identity, 0.03f);
		polyMesh3.Tube(new Vector3(0.3f, 3.35f, -1.2f), new Vector3(0.32f, 3.9f, -1.3f), 0.015f, 0.01f, 5);
		polyMesh3.Tube(new Vector3(0.5f, 1.15f, 6.4f), new Vector3(0.5f, 1.15f, 7.6f), 0.02f, 0.015f, 6);
		polyMesh3.Tube(new Vector3(0.62f, 1.05f, 6.1f), new Vector3(0.62f, 0.85f, 6.2f), 0.09f, 0.09f, 12);
		Fittings = polyMesh3.Build("Mi-24 fittings");
		PolyMesh polyMesh4 = new PolyMesh();
		array = new int[2] { -1, 1 };
		foreach (int num4 in array)
		{
			float[] array2 = new float[2] { 1.75f, 2.65f };
			foreach (float num5 in array2)
			{
				float num6 = WingY(num5);
				polyMesh4.Box(new Vector3((float)num4 * num5, num6 - 0.15f, -0.35f), new Vector3(0.08f, 0.24f, 0.7f), Quaternion.identity, 0.02f);
				HeliKit.Pod(polyMesh4, new Vector3((float)num4 * num5, num6 - 0.38f, 0.8f), 2f, 0.25f);
			}
			for (int l = 0; l < 2; l++)
			{
				polyMesh4.Tube(new Vector3((float)num4 * 3.36f, 1.2f + (float)l * 0.24f, 0.65f), new Vector3((float)num4 * 3.36f, 1.2f + (float)l * 0.24f, -1.1f), 0.07f, 0.07f, 12);
			}
			polyMesh4.Box(new Vector3((float)num4 * 3.33f, 1.32f, -0.25f), new Vector3(0.06f, 0.4f, 1.2f), Quaternion.identity, 0.02f);
		}
		Stores = polyMesh4.Build("Mi-24 weapons");
		Rotor = HeliKit.Rotor(5, 8.2f, 0.58f, 0.32f, "Mi-24 main rotor");
		TailRotor = HeliKit.TailRotor(3, 1.9f, 0.3f, "Mi-24 tail rotor");
		PolyMesh polyMesh5 = new PolyMesh();
		polyMesh5.Loft(new Vector3[5][]
		{
			PolyMesh.Oval(0.22f, 0f, 0.08f, 0.08f, 16, 2f),
			PolyMesh.Oval(0.15f, 0f, 0.2f, 0.2f, 16, 2f),
			PolyMesh.Oval(-0.05f, 0f, 0.24f, 0.24f, 16, 2f),
			PolyMesh.Oval(-0.25f, 0f, 0.2f, 0.2f, 16, 2f),
			PolyMesh.Oval(-0.35f, 0f, 0.08f, 0.08f, 16, 2f)
		}, smooth: true);
		polyMesh5.Tube(new Vector3(0f, 0f, 0.1f), new Vector3(0f, 0f, 0.45f), 0.09f, 0.08f, 12);
		for (int m = 0; m < 4; m++)
		{
			float f = (float)m * Mathf.PI * 0.5f + Mathf.PI * 0.25f;
			Vector3 vector = new Vector3(Mathf.Cos(f), Mathf.Sin(f), 0f) * 0.04f;
			polyMesh5.Tube(vector + new Vector3(0f, 0f, 0.4f), vector + new Vector3(0f, 0f, 1.15f), 0.018f, 0.018f, 8);
		}
		Gun = polyMesh5.Build("YakB-12.7 turret");
		Tyre = VehicleMesh.Tyre(0.36f, 0.25f, "Mi-24 tyre");
	}
}

internal static class ApacheMeshes
{
	public static readonly Vector3 Mast = new Vector3(0f, 3.75f, 0.4f);

	public static readonly Vector3 TailHub = new Vector3(-0.25f, 4.15f, -9.6f);

	public static readonly Vector3 ChinGun = new Vector3(0f, 0.8f, 3.7f);

	public static readonly Vector3[] Pods = new Vector3[4]
	{
		new Vector3(-1.35f, 1.42f, 1.38f),
		new Vector3(1.35f, 1.42f, 1.38f),
		new Vector3(-2.15f, 1.4f, 1.3f),
		new Vector3(2.15f, 1.4f, 1.3f)
	};

	public static Mesh Fuselage;

	public static Mesh Glass;

	public static Mesh Fittings;

	public static Mesh Stores;

	public static Mesh Rotor;

	public static Mesh TailRotor;

	public static Mesh Gun;

	public static Mesh Tyre;

	public static Mesh TailTyre;

	public static bool Ready => Fuselage != null;

	public static void Build()
	{
		PolyMesh polyMesh = new PolyMesh();
		polyMesh.Loft(new Vector3[12][]
		{
			PolyMesh.Oval(6.95f, 1.25f, 0.16f, 0.2f, 16, 3f),
			PolyMesh.Oval(6.7f, 1.28f, 0.38f, 0.42f, 16, 3f),
			PolyMesh.Oval(6f, 1.35f, 0.48f, 0.5f, 16, 3f),
			PolyMesh.Oval(5f, 1.45f, 0.52f, 0.56f, 16, 3f),
			PolyMesh.Oval(4f, 1.6f, 0.55f, 0.66f, 16, 3f),
			PolyMesh.Oval(2.6f, 1.82f, 0.58f, 0.9f, 16, 3f),
			PolyMesh.Oval(0.6f, 1.95f, 0.6f, 0.95f, 16, 3f),
			PolyMesh.Oval(-1.2f, 2f, 0.55f, 0.85f, 16, 3f),
			PolyMesh.Oval(-2.6f, 2.1f, 0.42f, 0.6f, 16, 3f),
			PolyMesh.Oval(-4.2f, 2.18f, 0.28f, 0.4f, 16, 2.8f),
			PolyMesh.Oval(-8.3f, 2.35f, 0.17f, 0.25f, 16, 2.6f),
			PolyMesh.Oval(-9f, 2.42f, 0.13f, 0.2f, 16, 2.6f)
		}, smooth: true);
		int[] array = new int[2] { -1, 1 };
		foreach (int num in array)
		{
			float xc = (float)num * 0.66f;
			polyMesh.Loft(new Vector3[4][]
			{
				PolyMesh.Oval(5.5f, 1.2f, 0.06f, 0.15f, 12, 3f, xc),
				PolyMesh.Oval(5.1f, 1.25f, 0.2f, 0.3f, 12, 3f, xc),
				PolyMesh.Oval(1.2f, 1.3f, 0.22f, 0.34f, 12, 3f, xc),
				PolyMesh.Oval(0.5f, 1.35f, 0.1f, 0.2f, 12, 3f, xc)
			}, smooth: true);
			float xc2 = (float)num * 0.92f;
			polyMesh.Loft(new Vector3[4][]
			{
				PolyMesh.Oval(0.6f, 2.45f, 0.18f, 0.22f, 14, 2.4f, xc2),
				PolyMesh.Oval(0.3f, 2.45f, 0.3f, 0.33f, 14, 2.4f, xc2),
				PolyMesh.Oval(-1.8f, 2.45f, 0.3f, 0.33f, 14, 2.4f, xc2),
				PolyMesh.Oval(-2.3f, 2.45f, 0.22f, 0.26f, 14, 2.4f, xc2)
			}, smooth: true);
			polyMesh.Box(new Vector3((float)num * 0.62f, 2.45f, -0.7f), new Vector3(0.5f, 0.3f, 1.8f), Quaternion.identity, 0.06f);
			HeliKit.Wing(polyMesh, (float)num * 0.55f, (float)num * 2.55f, 1.95f, 1.92f, 0.85f, 1f, 0.8f, 0.9f, 0.12f);
			HeliKit.Wing(polyMesh, (float)num * 0.12f, (float)num * 1.7f, 2.5f, 2.5f, -9.05f, 0.8f, -9.15f, 0.65f, 0.1f);
		}
		HeliKit.Fin(polyMesh, 0f, 0.2f, new Vector2(-8f, 2.5f), new Vector2(-9.3f, 4.55f), new Vector2(-10f, 4.55f), new Vector2(-9.6f, 2.45f));
		Fuselage = polyMesh.Build("AH-64 fuselage");
		PolyMesh polyMesh2 = new PolyMesh();
		polyMesh2.Loft(new Vector3[4][]
		{
			PolyMesh.Oval(6.45f, 1.82f, 0.2f, 0.08f, 8, 8f),
			PolyMesh.Oval(6.1f, 1.98f, 0.44f, 0.26f, 8, 8f),
			PolyMesh.Oval(5.2f, 2.05f, 0.48f, 0.32f, 8, 8f),
			PolyMesh.Oval(4.95f, 2.02f, 0.44f, 0.27f, 8, 8f)
		});
		polyMesh2.Loft(new Vector3[4][]
		{
			PolyMesh.Oval(4.85f, 2.3f, 0.35f, 0.14f, 8, 8f),
			PolyMesh.Oval(4.4f, 2.48f, 0.48f, 0.33f, 8, 8f),
			PolyMesh.Oval(3.4f, 2.55f, 0.5f, 0.36f, 8, 8f),
			PolyMesh.Oval(3f, 2.45f, 0.38f, 0.24f, 8, 8f)
		});
		Glass = polyMesh2.Build("AH-64 canopy");
		PolyMesh polyMesh3 = new PolyMesh();
		polyMesh3.Tube(new Vector3(0f, 1f, 7.15f), new Vector3(0f, 1.52f, 7.15f), 0.3f, 0.3f, 16);
		array = new int[2] { -1, 1 };
		foreach (int num2 in array)
		{
			polyMesh3.Box(new Vector3((float)num2 * 0.32f, 1.15f, 7.2f), new Vector3(0.2f, 0.38f, 0.45f), Quaternion.identity, 0.04f);
			polyMesh3.Box(new Vector3((float)num2 * 1.05f, 2.5f, -2.55f), new Vector3(0.25f, 0.3f, 0.5f), Quaternion.Euler(0f, (float)num2 * 25f, 0f), 0.04f);
			polyMesh3.Tube(new Vector3((float)num2 * 0.75f, 1.05f, 3.55f), new Vector3((float)num2 * 1.05f, 0.32f, 3.3f), 0.07f, 0.06f, 8);
			polyMesh3.Tube(new Vector3((float)num2 * 0.7f, 1f, 2.8f), new Vector3((float)num2 * 1.05f, 0.4f, 3.3f), 0.05f, 0.05f, 8);
		}
		polyMesh3.Tube(new Vector3(0f, 1.72f, 6.95f), new Vector3(0f, 1.72f, 7.25f), 0.12f, 0.12f, 12);
		polyMesh3.Tube(new Vector3(0f, 2.85f, 0.4f), Mast, 0.14f, 0.12f, 14);
		polyMesh3.Tube(Mast + Vector3.up * 0.25f, Mast + Vector3.up * 0.45f, 0.09f, 0.09f, 10);
		polyMesh3.Loft(new Vector3[4][]
		{
			HeliKit.Circle(Mast + Vector3.up * 0.42f, 0.3f),
			HeliKit.Circle(Mast + Vector3.up * 0.52f, 0.52f),
			HeliKit.Circle(Mast + Vector3.up * 0.74f, 0.52f),
			HeliKit.Circle(Mast + Vector3.up * 0.84f, 0.3f)
		}, smooth: true);
		polyMesh3.Tube(new Vector3(0f, 2.15f, -8.6f), new Vector3(0f, 0.3f, -8.75f), 0.05f, 0.05f, 8);
		polyMesh3.Tube(new Vector3(0.25f, 2.8f, -1.5f), new Vector3(0.27f, 3.3f, -1.6f), 0.012f, 0.008f, 5);
		Fittings = polyMesh3.Build("AH-64 fittings");
		PolyMesh polyMesh4 = new PolyMesh();
		array = new int[2] { -1, 1 };
		foreach (int num3 in array)
		{
			polyMesh4.Box(new Vector3((float)num3 * 1.35f, 1.76f, 0.4f), new Vector3(0.08f, 0.3f, 0.6f), Quaternion.identity, 0.02f);
			polyMesh4.Box(new Vector3((float)num3 * 2.15f, 1.76f, 0.4f), new Vector3(0.08f, 0.3f, 0.6f), Quaternion.identity, 0.02f);
			HeliKit.Pod(polyMesh4, new Vector3((float)num3 * 1.35f, 1.42f, 1.38f), 1.65f, 0.22f);
			polyMesh4.Box(new Vector3((float)num3 * 2.15f, 1.6f, 0.45f), new Vector3(0.5f, 0.06f, 1.5f), Quaternion.identity, 0.015f);
			for (int j = 0; j < 4; j++)
			{
				HeliKit.Missile(polyMesh4, new Vector3((float)num3 * 2.15f + ((j % 2 == 0) ? (-0.15f) : 0.15f), (j < 2) ? 1.45f : 1.22f, 1.25f), 1.6f, 0.09f);
			}
			polyMesh4.Box(new Vector3((float)num3 * 2.15f, 1.33f, 0.45f), new Vector3(0.06f, 0.24f, 1.2f), Quaternion.identity, 0.015f);
		}
		Stores = polyMesh4.Build("AH-64 weapons");
		Rotor = HeliKit.Rotor(4, 7f, 0.53f, 0.3f, "AH-64 main rotor");
		TailRotor = HeliKit.TailRotor(4, 1.4f, 0.25f, "AH-64 tail rotor");
		PolyMesh polyMesh5 = new PolyMesh();
		polyMesh5.Box(new Vector3(0f, 0.05f, -0.1f), new Vector3(0.3f, 0.26f, 0.5f), Quaternion.identity, 0.04f);
		polyMesh5.Tube(new Vector3(0f, 0f, 0.1f), new Vector3(0f, 0f, 0.55f), 0.07f, 0.065f, 12);
		polyMesh5.Tube(new Vector3(0f, 0f, 0.5f), new Vector3(0f, 0f, 1.6f), 0.03f, 0.028f, 10);
		polyMesh5.Tube(new Vector3(0f, 0f, 1.55f), new Vector3(0f, 0f, 1.75f), 0.042f, 0.04f, 10);
		polyMesh5.Box(new Vector3(0.18f, 0.05f, -0.05f), new Vector3(0.1f, 0.2f, 0.3f), Quaternion.identity, 0.02f);
		Gun = polyMesh5.Build("M230 chain gun");
		Tyre = VehicleMesh.Tyre(0.32f, 0.22f, "AH-64 tyre");
		TailTyre = VehicleMesh.Tyre(0.22f, 0.12f, "AH-64 tail tyre");
	}
}
