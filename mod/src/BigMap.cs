using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CombatSandbox.Demo;

public static class BigMap
{
	public const float Play = 1000f;

	public const float Extent = 1800f;

	private const float Cell = 8f;

	private const int ChunkCells = 25;

	public const float BaseOffset = 410f;

	public static readonly Vector3 Centre = new Vector3(0f, 0f, 6000f);

	private static float seedX;

	private static float seedZ;

	private static float baseHeight;

	private static Material groundPaint;

	private static Material barkPaint;

	private static Material leafPaint;

	private static Material wallPaint;

	private static Material roofPaint;

	private static Material rockPaint;

	private static Material sandbagPaint;

	private static Material canvasPaint;

	private static Transform root;

	private static Texture2D preview;

	public static bool Active { get; private set; }

	public static Texture2D Preview()
	{
		if ((bool)preview)
		{
			return preview;
		}
		float num = seedX;
		float num2 = seedZ;
		float num3 = baseHeight;
		seedX = 17f;
		seedZ = 211f;
		baseHeight = (Raw(0f, -410f) + Raw(0f, 410f)) * 0.5f;
		preview = new Texture2D(336, 186, TextureFormat.RGB24, mipChain: false)
		{
			name = "Big Steppe preview",
			wrapMode = TextureWrapMode.Clamp
		};
		Color a = new Color(0.25f, 0.3f, 0.16f);
		Color b = new Color(0.55f, 0.5f, 0.34f);
		for (int i = 0; i < 186; i++)
		{
			for (int j = 0; j < 336; j++)
			{
				float num4 = ((float)i / 185f - 0.5f) * 1000f;
				float num5 = ((float)j / 335f - 0.5f) * 1000f * 336f / 186f;
				float x = num4;
				float z = num5;
				float num6 = Height(x, z);
				float num7 = Height(x + 4f, z) - Height(x - 4f, z) + (Height(x, z + 4f) - Height(x, z - 4f));
				Color color = Color.Lerp(a, b, Mathf.InverseLerp(0f, 14f, num6)) * (1f + num7 * 0.12f);
				float num8 = Mathf.Repeat(num5 + 37f, 140f);
				float[] array = new float[6] { -300f, -190f, -70f, 60f, 180f, 300f };
				foreach (float num9 in array)
				{
					if (Mathf.Abs(num5 - num9) < 5f && num8 > 14f && Mathf.Abs(num4) < 470f)
					{
						color = new Color(0.12f, 0.18f, 0.08f);
					}
				}
				for (int l = -1; l <= 1; l += 2)
				{
					if (new Vector2(num4, num5 - (float)l * 410f).magnitude < 14f)
					{
						color = ((l < 0) ? new Color(0.25f, 0.55f, 1f) : new Color(1f, 0.35f, 0.25f));
					}
				}
				preview.SetPixel(j, i, color);
			}
		}
		preview.Apply();
		seedX = num;
		seedZ = num2;
		baseHeight = num3;
		return preview;
	}

	public static bool Wanted
	{
		get
		{
			if (BattleSetup.BigMap)
			{
				return BattleSetup.Mode != GameMode.Capture;
			}
			return false;
		}
	}

	public static Vector3 BaseOf(int team)
	{
		Vector3 vector = Centre + new Vector3(0f, 0f, (team == 0) ? (-410f) : 410f);
		vector.y = Height(vector.x - Centre.x, vector.z - Centre.z) + 0.1f;
		return vector;
	}

	public static bool Setup(BattleDirector director)
	{
		Active = false;
		if (!Wanted)
		{
			return false;
		}
		Active = true;
		seedX = Random.Range(0f, 500f);
		seedZ = Random.Range(0f, 500f);
		baseHeight = 0f;
		baseHeight = (Raw(0f, -410f) + Raw(0f, 410f)) * 0.5f;
		Paints();
		root = new GameObject("Big map").transform;
		BuildGround();
		System.Random random = new System.Random((int)(seedX * 1000f));
		BuildTreeBelts(random);
		BuildVillages(random);
		BuildRocks(random);
		for (int i = 0; i < 2; i++)
		{
			BuildBase(i, random);
		}
		for (int j = 0; j < 2; j++)
		{
			Transform transform = new GameObject((j == 0) ? "Base A (big map)" : "Base B (big map)").transform;
			transform.position = BaseOf(j);
			transform.rotation = Quaternion.LookRotation((j == 0) ? Vector3.forward : Vector3.back);
			director.Teams[j].Base = transform;
		}
		DistantWar distantWar = Object.FindFirstObjectByType<DistantWar>();
		if ((bool)distantWar)
		{
			distantWar.Centre = Centre;
			distantWar.BlastDistance = new Vector2(1000f, 1800f);
			distantWar.GunfireDistance = new Vector2(950f, 1400f);
		}
		Physics.SyncTransforms();
		BuildNavMesh();
		Vehicle.ResetArea();
		return true;
	}

	private static float Raw(float x, float z)
	{
		return 9f * Mathf.PerlinNoise(seedX + x / 280f, seedZ + z / 280f) + 4f * Mathf.PerlinNoise(seedX * 1.7f + 13f + x / 95f, seedZ * 1.3f + 7f + z / 95f) + 1.2f * Mathf.PerlinNoise(seedX + 51f + x / 30f, seedZ + 23f + z / 30f);
	}

	public static float Height(float x, float z)
	{
		float num = Raw(x, z);
		for (int i = -1; i <= 1; i += 2)
		{
			float magnitude = new Vector2(x, z - (float)i * 410f).magnitude;
			float t = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(70f, 140f, magnitude));
			num = Mathf.Lerp(baseHeight, num, t);
		}
		float num2 = Mathf.Max(Mathf.Abs(x), Mathf.Abs(z)) - 520f;
		if (num2 > 0f)
		{
			num += num2 * 0.06f;
		}
		return num;
	}

	private static float WorldHeight(Vector3 p)
	{
		return Height(p.x - Centre.x, p.z - Centre.z);
	}

	private static Material Paint(string name, Color c, float smooth)
	{
		Material material = new Material(Shader.Find("Standard"))
		{
			name = name,
			color = c
		};
		material.SetFloat("_Glossiness", smooth);
		return material;
	}

	private static void Paints()
	{
		if ((bool)groundPaint)
		{
			return;
		}
		Texture2D texture2D = new Texture2D(256, 256, TextureFormat.RGB24, mipChain: true)
		{
			name = "Steppe grass",
			wrapMode = TextureWrapMode.Repeat,
			filterMode = FilterMode.Trilinear,
			anisoLevel = 4
		};
		Color a = new Color(0.33f, 0.37f, 0.2f);
		Color b = new Color(0.45f, 0.41f, 0.27f);
		Color b2 = new Color(0.25f, 0.3f, 0.15f);
		for (int i = 0; i < 256; i++)
		{
			for (int j = 0; j < 256; j++)
			{
				float t = Mathf.PerlinNoise((float)j / 32f, (float)i / 32f);
				float t2 = Mathf.PerlinNoise((float)j / 8f + 40f, (float)i / 8f + 40f);
				Color color = Color.Lerp(Color.Lerp(a, b, t), b2, t2 * 0.5f);
				color *= 0.88f + 0.24f * Mathf.PerlinNoise((float)j / 2f + 90f, (float)i / 2f + 90f);
				texture2D.SetPixel(j, i, color);
			}
		}
		texture2D.Apply(updateMipmaps: true);
		groundPaint = Paint("Steppe ground", Color.white, 0.04f);
		groundPaint.mainTexture = texture2D;
		barkPaint = Paint("Bark", new Color(0.24f, 0.19f, 0.14f), 0.05f);
		leafPaint = Paint("Leaves", new Color(0.2f, 0.29f, 0.13f), 0.08f);
		wallPaint = Paint("Plaster", new Color(0.62f, 0.58f, 0.5f), 0.08f);
		roofPaint = Paint("Roof", new Color(0.36f, 0.25f, 0.2f), 0.12f);
		rockPaint = Paint("Rock", new Color(0.45f, 0.43f, 0.4f), 0.1f);
		sandbagPaint = Paint("Sandbags", new Color(0.47f, 0.42f, 0.3f), 0.05f);
		canvasPaint = Paint("Canvas", new Color(0.32f, 0.34f, 0.22f), 0.08f);
	}

	private static void BuildGround()
	{
		int num = Mathf.RoundToInt(225f);
		int num2 = Mathf.CeilToInt((float)num / 25f);
		float num3 = -900f;
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				Mesh mesh = Chunk(num3 + (float)(j * 25) * 8f, num3 + (float)(i * 25) * 8f);
				GameObject gameObject = new GameObject("Ground " + j + "," + i);
				gameObject.transform.SetParent(root, worldPositionStays: false);
				gameObject.transform.position = Centre;
				gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
				gameObject.AddComponent<MeshRenderer>().sharedMaterial = groundPaint;
				gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
			}
		}
	}

	private static Mesh Chunk(float x0, float z0)
	{
		int num = 26;
		Vector3[] array = new Vector3[num * num];
		Vector3[] array2 = new Vector3[num * num];
		Vector2[] array3 = new Vector2[num * num];
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				float num2 = x0 + (float)j * 8f;
				float num3 = z0 + (float)i * 8f;
				float num4 = Height(num2, num3);
				array[i * num + j] = new Vector3(num2, num4, num3);
				float num5 = Height(num2 + 1f, num3) - Height(num2 - 1f, num3);
				float num6 = Height(num2, num3 + 1f) - Height(num2, num3 - 1f);
				array2[i * num + j] = new Vector3((0f - num5) * 0.5f, 1f, (0f - num6) * 0.5f).normalized;
				array3[i * num + j] = new Vector2(num2 / 12f, num3 / 12f);
			}
		}
		int[] array4 = new int[3750];
		int num7 = 0;
		for (int k = 0; k < 25; k++)
		{
			for (int l = 0; l < 25; l++)
			{
				int num8 = k * num + l;
				array4[num7++] = num8;
				array4[num7++] = num8 + num;
				array4[num7++] = num8 + 1;
				array4[num7++] = num8 + 1;
				array4[num7++] = num8 + num;
				array4[num7++] = num8 + num + 1;
			}
		}
		Mesh mesh = new Mesh
		{
			name = "Big map ground"
		};
		mesh.vertices = array;
		mesh.normals = array2;
		mesh.uv = array3;
		mesh.triangles = array4;
		mesh.RecalculateBounds();
		return mesh;
	}

	private static void Show(string name, PolyMesh mesh, Material paint)
	{
		if (mesh.Count != 0)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.SetParent(root, worldPositionStays: false);
			gameObject.AddComponent<MeshFilter>().sharedMesh = mesh.Build(name);
			gameObject.AddComponent<MeshRenderer>().sharedMaterial = paint;
		}
	}

	private static void Solid(Vector3 at, Vector3 size, Quaternion q)
	{
		GameObject gameObject = new GameObject("Collider");
		gameObject.transform.SetParent(root, worldPositionStays: false);
		gameObject.transform.SetPositionAndRotation(at, q);
		gameObject.AddComponent<BoxCollider>().size = size;
	}

	private static float Range(System.Random r, float a, float b)
	{
		return a + (float)r.NextDouble() * (b - a);
	}

	private static Vector3 Ground(float x, float z)
	{
		return new Vector3(Centre.x + x, Centre.y + Height(x, z), Centre.z + z);
	}

	private static void Tree(PolyMesh trunks, PolyMesh crowns, Vector3 at, float s)
	{
		trunks.Tube(at + Vector3.down * 0.4f, at + Vector3.up * 3.4f * s, 0.2f * s, 0.14f * s, 7, smooth: true);
		crowns.Loft(new Vector3[5][]
		{
			HeliKit.Circle(at + Vector3.up * 2.4f * s, 0.5f * s, 9),
			HeliKit.Circle(at + Vector3.up * 3.4f * s, 2.1f * s, 9),
			HeliKit.Circle(at + Vector3.up * 5f * s, 2.4f * s, 9),
			HeliKit.Circle(at + Vector3.up * 6.6f * s, 1.5f * s, 9),
			HeliKit.Circle(at + Vector3.up * 7.4f * s, 0.25f * s, 9)
		}, smooth: true);
		GameObject gameObject = new GameObject("Tree");
		gameObject.transform.SetParent(root, worldPositionStays: false);
		gameObject.transform.position = at + Vector3.up * 1.8f * s;
		CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
		capsuleCollider.radius = 0.25f * s;
		capsuleCollider.height = 4f * s;
	}

	private static void BuildTreeBelts(System.Random r)
	{
		PolyMesh polyMesh = new PolyMesh();
		PolyMesh polyMesh2 = new PolyMesh();
		float[] array = new float[6] { -300f, -190f, -70f, 60f, 180f, 300f };
		foreach (float num in array)
		{
			float num2 = Range(r, -480f, -150f);
			float num3 = Range(r, 150f, 480f);
			float num4 = Range(r, -0.08f, 0.08f);
			for (float num5 = num2; num5 < num3; num5 += Range(r, 5f, 8f))
			{
				if (!(Mathf.Repeat(num5 + 37f, 140f) < 14f))
				{
					float num6 = num + num5 * num4 + Range(r, -1.5f, 1.5f);
					Tree(polyMesh, polyMesh2, Ground(num5, num6), Range(r, 0.8f, 1.3f));
				}
			}
			if (polyMesh2.Count > 40000)
			{
				Show("Tree trunks", polyMesh, barkPaint);
				Show("Tree crowns", polyMesh2, leafPaint);
				polyMesh = new PolyMesh();
				polyMesh2 = new PolyMesh();
			}
		}
		for (int j = 0; j < 70; j++)
		{
			float num7 = Range(r, -490f, 490f);
			float num8 = Range(r, -340f, 340f);
			Tree(polyMesh, polyMesh2, Ground(num7, num8), Range(r, 0.8f, 1.4f));
		}
		Show("Tree trunks", polyMesh, barkPaint);
		Show("Tree crowns", polyMesh2, leafPaint);
	}

	private static void Wall(PolyMesh m, Vector3 a, Vector3 b, float baseY, float height, float thick)
	{
		Vector3 vector = b - a;
		vector.y = 0f;
		float magnitude = vector.magnitude;
		if (!(magnitude < 0.2f) && !(height < 0.2f))
		{
			Quaternion quaternion = Quaternion.LookRotation(vector / magnitude);
			Vector3 vector2 = (a + b) * 0.5f;
			vector2.y = baseY + height * 0.5f - 0.8f;
			Vector3 size = new Vector3(thick, height + 0.8f, magnitude);
			m.Box(vector2, size, quaternion);
			Solid(vector2, size, quaternion);
		}
	}

	private static void House(PolyMesh walls, PolyMesh roofs, System.Random r, Vector3 centre, float yaw)
	{
		float num = Range(r, 6f, 8f);
		float num2 = Range(r, 7.5f, 10f);
		float num3 = 2.8f;
		bool flag = r.NextDouble() < 0.6;
		Quaternion quaternion = Quaternion.Euler(0f, yaw, 0f);
		float num4 = centre.y;
		Vector3[] array = new Vector3[4]
		{
			new Vector3(0f - num, 0f, 0f - num2) * 0.5f,
			new Vector3(num, 0f, 0f - num2) * 0.5f,
			new Vector3(num, 0f, num2) * 0.5f,
			new Vector3(0f - num, 0f, num2) * 0.5f
		};
		for (int i = 0; i < 4; i++)
		{
			Vector3 vector = centre + quaternion * array[i];
			Vector3 vector2 = centre + quaternion * array[(i + 1) % 4];
			float num5 = Vector3.Distance(vector, vector2);
			float num6 = ((i == 0) ? 0.35f : Range(r, 0.25f, 0.55f));
			float num7 = ((i == 0) ? 1.2f : 1.4f);
			Vector3 vector3 = (vector2 - vector) / num5;
			Vector3 vector4 = vector + vector3 * (num5 * num6 - num7 * 0.5f);
			Vector3 vector5 = vector + vector3 * (num5 * num6 + num7 * 0.5f);
			float height = ((flag && r.NextDouble() < 0.5) ? Range(r, 0.9f, 2f) : num3);
			float height2 = ((flag && r.NextDouble() < 0.5) ? Range(r, 0.9f, 2f) : num3);
			Wall(walls, vector, vector4, num4, height, 0.3f);
			Wall(walls, vector5, vector2, num4, height2, 0.3f);
			if (i != 0)
			{
				Wall(walls, vector4, vector5, num4, 0.9f, 0.3f);
				if (!flag)
				{
					Vector3 vector6 = (vector4 + vector5) * 0.5f;
					Vector3 vector7 = new Vector3(0.3f, 0.6f, num7);
					Vector3 vector8 = new Vector3(vector6.x, num4 + num3 - 0.3f, vector6.z);
					Quaternion q = Quaternion.LookRotation(vector3);
					walls.Box(vector8, vector7, q);
					Solid(vector8, vector7, q);
				}
			}
			else if (!flag)
			{
				Vector3 vector9 = (vector4 + vector5) * 0.5f;
				Vector3 vector10 = new Vector3(0.3f, 0.6f, num7);
				Vector3 vector11 = new Vector3(vector9.x, num4 + num3 - 0.3f, vector9.z);
				Quaternion q2 = Quaternion.LookRotation(vector3);
				walls.Box(vector11, vector10, q2);
				Solid(vector11, vector10, q2);
			}
		}
		if (!flag)
		{
			float num8 = Mathf.Sqrt(num * num * 0.25f + 2.25f) + 0.3f;
			float num9 = Mathf.Atan2(1.5f, num * 0.5f) * 57.29578f;
			for (int j = -1; j <= 1; j += 2)
			{
				Quaternion quaternion2 = quaternion * Quaternion.Euler(0f, 0f, (float)(-j) * num9);
				Vector3 vector12 = centre + quaternion * new Vector3((float)j * num * 0.25f, 0f, 0f) + Vector3.up * (num3 + 0.75f);
				Vector3 size = new Vector3(num8, 0.12f, num2 + 0.6f);
				roofs.Box(vector12, size, quaternion2);
				Solid(vector12, size, quaternion2);
			}
		}
	}

	private static void BuildVillages(System.Random r)
	{
		PolyMesh polyMesh = new PolyMesh();
		PolyMesh polyMesh2 = new PolyMesh();
		Vector2[] array = new Vector2[3]
		{
			new Vector2(Range(r, -320f, -180f), Range(r, -60f, 60f)),
			new Vector2(Range(r, 180f, 320f), Range(r, -60f, 60f)),
			new Vector2(Range(r, -60f, 60f), Range(r, -40f, 40f))
		};
		for (int i = 0; i < array.Length; i++)
		{
			Vector2 vector = array[i];
			int num = ((i == 2) ? 5 : 7);
			for (int j = 0; j < num; j++)
			{
				float num2 = (float)j / (float)num * Mathf.PI * 2f + Range(r, -0.3f, 0.3f);
				float num3 = Range(r, 14f, 34f);
				float num4 = vector.x + Mathf.Cos(num2) * num3;
				float num5 = vector.y + Mathf.Sin(num2) * num3;
				House(polyMesh, polyMesh2, r, Ground(num4, num5), Range(r, 0f, 360f));
			}
		}
		Show("Village walls", polyMesh, wallPaint);
		Show("Village roofs", polyMesh2, roofPaint);
	}

	private static void BuildRocks(System.Random r)
	{
		PolyMesh polyMesh = new PolyMesh();
		for (int i = 0; i < 70; i++)
		{
			float x = Range(r, -480f, 480f);
			float z = Range(r, -330f, 330f);
			Vector3 vector = new Vector3(Range(r, 1f, 2.6f), Range(r, 0.8f, 1.8f), Range(r, 1f, 2.6f));
			Quaternion quaternion = Quaternion.Euler(Range(r, -12f, 12f), Range(r, 0f, 360f), Range(r, -12f, 12f));
			Vector3 vector2 = Ground(x, z) + Vector3.up * (vector.y * 0.3f);
			polyMesh.Box(vector2, vector, quaternion, Mathf.Min(vector.x, vector.y) * 0.3f);
			Solid(vector2, vector * 0.95f, quaternion);
		}
		Show("Rocks", polyMesh, rockPaint);
	}

	private static void Sandbags(PolyMesh m, Vector3 a, Vector3 b)
	{
		Vector3 vector = b - a;
		vector.y = 0f;
		float magnitude = vector.magnitude;
		Vector3 vector2 = vector / magnitude;
		Quaternion quaternion = Quaternion.LookRotation(vector2);
		int num = Mathf.Max(1, Mathf.RoundToInt(magnitude / 1.2f));
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < num; j++)
			{
				float num2 = ((float)j + 0.5f + ((i % 2 == 1) ? 0.25f : 0f)) / (float)num;
				if (!(num2 > 1f))
				{
					Vector3 vector3 = a + vector * num2;
					vector3.y = WorldHeight(vector3) + 0.12f + (float)i * 0.24f;
					m.Box(vector3, new Vector3(0.62f, 0.26f, 1.15f), quaternion, 0.08f);
				}
			}
		}
		Vector3 vector4 = (a + b) * 0.5f;
		vector4.y = WorldHeight(vector4) + 0.4f;
		Solid(vector4, new Vector3(0.6f, 1.1f, magnitude), quaternion);
	}

	private static void BuildBase(int team, System.Random r)
	{
		PolyMesh polyMesh = new PolyMesh();
		PolyMesh polyMesh2 = new PolyMesh();
		float num = ((team == 0) ? 1f : (-1f));
		float num2 = (0f - num) * 410f;
		for (int i = 0; i < 3; i++)
		{
			float num3 = num2 + num * (75f + (float)i * 22f);
			for (int j = -2; j <= 2; j++)
			{
				float num4 = (float)j * 38f + Range(r, -6f, 6f) + ((i % 2 == 1) ? 19f : 0f);
				float num5 = Range(r, 7f, 13f);
				Sandbags(polyMesh, Ground(num4 - num5 * 0.5f, num3 + Range(r, -2f, 2f)), Ground(num4 + num5 * 0.5f, num3 + Range(r, -2f, 2f)));
			}
		}
		for (int k = -1; k <= 1; k += 2)
		{
			Vector3 vector = Ground((float)k * 22f, num2 - num * 18f);
			Quaternion quaternion = Quaternion.identity;
			Vector3[] array = new Vector3[3]
			{
				new Vector3(-2.2f, 0f, 0f),
				new Vector3(0f, 2.4f, 0f),
				new Vector3(2.2f, 0f, 0f)
			};
			Vector3[] array2 = new Vector3[3];
			Vector3[] array3 = new Vector3[3];
			for (int l = 0; l < 3; l++)
			{
				array2[l] = vector + quaternion * (array[l] + new Vector3(0f, -0.2f, -3f));
				array3[l] = vector + quaternion * (array[l] + new Vector3(0f, -0.2f, 3f));
			}
			polyMesh2.Loft(new Vector3[2][] { array2, array3 });
			Solid(vector + Vector3.up * 0.9f, new Vector3(3.6f, 1.8f, 6f), quaternion);
		}
		Show((team == 0) ? "Base A sandbags" : "Base B sandbags", polyMesh, sandbagPaint);
		Show((team == 0) ? "Base A tents" : "Base B tents", polyMesh2, canvasPaint);
	}

	private static void BuildNavMesh()
	{
		CombatNavMesh combatNavMesh = Object.FindFirstObjectByType<CombatNavMesh>();
		if (!combatNavMesh)
		{
			GameObject gameObject = new GameObject("Combat nav mesh");
			gameObject.SetActive(value: false);
			combatNavMesh = gameObject.AddComponent<CombatNavMesh>();
			gameObject.SetActive(value: true);
		}
		combatNavMesh.enabled = false;
		combatNavMesh.transform.SetPositionAndRotation(Centre, Quaternion.identity);
		combatNavMesh.Size = new Vector3(1000f, 160f, 1000f);
		Bounds includedWorldBounds = new Bounds(Centre + Vector3.up * 40f, new Vector3(1000f, 160f, 1000f));
		List<NavMeshBuildSource> list = new List<NavMeshBuildSource>();
		NavMeshBuilder.CollectSources(includedWorldBounds, -261, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>(), list);
		list.RemoveAll((NavMeshBuildSource s) => (bool)s.component && ((bool)s.component.GetComponentInParent<Vehicle>() || (bool)s.component.GetComponentInParent<HitZone>() || ((bool)s.component.GetComponentInParent<Rigidbody>() && !s.component.GetComponentInParent<Rigidbody>().isKinematic)));
		NavMeshBuildSettings buildSettings = combatNavMesh.Settings();
		buildSettings.overrideVoxelSize = true;
		buildSettings.voxelSize = 0.25f;
		buildSettings.overrideTileSize = true;
		buildSettings.tileSize = 256;
		combatNavMesh.Data = NavMeshBuilder.BuildNavMeshData(buildSettings, list, new Bounds(Vector3.up * 40f, new Vector3(1000f, 160f, 1000f)), Centre, Quaternion.identity);
		combatNavMesh.enabled = true;
	}
}
