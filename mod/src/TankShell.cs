using System.Collections.Generic;
using UnityEngine;

namespace CombatSandbox.Demo;

public sealed class TankShell : MonoBehaviour
{
	private static readonly int Hits = -5;

	private static readonly RaycastHit[] cast = new RaycastHit[16];

	private static Material shellPaint;

	private static Material tracerPaint;

	private Vector3 velocity;

	private ArenaTarget shooter;

	private Vehicle own;

	private int kind;

	private float power = 2.4f;

	private float life = 6f;

	private float age;

	private bool done;

	private readonly HashSet<Collider> pierced = new HashSet<Collider>();

	public static TankShell Fire(ArenaTarget shooter, Vehicle own, Vector3 at, Vector3 velocity, int kind, float power = 2.4f, bool tracer = true, float life = 6f)
	{
		GameObject gameObject = new GameObject((kind == 1) ? "APFSDS dart" : "HE-FRAG shell");
		gameObject.layer = 2;
		gameObject.transform.SetPositionAndRotation(at, Quaternion.LookRotation(velocity));
		TankShell tankShell = gameObject.AddComponent<TankShell>();
		tankShell.shooter = shooter;
		tankShell.own = own;
		tankShell.velocity = velocity;
		tankShell.kind = kind;
		tankShell.power = power;
		tankShell.life = life;
		tankShell.Build(tracer);
		return tankShell;
	}

	private void Build(bool tracer)
	{
		if (!shellPaint)
		{
			shellPaint = new Material(Shader.Find("Standard"))
			{
				color = new Color(0.2f, 0.22f, 0.15f)
			};
			tracerPaint = new Material(Shader.Find("Sprites/Default"));
		}
		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
		Object.Destroy(obj.GetComponent<Collider>());
		obj.layer = 2;
		obj.transform.SetParent(base.transform, worldPositionStays: false);
		obj.transform.localScale = ((kind == 1) ? new Vector3(0.035f, 0.35f, 0.035f) : new Vector3(0.125f, 0.3f, 0.125f));
		obj.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
		obj.GetComponent<Renderer>().sharedMaterial = shellPaint;
		if (tracer)
		{
			TrailRenderer trailRenderer = base.gameObject.AddComponent<TrailRenderer>();
			trailRenderer.sharedMaterial = tracerPaint;
			trailRenderer.time = ((kind == 1) ? 0.05f : 0.09f);
			trailRenderer.startWidth = ((kind == 1) ? 0.09f : 0.14f);
			trailRenderer.endWidth = 0.02f;
			trailRenderer.minVertexDistance = 0.5f;
			trailRenderer.startColor = ((kind == 1) ? new Color(1f, 0.9f, 0.6f, 1f) : new Color(1f, 0.75f, 0.35f, 0.9f));
			trailRenderer.endColor = new Color(1f, 0.5f, 0.2f, 0f);
			trailRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			Light light = new GameObject("Tracer light").AddComponent<Light>();
			light.transform.SetParent(base.transform, worldPositionStays: false);
			light.type = LightType.Point;
			light.color = new Color(1f, 0.7f, 0.4f);
			light.range = 6f;
			light.intensity = 1.6f;
			light.shadows = LightShadows.None;
		}
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		if (done || deltaTime <= 0f)
		{
			return;
		}
		float num = deltaTime;
		while (num > 1E-06f && !done)
		{
			float num2 = Mathf.Min(num, 1f / 120f);
			num -= num2;
			Step(num2);
		}
		if (!done && velocity.sqrMagnitude > 0.01f)
		{
			base.transform.rotation = Quaternion.LookRotation(velocity);
		}
	}

	private void Step(float h)
	{
		age += h;
		velocity += Physics.gravity * h;
		Vector3 position = base.transform.position;
		Vector3 vector = position + velocity * h;
		if (Cast(position, vector, out var hit))
		{
			base.transform.position = hit.point;
			Strike(hit);
			if (!done)
			{
				base.transform.position = vector;
			}
		}
		else
		{
			base.transform.position = vector;
			if (age >= life)
			{
				if (kind == 1)
				{
					done = true;
					Object.Destroy(base.gameObject);
				}
				else
				{
					Burst(vector);
				}
			}
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
			if (!(cast[i].distance >= num2) && !pierced.Contains(collider) && (!own || !own.Holds(collider)) && !ShotQuery.IsShooter(collider))
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
		Vector3 normalized = velocity.normalized;
		GlassPane component = hit.collider.GetComponent<GlassPane>();
		if ((bool)component)
		{
			component.Blast(hit.point, 1.5f);
		}
		if (kind == 1)
		{
			Dart(hit, normalized);
			return;
		}
		Vehicle.Struck(hit.collider, hit.point, normalized, 40f);
		Burst(hit.point + hit.normal * 0.1f);
	}

	private void Dart(RaycastHit hit, Vector3 direction)
	{
		HitZone hitZone = ShotQuery.ZoneOf(hit.collider);
		if ((bool)hitZone)
		{
			pierced.Add(hit.collider);
			if ((bool)ReconstructionDemo.Instance)
			{
				ReconstructionDemo.Instance.Strike(shooter, hitZone, hit.point, hit.normal, direction, 14f, exits: true);
			}
			velocity *= 0.9f;
			return;
		}
		done = true;
		if (!Vehicle.Penetrate(hit.collider, hit.point, direction))
		{
			if ((bool)ReconstructionDemo.Instance)
			{
				ReconstructionDemo.Instance.SurfaceHit(hit, direction);
			}
			if ((bool)ShotEffects.Instance)
			{
				ShotEffects.Instance.Sparks(hit.point, hit.normal, 20, 10f);
				ShotEffects.Instance.Explosion(hit.point + hit.normal * 0.1f, 0.35f);
			}
		}
		Object.Destroy(base.gameObject);
	}

	private void Burst(Vector3 centre)
	{
		done = true;
		if ((bool)ReconstructionDemo.Instance)
		{
			ReconstructionDemo.Instance.Detonate(centre, shooter, power);
		}
		if ((bool)Ballistics.Instance)
		{
			for (int i = 0; i < 30; i++)
			{
				Vector3 onUnitSphere = Random.onUnitSphere;
				if (onUnitSphere.y < -0.2f)
				{
					onUnitSphere.y = 0f - onUnitSphere.y;
				}
				Ballistics.Instance.Fragment(shooter, centre, onUnitSphere, Random.Range(500f, 1100f), Random.Range(0.002f, 0.008f));
			}
		}
		Object.Destroy(base.gameObject);
	}
}
