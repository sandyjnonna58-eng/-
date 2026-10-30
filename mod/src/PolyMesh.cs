using System;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSandbox.Demo;

internal sealed class PolyMesh
{
	private readonly List<Vector3> v = new List<Vector3>();

	private readonly List<int> t = new List<int>();

	public int Count => v.Count;

	public static Vector3[] Ring(float y, params Vector2[] plan)
	{
		Vector3[] array = new Vector3[plan.Length];
		for (int i = 0; i < plan.Length; i++)
		{
			array[i] = new Vector3(plan[i].x, y, plan[i].y);
		}
		return array;
	}

	public static Vector3[] Mirror(float y, float scale, Vector2 offset, params Vector2[] half)
	{
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < half.Length; i++)
		{
			list.Add(new Vector3(half[i].x * scale + offset.x, y, half[i].y * scale + offset.y));
		}
		for (int num = half.Length - 1; num >= 0; num--)
		{
			if (!(Mathf.Abs(half[num].x) < 0.0001f))
			{
				list.Add(new Vector3((0f - half[num].x) * scale + offset.x, y, half[num].y * scale + offset.y));
			}
		}
		return list.ToArray();
	}

	public static Vector3[] Oval(float z, float yc, float w, float h, int n = 16, float square = 2.6f, float xc = 0f)
	{
		Vector3[] array = new Vector3[n];
		float num = 2f / square;
		for (int i = 0; i < n; i++)
		{
			float f = (float)i / (float)n * MathF.PI * 2f;
			float num2 = Mathf.Cos(f);
			float num3 = Mathf.Sin(f);
			array[i] = new Vector3(xc + w * Mathf.Sign(num2) * Mathf.Pow(Mathf.Abs(num2), num), yc + h * Mathf.Sign(num3) * Mathf.Pow(Mathf.Abs(num3), num), z);
		}
		return array;
	}

	public static Vector3[] Shift(Vector3[] ring, Vector3 by)
	{
		Vector3[] array = new Vector3[ring.Length];
		for (int i = 0; i < ring.Length; i++)
		{
			array[i] = ring[i] + by;
		}
		return array;
	}

	public void Loft(IList<Vector3[]> rings, bool smooth = false, bool capStart = true, bool capEnd = true)
	{
		int num = rings[0].Length;
		int count = v.Count;
		if (smooth)
		{
			foreach (Vector3[] ring in rings)
			{
				v.AddRange(ring);
			}
		}
		for (int i = 0; i + 1 < rings.Count; i++)
		{
			Vector3 vector = Centre(rings[i]);
			Vector3 vector2 = Centre(rings[i + 1]);
			for (int j = 0; j < num; j++)
			{
				int num2 = (j + 1) % num;
				Vector3 vector3 = rings[i][j];
				Vector3 vector4 = rings[i][num2];
				Vector3 vector5 = rings[i + 1][num2];
				Vector3 vector6 = rings[i + 1][j];
				Vector3 vector7 = (vector3 + vector4 + vector5 + vector6) * 0.25f - (vector + vector2) * 0.5f;
				if (smooth)
				{
					Tri(count + i * num + j, count + i * num + num2, count + (i + 1) * num + num2, vector7);
					Tri(count + i * num + j, count + (i + 1) * num + num2, count + (i + 1) * num + j, vector7);
				}
				else
				{
					Face(vector3, vector4, vector5, vector6, vector7);
				}
			}
		}
		if (capStart)
		{
			Cap(rings[0], Centre(rings[0]) - Centre(rings[1]));
		}
		if (capEnd)
		{
			Cap(rings[rings.Count - 1], Centre(rings[rings.Count - 1]) - Centre(rings[rings.Count - 2]));
		}
	}

	public void Cap(Vector3[] ring, Vector3 outward)
	{
		Vector3 item = Centre(ring);
		int count = v.Count;
		v.Add(item);
		v.AddRange(ring);
		for (int i = 0; i < ring.Length; i++)
		{
			Tri(count, count + 1 + i, count + 1 + (i + 1) % ring.Length, outward);
		}
	}

	public void Face(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 outward)
	{
		int count = v.Count;
		v.Add(a);
		v.Add(b);
		v.Add(c);
		v.Add(d);
		Tri(count, count + 1, count + 2, outward);
		Tri(count, count + 2, count + 3, outward);
	}

	public void Box(Vector3 c, Vector3 size, Quaternion q, float bevel = 0f)
	{
		Vector3 vector = size * 0.5f;
		if (bevel <= 0f)
		{
			Vector3[] array = new Vector3[8];
			for (int i = 0; i < 8; i++)
			{
				array[i] = c + q * new Vector3(((i & 1) == 0) ? (0f - vector.x) : vector.x, ((i & 2) == 0) ? (0f - vector.y) : vector.y, ((i & 4) == 0) ? (0f - vector.z) : vector.z);
			}
			int[][] array2 = new int[6][]
			{
				new int[4] { 0, 2, 3, 1 },
				new int[4] { 4, 5, 7, 6 },
				new int[4] { 0, 1, 5, 4 },
				new int[4] { 2, 6, 7, 3 },
				new int[4] { 0, 4, 6, 2 },
				new int[4] { 1, 3, 7, 5 }
			};
			foreach (int[] array3 in array2)
			{
				Vector3 outward = (array[array3[0]] + array[array3[1]] + array[array3[2]] + array[array3[3]]) * 0.25f - c;
				Face(array[array3[0]], array[array3[1]], array[array3[2]], array[array3[3]], outward);
			}
			return;
		}
		float num = Mathf.Min(bevel, Mathf.Min(vector.x, Mathf.Min(vector.y, vector.z)) * 0.9f);
		Vector2[] array4 = new Vector2[8]
		{
			new Vector2(vector.x - num, vector.z),
			new Vector2(vector.x, vector.z - num),
			new Vector2(vector.x, 0f - vector.z + num),
			new Vector2(vector.x - num, 0f - vector.z),
			new Vector2(0f - vector.x + num, 0f - vector.z),
			new Vector2(0f - vector.x, 0f - vector.z + num),
			new Vector2(0f - vector.x, vector.z - num),
			new Vector2(0f - vector.x + num, vector.z)
		};
		Vector2[] array5 = new Vector2[8];
		for (int k = 0; k < 8; k++)
		{
			array5[k] = new Vector2(array4[k].x - Mathf.Sign(array4[k].x) * num, array4[k].y - Mathf.Sign(array4[k].y) * num);
		}
		List<Vector3[]> list = new List<Vector3[]>
		{
			Place(Ring(0f - vector.y, array5), c, q),
			Place(Ring(0f - vector.y + num, array4), c, q),
			Place(Ring(vector.y - num, array4), c, q),
			Place(Ring(vector.y, array5), c, q)
		};
		Loft(list);
	}

	public void Tube(Vector3 a, Vector3 b, float ra, float rb, int sides = 12, bool smooth = true)
	{
		Vector3 normalized = (b - a).normalized;
		Vector3 normalized2 = Vector3.Cross(normalized, (Mathf.Abs(normalized.y) < 0.9f) ? Vector3.up : Vector3.right).normalized;
		Vector3 vector = Vector3.Cross(normalized, normalized2);
		Vector3[] array = new Vector3[sides];
		Vector3[] array2 = new Vector3[sides];
		for (int i = 0; i < sides; i++)
		{
			float f = (float)i / (float)sides * MathF.PI * 2f;
			Vector3 vector2 = normalized2 * Mathf.Cos(f) + vector * Mathf.Sin(f);
			array[i] = a + vector2 * ra;
			array2[i] = b + vector2 * rb;
		}
		Loft(new Vector3[2][] { array, array2 }, smooth);
	}

	public void Wedge(Vector3[] bottom, Vector3[] top)
	{
		Loft(new Vector3[2][] { bottom, top });
	}

	public static Vector3[] Place(Vector3[] ring, Vector3 c, Quaternion q)
	{
		Vector3[] array = new Vector3[ring.Length];
		for (int i = 0; i < ring.Length; i++)
		{
			array[i] = c + q * ring[i];
		}
		return array;
	}

	private void Tri(int a, int b, int c, Vector3 outward)
	{
		Vector3 vector = v[a];
		Vector3 lhs = v[b] - vector;
		Vector3 rhs = v[c] - vector;
		Vector3 lhs2 = Vector3.Cross(lhs, rhs);
		if (!(lhs2.sqrMagnitude < 1E-12f))
		{
			if (Vector3.Dot(lhs2, outward) < 0f)
			{
				t.Add(a);
				t.Add(c);
				t.Add(b);
			}
			else
			{
				t.Add(a);
				t.Add(b);
				t.Add(c);
			}
		}
	}

	private static Vector3 Centre(Vector3[] ring)
	{
		Vector3 zero = Vector3.zero;
		foreach (Vector3 vector in ring)
		{
			zero += vector;
		}
		return zero / ring.Length;
	}

	public Mesh Build(string name)
	{
		Mesh mesh = new Mesh
		{
			name = name
		};
		if (v.Count > 65000)
		{
			mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		}
		mesh.SetVertices(v);
		mesh.SetTriangles(t, 0);
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		return mesh;
	}
}
