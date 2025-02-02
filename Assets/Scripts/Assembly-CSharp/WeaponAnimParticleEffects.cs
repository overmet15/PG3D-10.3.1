using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponAnimParticleEffects : MonoBehaviour
{
	public WeaponAnimEffectData[] effects;

	private List<GameObject> _effectObjects;

	private WeaponAnimEffectData _currentEffect;

	private string _lastAnimationName;

	private bool _isCanStopNotLoopEffect;

	private void Start()
	{
		_effectObjects = new List<GameObject>();
		for (int i = 0; i < effects.Length; i++)
		{
			InitiAnimatonEventForEffect(effects[i]);
			AddEffectObjectsToList(effects[i]);
		}
	}

	private void AddEffectObjectsToList(WeaponAnimEffectData effectData)
	{
		ParticleSystem[] particleSystems = effectData.particleSystems;
		if (particleSystems.Length != 0)
		{
			for (int i = 0; i < particleSystems.Length; i++)
			{
				_effectObjects.Add(particleSystems[i].gameObject);
			}
		}
	}

	public List<GameObject> GetListAnimEffects()
	{
		if (_effectObjects != null)
		{
			return _effectObjects;
		}
		_effectObjects = new List<GameObject>();
		for (int i = 0; i < effects.Length; i++)
		{
			AddEffectObjectsToList(effects[i]);
		}
		return _effectObjects;
	}

	private void InitiAnimatonEventForEffect(WeaponAnimEffectData effectData)
	{
		AnimationClip clip = GetComponent<Animation>().GetClip(effectData.animationName);
		if (!(clip == null))
		{
			AnimationEvent animationEvent = new AnimationEvent();
			animationEvent.stringParameter = effectData.animationName;
			animationEvent.functionName = "OnStartAnimEffects";
			animationEvent.time = 0f;
			clip.AddEvent(animationEvent);
			effectData.animationLength = ((!effectData.isLoop) ? clip.length : 0f);
		}
	}

	private void SetActiveEffect(WeaponAnimEffectData effect, bool active)
	{
		if (effect != null && effect.particleSystems.Length != 0)
		{
			for (int i = 0; i < effect.particleSystems.Length; i++)
			{
				ParticleSystem particleSystem = effect.particleSystems[i];
				particleSystem.gameObject.SetActive(active);
				particleSystem.Play();
			}
		}
	}

	private WeaponAnimEffectData GetEffectData(string animationName)
	{
		for (int i = 0; i < effects.Length; i++)
		{
			if (effects[i].animationName == animationName)
			{
				return effects[i];
			}
		}
		return null;
	}

	private bool CheckSkipStartEffectForAnimation(string animationName)
	{
		if (_currentEffect == null)
		{
			return false;
		}
		if (_currentEffect.isLoop)
		{
			return _lastAnimationName == animationName;
		}
		WeaponAnimEffectData effectData = GetEffectData(animationName);
		if (effectData != null && !effectData.isLoop)
		{
			CancelInvoke("ChangeEffectAfterStopAnimation");
			return false;
		}
		return !_isCanStopNotLoopEffect;
	}

	private void OnStartAnimEffects(string animationName)
	{
		if (CheckSkipStartEffectForAnimation(animationName))
		{
			return;
		}
		_lastAnimationName = animationName;
		WeaponAnimEffectData effectData = GetEffectData(animationName);
		bool flag = false;
		if (_currentEffect != null)
		{
			flag = _currentEffect.particleSystems.SequenceEqual(effectData.particleSystems) && _currentEffect.isLoop && effectData.isLoop;
			if (!flag)
			{
				SetActiveEffect(_currentEffect, false);
			}
		}
		_currentEffect = effectData;
		if (effectData != null)
		{
			if (!flag)
			{
				SetActiveEffect(effectData, true);
			}
			if (!effectData.isLoop)
			{
				_isCanStopNotLoopEffect = false;
				Invoke("ChangeEffectAfterStopAnimation", effectData.animationLength);
			}
		}
	}

	private string GetNamePlayingAnimation()
	{
		if (GetComponent<Animation>() == null)
		{
			return string.Empty;
		}
		foreach (AnimationState item in GetComponent<Animation>())
		{
			if (GetComponent<Animation>().IsPlaying(item.name))
			{
				return item.name;
			}
		}
		return string.Empty;
	}

	public void ChangeEffectAfterStopAnimation()
	{
		_isCanStopNotLoopEffect = true;
		string namePlayingAnimation = GetNamePlayingAnimation();
		if (!string.IsNullOrEmpty(namePlayingAnimation))
		{
			OnStartAnimEffects(namePlayingAnimation);
		}
	}
}
