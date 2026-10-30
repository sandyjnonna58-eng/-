using UnityEngine;

namespace CombatSandbox.Demo;

public static class Crews
{
	public static int Count { get; private set; }

	private static bool Crewed(Vehicle v)
	{
		if (!(v is Tank) && !(v is Helicopter) && !(v is Btr))
		{
			return v is M113;
		}
		return true;
	}

	public static void Spawn(BattleDirector director)
	{
		Count = 0;
		if (!GameSettings.ReadyCrews || !director)
		{
			return;
		}
		int num = 0;
		foreach (Vehicle item in Vehicle.All)
		{
			if ((bool)item && item.Team >= 0 && Crewed(item))
			{
				num += ((item.Gun != null && item.Gun.Seat != 0) ? 2 : 1);
			}
		}
		director.MaxSoldiers += num;
		foreach (Vehicle item2 in Vehicle.All.ToArray())
		{
			if (!item2 || item2.Team < 0 || !Crewed(item2))
			{
				continue;
			}
			int[] array = ((item2.Gun != null && item2.Gun.Seat != 0) ? new int[2] { 0, item2.Gun.Seat } : new int[1]);
			foreach (int num2 in array)
			{
				Place place = item2.Places[num2];
				Vector3 at = (place.Inside ? (item2.World(place.Door) + item2.Rotation * place.Out.normalized * 1.2f) : (item2.Seat(num2) + item2.Rotation * Vector3.right * ((place.Side != 0) ? ((float)place.Side) : 1f) * 1.4f));
				CombatBrain combatBrain = director.Spawn(item2.Team, at, CombatBrain.Role.Crewman);
				if (!combatBrain)
				{
					return;
				}
				TeamRoster teamRoster = director.RosterOf(item2.Team);
				string text = ((teamRoster.Units.Count > 0) ? teamRoster.Units[0].Primary : "ak74");
				if (text == "pkm" || text == "svd" || text == "rpg7")
				{
					text = "ak74";
				}
				combatBrain.Equip(text, (teamRoster.Units.Count > 0) ? teamRoster.Units[0].Pistol : "g17", 1);
				bool flag = item2 is Helicopter;
				combatBrain.name = Loc.T(item2.Kind) + " - " + (Loc.Russian ? ((num2 == 0) ? (flag ? "пилот" : "мехвод") : "наводчик") : ((num2 == 0) ? (flag ? "pilot" : "driver") : "gunner"));
				combatBrain.TakeCrew(item2, num2);
				Count++;
			}
		}
	}
}
