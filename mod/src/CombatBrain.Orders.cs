using System;
using UnityEngine;

namespace CombatSandbox.Demo;

public sealed partial class CombatBrain
{
	[NonSerialized]
	public float ObeyUntil;

	public void Obey(float seconds)
	{
		ObeyUntil = Time.time + seconds;
		nextPostLook = 0f;
		bounding = false;
		holdUntil = 0f;
		Release();
	}

	private Mode Obeyed(Mode mode)
	{
		if (Time.time >= ObeyUntil || PlayerControlled || !Fighter)
		{
			return mode;
		}
		if (mode == Mode.Fight && EnemyVisible && EnemyRange < 25f)
		{
			return mode;
		}
		if (mode != Mode.Fight && mode != Mode.Alert && mode != Mode.Patrol && mode != Mode.Standby && mode != Mode.Guard && mode != Mode.Defend && mode != Mode.Advance)
		{
			return mode;
		}
		if (Attacking && HasObjective)
		{
			return Mode.Advance;
		}
		if (Defending)
		{
			return Mode.Defend;
		}
		return mode;
	}
}
