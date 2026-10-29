using UnityEngine;

namespace CombatSandbox.Demo;

public sealed class TankShell : MonoBehaviour
{
	public const float Power = 2.4f;

	public const float Penetration = 300f;

	private const float Life = 6f;

	private static readonly int Hits = -5;

	private static readonly RaycastHit[] cast = new RaycastHit[16];

	private static Material shellPaint;

	private static Material tracerPaint;

	private Vector3 velocity;

	private ArenaTarget shooter;

	private Vehicle own;

	private float age;

	private bool done;

	public static TankShell Fire(ArenaTarget shooter, Vehicle own, Vector3 at, Vector3 velocity)
	{
		GameObject gameObject = new GameObject("125 mm HE-FRAG shell");
		gameObject.layer = 2;
		gameObject.transform.SetPositionAndRotation(at, Quaternion.LookRotation(velocity));
		TankShell tankShell = gameObject.AddComponent<TankShell>();
		tankShell.shooter = shooter;
		tankShell.own = own;
		tankShell.velocity = velocity;
		tankShell.Build();
		return tankShell;
	}

	private void Build()
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
		obj.transform.localScale = new Vector3(0.125f, 0.3f, 0.125f);
		obj.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
		obj.GetComponent<Renderer>().sharedMaterial = shellPaint;
		TrailRenderer trailRenderer = base.gameObject.AddComponent<TrailRenderer>();
		trailRenderer.sharedMaterial = tracerPaint;
		trailRenderer.time = 0.09f;
		trailRenderer.startWidth = 0.14f;
		trailRenderer.endWidth = 0.02f;
		trailRenderer.minVertexDistance = 0.5f;
		trailRenderer.startColor = new Color(1f, 0.75f, 0.35f, 0.9f);
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
		}
		else
		{
			base.transform.position = vector;
			if (age >= 6f)
			{
				Burst(vector, null);
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
		Vector3 normalized = velocity.normalized;
		Vehicle.Struck(hit.collider, hit.point, normalized, 300f);
		GlassPane component = hit.collider.GetComponent<GlassPane>();
		if ((bool)component)
		{
			component.Blast(hit.point, 1.5f);
		}
		Burst(hit.point + hit.normal * 0.1f, hit.collider);
	}

	private void Burst(Vector3 centre, Collider struck)
	{
		done = true;
		if ((bool)ReconstructionDemo.Instance)
		{
			ReconstructionDemo.Instance.Detonate(centre, shooter, 2.4f);
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
