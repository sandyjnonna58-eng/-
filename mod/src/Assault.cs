using UnityEngine;
using UnityEngine.AI;

namespace CombatSandbox.Demo;

public static class Assault
{
	public static int Attacker
	{
		get
		{
			if (CaptureMode.Active)
			{
				return -1;
			}
			return GameSettings.BattleType - 1;
		}
	}

	public static bool Active => Attacker >= 0;

	public static void Begin(BattleDirector director)
	{
		if (!director)
		{
			return;
		}
		int attacker = Attacker;
		if (attacker < 0)
		{
			if (GameSettings.Artillery && BattleSetup.Mode != GameMode.Capture)
			{
				for (int i = 0; i < 2; i++)
				{
					Guns(director, i, 2);
				}
			}
			return;
		}
		int num = 1 - attacker;
		director.Teams[attacker].Attack = true;
		director.Teams[num].Attack = false;
		Guns(director, num, 3);
		director.Notice?.Invoke(director.Teams[attacker].Name + " attacks, " + director.Teams[num].Name + " defends");
	}

	private static void Guns(BattleDirector director, int team, int mortars)
	{
		Transform @base = director.Teams[team].Base;
		Transform base2 = director.Teams[1 - team].Base;
		if (!@base)
		{
			return;
		}
		Vector3 position = @base.position;
		Vector3 vector = (base2 ? Vector3.ProjectOnPlane(base2.position - position, Vector3.up).normalized : Vector3.forward);
		if (vector.sqrMagnitude < 0.01f)
		{
			vector = Vector3.forward;
		}
		Vector3 vector2 = Vector3.Cross(Vector3.up, vector);
		int[] array = new int[2] { -1, 1 };
		foreach (int num in array)
		{
			if (Ground(position - vector * 14f + vector2 * num * 7f, out var at))
			{
				FieldGun.Create(FieldGun.GunKind.Howitzer, team, at, vector);
			}
		}
		for (int j = 0; j < mortars; j++)
		{
			float num2 = ((float)j - (float)(mortars - 1) * 0.5f) * 18f;
			if (Ground(position + vector * 40f + vector2 * num2, out var at2))
			{
				FieldGun.Create(FieldGun.GunKind.Mortar, team, at2, vector);
			}
		}
	}

	private static bool Ground(Vector3 near, out Vector3 at)
	{
		at = near;
		if (NavMesh.SamplePosition(near, out var hit, 12f, -1))
		{
			at = hit.position;
		}
		if (Physics.Raycast(at + Vector3.up * 30f, Vector3.down, out var hitInfo, 60f, -261, QueryTriggerInteraction.Ignore))
		{
			at = hitInfo.point;
			return hitInfo.normal.y > 0.8f;
		}
		return false;
	}
}
