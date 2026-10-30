using UnityEngine;

namespace CombatSandbox.Demo;

public static partial class BigMap
{
	private const float BlockPitch = 84f;

	private const float BlockSize = 70f;

	private static Material cityGround;

	private static Material asphaltPaint;

	private static Material[] panelPaint;

	private static Material glassPaint;

	private static Material slabPaint;

	private static Material burntPaint;

	private static Texture2D cityPreview;

	private static void CityPaints()
	{
		if ((bool)cityGround)
		{
			return;
		}
		Texture2D texture2D = new Texture2D(256, 256, TextureFormat.RGB24, mipChain: true)
		{
			name = "City ground",
			wrapMode = TextureWrapMode.Repeat,
			filterMode = FilterMode.Trilinear,
			anisoLevel = 4
		};
		Color a = new Color(0.36f, 0.37f, 0.3f);
		Color b = new Color(0.45f, 0.43f, 0.37f);
		Color b2 = new Color(0.3f, 0.34f, 0.2f);
		for (int i = 0; i < 256; i++)
		{
			for (int j = 0; j < 256; j++)
			{
				float t = Mathf.PerlinNoise((float)j / 28f, (float)i / 28f);
				float t2 = Mathf.PerlinNoise((float)j / 9f + 20f, (float)i / 9f + 20f);
				Color color = Color.Lerp(Color.Lerp(a, b, t), b2, t2 * 0.45f) * (0.9f + 0.2f * Mathf.PerlinNoise((float)j / 2f + 70f, (float)i / 2f + 70f));
				texture2D.SetPixel(j, i, color);
			}
		}
		texture2D.Apply(updateMipmaps: true);
		cityGround = Paint("City ground", Color.white, 0.05f);
		cityGround.mainTexture = texture2D;
		asphaltPaint = Paint("Asphalt", new Color(0.2f, 0.2f, 0.21f), 0.15f);
		panelPaint = new Material[3]
		{
			Paint("Panel walls, beige", new Color(0.66f, 0.62f, 0.53f), 0.08f),
			Paint("Panel walls, grey", new Color(0.56f, 0.57f, 0.56f), 0.08f),
			Paint("Brick walls", new Color(0.52f, 0.33f, 0.26f), 0.06f)
		};
		glassPaint = Paint("Dark windows", new Color(0.08f, 0.09f, 0.1f), 0.7f);
		slabPaint = Paint("Roof slabs", new Color(0.3f, 0.3f, 0.3f), 0.1f);
		burntPaint = Paint("Burnt metal", new Color(0.12f, 0.1f, 0.09f), 0.3f);
	}

	private static void Block(PolyMesh wall, PolyMesh glass, PolyMesh roof, float x, float z, float w, float d, int floors, float yaw)
	{
		Vector3 vector = Ground(x, z);
		float num = (float)floors * 3f;
		Quaternion quaternion = Quaternion.Euler(0f, yaw, 0f);
		Vector3 vector2 = vector + Vector3.up * (num * 0.5f - 0.5f);
		Vector3 size = new Vector3(w, num + 1f, d);
		wall.Box(vector2, size, quaternion);
		Solid(vector2, size, quaternion);
		roof.Box(vector + Vector3.up * (num + 0.1f), new Vector3(w + 0.3f, 0.3f, d + 0.3f), quaternion);
		for (int i = 0; i < floors; i++)
		{
			float y = (float)i * 3f + 1.7f;
			glass.Box(vector + quaternion * new Vector3(0f, y, d * 0.5f + 0.04f), new Vector3(w - 1.4f, 1.3f, 0.08f), quaternion);
			glass.Box(vector + quaternion * new Vector3(0f, y, (0f - d) * 0.5f - 0.04f), new Vector3(w - 1.4f, 1.3f, 0.08f), quaternion);
			glass.Box(vector + quaternion * new Vector3(w * 0.5f + 0.04f, y, 0f), new Vector3(0.08f, 1.3f, d - 1.4f), quaternion);
			glass.Box(vector + quaternion * new Vector3((0f - w) * 0.5f - 0.04f, y, 0f), new Vector3(0.08f, 1.3f, d - 1.4f), quaternion);
		}
		int num2 = Mathf.Max(1, Mathf.FloorToInt(w / 16f));
		for (int j = 0; j < num2; j++)
		{
			float x2 = (0f - w) * 0.5f + w * ((float)j + 0.5f) / (float)num2;
			glass.Box(vector + quaternion * new Vector3(x2, 1.1f, (0f - d) * 0.5f - 0.06f), new Vector3(1.6f, 2.2f, 0.12f), quaternion);
		}
	}

	private static void Ruin(PolyMesh wall, PolyMesh rubble, System.Random r, float x, float z, float w, float d, float yaw)
	{
		Vector3 vector = Ground(x, z);
		Quaternion quaternion = Quaternion.Euler(0f, yaw, 0f);
		Vector3[] array = new Vector3[4]
		{
			new Vector3((0f - w) * 0.5f, 0f, (0f - d) * 0.5f),
			new Vector3(w * 0.5f, 0f, (0f - d) * 0.5f),
			new Vector3(w * 0.5f, 0f, d * 0.5f),
			new Vector3((0f - w) * 0.5f, 0f, d * 0.5f)
		};
		for (int i = 0; i < 4; i++)
		{
			Vector3 vector2 = vector + quaternion * array[i];
			Vector3 vector3 = vector + quaternion * array[(i + 1) % 4];
			float num = Vector3.Distance(vector2, vector3);
			Vector3 vector4 = (vector3 - vector2) / num;
			float num2 = 0f;
			while (num2 < num - 0.5f)
			{
				float num3 = Mathf.Min(Range(r, 2.5f, 6f), num - num2);
				float height = ((r.NextDouble() < 0.25) ? Range(r, 0.6f, 1.6f) : Range(r, 2.5f, 7.5f));
				Wall(wall, vector2 + vector4 * num2, vector2 + vector4 * (num2 + num3), vector.y, height, 0.4f);
				num2 += num3 + ((r.NextDouble() < 0.45) ? Range(r, 1.4f, 2.4f) : 0f);
			}
		}
		for (int j = 0; j < 5; j++)
		{
			Vector3 vector5 = vector + quaternion * new Vector3(Range(r, (0f - w) * 0.4f, w * 0.4f), 0f, Range(r, (0f - d) * 0.4f, d * 0.4f));
			Vector3 vector6 = new Vector3(Range(r, 1.2f, 3f), Range(r, 0.4f, 1.1f), Range(r, 1.2f, 3f));
			Quaternion quaternion2 = Quaternion.Euler(Range(r, -10f, 10f), Range(r, 0f, 360f), Range(r, -10f, 10f));
			rubble.Box(vector5 + Vector3.up * vector6.y * 0.3f, vector6, quaternion2, 0.15f);
			Solid(vector5 + Vector3.up * vector6.y * 0.3f, vector6 * 0.9f, quaternion2);
		}
	}

	private static void Wreck(PolyMesh m, float x, float z, float yaw)
	{
		Vector3 vector = Ground(x, z);
		Quaternion quaternion = Quaternion.Euler(0f, yaw, 0f);
		m.Box(vector + Vector3.up * 0.65f, new Vector3(1.8f, 0.8f, 4.3f), quaternion, 0.12f);
		m.Box(vector + quaternion * new Vector3(0f, 1.35f, -0.3f), new Vector3(1.6f, 0.65f, 2.1f), quaternion, 0.12f);
		Solid(vector + Vector3.up * 0.85f, new Vector3(1.8f, 1.6f, 4.3f), quaternion);
	}

	private static void Road(PolyMesh m, Vector3 a, Vector3 b, float width)
	{
		Vector3 vector = b - a;
		vector.y = 0f;
		float magnitude = vector.magnitude;
		Quaternion q = Quaternion.LookRotation(vector / magnitude);
		int num = Mathf.Max(1, Mathf.CeilToInt(magnitude / 14f));
		for (int i = 0; i < num; i++)
		{
			Vector3 vector2 = a + vector * (((float)i + 0.5f) / (float)num);
			vector2.y = WorldHeight(vector2) + 0.02f;
			m.Box(vector2, new Vector3(width, 0.06f, magnitude / (float)num + 0.4f), q);
		}
	}

	private static void BuildCity(System.Random r)
	{
		PolyMesh[] array = new PolyMesh[3]
		{
			new PolyMesh(),
			new PolyMesh(),
			new PolyMesh()
		};
		PolyMesh polyMesh = new PolyMesh();
		PolyMesh polyMesh2 = new PolyMesh();
		PolyMesh polyMesh3 = new PolyMesh();
		PolyMesh polyMesh4 = new PolyMesh();
		PolyMesh polyMesh5 = new PolyMesh();
		PolyMesh polyMesh6 = new PolyMesh();
		PolyMesh polyMesh7 = new PolyMesh();
		PolyMesh polyMesh8 = new PolyMesh();
		PolyMesh polyMesh9 = new PolyMesh();
		PolyMesh polyMesh10 = new PolyMesh();
		for (int i = 0; i <= 10; i++)
		{
			float num = -420f + (float)i * 84f;
			Road(polyMesh5, Ground(num, -336f), Ground(num, 336f), 14f);
		}
		for (int j = 0; j <= 8; j++)
		{
			float num2 = -336f + (float)j * 84f;
			Road(polyMesh5, Ground(-420f, num2), Ground(420f, num2), 14f);
		}
		Road(polyMesh5, Ground(0f, -470f), Ground(0f, -336f), 14f);
		Road(polyMesh5, Ground(0f, 336f), Ground(0f, 470f), 14f);
		for (int k = 0; k < 10; k++)
		{
			for (int l = 0; l < 8; l++)
			{
				float num3 = -378f + (float)k * 84f;
				float num4 = -294f + (float)l * 84f;
				if (new Vector2(num3, num4 - 410f).magnitude < 130f || new Vector2(num3, num4 + 410f).magnitude < 130f)
				{
					continue;
				}
				double num5 = r.NextDouble();
				PolyMesh wall = array[r.Next(3)];
				if (num5 < 0.42)
				{
					int floors = ((r.NextDouble() < 0.6) ? 5 : 9);
					Block(wall, polyMesh, polyMesh2, num3, num4 + 22f, Range(r, 50f, 62f), 12f, floors, 0f);
					Block(array[r.Next(3)], polyMesh, polyMesh2, num3, num4 - 22f, Range(r, 50f, 62f), 12f, floors, 180f);
					if (r.NextDouble() < 0.5)
					{
						Block(wall, polyMesh, polyMesh2, num3 + 26f, num4, 12f, 20f, floors, 90f);
					}
					for (int m = 0; m < 3; m++)
					{
						Tree(polyMesh7, polyMesh8, Ground(num3 + Range(r, -18f, 18f), num4 + Range(r, -9f, 9f)), Range(r, 0.8f, 1.2f));
					}
					if (r.NextDouble() < 0.6)
					{
						Wreck(polyMesh6, num3 + Range(r, -15f, 10f), num4 + Range(r, -6f, 6f), Range(r, 0f, 360f));
					}
				}
				else if (num5 < 0.57)
				{
					int floors2 = r.Next(9, 14);
					Block(wall, polyMesh, polyMesh2, num3 - 17f, num4 + 15f, 20f, 20f, floors2, 0f);
					Block(array[r.Next(3)], polyMesh, polyMesh2, num3 + 17f, num4 - 15f, 20f, 20f, floors2, 90f);
					Tree(polyMesh7, polyMesh8, Ground(num3 + 15f, num4 + 15f), 1.1f);
					Tree(polyMesh7, polyMesh8, Ground(num3 - 15f, num4 - 15f), 1f);
				}
				else if (num5 < 0.73)
				{
					for (int n = 0; n < 6; n++)
					{
						float x = num3 + (float)(n % 3 - 1) * 21f + Range(r, -2f, 2f);
						float z = num4 + (float)(n / 3 * 2 - 1) * 15f + Range(r, -2f, 2f);
						House(polyMesh9, polyMesh10, r, Ground(x, z), (float)(r.Next(4) * 90) + Range(r, -6f, 6f));
					}
				}
				else if (num5 < 0.9)
				{
					Ruin(polyMesh3, polyMesh4, r, num3 - 14f, num4 + 12f, Range(r, 14f, 22f), Range(r, 10f, 16f), (float)(r.Next(2) * 90));
					Ruin(polyMesh3, polyMesh4, r, num3 + 15f, num4 - 13f, Range(r, 12f, 20f), Range(r, 10f, 14f), (float)(r.Next(2) * 90));
					if (r.NextDouble() < 0.5)
					{
						Block(wall, polyMesh, polyMesh2, num3 + 14f, num4 + 20f, 26f, 12f, 3, 0f);
					}
					if (r.NextDouble() < 0.5)
					{
						Wreck(polyMesh6, num3 - 12f, num4 - 18f, Range(r, 0f, 360f));
					}
				}
				else
				{
					for (int num6 = 0; num6 < 14; num6++)
					{
						Tree(polyMesh7, polyMesh8, Ground(num3 + Range(r, -30f, 30f), num4 + Range(r, -30f, 30f)), Range(r, 0.9f, 1.4f));
					}
					Sandbags(polyMesh4, Ground(num3 - 6f, num4 + 5f), Ground(num3 + 6f, num4 + 5f));
				}
			}
		}
		for (int num7 = 0; num7 < 90; num7++)
		{
			bool flag = r.NextDouble() < 0.5;
			float num8 = -420f + (float)r.Next(11) * 84f;
			float num9 = Range(r, -330f, 330f);
			float num10 = (flag ? num8 : num9) + Range(r, -4f, 4f);
			float num11 = (flag ? num9 : (-336f + (float)r.Next(9) * 84f)) + Range(r, -4f, 4f);
			if (!(Mathf.Abs(num10) < 9f) || !(Mathf.Abs(num11) > 300f))
			{
				Wreck(polyMesh6, num10, num11, (flag ? 0f : 90f) + Range(r, -25f, 25f));
			}
		}
		for (int num12 = 0; num12 < 12; num12++)
		{
			float num13 = -336f + (float)r.Next(9) * 84f;
			float num14 = -378f + (float)r.Next(10) * 84f + Range(r, -20f, 20f);
			Sandbags(polyMesh4, Ground(num14 - 4f, num13 + Range(r, -3f, 3f)), Ground(num14 + 4f, num13 + Range(r, -3f, 3f)));
		}
		Show("City roads", polyMesh5, asphaltPaint);
		for (int num15 = 0; num15 < 3; num15++)
		{
			Show("City houses " + num15, array[num15], panelPaint[num15]);
		}
		Show("City windows", polyMesh, glassPaint);
		Show("City roofs", polyMesh2, slabPaint);
		Show("City ruins", polyMesh3, panelPaint[1]);
		Show("City rubble and sandbags", polyMesh4, sandbagPaint);
		Show("Burnt-out cars", polyMesh6, burntPaint);
		Show("City tree trunks", polyMesh7, barkPaint);
		Show("City tree crowns", polyMesh8, leafPaint);
		Show("Private houses", polyMesh9, wallPaint);
		Show("Private house roofs", polyMesh10, roofPaint);
	}

	public static Texture2D CityPreview()
	{
		if ((bool)cityPreview)
		{
			return cityPreview;
		}
		cityPreview = new Texture2D(336, 186, TextureFormat.RGB24, mipChain: false)
		{
			name = "Big City preview",
			wrapMode = TextureWrapMode.Clamp
		};
		System.Random random = new System.Random(7);
		Color color = new Color(0.36f, 0.37f, 0.3f);
		Color color2 = new Color(0.18f, 0.18f, 0.19f);
		Color[] array = new Color[3]
		{
			new Color(0.66f, 0.62f, 0.53f),
			new Color(0.56f, 0.57f, 0.56f),
			new Color(0.52f, 0.33f, 0.26f)
		};
		Color[] array2 = new Color[80];
		for (int i = 0; i < 80; i++)
		{
			array2[i] = ((random.NextDouble() < 0.12) ? new Color(0.2f, 0.32f, 0.15f) : array[random.Next(3)]);
		}
		for (int j = 0; j < 186; j++)
		{
			for (int k = 0; k < 336; k++)
			{
				float num = ((float)j / 185f - 0.5f) * 1000f;
				float num2 = ((float)k / 335f - 0.5f) * 1000f * 336f / 186f;
				Color color3 = color;
				if (Mathf.Abs(num) < 427f && Mathf.Abs(num2) < 343f)
				{
					float num3 = Mathf.Repeat(num + 420f + 7f, 84f);
					float num4 = Mathf.Repeat(num2 + 336f + 7f, 84f);
					if (num3 < 14f || num4 < 14f)
					{
						color3 = color2;
					}
					else
					{
						int num5 = Mathf.Clamp(Mathf.FloorToInt((num + 420f) / 84f), 0, 9);
						int num6 = Mathf.Clamp(Mathf.FloorToInt((num2 + 336f) / 84f), 0, 7);
						float num7 = num4 - 14f;
						if (num7 < 20f || num7 > 50f)
						{
							color3 = array2[num5 * 8 + num6];
						}
					}
				}
				else if (Mathf.Abs(num) < 7f)
				{
					color3 = color2;
				}
				for (int l = -1; l <= 1; l += 2)
				{
					if (new Vector2(num, num2 - (float)l * 410f).magnitude < 14f)
					{
						color3 = ((l < 0) ? new Color(0.25f, 0.55f, 1f) : new Color(1f, 0.35f, 0.25f));
					}
				}
				cityPreview.SetPixel(k, j, color3);
			}
		}
		cityPreview.Apply();
		return cityPreview;
	}
}
