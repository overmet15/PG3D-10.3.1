using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
	[RequireComponent(typeof(ThirdPersonCharacter))]
	[RequireComponent(typeof(NavMeshAgent))]
	public class AICharacterControl : MonoBehaviour
	{
		public Transform target;

		public NavMeshAgent agent { get; private set; }

		public ThirdPersonCharacter character { get; private set; }

		private void Start()
		{
			agent = GetComponentInChildren<NavMeshAgent>();
			character = GetComponent<ThirdPersonCharacter>();
			agent.updateRotation = false;
			agent.updatePosition = true;
		}

		private void Update()
		{
			if (target != null)
			{
				agent.SetDestination(target.position);
				character.Move(agent.desiredVelocity, false, false);
			}
			else
			{
				character.Move(Vector3.zero, false, false);
			}
		}

		public void SetTarget(Transform target)
		{
			this.target = target;
		}
	}
}
