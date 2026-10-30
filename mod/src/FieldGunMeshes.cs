using UnityEngine;

namespace CombatSandbox.Demo;

internal static class FieldGunMeshes
{
	public static readonly Vector3 MortarPivot = new Vector3(0f, 0.08f, 0f);

	public static readonly Vector3 HowitzerPivot = new Vector3(0f, 1.1f, 0f);

	public static readonly Vector3[] Wheels = new Vector3[2]
	{
		new Vector3(-0.95f, 0.45f, -0.2f),
		new Vector3(0.95f, 0.45f, -0.2f)
	};

	public static Mesh Sandbags;

	public static Mesh Baseplate;

	public static Mesh MortarTube;

	public static Mesh Carriage;

	public static Mesh Shield;

	public static Mesh Barrel;

	public static Mesh Tyre;

	public static Mesh Hub;

	public static bool Ready => Sandbags != null;

	public static (Vector3 at, float yaw)[] Bags()
	{
		(Vector3, float)[] array = new(Vector3, float)[24];
		for (int i = 0; i < 24; i++)
		{
			int num = i / 12;
			float num2 = ((float)(i % 12) + (float)num * 0.5f) * 30f;
			float f = num2 * (Mathf.PI / 180f);
			array[i] = (new Vector3(Mathf.Sin(f) * 1.6f, 0.12f + (float)num * 0.23f, Mathf.Cos(f) * 1.6f), num2);
		}
		return array;
	}

	public static void Build()
	{
		PolyMesh polyMesh = new PolyMesh();
		(Vector3, float)[] array = Bags();
		for (int i = 0; i < array.Length; i++)
		{
			(Vector3, float) tuple = array[i];
			polyMesh.Box(tuple.Item1, new Vector3(0.86f, 0.24f, 0.42f), Quaternion.Euler(0f, tuple.Item2, 0f), 0.08f);
		}
		Sandbags = polyMesh.Build("Sandbag ring");
		PolyMesh polyMesh2 = new PolyMesh();
		polyMesh2.Loft(new Vector3[3][]
		{
			HeliKit.Circle(new Vector3(0f, 0f, 0f), 0.42f),
			HeliKit.Circle(new Vector3(0f, 0.05f, 0f), 0.42f),
			HeliKit.Circle(new Vector3(0f, 0.09f, 0f), 0.24f)
		});
		int[] array2 = new int[2] { -1, 1 };
		foreach (int num in array2)
		{
			polyMesh2.Tube(new Vector3((float)num * 0.32f, 0.02f, 0.66f), new Vector3((float)num * 0.05f, 0.5f, 0.38f), 0.022f, 0.02f, 8);
			polyMesh2.Box(new Vector3((float)num * 0.32f, 0.02f, 0.66f), new Vector3(0.08f, 0.03f, 0.1f), Quaternion.identity, 0.01f);
		}
		polyMesh2.Box(new Vector3(0f, 0.5f, 0.38f), new Vector3(0.18f, 0.08f, 0.1f), Quaternion.Euler(-40f, 0f, 0f), 0.015f);
		polyMesh2.Tube(new Vector3(-0.15f, 0.48f, 0.4f), new Vector3(-0.15f, 0.36f, 0.48f), 0.015f, 0.015f, 6);
		Baseplate = polyMesh2.Build("Mortar baseplate and bipod");
		PolyMesh polyMesh3 = new PolyMesh();
		polyMesh3.Tube(new Vector3(0f, 0f, -0.06f), new Vector3(0f, 0f, 0.04f), 0.055f, 0.06f, 14);
		polyMesh3.Tube(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 1.2f), 0.05f, 0.048f, 14);
		polyMesh3.Tube(new Vector3(0f, 0f, 1.12f), new Vector3(0f, 0f, 1.2f), 0.058f, 0.058f, 14);
		polyMesh3.Tube(new Vector3(0f, 0f, 0.5f), new Vector3(0f, 0f, 0.6f), 0.062f, 0.062f, 14);
		MortarTube = polyMesh3.Build("82 mm mortar tube");
		PolyMesh polyMesh4 = new PolyMesh();
		polyMesh4.Tube(Wheels[0], Wheels[1], 0.06f, 0.06f, 10);
		polyMesh4.Box(new Vector3(0f, 0.72f, -0.2f), new Vector3(0.62f, 0.4f, 1.2f), Quaternion.identity, 0.05f);
		polyMesh4.Box(new Vector3(0f, 0.5f, -0.25f), new Vector3(0.5f, 0.2f, 0.7f), Quaternion.identity, 0.04f);
		array2 = new int[2] { -1, 1 };
		foreach (int num2 in array2)
		{
			polyMesh4.Box(new Vector3((float)num2 * 0.3f, 1.02f, 0f), new Vector3(0.08f, 0.42f, 0.5f), Quaternion.identity, 0.02f);
			Vector3 vector = new Vector3((float)num2 * 0.22f, 0.45f, -0.65f);
			Vector3 vector2 = new Vector3((float)num2 * 0.9f, 0.12f, -3f);
			polyMesh4.Box((vector + vector2) * 0.5f, new Vector3(0.16f, 0.18f, (vector2 - vector).magnitude), Quaternion.LookRotation(vector2 - vector), 0.025f);
			polyMesh4.Box(vector2 + new Vector3((float)num2 * 0.02f, 0.05f, -0.05f), new Vector3(0.42f, 0.36f, 0.06f), Quaternion.Euler(-15f, (float)num2 * 20f, 0f), 0.015f);
			polyMesh4.Box(vector2 + new Vector3(0f, 0.1f, 0.4f), new Vector3(0.12f, 0.06f, 0.4f), Quaternion.LookRotation(vector2 - vector), 0.01f);
		}
		Carriage = polyMesh4.Build("Howitzer carriage");
		PolyMesh polyMesh5 = new PolyMesh();
		array2 = new int[2] { -1, 1 };
		foreach (int num3 in array2)
		{
			polyMesh5.Box(new Vector3((float)num3 * 0.6f, 1.2f, 0.32f), new Vector3(0.9f, 0.95f, 0.04f), Quaternion.Euler(-8f, (float)num3 * 12f, 0f), 0.01f);
			polyMesh5.Box(new Vector3((float)num3 * 1.06f, 0.95f, 0.22f), new Vector3(0.04f, 0.5f, 0.3f), Quaternion.identity, 0.01f);
		}
		polyMesh5.Box(new Vector3(0f, 0.62f, 0.38f), new Vector3(2f, 0.32f, 0.04f), Quaternion.Euler(-8f, 0f, 0f), 0.01f);
		polyMesh5.Box(new Vector3(0f, 1.62f, 0.25f), new Vector3(0.36f, 0.12f, 0.04f), Quaternion.Euler(-8f, 0f, 0f), 0.01f);
		Shield = polyMesh5.Build("Howitzer shield");
		PolyMesh polyMesh6 = new PolyMesh();
		polyMesh6.Box(new Vector3(0f, 0f, 0.3f), new Vector3(0.34f, 0.3f, 1.6f), Quaternion.identity, 0.04f);
		array2 = new int[2] { -1, 1 };
		foreach (int num4 in array2)
		{
			polyMesh6.Tube(new Vector3((float)num4 * 0.1f, 0.2f, -0.4f), new Vector3((float)num4 * 0.1f, 0.2f, 1.2f), 0.06f, 0.06f, 12);
		}
		polyMesh6.Box(new Vector3(0f, 0f, -0.65f), new Vector3(0.36f, 0.36f, 0.5f), Quaternion.identity, 0.04f);
		polyMesh6.Tube(new Vector3(0f, 0f, 0.9f), new Vector3(0f, 0f, 2f), 0.11f, 0.1f, 16);
		polyMesh6.Tube(new Vector3(0f, 0f, 1.9f), new Vector3(0f, 0f, 4.45f), 0.085f, 0.075f, 16);
		polyMesh6.Box(new Vector3(0f, 0f, 4.6f), new Vector3(0.3f, 0.2f, 0.36f), Quaternion.identity, 0.03f);
		array2 = new int[2] { -1, 1 };
		foreach (int num5 in array2)
		{
			polyMesh6.Box(new Vector3((float)num5 * 0.155f, 0f, 4.6f), new Vector3(0.01f, 0.14f, 0.08f), Quaternion.identity);
		}
		Barrel = polyMesh6.Build("Howitzer barrel");
		Tyre = VehicleMesh.Tyre(0.45f, 0.3f, "Howitzer tyre");
		Hub = Kit.WheelHub(0.4f, 0.3f, 6, "Howitzer hub");
	}
}
