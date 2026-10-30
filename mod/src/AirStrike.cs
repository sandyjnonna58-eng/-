using UnityEngine;

namespace CombatSandbox.Demo;

public sealed class AirStrike : MonoBehaviour
{
	private const float Speed = 150f;

	private const float Cooldown = 90f;

	private const int RocketCount = 12;

	private static readonly float[] ready = new float[2];

	private static Material paint;

	private static Material glass;

	private static Material stores;

	private int team;

	private ArenaTarget caller;

	private Vector3 target;

	private Vector3 heading;

	private int rocketsFired;

	private int bombsDropped;

	private float nextRocket;

	private float nextBomb;

	public static float Ready(int team)
	{
		return Mathf.Max(0f, ready[Mathf.Clamp(team, 0, 1)] - Time.time);
	}

	public static bool Call(int team, ArenaTarget caller, Vector3 target)
	{
		team = Mathf.Clamp(team, 0, 1);
		if (Time.time < ready[team])
		{
			return false;
		}
		ready[team] = Time.time + 90f;
		Vector3 vector = Vector3.forward;
		BattleDirector instance = BattleDirector.Instance;
		if ((bool)instance && (bool)instance.Teams[team].Base)
		{
			Vector3 vector2 = target - instance.Teams[team].Base.position;
			vector2.y = 0f;
			if (vector2.sqrMagnitude > 2500f)
			{
				vector = vector2.normalized;
			}
		}
		float num = target.y;
		for (int i = -26; i <= 26; i++)
		{
			num = Mathf.Max(num, Ground(target + vector * ((float)i * 100f)));
		}
		GameObject gameObject = new GameObject("Air strike jet");
		gameObject.transform.SetPositionAndRotation(new Vector3(target.x, num + 110f, target.z) - vector * 2600f, Quaternion.LookRotation(vector));
		AirStrike airStrike = gameObject.AddComponent<AirStrike>();
		airStrike.team = team;
		airStrike.caller = caller;
		airStrike.target = target;
		airStrike.heading = vector;
		airStrike.Build();
		return true;
	}

	private static float Ground(Vector3 at)
	{
		if (Physics.Raycast(new Vector3(at.x, at.y + 1500f, at.z), Vector3.down, out var hitInfo, 4000f, -261, QueryTriggerInteraction.Ignore))
		{
			return hitInfo.point.y;
		}
		return at.y;
	}

	private void Build()
	{
		if (!JetMeshes.Ready)
		{
			JetMeshes.Build();
		}
		if (!paint)
		{
			paint = new Material(Shader.Find("Standard"))
			{
				color = new Color(0.36f, 0.4f, 0.36f)
			};
			paint.SetFloat("_Glossiness", 0.3f);
			glass = new Material(Shader.Find("Standard"))
			{
				color = new Color(0.12f, 0.16f, 0.2f)
			};
			glass.SetFloat("_Glossiness", 0.9f);
			stores = new Material(Shader.Find("Standard"))
			{
				color = new Color(0.25f, 0.27f, 0.2f)
			};
			stores.SetFloat("_Glossiness", 0.25f);
		}
		Draw(JetMeshes.Airframe, paint);
		Draw(JetMeshes.Canopy, glass);
		Draw(JetMeshes.Stores, stores);
		AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.clip = RocketEffects.Roar();
		audioSource.loop = true;
		audioSource.spatialBlend = 1f;
		audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
		audioSource.minDistance = 60f;
		audioSource.maxDistance = 3000f;
		audioSource.dopplerLevel = 1f;
		audioSource.pitch = 0.45f;
		audioSource.volume = GameSettings.EffectsVolume;
		audioSource.Play();
	}

	private void Draw(Mesh mesh, Material material)
	{
		GameObject gameObject = new GameObject(mesh.name);
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
		gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
		gameObject.AddComponent<MeshRenderer>().sharedMaterial = material;
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		if (deltaTime <= 0f)
		{
			return;
		}
		Vector3 vector = heading * 150f;
		base.transform.position += vector * deltaTime;
		Vector3 position = base.transform.position;
		float num = Vector3.Dot(target - position, heading);
		if (num < 1150f && rocketsFired < 12 && Time.time >= nextRocket)
		{
			Vector3 vector2 = base.transform.TransformPoint(JetMeshes.Pods[rocketsFired % JetMeshes.Pods.Length]);
			Vector2 vector3 = Random.insideUnitCircle * 10f;
			Vector3 to = target + new Vector3(vector3.x, 0f, vector3.y);
			if (!Rocket.Solve(vector2, to, out var direction, out var _))
			{
				direction = (to - vector2).normalized;
			}
			Rocket.Fire(caller, null, vector2, direction, vector * 0.5f);
			rocketsFired++;
			nextRocket = Time.time + 0.09f;
		}
		float num2 = Mathf.Sqrt(2f * Mathf.Max(1f, position.y - target.y) / 9.81f);
		if (bombsDropped < 2 && num <= 150f * num2 + 10f && Time.time >= nextBomb)
		{
			Vector3 at = base.transform.TransformPoint(JetMeshes.Bombs[bombsDropped % JetMeshes.Bombs.Length]);
			TankShell.Fire(caller, null, at, vector, 0, 3.5f, tracer: false, 30f);
			bombsDropped++;
			nextBomb = Time.time + 0.18f;
		}
		if (num < -2600f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}

internal static class JetMeshes
{
	public static readonly Vector3[] Pods = new Vector3[2]
	{
		new Vector3(-3.2f, -0.6f, 2.7f),
		new Vector3(3.2f, -0.6f, 2.7f)
	};

	public static readonly Vector3[] Bombs = new Vector3[2]
	{
		new Vector3(-2f, -0.9f, 1f),
		new Vector3(2f, -0.9f, 1f)
	};

	public static Mesh Airframe;

	public static Mesh Canopy;

	public static Mesh Stores;

	public static bool Ready => Airframe != null;

	public static void Build()
	{
		PolyMesh polyMesh = new PolyMesh();
		polyMesh.Loft(new Vector3[7][]
		{
			PolyMesh.Oval(7.5f, 0f, 0.1f, 0.1f, 18, 2.2f),
			PolyMesh.Oval(6.8f, 0f, 0.4f, 0.45f, 18, 2.2f),
			PolyMesh.Oval(5f, 0.05f, 0.65f, 0.75f, 18, 2.3f),
			PolyMesh.Oval(2f, 0.05f, 0.75f, 0.85f, 18, 2.4f),
			PolyMesh.Oval(-2f, 0.1f, 0.65f, 0.75f, 18, 2.4f),
			PolyMesh.Oval(-5.5f, 0.25f, 0.35f, 0.45f, 18, 2.2f),
			PolyMesh.Oval(-7f, 0.3f, 0.15f, 0.2f, 18, 2.2f)
		}, smooth: true);
		int[] array = new int[2] { -1, 1 };
		foreach (int num in array)
		{
			HeliKit.Wing(polyMesh, (float)num * 0.6f, (float)num * 7.5f, -0.2f, 0.05f, 1.2f, 3f, 0.6f, 1.6f, 0.12f);
			HeliKit.Wing(polyMesh, (float)num * 0.3f, (float)num * 3.2f, 0.35f, 0.4f, -5.3f, 1.6f, -5.7f, 1f, 0.08f);
			float xc = (float)num * 1.2f;
			polyMesh.Loft(new Vector3[4][]
			{
				PolyMesh.Oval(-0.5f, 0.8f, 0.42f, 0.42f, 16, 2f, xc),
				PolyMesh.Oval(-1f, 0.8f, 0.55f, 0.55f, 16, 2f, xc),
				PolyMesh.Oval(-4f, 0.8f, 0.55f, 0.55f, 16, 2f, xc),
				PolyMesh.Oval(-4.6f, 0.8f, 0.42f, 0.42f, 16, 2f, xc)
			}, smooth: true);
			polyMesh.Box(new Vector3((float)num * 0.85f, 0.75f, -2.3f), new Vector3(0.5f, 0.25f, 2f), Quaternion.identity, 0.05f);
		}
		HeliKit.Fin(polyMesh, 0f, 0.2f, new Vector2(-4.6f, 0.6f), new Vector2(-6.4f, 3f), new Vector2(-7.4f, 3f), new Vector2(-7.1f, 0.5f));
		Airframe = polyMesh.Build("Attack jet airframe");
		PolyMesh polyMesh2 = new PolyMesh();
		polyMesh2.Loft(new Vector3[4][]
		{
			PolyMesh.Oval(5.4f, 0.62f, 0.2f, 0.12f, 18, 2f),
			PolyMesh.Oval(4.8f, 0.78f, 0.38f, 0.32f, 18, 2f),
			PolyMesh.Oval(3.6f, 0.82f, 0.4f, 0.34f, 18, 2f),
			PolyMesh.Oval(2.6f, 0.7f, 0.25f, 0.15f, 18, 2f)
		}, smooth: true);
		Canopy = polyMesh2.Build("Attack jet canopy");
		PolyMesh polyMesh3 = new PolyMesh();
		array = new int[2] { -1, 1 };
		foreach (int num2 in array)
		{
			polyMesh3.Box(new Vector3((float)num2 * 3.2f, -0.3f, 1.5f), new Vector3(0.08f, 0.3f, 0.9f), Quaternion.identity, 0.02f);
			HeliKit.Pod(polyMesh3, new Vector3((float)num2 * 3.2f, -0.6f, 2.6f), 2f, 0.25f);
			polyMesh3.Box(new Vector3((float)num2 * 2f, -0.4f, 0.6f), new Vector3(0.08f, 0.3f, 0.9f), Quaternion.identity, 0.02f);
			HeliKit.Missile(polyMesh3, new Vector3((float)num2 * 2f, -0.8f, 1.8f), 2.3f, 0.26f);
		}
		Stores = polyMesh3.Build("Attack jet weapons");
	}
}
