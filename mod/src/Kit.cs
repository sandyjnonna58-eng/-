using System;
using UnityEngine;

namespace CombatSandbox.Demo;

internal static class Kit
{
	public static Mesh WheelTyres(float r, float w, string name)
	{
		VehicleMesh vehicleMesh = new VehicleMesh();
		float num = w * 0.42f;
		float[] array = new float[2]
		{
			0f - w * 0.5f,
			w * 0.5f - num
		};
		foreach (float num2 in array)
		{
			vehicleMesh.Round(Vector3.zero, Vector3.right, new Vector2[10]
			{
				new Vector2(num2, 0f),
				new Vector2(num2, r - 0.03f),
				new Vector2(num2, r - 0.03f),
				new Vector2(num2 + 0.02f, r),
				new Vector2(num2 + 0.02f, r),
				new Vector2(num2 + num - 0.02f, r),
				new Vector2(num2 + num - 0.02f, r),
				new Vector2(num2 + num, r - 0.03f),
				new Vector2(num2 + num, r - 0.03f),
				new Vector2(num2 + num, 0f)
			}, 26);
		}
		return vehicleMesh.Build(name);
	}

	public static Mesh WheelHub(float r, float w, int bolts, string name)
	{
		VehicleMesh vehicleMesh = new VehicleMesh();
		float num = w * 0.5f + 0.01f;
		vehicleMesh.Round(Vector3.zero, Vector3.right, new Vector2[10]
		{
			new Vector2(0f - num, 0f),
			new Vector2(0f - num, r * 0.3f),
			new Vector2(0f - num, r * 0.3f),
			new Vector2(0f - num + 0.03f, r * 0.62f),
			new Vector2(0f - num + 0.03f, r * 0.62f),
			new Vector2(0f - num + 0.015f, r * 0.84f),
			new Vector2(0f - num + 0.015f, r * 0.84f),
			new Vector2(num - 0.015f, r * 0.84f),
			new Vector2(num - 0.015f, r * 0.84f),
			new Vector2(num, 0f)
		}, 22);
		for (int i = 0; i < bolts; i++)
		{
			float f = (float)i / (float)bolts * MathF.PI * 2f;
			vehicleMesh.Box(new Vector3(0f - num - 0.005f, Mathf.Cos(f) * r * 0.45f, Mathf.Sin(f) * r * 0.45f), new Vector3(0.025f, 0.04f, 0.04f), Quaternion.identity);
		}
		vehicleMesh.Round(new Vector3(0f - num, 0f, 0f), Vector3.left, new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, r * 0.2f),
			new Vector2(0.05f, r * 0.16f),
			new Vector2(0.06f, 0f)
		}, 14);
		return vehicleMesh.Build(name);
	}

	public static Mesh Sprocket(float r, float w, int teeth, string name)
	{
		VehicleMesh vehicleMesh = new VehicleMesh();
		float[] array = new float[2]
		{
			0f - w * 0.42f,
			w * 0.42f - 0.08f
		};
		foreach (float num in array)
		{
			vehicleMesh.Round(Vector3.zero, Vector3.right, new Vector2[5]
			{
				new Vector2(num, 0f),
				new Vector2(num, r * 0.82f),
				new Vector2(num, r * 0.82f),
				new Vector2(num + 0.08f, r * 0.82f),
				new Vector2(num + 0.08f, 0f)
			}, 24);
			for (int j = 0; j < teeth; j++)
			{
				Quaternion quaternion = Quaternion.Euler((float)j * 360f / (float)teeth, 0f, 0f);
				vehicleMesh.Box(quaternion * new Vector3(num + 0.04f, r * 0.9f, 0f), new Vector3(0.08f, r * 0.22f, r * 0.17f), quaternion);
			}
		}
		vehicleMesh.Round(Vector3.zero, Vector3.right, new Vector2[6]
		{
			new Vector2(0f - w * 0.5f, 0f),
			new Vector2(0f - w * 0.5f, r * 0.35f),
			new Vector2(0f - w * 0.5f, r * 0.35f),
			new Vector2(w * 0.5f, r * 0.35f),
			new Vector2(w * 0.5f, r * 0.35f),
			new Vector2(w * 0.5f, 0f)
		}, 14);
		return vehicleMesh.Build(name);
	}

	public static Mesh Shoe(float width, float pitch, string name)
	{
		PolyMesh polyMesh = new PolyMesh();
		polyMesh.Box(Vector3.zero, new Vector3(width, 0.045f, pitch * 0.92f), Quaternion.identity, 0.012f);
		polyMesh.Box(new Vector3(width * 0.25f, 0.035f, 0f), new Vector3(width * 0.36f, 0.03f, pitch * 0.55f), Quaternion.identity, 0.008f);
		polyMesh.Box(new Vector3((0f - width) * 0.25f, 0.035f, 0f), new Vector3(width * 0.36f, 0.03f, pitch * 0.55f), Quaternion.identity, 0.008f);
		polyMesh.Box(new Vector3(0f, -0.055f, 0f), new Vector3(0.06f, 0.075f, pitch * 0.45f), Quaternion.identity, 0.01f);
		return polyMesh.Build(name);
	}

	public static void Barrel(PolyMesh m, float length, float r, float extractorAt, float extractorR, float muzzleR, int sides = 16)
	{
		m.Tube(new Vector3(0f, 0f, 0.1f), new Vector3(0f, 0f, length), r, r * 0.9f, sides);
		m.Tube(new Vector3(0f, 0f, 0.15f), new Vector3(0f, 0f, length * 0.35f), r * 1.3f, r * 1.25f, sides);
		if (extractorR > 0f)
		{
			m.Tube(new Vector3(0f, 0f, extractorAt - 0.25f), new Vector3(0f, 0f, extractorAt - 0.08f), r * 1.1f, extractorR, sides);
			m.Tube(new Vector3(0f, 0f, extractorAt - 0.08f), new Vector3(0f, 0f, extractorAt + 0.25f), extractorR, extractorR, sides);
			m.Tube(new Vector3(0f, 0f, extractorAt + 0.25f), new Vector3(0f, 0f, extractorAt + 0.4f), extractorR, r * 1.05f, sides);
		}
		m.Tube(new Vector3(0f, 0f, extractorAt + 0.45f), new Vector3(0f, 0f, length * 0.8f), r * 1.18f, r * 1.12f, sides);
		if (muzzleR > 0f)
		{
			m.Tube(new Vector3(0f, 0f, length - 0.18f), new Vector3(0f, 0f, length + 0.02f), muzzleR, muzzleR, sides);
		}
	}

	public static void SmokeLaunchers(VehicleMesh m, Vector3 at, int side, int count, float spacing)
	{
		for (int i = 0; i < count; i++)
		{
			Vector3 vector = at + new Vector3((float)side * (float)i * 0.04f, (float)(i % 2) * 0.1f, (0f - (float)i) * spacing);
			m.Tube(new Vector3[2]
			{
				vector,
				vector + new Vector3((float)side * 0.1f, 0.12f, 0.2f)
			}, 0.042f, 10);
		}
	}
}
