using System;
using UnityEngine;

namespace CombatSandbox.Demo;

public class Helicopter : Vehicle
{
	public const int Gunner = 1;

	private const float Cruise = 35f;

	private static readonly Vector3 Mast = new Vector3(0f, 3.05f, 0.2f);

	private static readonly Vector3 TailHub = new Vector3(0.28f, 2.55f, -8.2f);

	private static readonly Vector3 ChinGun = new Vector3(0f, 0.72f, 3.65f);

	private static Mesh fuselage;

	private static Mesh cowling;

	private static Mesh glassMesh;

	private static Mesh fittings;

	private static Mesh[] blade = new Mesh[2];

	private static Mesh tailBlades;

	private static Mesh gunMesh;

	private static Mesh tyre;

	private static Material[] paint = new Material[2];

	private static Material glass;

	private static Material dark;

	private static Material rubber;

	private static Material bladePaint;

	protected Transform rotor;

	protected Transform tailRotor;

	private float rotorSpeed;

	private float spin;

	private ArenaTarget lastPilot;

	private Vector3 orbitDir = Vector3.forward;

	private bool Bravo => base.Team == 1;

	protected virtual string PilotHelp => "Helicopter: W / S - forward / back, A / D - turn, Space - up, Ctrl - down";

	protected virtual float CruiseSpeed => 22f;

	protected virtual void PilotWeapons(ArenaTarget pilot, PlayerInput input)
	{
	}

	protected override Vector3 FlagMount => new Vector3(0f, 2.15f, -7.6f);

	protected override float FlagLength => 1f;

	protected override void Describe()
	{
		Wheel[] array = new Wheel[4];
		for (int i = 0; i < 4; i++)
		{
			array[i] = new Wheel
			{
				At = new Vector3((i % 2 == 0) ? (-1.15f) : 1.15f, 0.3f, (i < 2) ? 2.2f : (-1.2f)),
				Radius = 0.3f,
				Width = 0.2f,
				Steer = 0f,
				Driven = false
			};
		}
		spec = new Spec
		{
			Name = ((base.Team == 1) ? "UH-60 Black Hawk" : "Mi-8AMTSh"),
			Mass = 7000f,
			CentreOfMass = new Vector3(0f, 1.3f, 0.2f),
			Wheels = array,
			Grip = 1f,
			Suspension = 0.2f,
			Spring = 90000f,
			Damper = 9000f,
			WheelMass = 40f,
			MaxTorque = 0f,
			ReverseTorque = 0f,
			Power = 1f,
			TopSpeed = 1f,
			ReverseTop = 1f,
			BrakeTorque = 9000f,
			EngineBrake = 9000f,
			MaxSteer = 1f,
			AntiRoll = 30000f,
			Drag = 3f,
			SteerPivot = new Vector3(0f, 1.2f, 3f),
			SteerAxis = Vector3.up,
			SteerRatio = 1f,
			SteerInertia = 0.6f,
			SteerArms = 300f,
			EngineCentre = new Vector3(0f, 2.55f, 0.4f),
			EngineSize = new Vector3(1.3f, 0.5f, 2.8f),
			EngineHealth = 260f,
			Armoured = true,
			ArmourEnergy = 2.5f,
			Hatches = new Vector3[2]
			{
				new Vector3(0f, 2.6f, 0.4f),
				new Vector3(0.9f, 1.4f, 0.6f)
			},
			EnginePitch = 1.15f,
			EngineLoudness = 2.4f,
			CameraOffset = new Vector3(0f, 4.5f, -16f)
		};
		places = new Place[6]
		{
			new Place
			{
				Role = SeatRole.Driver,
				Pelvis = new Vector3(0f, 1.3f, 2.75f),
				FootL = new Vector3(-0.13f, 0.95f, 3.25f),
				FootR = new Vector3(0.13f, 0.95f, 3.25f),
				Door = new Vector3(-0.85f, 1f, 2.7f),
				Out = Vector3.left,
				Stand = 1.9f,
				Lean = 8f
			},
			new Place
			{
				Role = SeatRole.Gunner,
				Pelvis = new Vector3(0f, 0.95f, 3.55f),
				FootL = new Vector3(-0.13f, 0.66f, 3.95f),
				FootR = new Vector3(0.13f, 0.66f, 3.95f),
				Door = new Vector3(0.85f, 0.9f, 3.4f),
				Out = Vector3.right,
				Stand = 1.9f,
				Lean = 6f
			},
			Bench(-1, 0.6f),
			Bench(-1, -0.4f),
			Bench(1, 0.6f),
			Bench(1, -0.4f)
		};
		gun = new GunMount(this, 1, ChinGun, (base.Team == 1) ? GunMount.M2 : GunMount.Kpvt)
		{
			MaxTraverse = 75f,
			MinPitch = -70f,
			MaxPitch = 12f,
			GunLocal = new Vector3(0f, -0.1f, 0.1f),
			Muzzle = new Vector3(0f, 0f, 1.1f),
			Port = new Vector3(0f, -0.05f, 0.2f),
			Casings = true,
			GripsElevate = false,
			GripL = new Vector3(-0.2f, 0.4f, -0.3f),
			GripR = new Vector3(0.2f, 0.4f, -0.3f),
			SeatTurn = 0f,
			Inertia = 4f,
			Laying = 1800f,
			Damping = 120f
		};
		static Place Bench(int side, float z)
		{
			float num = ((side < 0) ? 1 : (-1));
			return new Place
			{
				Role = SeatRole.Holder,
				Pelvis = new Vector3((float)side * 0.72f, 1.12f, z),
				Yaw = -side * 90,
				Lean = 5f,
				FootL = new Vector3((float)side * 0.3f, 0.66f, z + 0.13f * num),
				FootR = new Vector3((float)side * 0.3f, 0.66f, z - 0.13f * num),
				HoldL = new Vector3((float)side * 0.6f, 1.05f, z + 0.25f * num),
				HoldR = new Vector3((float)side * 0.6f, 1.05f, z - 0.25f * num),
				Door = new Vector3(1.05f, 0.9f, 0.3f),
				Out = Vector3.right,
				Stand = 1.9f
			};
		}
	}

	protected override void Colliders()
	{
		Box("Belly", new Vector3(0f, 0.55f, 0.5f), new Vector3(2.1f, 0.1f, 5.8f));
		Vehicle.Skid(colliders[colliders.Count - 1]);
		int[] array = new int[2] { -1, 1 };
		foreach (int num in array)
		{
			Box("Side", new Vector3((float)num * 1.15f, 1.45f, 0.4f), new Vector3(0.06f, 1.7f, 5.8f));
		}
		Box("Roof", new Vector3(0f, 2.28f, 0.4f), new Vector3(2.3f, 0.06f, 5.6f));
		Box("Nose", new Vector3(0f, 1.3f, 3.6f), new Vector3(1.6f, 1.4f, 1.2f));
		Box("Tail", new Vector3(0f, 2f, -3.6f), new Vector3(1.4f, 0.9f, 1.8f));
		Box("Boom", new Vector3(0f, 2.05f, -6.3f), new Vector3(0.45f, 0.45f, 4.6f));
		Box("Engine", spec.EngineCentre, spec.EngineSize);
	}

	protected override void Model(out Transform[] wheelModels, out Transform steering)
	{
		int num = (Bravo ? 1 : 0);
		if (!fuselage)
		{
			Meshes();
		}
		if (!paint[num])
		{
			paint[num] = (Bravo ? Vehicle.Paint("Helicopter paint, dark olive (UH-60)", new Color(0.13f, 0.15f, 0.11f), 0.25f, 0.1f) : Vehicle.Paint("Helicopter paint, grey-green (Mi-8)", new Color(0.33f, 0.37f, 0.27f), 0.25f, 0.1f));
		}
		Transform transform = new GameObject("Model").transform;
		transform.SetParent(base.transform, worldPositionStays: false);
		Vehicle.Draw(transform, "Fuselage", fuselage, paint[num]);
		Vehicle.Draw(transform, "Cowling", cowling, paint[num]);
		Vehicle.Draw(transform, "Glazing", glassMesh, glass);
		Vehicle.Draw(transform, "Fittings", fittings, dark);
		rotor = new GameObject("Main rotor").transform;
		rotor.SetParent(transform, worldPositionStays: false);
		rotor.localPosition = Mast;
		Vehicle.Draw(rotor, "Blades", blade[num], bladePaint);
		tailRotor = new GameObject("Tail rotor").transform;
		tailRotor.SetParent(transform, worldPositionStays: false);
		tailRotor.localPosition = TailHub;
		Vehicle.Draw(tailRotor, "Blades", tailBlades, bladePaint);
		Vehicle.Draw(gun.Build(transform), "Chin gun", gunMesh, dark);
		wheelModels = new Transform[spec.Wheels.Length];
		for (int i = 0; i < wheelModels.Length; i++)
		{
			Transform transform2 = new GameObject("Wheel " + i).transform;
			transform2.SetParent(base.transform, worldPositionStays: false);
			Vehicle.Draw(transform2, "Tyre", tyre, rubber);
			wheelModels[i] = transform2;
		}
		steering = null;
	}

	protected override void Update()
	{
		base.Update();
		spin += rotorSpeed * Time.deltaTime * 1500f;
		if ((bool)rotor)
		{
			rotor.localRotation = Quaternion.Euler(0f, spin, 0f);
		}
		if ((bool)tailRotor)
		{
			tailRotor.localRotation = Quaternion.Euler(spin * 4.5f, 0f, 0f);
		}
		ArenaTarget arenaTarget = base.Riders[0];
		if ((bool)arenaTarget && arenaTarget.Seated && arenaTarget != lastPilot && (bool)arenaTarget.Brain && arenaTarget.Brain.PlayerControlled && (bool)BattleDirector.Instance && BattleDirector.Instance.Notice != null)
		{
			BattleDirector.Instance.Notice(PilotHelp);
		}
		lastPilot = (((bool)arenaTarget && arenaTarget.Seated) ? arenaTarget : null);
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		Rigidbody body = base.Body;
		if (!body)
		{
			return;
		}
		float fixedDeltaTime = Time.fixedDeltaTime;
		ArenaTarget arenaTarget = base.Riders[0];
		bool flag = (bool)arenaTarget && arenaTarget.Seated && !arenaTarget.Body.Dead && !base.Wrecked;
		rotorSpeed = Mathf.MoveTowards(rotorSpeed, flag ? 1f : 0f, fixedDeltaTime / (flag ? 5f : 9f));
		if (rotorSpeed < 0.02f)
		{
			return;
		}
		bool flag2 = flag && (bool)arenaTarget.Brain && arenaTarget.Brain.PlayerControlled;
		Vector3 position = body.position;
		Vector3 velocity = body.velocity;
		float num = Ground(position);
		float num2 = position.y - num;
		Vector3 vector = Vector3.zero;
		float num3 = 0f;
		float num4 = 0f;
		bool flag3 = false;
		if (!flag)
		{
			num4 = -3f;
		}
		else if (flag2)
		{
			PlayerInput input = arenaTarget.Brain.Input;
			Vector3 vector2 = Flat(base.Rotation * Vector3.forward);
			vector = vector2 * Mathf.Clamp(Throttle, -1f, 1f) * 7f;
			num3 = Mathf.Clamp(SteerInput, -1f, 1f) * 55f;
			num4 = (Brake ? 6f : (input.Crouch ? (-5f) : 0f));
			flag3 = !Brake && input.Crouch && num2 < 1.2f;
			PilotWeapons(arenaTarget, input);
		}
		else
		{
			Autopilot(position, velocity, num, num2, out vector, out num3, out num4, out flag3);
		}
		float lift = rotorSpeed * Mathf.Clamp01(Condition * 2f + 0.3f);
		float num5 = 9.81f + Mathf.Clamp(num4 - velocity.y, -6f, 6f) * 1.6f;
		if (flag3 && num2 < 0.8f)
		{
			num5 = 9.81f * 0.7f;
		}
		Vector3 vector3 = Vector3.ClampMagnitude(vector - Flat(velocity) * 0.25f, 8f);
		Vector3 normalized = (Vector3.up * 9.81f + vector3).normalized;
		Vector3 vector4 = base.Rotation * Vector3.up;
		float num6 = Mathf.Max(0.5f, Vector3.Dot(vector4, Vector3.up));
		body.AddForce(vector4 * body.mass * num5 * lift / num6);
		Vector3 vector5 = Vector3.Cross(vector4, normalized) * 14f - Flat(body.angularVelocity) * 12f;
		float num7 = num3 * (MathF.PI / 180f) - body.angularVelocity.y;
		body.AddTorque((vector5 + Vector3.up * num7 * 8f) * body.mass * 1.4f * lift);
		body.AddForce(-Flat(velocity) * body.mass * 0.05f * lift);
		if (!Vehicle.OnField(position, 25f))
		{
			CombatNavMesh combatNavMesh = UnityEngine.Object.FindFirstObjectByType<CombatNavMesh>();
			Vector3 vector6 = (combatNavMesh ? combatNavMesh.transform.position : Vector3.zero);
			body.AddForce(Flat(vector6 - position).normalized * body.mass * 4f);
		}
	}

	protected virtual void Autopilot(Vector3 here, Vector3 velocity, float ground, float height, out Vector3 accel, out float yawRate, out float climb, out bool land)
	{
		accel = Vector3.zero;
		yawRate = 0f;
		climb = 0f;
		land = false;
		Motorpool.Task task = Motorpool.TaskOf(this);
		Vector3 vector = here;
		bool flag = true;
		bool flag2 = false;
		if (task != null)
		{
			switch (task.Stage)
			{
			case Motorpool.Stage.Loading:
			case Motorpool.Stage.Unloading:
				vector = here;
				flag = false;
				break;
			case Motorpool.Stage.Outbound:
			case Motorpool.Stage.Returning:
				vector = task.Goal;
				break;
			case Motorpool.Stage.Support:
			{
				Vector3 vector2 = ((task.Target != Vector3.zero) ? task.Target : task.Goal);
				orbitDir = Quaternion.Euler(0f, 6f * Time.fixedDeltaTime, 0f) * orbitDir;
				vector = vector2 + Flat(orbitDir).normalized * 110f;
				flag2 = true;
				break;
			}
			}
		}
		Vector3 vector3 = Flat(vector - here);
		float magnitude = vector3.magnitude;
		float num = Mathf.Max(ground, Ground(here + Flat(velocity) * 3f), Ground(here + Flat(velocity) * 6f));
		if (!flag)
		{
			climb = ((height > 1.5f) ? Mathf.Clamp(-height * 0.5f, -5f, -1f) : (-1f));
			land = height < 1.5f;
		}
		else if (!flag2 && magnitude < 25f && task != null && task.Moving)
		{
			climb = Mathf.Clamp((ground + 0.2f - here.y) * 0.5f, -5f, 2f);
			land = height < 1.5f;
		}
		else
		{
			float num2 = num + 35f;
			climb = Mathf.Clamp((num2 - here.y) * 0.4f, -5f, 6f);
		}
		float num3 = ((height < 8f && flag && (flag2 || magnitude > 25f)) ? 0f : Mathf.Clamp(magnitude * 0.25f, 0f, flag2 ? 8f : CruiseSpeed));
		Vector3 vector4 = ((magnitude > 0.5f) ? (vector3 / magnitude * num3) : Vector3.zero);
		accel = (vector4 - Flat(velocity)) * 0.8f;
		Vector3 vector5 = (flag2 ? Flat(((task.Target != Vector3.zero) ? task.Target : task.Goal) - here) : ((Flat(velocity).magnitude > 4f) ? Flat(velocity) : vector3));
		if (vector5.sqrMagnitude > 1f)
		{
			float num4 = Vector3.SignedAngle(Flat(base.Rotation * Vector3.forward), vector5, Vector3.up);
			yawRate = Mathf.Clamp(num4 * 1.2f, -40f, 40f);
		}
	}

	protected static Vector3 Flat(Vector3 v)
	{
		v.y = 0f;
		return v;
	}

	protected static float Ground(Vector3 at)
	{
		if (Physics.Raycast(new Vector3(at.x, at.y + 0.35f, at.z), Vector3.down, out var hitInfo, 400f, -261, QueryTriggerInteraction.Ignore))
		{
			return hitInfo.point.y;
		}
		return at.y - 50f;
	}

	private static Vector2[] Section(float half, float bottom, float top)
	{
		float num = top - bottom;
		return new Vector2[6]
		{
			new Vector2(0f, bottom),
			new Vector2(half * 0.7f, bottom),
			new Vector2(half, bottom + num * 0.3f),
			new Vector2(half, bottom + num * 0.72f),
			new Vector2(half * 0.65f, top),
			new Vector2(0f, top)
		};
	}

	private static void Meshes()
	{
		glass = Vehicle.Paint("Cockpit glass (helicopter)", new Color(0.12f, 0.17f, 0.2f), 0.92f, 0.3f);
		dark = Vehicle.Paint("Black fittings (helicopter)", new Color(0.06f, 0.06f, 0.06f), 0.3f, 0.4f);
		rubber = Vehicle.Paint("Tyres (helicopter)", new Color(0.04f, 0.04f, 0.04f), 0.1f);
		bladePaint = Vehicle.Paint("Rotor blades", new Color(0.09f, 0.09f, 0.08f), 0.3f, 0.2f);
		VehicleMesh vehicleMesh = new VehicleMesh();
		vehicleMesh.Hull(new(float, Vector2[])[8]
		{
			(4.25f, Section(0.3f, 0.95f, 1.45f)),
			(3.7f, Section(0.8f, 0.6f, 1.8f)),
			(2.8f, Section(1.15f, 0.5f, 2.3f)),
			(-1.8f, Section(1.15f, 0.5f, 2.3f)),
			(-2.8f, Section(0.75f, 0.95f, 2.25f)),
			(-4.2f, Section(0.3f, 1.75f, 2.25f)),
			(-8.2f, Section(0.17f, 1.92f, 2.18f)),
			(-8.6f, Section(0.1f, 1.95f, 2.15f))
		});
		vehicleMesh.Box(new Vector3(0f, 2.85f, -8.35f), new Vector3(0.12f, 1.5f, 0.85f), Quaternion.Euler(-18f, 0f, 0f));
		vehicleMesh.Box(new Vector3(0f, 2.05f, -7.3f), new Vector3(2.2f, 0.06f, 0.5f), Quaternion.identity);
		hull = vehicleMesh.Build("Helicopter fuselage");
		fuselage = hull;
		VehicleMesh vehicleMesh2 = new VehicleMesh();
		vehicleMesh2.Box(new Vector3(0f, 2.55f, 0.4f), new Vector3(1.4f, 0.5f, 3.2f), Quaternion.identity);
		vehicleMesh2.Box(new Vector3(0f, 2.5f, -1.4f), new Vector3(1.1f, 0.4f, 0.6f), Quaternion.Euler(-20f, 0f, 0f));
		int[] array = new int[2] { -1, 1 };
		foreach (int num in array)
		{
			vehicleMesh2.Tube(new Vector3[2]
			{
				new Vector3((float)num * 0.45f, 2.6f, -1.35f),
				new Vector3((float)num * 0.75f, 2.5f, -1.75f)
			}, 0.18f, 10);
		}
		cowling = vehicleMesh2.Build("Helicopter cowling");
		VehicleMesh vehicleMesh3 = new VehicleMesh();
		array = new int[2] { -1, 1 };
		foreach (int num2 in array)
		{
			vehicleMesh3.Box(new Vector3((float)num2 * 0.86f, 1.55f, 3.4f), new Vector3(0.04f, 0.6f, 1f), Quaternion.Euler(0f, (float)num2 * 14f, 0f));
		}
		vehicleMesh3.Box(new Vector3(0f, 1.62f, 3.95f), new Vector3(1.1f, 0.5f, 0.04f), Quaternion.Euler(-48f, 0f, 0f));
		vehicleMesh3.Box(new Vector3(0f, 1.15f, 4.15f), new Vector3(0.5f, 0.35f, 0.04f), Quaternion.Euler(-20f, 0f, 0f));
		array = new int[2] { -1, 1 };
		foreach (int num3 in array)
		{
			for (int j = 0; j < 4; j++)
			{
				vehicleMesh3.Box(new Vector3((float)num3 * 1.16f, 1.6f, 1.5f - (float)j * 0.85f), new Vector3(0.03f, 0.35f, 0.45f), Quaternion.identity);
			}
		}
		glassMesh = vehicleMesh3.Build("Helicopter glazing");
		VehicleMesh vehicleMesh4 = new VehicleMesh();
		vehicleMesh4.Tube(new Vector3[2]
		{
			new Vector3(0f, 2.75f, 0.2f),
			Mast
		}, 0.12f, 10);
		vehicleMesh4.Box(new Vector3(1.17f, 1.35f, 0.4f), new Vector3(0.03f, 1.3f, 1.3f), Quaternion.identity);
		array = new int[2] { -1, 1 };
		foreach (int num4 in array)
		{
			vehicleMesh4.Tube(new Vector3[2]
			{
				new Vector3((float)num4 * 0.7f, 0.55f, 2.2f),
				new Vector3((float)num4 * 1.15f, 0.3f, 2.2f)
			}, 0.05f, 8);
			vehicleMesh4.Tube(new Vector3[2]
			{
				new Vector3((float)num4 * 0.7f, 0.55f, -1.2f),
				new Vector3((float)num4 * 1.15f, 0.3f, -1.2f)
			}, 0.05f, 8);
			vehicleMesh4.Box(new Vector3((float)num4 * 1.55f, 1.2f, 0.1f), new Vector3(0.9f, 0.08f, 0.5f), Quaternion.Euler(0f, 0f, (float)num4 * -8f));
			for (int k = 0; k < 2; k++)
			{
				vehicleMesh4.Tube(new Vector3[2]
				{
					new Vector3((float)num4 * (1.6f + (float)k * 0.25f), 1.1f, 0.6f),
					new Vector3((float)num4 * (1.6f + (float)k * 0.25f), 1.1f, -0.5f)
				}, 0.11f, 10);
			}
		}
		fittings = vehicleMesh4.Build("Helicopter fittings");
		for (int l = 0; l < 2; l++)
		{
			int num5 = ((l == 0) ? 5 : 4);
			float num6 = ((l == 0) ? 8.5f : 7.4f);
			VehicleMesh vehicleMesh5 = new VehicleMesh();
			vehicleMesh5.Round(Vector3.zero, Vector3.up, new Vector2[4]
			{
				new Vector2(-0.1f, 0f),
				new Vector2(-0.1f, 0.3f),
				new Vector2(0.15f, 0.25f),
				new Vector2(0.15f, 0f)
			}, 12);
			for (int m = 0; m < num5; m++)
			{
				Quaternion quaternion = Quaternion.Euler(0f, (float)m * 360f / (float)num5, 0f);
				vehicleMesh5.Box(quaternion * new Vector3(0f, 0.05f, num6 * 0.5f + 0.2f), new Vector3(0.5f, 0.04f, num6), quaternion * Quaternion.Euler(0f, 0f, 4f));
			}
			blade[l] = vehicleMesh5.Build("Main rotor blades");
		}
		VehicleMesh vehicleMesh6 = new VehicleMesh();
		for (int n = 0; n < 3; n++)
		{
			Quaternion quaternion2 = Quaternion.Euler((float)n * 120f, 0f, 0f);
			vehicleMesh6.Box(quaternion2 * new Vector3(0f, 0.75f, 0f), new Vector3(0.03f, 1.5f, 0.22f), quaternion2);
		}
		tailBlades = vehicleMesh6.Build("Tail rotor blades");
		VehicleMesh vehicleMesh7 = new VehicleMesh();
		vehicleMesh7.Round(new Vector3(0f, 0f, -0.1f), Vector3.up, new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 0.22f),
			new Vector2(0.3f, 0.2f),
			new Vector2(0.32f, 0f)
		}, 14);
		vehicleMesh7.Tube(new Vector3[2]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(0f, 0f, 1.1f)
		}, 0.045f, 10);
		vehicleMesh7.Box(new Vector3(0f, 0f, 0.15f), new Vector3(0.16f, 0.16f, 0.4f), Quaternion.identity);
		gunMesh = vehicleMesh7.Build("Chin gun");
		tyre = VehicleMesh.Tyre(0.3f, 0.2f, "Helicopter tyre");
	}

	private static Mesh hull;
}
