using System;
using UnityEngine;

namespace CombatSandbox.Demo;

public abstract partial class Vehicle
{
	protected int atgmLoad;

	protected Vector3[] atgmRails;

	protected bool atgmOnTurret;

	private int atgmUsed;

	private int atgmRail;

	private float atgmNext;

	private float atgmRestock;

	public bool HasAtgm
	{
		get
		{
			if (atgmLoad > 0)
			{
				return atgmRails != null;
			}
			return false;
		}
	}

	public int Missiles
	{
		get
		{
			if (atgmUsed > 0 && Time.time >= atgmRestock)
			{
				atgmUsed--;
				atgmRestock = Time.time + 75f;
			}
			return Mathf.Max(0, atgmLoad - atgmUsed);
		}
	}

	public bool AtgmReady
	{
		get
		{
			if (HasAtgm && !Wrecked && Missiles > 0)
			{
				return Time.time >= atgmNext;
			}
			return false;
		}
	}

	public bool FireAtgm(ArenaTarget shooter, Func<Vector3?> guide, Vector3 firstAim)
	{
		if (!AtgmReady)
		{
			return false;
		}
		Vector3 position = atgmRails[atgmRail % atgmRails.Length];
		atgmRail++;
		Transform transform = ((atgmOnTurret && gun != null && (bool)gun.Turret) ? gun.Turret : base.transform);
		Vector3 vector = transform.TransformPoint(position);
		Vector3 vector2 = Vector3.RotateTowards(transform.forward, (firstAim - vector).normalized, 0.35f, 0f);
		vector2 = (vector2 + Vector3.up * 0.04f).normalized;
		Atgm.Fire(shooter, this, vector + vector2 * 0.8f, vector2, guide);
		atgmUsed++;
		atgmNext = Time.time + 6f;
		atgmRestock = Time.time + 75f;
		return true;
	}
}
