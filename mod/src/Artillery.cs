using System.Collections;
using UnityEngine;

namespace CombatSandbox.Demo;

public sealed class Artillery : MonoBehaviour
{
	public const float PlayerCooldown = 40f;

	private const float Power = 3f;

	private const int Salvo = 6;

	private const float Spread = 14f;

	private readonly float[] nextAuto = new float[2];

	private readonly float[] nextCall = new float[2];

	private readonly bool[] firing = new bool[2];

	private static readonly int[] stock = new int[2];

	public const int OffMapShells = 36;

	public static Artillery Instance { get; private set; }

	public static Artillery Begin()
	{
		if ((bool)Instance)
		{
			return Instance;
		}
		return new GameObject("Artillery").AddComponent<Artillery>();
	}

	private void Awake()
	{
		Instance = this;
		stock[0] = (stock[1] = 36);
		for (int i = 0; i < 2; i++)
		{
			nextAuto[i] = Time.time + Random.Range(60f, 100f);
		}
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	public float Ready(int team)
	{
		return Mathf.Max(0f, nextCall[Mathf.Clamp(team, 0, 1)] - Time.time);
	}

	public bool Call(int team, Vector3 at)
	{
		team = Mathf.Clamp(team, 0, 1);
		if (!Allowed(team) || firing[team] || Time.time < nextCall[team])
		{
			return false;
		}
		nextCall[team] = Time.time + 40f;
		StartCoroutine(Fire(team, at));
		return true;
	}

	public static bool Allowed(int team)
	{
		if (!GameSettings.Artillery)
		{
			return false;
		}
		if (Assault.Active && team == Assault.Attacker)
		{
			return false;
		}
		if (FieldGun.AnyHowitzers(team))
		{
			if (FieldGun.Battery(team))
			{
				return FieldGun.Shells(team) > 0;
			}
			return false;
		}
		return stock[team] > 0;
	}

	public static int ShellsLeft(int team)
	{
		team = Mathf.Clamp(team, 0, 1);
		if (!FieldGun.AnyHowitzers(team))
		{
			return stock[team];
		}
		return FieldGun.Shells(team);
	}

	public static string Refusal(int team)
	{
		team = Mathf.Clamp(team, 0, 1);
		if (!GameSettings.Artillery)
		{
			return "Artillery is off in the settings";
		}
		if (Assault.Active && team == Assault.Attacker)
		{
			return "No artillery for this side";
		}
		if (FieldGun.AnyHowitzers(team))
		{
			if (!FieldGun.Intact(team))
			{
				return "The guns are destroyed";
			}
			if (FieldGun.Shells(team) <= 0)
			{
				return "The battery is out of shells";
			}
			if (!FieldGun.Battery(team))
			{
				return "No crew at the guns - gunners are on their way";
			}
		}
		else if (stock[team] <= 0)
		{
			return "The battery is out of shells";
		}
		return null;
	}

	private void Update()
	{
		if (!GameSettings.Artillery || (CaptureMode.Active && !CaptureMode.Instance.Started))
		{
			return;
		}
		for (int i = 0; i < 2; i++)
		{
			if (!(Time.time < nextAuto[i]) && !firing[i] && Allowed(i))
			{
				nextAuto[i] = Time.time + (Assault.Active ? Random.Range(35f, 55f) : Random.Range(70f, 110f));
				if (Target(i, out var at))
				{
					StartCoroutine(Fire(i, at));
				}
			}
		}
	}

	private static bool Target(int team, out Vector3 at)
	{
		at = Vector3.zero;
		int num = 1;
		foreach (CombatBrain item in CombatBrain.All)
		{
			if (!CaptureMode.Standing(item) || item.Team == team || item.PlayerControlled)
			{
				continue;
			}
			Vector3 here = item.Here;
			int num2 = 0;
			bool flag = false;
			foreach (CombatBrain item2 in CombatBrain.All)
			{
				if (CaptureMode.Standing(item2))
				{
					float magnitude = (item2.Here - here).magnitude;
					if (item2.Team == team && magnitude < 40f)
					{
						flag = true;
						break;
					}
					if (item2.Team != team && magnitude < 15f)
					{
						num2++;
					}
				}
			}
			if (!flag && num2 > num)
			{
				num = num2;
				at = here;
			}
		}
		if (num >= 2)
		{
			return true;
		}
		float num3 = float.MaxValue;
		foreach (Vehicle item3 in Vehicle.All)
		{
			if (!((bool)item3) || item3.Wrecked || item3.Team < 0 || item3.Team == team || !item3.Armoured || item3 is Helicopter)
			{
				continue;
			}
			bool flag2 = false;
			foreach (CombatBrain item4 in CombatBrain.All)
			{
				if (CaptureMode.Standing(item4) && item4.Team == team && (item4.Here - item3.transform.position).magnitude < 40f)
				{
					flag2 = true;
					break;
				}
			}
			float magnitude2 = item3.Body ? item3.Body.velocity.magnitude : 0f;
			if (!flag2 && magnitude2 < num3)
			{
				num3 = magnitude2;
				at = item3.transform.position;
			}
		}
		return num3 < float.MaxValue;
	}

	private IEnumerator Fire(int team, Vector3 at)
	{
		firing[team] = true;
		BattleDirector instance = BattleDirector.Instance;
		if ((bool)instance)
		{
			instance.Event(instance.Teams[team].Name + ": artillery fire mission", team);
		}
		if ((bool)Acoustics.Instance)
		{
			Vector3 vector = at + (at - Vector3.zero).normalized * 600f + Vector3.up * 50f;
			Acoustics.Instance.Distant(vector, blast: true, 1.4f);
		}
		yield return new WaitForSeconds(Random.Range(2.5f, 4f));
		foreach (CombatBrain item in CombatBrain.All)
		{
			if (CaptureMode.Standing(item) && (item.Here - at).magnitude < 35f)
			{
				item.Body.SenseShot(at + Vector3.up * 30f, at);
			}
		}
		bool flag = FieldGun.AnyHowitzers(team);
		for (int i = 0; i < 6; i++)
		{
			if (flag)
			{
				if (!FieldGun.TakeShell(team))
				{
					break;
				}
			}
			else
			{
				if (stock[team] <= 0)
				{
					break;
				}
				stock[team]--;
			}
			Vector2 vector2 = Random.insideUnitCircle * 14f;
			Vector3 vector3 = at + new Vector3(vector2.x, 0f, vector2.y);
			Vector3 vector4 = new Vector3(Random.Range(-25f, 25f), -300f, Random.Range(-25f, 25f));
			TankShell.Fire(null, null, vector3 - vector4 * 1.4f, vector4, 0, 3f, tracer: false, 12f, smoke: true);
			yield return new WaitForSeconds(Random.Range(0.35f, 0.9f));
		}
		yield return new WaitForSeconds(2f);
		firing[team] = false;
	}
}
