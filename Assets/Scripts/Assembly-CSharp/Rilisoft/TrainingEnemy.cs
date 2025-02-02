using System.Collections;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class TrainingEnemy : MonoBehaviour
	{
		private enum State
		{
			None = 0,
			Awakened = 1,
			Dead = 2
		}

		public AudioClip wakeUpAudioClip;

		public AudioClip dieAudioClip;

		public GameObject aimTarget;

		public int hitPoints = 3;

		private SkinnedMeshRenderer meshRender;

		public Texture hitTexture;

		private Texture skinTexture;

		private AudioSource _audioSource;

		private Animation _animation;

		private State _currentState;

		public void WakeUp(float delaySeconds = 0f)
		{
			StartCoroutine(AwakeCoroutine(delaySeconds));
		}

		public void ApplyDamage(float damage)
		{
			if (_currentState != State.Awakened)
			{
				return;
			}
			StartCoroutine(HighlightHitCoroutine());
			if (_animation.IsPlaying("Dummy_Damage"))
			{
				return;
			}
			hitPoints--;
			if (_animation != null)
			{
				_animation.Play("Dummy_Damage", PlayMode.StopSameLayer);
			}
			if (hitPoints <= 0)
			{
				_currentState = State.Dead;
				if (aimTarget != null)
				{
					Object.Destroy(aimTarget);
					aimTarget = null;
				}
				StartCoroutine(DieCoroutine());
			}
		}

		private void Awake()
		{
			if (aimTarget != null)
			{
				aimTarget.SetActive(false);
			}
		}

		private void Start()
		{
			_audioSource = GetComponent<AudioSource>();
			_animation = GetComponent<Animation>();
			if (_animation != null)
			{
				_animation.Play("Dummy_Idle", PlayMode.StopSameLayer);
			}
			meshRender = GetComponentInChildren<SkinnedMeshRenderer>();
			if ((bool)meshRender)
			{
				meshRender.sharedMaterial = new Material(meshRender.sharedMaterial);
				skinTexture = meshRender.sharedMaterial.mainTexture;
			}
		}

		private IEnumerator HighlightHitCoroutine()
		{
			SetTexture(hitTexture);
			yield return new WaitForSeconds(0.125f);
			SetTexture(skinTexture);
		}

		public void SetTexture(Texture needTx)
		{
			if (meshRender != null)
			{
				meshRender.sharedMaterial.mainTexture = needTx;
			}
		}

		private IEnumerator AwakeCoroutine(float delaySeconds = 0f)
		{
			if (delaySeconds > 0f)
			{
				yield return new WaitForSeconds(delaySeconds);
			}
			if (_animation != null)
			{
				if (_audioSource != null && wakeUpAudioClip != null)
				{
					_audioSource.PlayOneShot(wakeUpAudioClip);
				}
				_animation.Play("Dummy_Up", PlayMode.StopSameLayer);
				while (_animation.isPlaying)
				{
					yield return null;
				}
				if (aimTarget != null)
				{
					aimTarget.SetActive(true);
				}
			}
			_currentState = State.Awakened;
		}

		private IEnumerator DieCoroutine()
		{
			if (_animation != null)
			{
				while (_animation.IsPlaying("Dummy_Damage"))
				{
					yield return null;
				}
				if (_audioSource != null && dieAudioClip != null)
				{
					_audioSource.PlayOneShot(dieAudioClip);
				}
				_animation.Play("Dead", PlayMode.StopSameLayer);
				while (_animation.isPlaying)
				{
					yield return null;
				}
			}
			if (ZombieCreator.sharedCreator != null)
			{
				ZombieCreator.sharedCreator.NumOfDeadZombies++;
			}
		}
	}
}
