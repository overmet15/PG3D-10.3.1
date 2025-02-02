using System;
using UnityEngine;

[Serializable]
public class WeaponAnimEffectData
{
	public string animationName;

	public bool isLoop = true;

	public ParticleSystem[] particleSystems;

	public float animationLength { get; set; }
}
