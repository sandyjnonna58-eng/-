using System;
using UnityEngine;

namespace CombatSandbox.Demo;

public sealed partial class CombatBrain
{
	[NonSerialized]
	public FieldGun GunPost;

	private bool liftWaiting;

	private float liftWaitSince = -1f;

	private float liftCheckAt;

	private float liftGiveUpUntil;

	private Vector3 liftSpot;

	private Mode Posted(Mode mode)
	{
		if (PlayerControlled || !Fighter)
		{
			liftWaiting = false;
			return mode;
		}
		bool flag = mode == Mode.Fight || mode == Mode.Alert || mode == Mode.Patrol || mode == Mode.Standby || mode == Mode.Guard || mode == Mode.Defend || mode == Mode.Advance;
		if ((bool)GunPost)
		{
			liftWaiting = false;
			if (GunPost.Destroyed)
			{
				GunPost = null;
				return mode;
			}
			if (mode == Mode.Fight && EnemyVisible && EnemyRange < 35f)
			{
				return mode;
			}
			if (!flag)
			{
				return mode;
			}
			return Mode.Guard;
		}
		liftWaiting = flag && WaitForLift(mode);
		if (!liftWaiting)
		{
			return mode;
		}
		return Mode.Guard;
	}

	private bool WaitForLift(Mode mode)
	{
		if (!BigMap.Active || (bool)Lift || (bool)ClaimedVehicle || (bool)CrewOf || (bool)target.Vehicle || Time.time < liftGiveUpUntil || Duty == Role.Commander || Duty == Role.Operator)
		{
			liftWaitSince = -1f;
			return false;
		}
		if ((mode == Mode.Fight && EnemyVisible) || UnderFire)
		{
			liftWaitSince = -1f;
			return false;
		}
		if (Time.time >= liftCheckAt)
		{
			liftCheckAt = Time.time + 2f;
			bool flag = TravelGoal(out var goal) && Flat(goal - Here).magnitude > 250f && Motorpool.LiftComing(this);
			if (!flag)
			{
				liftWaitSince = -1f;
				return false;
			}
			if (liftWaitSince < 0f)
			{
				liftWaitSince = Time.time;
				liftSpot = Here;
			}
		}
		if (liftWaitSince < 0f)
		{
			return false;
		}
		if (Time.time - liftWaitSince > 75f)
		{
			liftWaitSince = -1f;
			liftGiveUpUntil = Time.time + 60f;
			return false;
		}
		return true;
	}

	private bool PostStep(ref CombatOrders orders)
	{
		if (!GunPost && !liftWaiting)
		{
			return false;
		}
		Vector3 vector = (GunPost ? GunPost.CrewSpot(this) : liftSpot);
		orders.Carry = RifleCarry.LowReady;
		if (Flat(vector - Here).magnitude > 0.8f)
		{
			Steer(vector, ref orders);
			return true;
		}
		orders.Face = true;
		orders.FaceTo = (GunPost ? (GunPost.transform.position + GunPost.transform.forward * 20f) : (ThreatKnown ? Threat : Objective));
		orders.Crouch = (GunPost ? 0.5f : 0f);
		return true;
	}
}
