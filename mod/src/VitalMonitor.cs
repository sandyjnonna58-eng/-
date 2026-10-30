using UnityEngine;

namespace CombatSandbox.Demo;

public sealed class VitalMonitor : MonoBehaviour
{
	private const int Samples = 240;

	private const float Rate = 60f;

	private BattleDirector director;

	private readonly float[] ecg = new float[240];

	private readonly float[] resp = new float[240];

	private int head;

	private float clock;

	private ArenaTarget watched;

	private GUIStyle style;

	private GUIStyle big;

	private void Start()
	{
		director = GetComponent<BattleDirector>();
	}

	private static float Wave(float f)
	{
		float num = Bump(f, 0.12f, 0.035f) * 0.12f;
		num -= Bump(f, 0.27f, 0.012f) * 0.12f;
		num += Bump(f, 0.3f, 0.018f) * 1f;
		num -= Bump(f, 0.335f, 0.014f) * 0.28f;
		return num + Bump(f, 0.55f, 0.05f) * 0.25f;
	}

	private static float Bump(float f, float at, float width)
	{
		float num = (f - at) / width;
		return Mathf.Exp((0f - num) * num);
	}

	private void Update()
	{
		ArenaTarget arenaTarget = (((bool)director && (bool)director.Possessed) ? director.Possessed.Body : null);
		if (arenaTarget != watched)
		{
			watched = arenaTarget;
			System.Array.Clear(ecg, 0, 240);
			System.Array.Clear(resp, 0, 240);
		}
		if (!watched)
		{
			return;
		}
		Physiology vitals = watched.Vitals;
		bool dead = watched.Body.Dead;
		clock += Time.deltaTime;
		while (clock >= 1f / 60f)
		{
			clock -= 1f / 60f;
			float num = 0f;
			float num2 = 0f;
			if (!dead && vitals.HeartFunction > 0.02f)
			{
				num = Wave(Mathf.Repeat(vitals.HeartPhase, 1f)) * Mathf.Lerp(0.35f, 1f, vitals.HeartFunction);
				if (vitals.HeartFunction < 0.35f)
				{
					num += (Mathf.PerlinNoise(Time.time * 9f, 0.5f) - 0.5f) * 0.5f * (1f - vitals.HeartFunction);
				}
			}
			if (!dead && vitals.BreathRate > 0.5f)
			{
				num2 = Mathf.Sin(vitals.BreathPhase * Mathf.PI * 2f) * Mathf.Clamp01(vitals.BreathRate / 10f) * Mathf.Lerp(0.4f, 1f, 1f - vitals.LungDamage * 0.6f);
			}
			ecg[head] = num + (Random.value - 0.5f) * 0.02f;
			resp[head] = num2;
			head = (head + 1) % 240;
		}
	}

	private void OnGUI()
	{
		if (!watched || GameMenu.Blocking || TreatmentScreen.Active || Event.current.type != EventType.Repaint)
		{
			return;
		}
		float num = Mathf.Clamp((float)Screen.height / 1080f, 0.7f, 2f);
		if (style == null)
		{
			style = new GUIStyle(GUI.skin.label)
			{
				fontStyle = FontStyle.Bold
			};
			big = new GUIStyle(style);
		}
		style.fontSize = Mathf.RoundToInt(12f * num);
		big.fontSize = Mathf.RoundToInt(20f * num);
		float num2 = 300f * num;
		float num3 = 70f * num;
		float num4 = 20f * num;
		float num5 = (float)Screen.height * 0.5f;
		Physiology vitals = watched.Vitals;
		bool dead = watched.Body.Dead;
		GUI.color = new Color(0f, 0f, 0f, 0.6f);
		GUI.DrawTexture(new Rect(num4 - 6f * num, num5 - 6f * num, num2 + 110f * num, num3 * 2f + 28f * num), Texture2D.whiteTexture);
		Color color = (dead ? new Color(0.9f, 0.25f, 0.2f) : new Color(0.35f, 1f, 0.45f));
		Color color2 = new Color(0.4f, 0.85f, 1f);
		Trace(ecg, new Rect(num4, num5 + 14f * num, num2, num3), color, 0.85f, num);
		Trace(resp, new Rect(num4, num5 + num3 + 26f * num, num2, num3 - 10f * num), color2, 1f, num);
		GUI.color = color;
		GUI.Label(new Rect(num4, num5 - 4f * num, 120f * num, 18f * num), Loc.T("ECG"), style);
		int num6 = (dead ? 0 : Mathf.RoundToInt(vitals.HeartRate * ((vitals.HeartFunction > 0.02f) ? 1f : 0f)));
		GUI.Label(new Rect(num4 + num2 + 10f * num, num5 + 10f * num, 100f * num, 28f * num), num6.ToString(), big);
		GUI.Label(new Rect(num4 + num2 + 10f * num, num5 + 36f * num, 100f * num, 18f * num), Loc.T("HR") + "  " + Mathf.RoundToInt(dead ? 0f : (120f * vitals.Pressure)) + "/" + Mathf.RoundToInt(dead ? 0f : (78f * vitals.Pressure)), style);
		GUI.color = color2;
		GUI.Label(new Rect(num4, num5 + num3 + 12f * num, 120f * num, 18f * num), Loc.T("RESP"), style);
		int num7 = (dead ? 0 : Mathf.RoundToInt(vitals.BreathRate));
		GUI.Label(new Rect(num4 + num2 + 10f * num, num5 + num3 + 20f * num, 100f * num, 28f * num), num7.ToString(), big);
		GUI.Label(new Rect(num4 + num2 + 10f * num, num5 + num3 + 46f * num, 100f * num, 18f * num), "SpO2 " + Mathf.RoundToInt(dead ? 0f : Mathf.Lerp(70f, 99f, Mathf.Clamp01((vitals.Oxygen - 0.4f) / 0.6f))) + "%", style);
		GUI.color = Color.white;
	}

	private void Trace(float[] data, Rect r, Color c, float scale, float ui)
	{
		GUI.color = new Color(c.r, c.g, c.b, 0.15f);
		GUI.DrawTexture(new Rect(r.x, r.y + r.height * 0.5f, r.width, 1f), Texture2D.whiteTexture);
		GUI.color = c;
		float num = r.width / 240f;
		float num2 = Mathf.Max(2f, 2f * ui);
		float num3 = r.y + r.height * 0.5f;
		float num4 = num3;
		for (int i = 0; i < 240; i++)
		{
			float num5 = data[(head + i) % 240];
			float num6 = num3 - Mathf.Clamp(num5 * scale, -0.6f, 1.1f) * r.height * 0.45f;
			float num7 = Mathf.Min(num6, num4);
			float num8 = Mathf.Max(num6, num4);
			GUI.DrawTexture(new Rect(r.x + (float)i * num, num7 - num2 * 0.5f, num + 0.5f, num8 - num7 + num2), Texture2D.whiteTexture);
			num4 = num6;
		}
	}
}
