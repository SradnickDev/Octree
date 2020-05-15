using UnityEngine;

namespace Steering
{
	public abstract class Steer : MonoBehaviour
	{
		[SerializeField] protected float MaxSpeed = 3;
		[SerializeField] protected float MaxForce = 15;
		protected Vector3 Velocity;
		protected Vector3 Target;

		public void DoSteer(Vector3 desiredVelocity)
		{
			var steeringForce = desiredVelocity - Velocity;
			steeringForce = Vector3.ClampMagnitude(steeringForce, MaxForce);
			steeringForce /= 15;

			Velocity = Vector3.ClampMagnitude(Velocity + steeringForce, MaxSpeed);
			transform.position += Velocity * Time.deltaTime;
			LookAt();
		}

		public void LookAt()
		{
			transform.forward = Velocity.normalized;
		}
	}
}