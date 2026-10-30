using UnityEngine;

namespace CombatSandbox.Demo;

public sealed partial class ArenaTarget
{
	[System.NonSerialized]
	public bool Ragdolled;

	[System.NonSerialized]
	public readonly bool[] GrabWant = new bool[2];

	[System.NonSerialized]
	public Vector3 LimpMove;

	[System.NonSerialized]
	public Vector3 ReachAt;

	private readonly ConfigurableJoint[] grabs = new ConfigurableJoint[2];

	private static readonly Collider[] grabNear = new Collider[16];

	public bool Grabbing(int side)
	{
		return grabs[side];
	}

	private bool LimpStep(float dt)
	{
		if (Ragdolled && (!Brain || !Brain.PlayerControlled || Phase == ActiveRagdollPhase.Riding || (bool)Vehicle))
		{
			Ragdolled = false;
		}
		if (!Ragdolled)
		{
			DropGrabs();
			return false;
		}
		if (Phase != ActiveRagdollPhase.Falling && Phase != ActiveRagdollPhase.OnGround)
		{
			FallReason = "went limp";
			Enter(ActiveRagdollPhase.Falling);
		}
		else if (Phase == ActiveRagdollPhase.Falling && phaseTime > 0.7f && IsGrounded && Body.CenterVelocity().magnitude < 1.25f)
		{
			Enter(ActiveRagdollPhase.OnGround);
		}
		Relax(0.012f);
		bool flag = false;
		for (int i = 0; i < 2; i++)
		{
			flag |= Grab(i);
		}
		Vector3 limpMove = LimpMove;
		limpMove.y = 0f;
		if (limpMove.sqrMagnitude > 0.01f && (IsGrounded || flag))
		{
			limpMove = Vector3.ClampMagnitude(limpMove, 1f);
			Body.Chest.Body.AddForce(limpMove * 170f);
			Body.Pelvis.Body.AddForce(limpMove * 90f);
		}
		return true;
	}

	private bool Grab(int side)
	{
		HitZone hitZone = Body.Hands[side];
		if (!hitZone || hitZone.Lost || !hitZone.Body || !GrabWant[side] || !ArmsWork(side))
		{
			Drop(side);
			return false;
		}
		if ((bool)grabs[side])
		{
			if (LimpMove.sqrMagnitude > 0.01f)
			{
				Vector3 vector = hitZone.Body.position - Body.Chest.Body.position;
				Body.Chest.Body.AddForce(vector.normalized * 380f + Vector3.up * 140f);
			}
			return true;
		}
		HitZone hitZone2 = Body.UpperArms[side];
		Vector3 vector2 = (hitZone2 ? hitZone2.Body.position : Body.Chest.Body.position);
		Vector3 vector3 = ReachAt - vector2;
		Vector3 vector4 = ((vector3.sqrMagnitude > 0.0001f) ? (vector2 + Vector3.ClampMagnitude(vector3, 0.7f)) : (vector2 + Body.Chest.transform.forward * 0.6f));
		Vector3 vector5 = (vector4 - hitZone.Body.position) * 70f - hitZone.Body.velocity * 6f;
		hitZone.Body.AddForce(Vector3.ClampMagnitude(vector5, 120f), ForceMode.Acceleration);
		HitZone hitZone3 = Body.Forearms[side];
		if ((bool)hitZone3 && !hitZone3.Lost)
		{
			hitZone3.Body.AddForce(Vector3.ClampMagnitude(vector5 * 0.5f, 60f), ForceMode.Acceleration);
		}
		Vector3 worldCenterOfMass = hitZone.Body.worldCenterOfMass;
		int num = Physics.OverlapSphereNonAlloc(worldCenterOfMass, 0.12f, grabNear, -5, QueryTriggerInteraction.Ignore);
		Collider collider = null;
		float num2 = float.MaxValue;
		for (int i = 0; i < num; i++)
		{
			Collider collider2 = grabNear[i];
			HitZone hitZone4 = ShotQuery.ZoneOf(collider2);
			if ((!hitZone4 || !(hitZone4.Owner == Body)) && !(collider2 == Body.Neck) && !(collider2.GetComponentInParent<RagdollBody>() == Body))
			{
				float sqrMagnitude = (collider2.ClosestPoint(worldCenterOfMass) - worldCenterOfMass).sqrMagnitude;
				if (sqrMagnitude < num2)
				{
					num2 = sqrMagnitude;
					collider = collider2;
				}
			}
		}
		if (!collider)
		{
			return false;
		}
		Vector3 position = collider.ClosestPoint(worldCenterOfMass);
		ConfigurableJoint configurableJoint = hitZone.gameObject.AddComponent<ConfigurableJoint>();
		configurableJoint.autoConfigureConnectedAnchor = true;
		configurableJoint.anchor = hitZone.transform.InverseTransformPoint(position);
		configurableJoint.connectedBody = collider.attachedRigidbody;
		configurableJoint.xMotion = ConfigurableJointMotion.Locked;
		configurableJoint.yMotion = ConfigurableJointMotion.Locked;
		configurableJoint.zMotion = ConfigurableJointMotion.Locked;
		configurableJoint.angularXMotion = ConfigurableJointMotion.Free;
		configurableJoint.angularYMotion = ConfigurableJointMotion.Free;
		configurableJoint.angularZMotion = ConfigurableJointMotion.Free;
		configurableJoint.enableCollision = false;
		configurableJoint.breakForce = 7000f;
		configurableJoint.breakTorque = float.PositiveInfinity;
		grabs[side] = configurableJoint;
		return true;
	}

	private void Drop(int side)
	{
		if ((bool)grabs[side])
		{
			Object.Destroy(grabs[side]);
		}
		grabs[side] = null;
	}

	private void DropGrabs()
	{
		if ((bool)grabs[0] || (bool)grabs[1])
		{
			Drop(0);
			Drop(1);
		}
	}
}
