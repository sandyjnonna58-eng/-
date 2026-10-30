using System;
using UnityEngine;
using UnityEngine.UI;

namespace CombatSandbox.Demo;

public sealed class BreathTrace : MaskableGraphic
{
	private const int Samples = 120;

	private readonly float[] trace = new float[120];

	private int head;

	private float carry;

	[NonSerialized]
	public float Phase;

	[NonSerialized]
	public float Depth = 1f;

	[NonSerialized]
	public bool Stopped;

	private void Update()
	{
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		if (unscaledDeltaTime <= 0f)
		{
			return;
		}
		carry += unscaledDeltaTime * 120f / 8f;
		if (carry < 1f)
		{
			return;
		}
		float num = (Stopped ? 0f : (Mathf.Sin(Phase * MathF.PI * 2f) * Depth));
		while (carry >= 1f)
		{
			carry -= 1f;
			trace[head] = num;
			head = (head + 1) % 120;
		}
		SetVerticesDirty();
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
		Rect pixelAdjustedRect = GetPixelAdjustedRect();
		if (pixelAdjustedRect.width < 2f)
		{
			return;
		}
		float y = pixelAdjustedRect.center.y;
		float num = pixelAdjustedRect.height * 0.42f;
		Vector2 a = default(Vector2);
		for (int i = 0; i < 120; i++)
		{
			Vector2 vector = new Vector2(pixelAdjustedRect.xMin + pixelAdjustedRect.width * (float)i / 119f, y + Mathf.Clamp(trace[(head + i) % 120], -1.1f, 1.1f) * num);
			if (i > 0)
			{
				float num2 = (float)i / 119f;
				Color32 c = new Color(color.r, color.g, color.b, color.a * Mathf.Lerp(0.15f, 1f, num2 * num2));
				Vector2 vector2 = vector - a;
				if (vector2.sqrMagnitude > 0.0001f)
				{
					Vector2 vector3 = new Vector2(0f - vector2.y, vector2.x).normalized * 1f;
					int currentVertCount = vh.currentVertCount;
					vh.AddVert(a - vector3, c, Vector2.zero);
					vh.AddVert(a + vector3, c, Vector2.zero);
					vh.AddVert(vector + vector3, c, Vector2.zero);
					vh.AddVert(vector - vector3, c, Vector2.zero);
					vh.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
					vh.AddTriangle(currentVertCount, currentVertCount + 2, currentVertCount + 3);
				}
			}
			a = vector;
		}
	}
}
