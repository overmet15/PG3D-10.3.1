using UnityEngine;

public class RocketSettings : MonoBehaviour
{
	public enum TypeFlyRocket
	{
		Rocket = 0,
		Grenade = 1,
		Bullet = 2,
		MegaBullet = 3,
		Autoaim = 4,
		Bomb = 5,
		AutoaimBullet = 6,
		Ball = 7,
		GravityRocket = 8
	}

	public WeaponSounds.TypeDead typeDead = WeaponSounds.TypeDead.explosion;

	public TypeFlyRocket typeFly;

	public Player_move_c.TypeKills typeKilsIconChat = Player_move_c.TypeKills.explosion;

	public Vector3 sizeBoxCollider = new Vector3(0.15f, 0.15f, 0.75f);
}
