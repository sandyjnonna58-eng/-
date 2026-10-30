using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CombatSandbox.Demo;

public abstract class Tank : Vehicle
{
	protected float[] stations;

	protected Vector3 idlerAt;

	protected Vector3 sprocketAt;

	protected float idlerR;

	protected float sprocketR;

	protected float wheelR;

	protected float trackX;

	protected int shoeCount = 80;

	private static readonly List<Vector2> loop = new List<Vector2>();

	private static readonly List<float> along = new List<float>();

	private Transform[] wheelModelsCache;

	private Transform sprocketL;

	private Transform sprocketRight;

	private Transform idlerL;

	private Transform idlerRight;

	private Transform[] shoesL;

	private Transform[] shoesR;

	private float wreckedAt = -1f;

	private bool tossed;

	private float nextScare;

	protected abstract Vector3 TurretPivot { get; }

	protected static WeaponDef Cannon(string id, string name, string caliber)
	{
		return new WeaponDef
		{
			Id = id,
			Name = name,
			Caliber = caliber,
			Class = WeaponClass.MachineGun,
			MuzzleVelocity = 850f,
			BulletMass = 23f,
			Drag = 0f,
			Rpm = 10f,
			Magazine = 1,
			Reload = 6.5f,
			Auto = false,
			Semi = true,
			Recoil = 300f,
			Dispersion = 1.5f,
			Mass = 2500f,
			Zero = 1000f,
			TracerEvery = 0,
			Sound = GunSound.Sniper,
			Pitch = 0.35f,
			Loudness = 3.5f,
			Flash = 9f,
			Burst = new Vector2Int(1, 1),
			EffectiveRange = 2500f
		};
	}

	protected void BuildRunningGear(Transform model, Mesh tyre, Mesh hub, Material hubPaint, Mesh sprocket, Mesh idler, Material steel, Material rubber, Mesh shoe, Material trackSteel, out Transform[] wheelModels)
	{
		wheelModels = new Transform[spec.Wheels.Length];
		for (int i = 0; i < wheelModels.Length; i++)
		{
			Transform transform = new GameObject("Road wheel " + i).transform;
			transform.SetParent(base.transform, worldPositionStays: false);
			Transform transform2 = new GameObject("Wheel").transform;
			transform2.SetParent(transform, worldPositionStays: false);
			if (spec.Wheels[i].At.x < 0f)
			{
				transform2.localScale = new Vector3(-1f, 1f, 1f);
			}
			Vehicle.Draw(transform2, "Tyres", tyre, rubber);
			Vehicle.Draw(transform2, "Hub", hub, hubPaint);
			wheelModels[i] = transform;
		}
		wheelModelsCache = wheelModels;
		sprocketL = Spin("Sprocket", sprocket, sprocketAt, -1);
		sprocketRight = Spin("Sprocket", sprocket, sprocketAt, 1);
		idlerL = Spin("Idler", idler, idlerAt, -1);
		idlerRight = Spin("Idler", idler, idlerAt, 1);
		shoesL = Shoes("Left track");
		shoesR = Shoes("Right track");
		Transform[] Shoes(string name)
		{
			Transform transform3 = new GameObject(name).transform;
			transform3.SetParent(model, worldPositionStays: false);
			Transform[] array = new Transform[shoeCount];
			for (int j = 0; j < array.Length; j++)
			{
				GameObject gameObject = new GameObject("Shoe");
				gameObject.transform.SetParent(transform3, worldPositionStays: false);
				gameObject.AddComponent<MeshFilter>().sharedMesh = shoe;
				MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
				meshRenderer.sharedMaterial = trackSteel;
				meshRenderer.shadowCastingMode = ((j % 2 == 0) ? ShadowCastingMode.On : ShadowCastingMode.Off);
				array[j] = gameObject.transform;
			}
			return array;
		}
		Transform Spin(string name, Mesh mesh, Vector3 at, int sx)
		{
			Transform transform4 = new GameObject(name).transform;
			transform4.SetParent(model, worldPositionStays: false);
			transform4.localPosition = new Vector3((float)sx * trackX, at.y, at.z);
			Transform transform5 = new GameObject("Wheel").transform;
			transform5.SetParent(transform4, worldPositionStays: false);
			if (sx < 0)
			{
				transform5.localScale = new Vector3(-1f, 1f, 1f);
			}
			Vehicle.Draw(transform5, name, mesh, steel);
			return transform4;
		}
	}

	protected override void Update()
	{
		base.Update();
		if ((bool)sprocketL)
		{
			float num = 57.29578f / (sprocketR + 0.02f);
			float num2 = 57.29578f / (idlerR + 0.02f);
			sprocketL.localRotation = Quaternion.Euler(base.RolledLeft * num, 0f, 0f);
			sprocketRight.localRotation = Quaternion.Euler(base.RolledRight * num, 0f, 0f);
			idlerL.localRotation = Quaternion.Euler(base.RolledLeft * num2, 0f, 0f);
			idlerRight.localRotation = Quaternion.Euler(base.RolledRight * num2, 0f, 0f);
			LayTrack(shoesL, -1, base.RolledLeft);
			LayTrack(shoesR, 1, base.RolledRight);
		}
		if (GameSettings.SmartAI && !base.Wrecked && Time.time >= nextScare)
		{
			nextScare = Time.time + UnityEngine.Random.Range(1.2f, 2f);
			Scare();
		}
		if (base.Wrecked && !tossed)
		{
			if (wreckedAt < 0f)
			{
				wreckedAt = Time.time + UnityEngine.Random.Range(2.5f, 4.5f);
			}
			else if (Time.time >= wreckedAt)
			{
				TossTurret();
			}
		}
	}

	private void Scare()
	{
		if (gun == null || !base.Riders[gun.Seat] || base.Team < 0)
		{
			return;
		}
		Vector3 sightAt = gun.SightAt;
		foreach (CombatBrain item in CombatBrain.All)
		{
			if (!CaptureMode.Standing(item) || item.Team == base.Team || item.PlayerControlled || (bool)item.Body.Vehicle)
			{
				continue;
			}
			Vector3 vector = item.Body.Body.Head.Body.position - sightAt;
			float magnitude = vector.magnitude;
			if (!(magnitude > 70f) && !Physics.Raycast(sightAt, vector / magnitude, out var _, magnitude - 0.5f, -261, QueryTriggerInteraction.Ignore))
			{
				item.Body.SenseShot(sightAt, item.Here);
			}
		}
	}

	private void TossTurret()
	{
		tossed = true;
		if (gun == null || !gun.Turret || UnityEngine.Random.value > 0.65f)
		{
			return;
		}
		Transform turret = gun.Turret;
		gun.Turret = null;
		gun.Cradle = null;
		gun.Gun = null;
		Vector3 position = turret.position;
		turret.SetParent(null, worldPositionStays: true);
		Rigidbody rigidbody = turret.gameObject.AddComponent<Rigidbody>();
		rigidbody.mass = 12000f;
		rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		rigidbody.velocity = (base.Body ? base.Body.velocity : Vector3.zero) + Vector3.up * UnityEngine.Random.Range(9f, 14f) + UnityEngine.Random.insideUnitSphere * 2.5f;
		rigidbody.angularVelocity = UnityEngine.Random.insideUnitSphere * 2.5f;
		if ((bool)ShotEffects.Instance)
		{
			ShotEffects.Instance.Explosion(position, 2.6f);
			ShotEffects.Instance.Flare(position, 1.6f);
		}
		if ((bool)Acoustics.Instance)
		{
			Acoustics.Instance.Explosion(position, rocket: true);
		}
		if ((bool)BattleDirector.Instance)
		{
			BattleDirector.Instance.Shake(position, 1.5f);
		}
		Transform obj = new GameObject("Turret ring fire").transform;
		obj.SetParent(base.transform, worldPositionStays: false);
		obj.localPosition = TurretPivot + Vector3.up * 0.1f;
		Plumes.Fire(obj, 0.9f, light: true);
		UnityEngine.Object.Destroy(obj.gameObject, UnityEngine.Random.Range(40, 70));
		UnityEngine.Object.Destroy(turret.gameObject, 300f);
	}

	private void LayTrack(Transform[] shoes, int side, float rolled)
	{
		loop.Clear();
		Vector2 vector = new Vector2(idlerAt.z, idlerAt.y);
		Vector2 vector2 = new Vector2(sprocketAt.z, sprocketAt.y);
		float num = idlerR + 0.025f;
		float num2 = sprocketR + 0.025f;
		for (int i = 0; i <= 10; i++)
		{
			float num3 = (float)i / 10f;
			loop.Add(Vector2.Lerp(vector + Vector2.up * num, vector2 + Vector2.up * num2, num3) + Vector2.down * Mathf.Sin(num3 * MathF.PI) * 0.04f);
		}
		for (int j = 1; j <= 8; j++)
		{
			float f = (float)j / 8f * MathF.PI;
			loop.Add(vector2 + new Vector2(0f - Mathf.Sin(f), Mathf.Cos(f)) * num2);
		}
		for (int num4 = stations.Length - 1; num4 >= 0; num4--)
		{
			Transform transform = WheelModel(num4 * 2 + ((side >= 0) ? 1 : 0));
			Vector3 vector3 = (transform ? base.transform.InverseTransformPoint(transform.position) : new Vector3(0f, wheelR + 0.05f, stations[num4]));
			float y = vector3.y - wheelR - 0.01f;
			loop.Add(new Vector2(vector3.z - 0.16f, y));
			loop.Add(new Vector2(vector3.z + 0.16f, y));
		}
		for (int k = 0; k < 8; k++)
		{
			float f2 = (float)k / 8f * MathF.PI;
			loop.Add(vector + new Vector2(Mathf.Sin(f2), 0f - Mathf.Cos(f2)) * num);
		}
		along.Clear();
		float num5 = 0f;
		for (int l = 0; l < loop.Count; l++)
		{
			along.Add(num5);
			num5 += Vector2.Distance(loop[l], loop[(l + 1) % loop.Count]);
		}
		float num6 = num5 / (float)shoes.Length;
		float num7 = Mathf.Repeat(0f - rolled, num5);
		int m = 0;
		float x = (float)side * trackX;
		for (int n = 0; n < shoes.Length; n++)
		{
			float num8 = Mathf.Repeat((float)n * num6 + num7, num5);
			while (m > 0 && along[m] > num8)
			{
				m--;
			}
			for (; m + 1 < loop.Count && along[m + 1] <= num8; m++)
			{
			}
			Vector2 vector4 = loop[m];
			Vector2 vector5 = loop[(m + 1) % loop.Count];
			float num9 = Vector2.Distance(vector4, vector5);
			Vector2 vector6 = ((num9 > 0.0001f) ? Vector2.Lerp(vector4, vector5, (num8 - along[m]) / num9) : vector4);
			Vector2 vector7 = ((num9 > 0.0001f) ? ((vector5 - vector4) / num9) : Vector2.right);
			Vector2 vector8 = new Vector2(vector7.y, 0f - vector7.x);
			shoes[n].localPosition = new Vector3(x, vector6.y, vector6.x);
			shoes[n].localRotation = Quaternion.LookRotation(new Vector3(0f, vector7.y, vector7.x), new Vector3(0f, vector8.y, vector8.x));
			if (n + 1 < shoes.Length && Mathf.Repeat((float)(n + 1) * num6 + num7, num5) < num8)
			{
				m = 0;
			}
		}
	}

	private Transform WheelModel(int i)
	{
		if (wheelModelsCache == null || i >= wheelModelsCache.Length)
		{
			return null;
		}
		return wheelModelsCache[i];
	}

	protected Place[] CrewAndRiders(Place driver, Place gunner, float riderX, float riderY, float footX, float footY, float[] riderZ)
	{
		List<Place> list = new List<Place> { driver, gunner };
		int[] array = new int[2] { -1, 1 };
		foreach (int num in array)
		{
			foreach (float num2 in riderZ)
			{
				float num3 = ((num < 0) ? 1 : (-1));
				list.Add(new Place
				{
					Role = SeatRole.Holder,
					Pelvis = new Vector3((float)num * riderX, riderY, num2),
					Yaw = num * 90,
					Lean = 6f,
					FootL = new Vector3((float)num * footX, footY, num2 + 0.14f * num3),
					FootR = new Vector3((float)num * footX, footY, num2 - 0.14f * num3),
					HoldL = new Vector3((float)num * (riderX - 0.2f), riderY - 0.11f, num2 + 0.28f * num3),
					HoldR = new Vector3((float)num * (riderX - 0.2f), riderY - 0.11f, num2 - 0.28f * num3),
					Side = num,
					Stand = 1f
				});
			}
		}
		return list.ToArray();
	}
}
