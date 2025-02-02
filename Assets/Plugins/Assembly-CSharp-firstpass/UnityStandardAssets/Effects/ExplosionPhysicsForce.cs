using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Effects
{
	public class ExplosionPhysicsForce : MonoBehaviour
	{
		public float explosionForce = 4f;

		private IEnumerator Start()
		{
			yield return null;
			float multiplier = GetComponent<ParticleSystemMultiplier>().multiplier;
			float r = 10f * multiplier;
			Collider[] cols = Physics.OverlapSphere(base.transform.position, r);
			List<Rigidbody> rigidbodies = new List<Rigidbody>();
			Collider[] array = cols;
			foreach (Collider col in array)
			{
				if (col.attachedRigidbody != null && !rigidbodies.Contains(col.attachedRigidbody))
				{
					rigidbodies.Add(col.attachedRigidbody);
				}
			}
			foreach (Rigidbody rb in rigidbodies)
			{
				rb.AddExplosionForce(explosionForce * multiplier, base.transform.position, r, 1f * multiplier, ForceMode.Impulse);
			}
		}
	}
}
