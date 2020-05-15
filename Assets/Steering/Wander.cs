using UnityEngine;
using Random = UnityEngine.Random;

namespace Steering
{
	public class Wander : Steer
	{
		[SerializeField] private float m_circleRadius = 3;
		[SerializeField] private float m_turnChance = 0.08f;
		[SerializeField] private float m_maxRadius = 10;

		private Vector3 m_wanderForce;

		private void Start()
		{
			Velocity = Random.onUnitSphere;
			UpdateDisplacement();
		}

		private void Update()
		{
			DoWander();
		}

		private void DoWander()
		{
			var desiredVelocity = WanderForce();
			desiredVelocity = desiredVelocity.normalized * MaxSpeed;

			DoSteer(desiredVelocity);
		}

		private Vector3 WanderForce()
		{
			if (transform.position.magnitude > m_maxRadius)
			{
				var directionToCenter = (Target - transform.position).normalized;
				m_wanderForce = Velocity.normalized + directionToCenter;
			}
			else if (Random.value < m_turnChance)
			{
				UpdateDisplacement();
			}

			return m_wanderForce;
		}

		private void UpdateDisplacement()
		{
			var circleCenter = Velocity.normalized;
			var randomPoint = Random.insideUnitCircle;

			var displacement = new Vector3(randomPoint.x, randomPoint.y) * m_circleRadius;
			displacement = Quaternion.LookRotation(Velocity) * displacement;
			m_wanderForce = circleCenter + displacement;
		}
	}
}