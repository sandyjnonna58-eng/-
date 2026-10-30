using UnityEngine;

namespace CombatSandbox.Demo;

public static class BodyArmour
{
	public const float PlateRating = 3.6f;

	public const float SoftRating = 1.2f;

	public const float HelmetRating = 1.3f;

	public static bool Stop(ArenaTarget t, HitZone zone, Vector3 point, Vector3 normal, Vector3 direction, ref float energy)
	{
		if (!GameSettings.BodyArmour || !t || !t.Body || !zone || t.Body.Dead)
		{
			return false;
		}
		RagdollBody body = t.Body;
		direction = direction.normalized;
		if (zone == body.Head)
		{
			Vector3 vector = zone.transform.InverseTransformPoint(point);
			bool flag = Vector3.Dot(direction, -zone.transform.forward) > 0.65f && vector.y < 0.06f;
			if (vector.y < -0.02f || flag || t.Helmet <= 0f)
			{
				return false;
			}
			bool flag2 = normal.sqrMagnitude > 0.1f && Mathf.Abs(Vector3.Dot(direction, normal.normalized)) < 0.35f;
			float num = HelmetRating * (flag2 ? 2.2f : 1f) * ((t.Helmet < 30f) ? 0.5f : 1f);
			t.Helmet = Mathf.Max(0f, t.Helmet - (15f + energy * 20f));
			return Resolve(t, zone, point, normal, ref energy, num, flag2 ? 0.1f : 0.25f);
		}
		if (zone.Region == BodyRegion.Chest || zone.Region == BodyRegion.Pelvis)
		{
			float num2 = Vector3.Dot(direction, body.Chest.transform.forward);
			bool flag3 = zone.Region == BodyRegion.Chest && Mathf.Abs(num2) > 0.45f;
			if (flag3)
			{
				bool flag4 = num2 < 0f;
				float num3 = (flag4 ? t.PlateFront : t.PlateBack);
				if (num3 > 0f)
				{
					float rating = PlateRating * ((num3 < 30f) ? 0.5f : 1f);
					num3 = Mathf.Max(0f, num3 - (10f + energy * 8f));
					if (flag4)
					{
						t.PlateFront = num3;
					}
					else
					{
						t.PlateBack = num3;
					}
					return Resolve(t, zone, point, normal, ref energy, rating, 0.35f);
				}
			}
			if (t.SoftArmour > 0f && (zone.Region == BodyRegion.Chest || num2 < 0f))
			{
				float rating2 = SoftRating * ((t.SoftArmour < 30f) ? 0.5f : 1f);
				t.SoftArmour = Mathf.Max(0f, t.SoftArmour - (5f + energy * 10f));
				return Resolve(t, zone, point, normal, ref energy, rating2, 0.15f);
			}
		}
		return false;
	}

	private static bool Resolve(ArenaTarget t, HitZone zone, Vector3 point, Vector3 normal, ref float energy, float rating, float blunt)
	{
		if (energy > rating)
		{
			energy *= Mathf.Lerp(0.45f, 0.85f, Mathf.Clamp01((energy - rating) / (rating * 2f)));
			return false;
		}
		t.Vitals.Stress(0.6f);
		zone.Body.AddForceAtPosition(-normal.normalized * 6f * Mathf.Sqrt(Mathf.Max(0.3f, energy)) * blunt * 4f, point, ForceMode.Impulse);
		if ((bool)ShotEffects.Instance)
		{
			ShotEffects.Instance.Sparks(point, normal, 6, 3f);
		}
		if ((bool)Acoustics.Instance)
		{
			Acoustics.Instance.Impact(point, ImpactSound.Hard, 0.8f);
		}
		if (zone == t.Body.Head)
		{
			t.Vitals.Concuss(Mathf.Clamp01(energy * 0.35f));
		}
		energy = 0f;
		return true;
	}
}
