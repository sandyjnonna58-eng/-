using System.Collections.Generic;
using UnityEngine;

namespace CombatSandbox.Demo;

public abstract class Ifv : Tank
{
	public static readonly WeaponDef Gun2A42 = Autocannon("2a42", "2A42 30 mm", "30×165 mm", 980f, 0.39f, 300f, 40, 30f, 0.45f, 2.6f, 3.5f);

	public static readonly WeaponDef M242 = Autocannon("m242", "M242 Bushmaster 25 mm", "25×137 mm", 1100f, 0.18f, 220f, 50, 22f, 0.5f, 2.4f, 3f);

	public static WeaponDef Autocannon(string id, string name, string caliber, float velocity, float mass, float rpm, int magazine, float recoil, float pitch, float loudness, float flash)
	{
		return new WeaponDef
		{
			Id = id,
			Name = name,
			Caliber = caliber,
			Class = WeaponClass.MachineGun,
			MuzzleVelocity = velocity,
			BulletMass = mass,
			Drag = 0f,
			Rpm = rpm,
			Magazine = magazine,
			Reload = 4.5f,
			Auto = true,
			Semi = false,
			Recoil = recoil,
			Dispersion = 2f,
			Mass = 115f,
			Zero = 1000f,
			TracerEvery = 1,
			Sound = GunSound.Sniper,
			Pitch = pitch,
			Loudness = loudness,
			Flash = flash,
			Burst = new Vector2Int(3, 6),
			EffectiveRange = 2000f
		};
	}

	protected static void Troops(List<Place> list, float[] rows, float x, float y, float footX, float footY, bool outward, Vector3 door)
	{
		int[] array = new int[2] { -1, 1 };
		foreach (int num in array)
		{
			foreach (float num2 in rows)
			{
				float num3 = (outward ? num : (-num));
				list.Add(new Place
				{
					Role = SeatRole.Holder,
					Pelvis = new Vector3((float)num * x, y, num2),
					Yaw = num3 * 90f,
					Lean = 6f,
					FootL = new Vector3((float)num * footX, footY, num2 + 0.13f * num3),
					FootR = new Vector3((float)num * footX, footY, num2 - 0.13f * num3),
					HoldL = new Vector3((float)num * (x + footX) * 0.5f, y - 0.08f, num2 + 0.2f * num3),
					HoldR = new Vector3((float)num * (x + footX) * 0.5f, y - 0.08f, num2 - 0.2f * num3),
					Door = door + Vector3.right * num * 0.3f,
					Out = Vector3.back,
					Stand = 1.85f
				});
			}
		}
	}
}
