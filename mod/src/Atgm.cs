using System;
using UnityEngine;

namespace CombatSandbox.Demo;

public sealed class Atgm : MonoBehaviour
{
	public const float Speed = 190f;

	public const float Life = 14f;

	private static readonly int Hits = -5;

	private static readonly RaycastHit[] cast = new RaycastHit[16];

	private static Material bodyPaint;

	private ArenaTarget shooter;

	private Vehicle own;

	private Func<Vector3?> guide;

	private Vector3 direction;

	private Vector3 aim;

	private Vector3 lastDrawn;

	private float speed = 45f;

	private float age;

	private float trailGap;

	private bool done;

	private AudioSource roar;

	private Light glow;

	public static Atgm Fire(ArenaTarget shooter, Vehicle own, Vector3 at, Vector3 direction, Func<Vector3?> guide)
	{
		GameObject gameObject = new GameObject("ATGM");
		gameObject.layer = 2;
		gameObject.transform.SetPositionAndRotation(at, Quaternion.LookRotation(direction));
		Atgm atgm = gameObject.AddComponent<Atgm>();
		atgm.shooter = shooter;
		atgm.own = own;
		atgm.direction = direction.normalized;
		atgm.guide = guide;
		atgm.aim = at + direction * 500f;
		atgm.lastDrawn = at;
		atgm.Build();
		if ((bool)RocketEffects.Instance)
		{
			RocketEffects.Instance.Ignite(at, -direction);
		}
		if ((bool)ShotEffects.Instance)
		{
			ShotEffects.Instance.Flash(at, direction, 3f);
		}
		if ((bool)Acoustics.Instance)
		{
			Acoustics.Instance.Explosion(at, rocket: true);
		}
		return atgm;
	}

	private void Build()
	{
		if (!bodyPaint)
		{
			bodyPaint = new Material(Shader.Find("Standard"))
			{
				color = new Color(0.25f, 0.28f, 0.18f)
			};
		}
		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
		UnityEngine.Object.Destroy(obj.GetComponent<Collider>());
		obj.layer = 2;
		obj.transform.SetParent(base.transform, worldPositionStays: false);
		obj.transform.localScale = new Vector3(0.12f, 0.5f, 0.12f);
		obj.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
		obj.GetComponent<Renderer>().sharedMaterial = bodyPaint;
		glow = new GameObject("Motor light").AddComponent<Light>();
		glow.transform.SetParent(base.transform, worldPositionStays: false);
		glow.transform.localPosition = new Vector3(0f, 0f, -0.7f);
		glow.type = LightType.Point;
		glow.color = new Color(1f, 0.62f, 0.3f);
		glow.range = 8f;
		glow.intensity = 3f;
		glow.shadows = LightShadows.None;
		roar = base.gameObject.AddComponent<AudioSource>();
		roar.clip = RocketEffects.Roar();
		roar.loop = true;
		roar.spatialBlend = 1f;
		roar.rolloffMode = AudioRolloffMode.Logarithmic;
		roar.minDistance = 7f;
		roar.maxDistance = 500f;
		roar.volume = 0.8f * GameSettings.EffectsVolume;
		roar.pitch = 1.15f;
		roar.Play();
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		if (done || deltaTime <= 0f)
		{
			return;
		}
		Vector3? vector = guide?.Invoke();
		if (vector.HasValue)
		{
			aim = vector.Value;
		}
		float num = deltaTime;
		while (num > 1E-06f && !done)
		{
			float num2 = Mathf.Min(num, 1f / 120f);
			num -= num2;
			Step(num2);
		}
		if (!done)
		{
			base.transform.rotation = Quaternion.LookRotation(direction);
			if ((bool)RocketEffects.Instance)
			{
				RocketEffects.Instance.Trail(lastDrawn, base.transform.position, 1, ref trailGap);
				RocketEffects.Instance.Flame(base.transform.position - direction * 0.6f, -direction);
			}
			lastDrawn = base.transform.position;
			glow.intensity = UnityEngine.Random.Range(2.4f, 3.4f);
		}
	}

	private void Step(float h)
	{
		age += h;
		speed = Mathf.MoveTowards(speed, Speed, h * 300f);
		Vector3 position = base.transform.position;
		Vector3 vector = aim - position;
		if (vector.sqrMagnitude > 1f)
		{
			float num = ((age < 0.35f) ? 0.6f : 1.7f);
			direction = Vector3.RotateTowards(direction, vector.normalized, num * h, 0f).normalized;
		}
		Vector3 vector2 = position + direction * speed * h;
		if (Cast(position, vector2, out var hit))
		{
			base.transform.position = hit.point;
			Strike(hit);
			return;
		}
		base.transform.position = vector2;
		if (age >= 14f)
		{
			Burst(vector2, -direction, 1.4f);
		}
	}

	private bool Cast(Vector3 from, Vector3 to, out RaycastHit hit)
	{
		hit = default(RaycastHit);
		Vector3 vector = to - from;
		float magnitude = vector.magnitude;
		if (magnitude < 1E-05f)
		{
			return false;
		}
		int num = Physics.RaycastNonAlloc(from, vector / magnitude, cast, magnitude, Hits, QueryTriggerInteraction.Ignore);
		float num2 = float.MaxValue;
		bool result = false;
		for (int i = 0; i < num; i++)
		{
			Collider collider = cast[i].collider;
			if (!(cast[i].distance >= num2) && (!own || !own.Holds(collider)) && !ShotQuery.IsShooter(collider))
			{
				HitZone hitZone = ShotQuery.ZoneOf(collider);
				if (!hitZone || !shooter || !(hitZone.Owner == shooter.Body))
				{
					num2 = cast[i].distance;
					hit = cast[i];
					result = true;
				}
			}
		}
		return result;
	}

	private void Strike(RaycastHit hit)
	{
		GlassPane component = hit.collider.GetComponent<GlassPane>();
		if ((bool)component)
		{
			component.Blast(hit.point, 2f);
		}
		if (Vehicle.Penetrate(hit.collider, hit.point, direction))
		{
			Burst(hit.point - direction * 0.3f, hit.normal, 0.8f);
			return;
		}
		if ((bool)ReconstructionDemo.Instance && !ShotQuery.ZoneOf(hit.collider))
		{
			ReconstructionDemo.Instance.SurfaceHit(hit, direction, sound: false);
		}
		Burst(hit.point + hit.normal * 0.15f, hit.normal, 1.6f);
	}

	private void Burst(Vector3 centre, Vector3 normal, float power)
	{
		done = true;
		if ((bool)ReconstructionDemo.Instance)
		{
			ReconstructionDemo.Instance.Detonate(centre, shooter, power);
		}
		if ((bool)RocketEffects.Instance)
		{
			RocketEffects.Instance.Burst(centre, normal, direction, air: false);
		}
		if ((bool)Ballistics.Instance)
		{
			for (int i = 0; i < 24; i++)
			{
				Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
				if (onUnitSphere.y < -0.2f)
				{
					onUnitSphere.y = 0f - onUnitSphere.y;
				}
				Ballistics.Instance.Fragment(shooter, centre, onUnitSphere, UnityEngine.Random.Range(500f, 1100f), UnityEngine.Random.Range(0.002f, 0.007f));
			}
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
