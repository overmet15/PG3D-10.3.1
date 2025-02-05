using System.Collections.Generic;

public static class EffectsController
{
	private static float slowdownCoeff = 1f;

	public static float SlowdownCoeff
	{
		get
		{
			return slowdownCoeff;
		}
		set
		{
			slowdownCoeff = value;
		}
	}

	public static float JumpModifier
	{
		get
		{
			float num = 1f;
			num += ((!Storager.getString(Defs.HatEquppedSN, false).Equals("hat_Samurai")) ? 0f : 0.05f);
			num += ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("cape_Custom")) ? 0f : 0.05f);
			num += ((!Storager.getString(Defs.BootsEquppedSN, false).Equals("boots_gray")) ? 0f : 0.1f);
			num += ((!Storager.getString(Defs.BootsEquppedSN, false).Equals("StormTrooperBoots_Up1")) ? 0f : 0.2f);
			num += ((!Storager.getString(Defs.BootsEquppedSN, false).Equals("StormTrooperBoots_Up2")) ? 0f : 0.3f);
			num += ((!Storager.getString("MaskEquippedSN", false).Equals("mask_demolition")) ? 0f : 0.1f);
			num += ((!Storager.getString("MaskEquippedSN", false).Equals("mask_demolition_up1")) ? 0f : 0.2f);
			num += ((!Storager.getString("MaskEquippedSN", false).Equals("mask_demolition_up2")) ? 0f : 0.3f);
			return num * SlowdownCoeff;
		}
	}

	public static bool NinjaJumpEnabled
	{
		get
		{
			return Storager.getString(Defs.BootsEquppedSN, false).Equals("boots_tabi") || Storager.getString(Defs.BootsEquppedSN, false).Equals("boots_black") || Storager.getString(Defs.BootsEquppedSN, false).Equals("BerserkBoots_Up1") || Storager.getString(Defs.BootsEquppedSN, false).Equals("BerserkBoots_Up2");
		}
	}

	public static float ExplosionImpulseRadiusIncreaseCoef
	{
		get
		{
			return ((!Storager.getString(Defs.BootsEquppedSN, false).Equals("boots_green")) ? 0f : 0.1f) + ((!Storager.getString(Defs.BootsEquppedSN, false).Equals("DemolitionBoots_Up1")) ? 0f : 0.15f) + ((!Storager.getString(Defs.BootsEquppedSN, false).Equals("DemolitionBoots_Up2")) ? 0f : 0.25f);
		}
	}

	public static float GrenadeExplosionDamageIncreaseCoef
	{
		get
		{
			return ((!Storager.getString(Defs.BootsEquppedSN, false).Equals("boots_green")) ? 0f : 0.5f) + ((!Storager.getString(Defs.BootsEquppedSN, false).Equals("DemolitionBoots_Up1")) ? 0f : 0.55f) + ((!Storager.getString(Defs.BootsEquppedSN, false).Equals("DemolitionBoots_Up2")) ? 0f : 0.6f);
		}
	}

	public static float GrenadeExplosionRadiusIncreaseCoef
	{
		get
		{
			float num = 1f;
			num += ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("cape_RoyalKnight")) ? 0f : 0.5f);
			num += ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("DemolitionCape_Up1")) ? 0f : 0.55f);
			num += ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("DemolitionCape_Up2")) ? 0f : 0.6f);
			return 1f;
		}
	}

	public static float SelfExplosionDamageDecreaseCoef
	{
		get
		{
			return 1f * ((!Storager.getString(Defs.HatEquppedSN, false).Equals("hat_KingsCrown")) ? 1f : 0.75f) * ((!Storager.getString(Defs.HatEquppedSN, false).Equals("hat_DiamondHelmet")) ? 1f : 0.5f) * ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("cape_RoyalKnight")) ? 1f : 0.5f) * ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("DemolitionCape_Up1")) ? 1f : 0.25f) * ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("DemolitionCape_Up2")) ? 1f : 0.1f);
		}
	}

	public static bool WeAreStealth
	{
		get
		{
			return Storager.getString(Defs.BootsEquppedSN, false).Equals("boots_blue") || Storager.getString(Defs.BootsEquppedSN, false).Equals("SniperBoots_Up1") || Storager.getString(Defs.BootsEquppedSN, false).Equals("SniperBoots_Up2");
		}
	}

	public static float ArmorBonus
	{
		get
		{
			float num = 0f;
			if (Storager.getString(Defs.BootsEquppedSN, false).Equals("boots_red"))
			{
				num += 1f;
			}
			if (Storager.getString(Defs.BootsEquppedSN, false).Equals("HitmanBoots_Up1"))
			{
				num += 2f;
			}
			if (Storager.getString(Defs.BootsEquppedSN, false).Equals("HitmanBoots_Up2"))
			{
				num += 3f;
			}
			if (Storager.getString("MaskEquippedSN", false).Equals("mask_berserk"))
			{
				num += 1f;
			}
			else if (Storager.getString("MaskEquippedSN", false).Equals("mask_berserk_up1"))
			{
				num += 2f;
			}
			else if (Storager.getString("MaskEquippedSN", false).Equals("mask_berserk_up2"))
			{
				num += 3f;
			}
			return num;
		}
	}

	public static float IcnreaseEquippedArmorPercentage
	{
		get
		{
			float num = 1f;
			num += ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("cape_BloodyDemon")) ? 0f : 0.1f);
			num += ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("BerserkCape_Up1")) ? 0f : 0.2f);
			return num + ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("BerserkCape_Up2")) ? 0f : 0.3f);
		}
	}

	public static float RegeneratingArmorTime
	{
		get
		{
			float result = 0f;
			if (Defs.isHunger)
			{
				return 0f;
			}
			if (Storager.getString(Defs.CapeEquppedSN, false).Equals("cape_Archimage"))
			{
				result = 12f;
			}
			if (Storager.getString(Defs.CapeEquppedSN, false).Equals("HitmanCape_Up1"))
			{
				result = 10f;
			}
			if (Storager.getString(Defs.CapeEquppedSN, false).Equals("HitmanCape_Up2"))
			{
				result = 8f;
			}
			return result;
		}
	}

	public static bool IsRegeneratingArmor
	{
		get
		{
			return RegeneratingArmorTime > 0f;
		}
	}

	public static float AmmoModForCategory(int i)
	{
		float num = 1f;
		switch (i)
		{
		case 0:
			num += ((!Storager.getString(Defs.BootsEquppedSN, false).Equals("boots_gray")) ? 0f : 0.1f);
			num += ((!Storager.getString(Defs.BootsEquppedSN, false).Equals("StormTrooperBoots_Up1")) ? 0f : 0.15f);
			num += ((!Storager.getString(Defs.BootsEquppedSN, false).Equals("StormTrooperBoots_Up2")) ? 0f : 0.2f);
			break;
		case 3:
			num += ((!Storager.getString("MaskEquippedSN", false).Equals("mask_engineer")) ? 0f : 0.1f);
			num += ((!Storager.getString("MaskEquippedSN", false).Equals("mask_engineer_up1")) ? 0f : 0.15f);
			num += ((!Storager.getString("MaskEquippedSN", false).Equals("mask_engineer_up2")) ? 0f : 0.2f);
			break;
		}
		return num;
	}

	public static float DamageModifsByCats(int i)
	{
		List<float> list = new List<float>(6);
		for (int j = 0; j < 6; j++)
		{
			list.Add(0f);
		}
		List<float> list2;
		List<float> list3 = (list2 = list);
		int index;
		int index2 = (index = 0);
		float num = list2[index];
		list3[index2] = num + ((!Storager.getString(Defs.HatEquppedSN, false).Equals("hat_Headphones")) ? 0f : 0.05f);
		List<float> list4;
		List<float> list5 = (list4 = list);
		int index3 = (index = 2);
		num = list4[index];
		list5[index3] = num + ((!Storager.getString("MaskEquippedSN", false).Equals("hat_ManiacMask")) ? 0f : 0.1f);
		List<float> list6;
		List<float> list7 = (list6 = list);
		int index4 = (index = 2);
		num = list6[index];
		list7[index4] = num + ((!Storager.getString(Defs.HatEquppedSN, false).Equals("hat_Samurai")) ? 0f : 0.05f);
		List<float> list8;
		List<float> list9 = (list8 = list);
		int index5 = (index = 1);
		num = list8[index];
		list9[index5] = num + ((!Storager.getString(Defs.HatEquppedSN, false).Equals("hat_SeriousManHat")) ? 0f : 0.05f);
		List<float> list10;
		List<float> list11 = (list10 = list);
		int index6 = (index = 0);
		num = list10[index];
		list11[index6] = num + ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("cape_EliteCrafter")) ? 0f : 0.1f);
		List<float> list12;
		List<float> list13 = (list12 = list);
		int index7 = (index = 0);
		num = list12[index];
		list13[index7] = num + ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("StormTrooperCape_Up1")) ? 0f : 0.15f);
		List<float> list14;
		List<float> list15 = (list14 = list);
		int index8 = (index = 0);
		num = list14[index];
		list15[index8] = num + ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("StormTrooperCape_Up2")) ? 0f : 0.2f);
		List<float> list16;
		List<float> list17 = (list16 = list);
		int index9 = (index = 1);
		num = list16[index];
		list17[index9] = num + ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("cape_Archimage")) ? 0f : 0.25f);
		List<float> list18;
		List<float> list19 = (list18 = list);
		int index10 = (index = 1);
		num = list18[index];
		list19[index10] = num + ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("HitmanCape_Up1")) ? 0f : 0.35f);
		List<float> list20;
		List<float> list21 = (list20 = list);
		int index11 = (index = 1);
		num = list20[index];
		list21[index11] = num + ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("HitmanCape_Up2")) ? 0f : 0.45f);
		List<float> list22;
		List<float> list23 = (list22 = list);
		int index12 = (index = 2);
		num = list22[index];
		list23[index12] = num + ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("cape_BloodyDemon")) ? 0f : 0.4f);
		List<float> list24;
		List<float> list25 = (list24 = list);
		int index13 = (index = 2);
		num = list24[index];
		list25[index13] = num + ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("BerserkCape_Up1")) ? 0f : 0.5f);
		List<float> list26;
		List<float> list27 = (list26 = list);
		int index14 = (index = 2);
		num = list26[index];
		list27[index14] = num + ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("BerserkCape_Up2")) ? 0f : 1f);
		return (i < 0 || i >= list.Count) ? 0f : list[i];
	}

	public static float SpeedModifier(int i)
	{
		float num = WeaponManager.sharedManager.currentWeaponSounds.speedModifier * ((!PotionsController.sharedController.PotionIsActive(PotionsController.HastePotion)) ? 1f : 1.25f) * ((!Storager.getString(Defs.HatEquppedSN, false).Equals("hat_KingsCrown")) ? 1f : 1.05f) * ((!Storager.getString(Defs.HatEquppedSN, false).Equals("hat_Samurai")) ? 1f : 1.05f) * ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("cape_Custom")) ? 1f : 1.05f) * SlowdownCoeff;
		if (i == 1 && Storager.getString(Defs.BootsEquppedSN, false).Equals("boots_red"))
		{
			num *= 1.1f;
		}
		if (i == 1 && Storager.getString(Defs.BootsEquppedSN, false).Equals("HitmanBoots_Up1"))
		{
			num *= 1.15f;
		}
		if (i == 1 && Storager.getString(Defs.BootsEquppedSN, false).Equals("HitmanBoots_Up2"))
		{
			num *= 1.2f;
		}
		if (i == 2 && Storager.getString(Defs.BootsEquppedSN, false).Equals("boots_black"))
		{
			num *= 1.2f;
		}
		if (i == 2 && Storager.getString(Defs.BootsEquppedSN, false).Equals("BerserkBoots_Up1"))
		{
			num *= 1.25f;
		}
		if (i == 2 && Storager.getString(Defs.BootsEquppedSN, false).Equals("BerserkBoots_Up2"))
		{
			num *= 1.3f;
		}
		if (i == 3 && Storager.getString(Defs.BootsEquppedSN, false).Equals("EngineerBoots"))
		{
			num *= 1.2f;
		}
		if (i == 3 && Storager.getString(Defs.BootsEquppedSN, false).Equals("EngineerBoots_Up1"))
		{
			num *= 1.25f;
		}
		if (i == 3 && Storager.getString(Defs.BootsEquppedSN, false).Equals("EngineerBoots_Up2"))
		{
			num *= 1.3f;
		}
		if (i == 4 && Storager.getString(Defs.BootsEquppedSN, false).Equals("boots_blue"))
		{
			num *= 1.2f;
		}
		if (i == 4 && Storager.getString(Defs.BootsEquppedSN, false).Equals("SniperBoots_Up1"))
		{
			num *= 1.25f;
		}
		if (i == 4 && Storager.getString(Defs.BootsEquppedSN, false).Equals("SniperBoots_Up2"))
		{
			num *= 1.3f;
		}
		if (i == 5 && Storager.getString(Defs.BootsEquppedSN, false).Equals("boots_green"))
		{
			num *= 1.2f;
		}
		if (i == 5 && Storager.getString(Defs.BootsEquppedSN, false).Equals("DemolitionBoots_Up1"))
		{
			num *= 1.25f;
		}
		if (i == 5 && Storager.getString(Defs.BootsEquppedSN, false).Equals("DemolitionBoots_Up2"))
		{
			num *= 1.3f;
		}
		if (i == 0 && Storager.getString("MaskEquippedSN", false).Equals("mask_trooper"))
		{
			num *= 1.2f;
		}
		if (i == 0 && Storager.getString("MaskEquippedSN", false).Equals("mask_trooper_up1"))
		{
			num *= 1.25f;
		}
		if (i == 0 && Storager.getString("MaskEquippedSN", false).Equals("mask_trooper_up2"))
		{
			num *= 1.3f;
		}
		return num;
	}

	public static float AddingForPotionDuration(string potion)
	{
		float num = 0f;
		if (potion == null)
		{
			return num;
		}
		if (potion.Equals("InvisibilityPotion") && Storager.getString(Defs.BootsEquppedSN, false).Equals("boots_blue"))
		{
			num += 5f;
		}
		if (potion.Equals("InvisibilityPotion") && Storager.getString(Defs.BootsEquppedSN, false).Equals("SniperBoots_Up1"))
		{
			num += 10f;
		}
		if (potion.Equals("InvisibilityPotion") && Storager.getString(Defs.BootsEquppedSN, false).Equals("SniperBoots_Up2"))
		{
			num += 15f;
		}
		if (!Defs.isDaterRegim)
		{
			num += ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("cape_Engineer")) ? 0f : 10f);
			num += ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("cape_Engineer_Up1")) ? 0f : 15f);
			num += ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("cape_Engineer_Up2")) ? 0f : 20f);
			num += ((!Storager.getString(Defs.BootsEquppedSN, false).Equals("EngineerBoots")) ? 0f : 10f);
			num += ((!Storager.getString(Defs.BootsEquppedSN, false).Equals("EngineerBoots_Up1")) ? 0f : 15f);
			num += ((!Storager.getString(Defs.BootsEquppedSN, false).Equals("EngineerBoots_Up2")) ? 0f : 20f);
		}
		return num;
	}

	public static float GetReloadAnimationSpeed(int categoryNabor, string currentCape, string currentMask)
	{
		float num = 1f;
		switch (categoryNabor)
		{
		case 1:
			num += ((!currentCape.Equals("cape_EliteCrafter")) ? 0f : 0.15f);
			num += ((!currentCape.Equals("StormTrooperCape_Up1")) ? 0f : 0.2f);
			num += ((!currentCape.Equals("StormTrooperCape_Up2")) ? 0f : 0.25f);
			break;
		case 2:
			num += ((!currentMask.Equals("mask_hitman_1")) ? 0f : 0.15f);
			num += ((!currentMask.Equals("mask_hitman_1_up1")) ? 0f : 0.2f);
			num += ((!currentMask.Equals("mask_hitman_1_up2")) ? 0f : 0.25f);
			break;
		case 4:
			num += ((!currentCape.Equals("cape_Engineer")) ? 0f : 0.2f);
			num += ((!currentCape.Equals("cape_Engineer_Up1")) ? 0f : 0.25f);
			num += ((!currentCape.Equals("cape_Engineer_Up2")) ? 0f : 0.3f);
			break;
		case 5:
			num += ((!currentCape.Equals("cape_SkeletonLord")) ? 0f : 0.25f);
			num += ((!currentCape.Equals("SniperCape_Up1")) ? 0f : 0.35f);
			num += ((!currentCape.Equals("SniperCape_Up2")) ? 0f : 0.5f);
			break;
		case 6:
			num += ((!currentCape.Equals("cape_RoyalKnight")) ? 0f : 0.25f);
			num += ((!currentCape.Equals("DemolitionCape_Up1")) ? 0f : 0.35f);
			num += ((!currentCape.Equals("DemolitionCape_Up2")) ? 0f : 0.5f);
			break;
		}
		return num;
	}

	public static float AddingForHeadshot(int cat)
	{
		List<float> list = new List<float>(6);
		for (int i = 0; i < 6; i++)
		{
			list.Add(0f);
		}
		List<float> list2;
		List<float> list3 = (list2 = list);
		int index;
		int index2 = (index = 4);
		float num = list2[index];
		list3[index2] = num + ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("cape_SkeletonLord")) ? 0f : 0.25f);
		List<float> list4;
		List<float> list5 = (list4 = list);
		int index3 = (index = 4);
		num = list4[index];
		list5[index3] = num + ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("SniperCape_Up1")) ? 0f : 0.35f);
		List<float> list6;
		List<float> list7 = (list6 = list);
		int index4 = (index = 4);
		num = list6[index];
		list7[index4] = num + ((!Storager.getString(Defs.CapeEquppedSN, false).Equals("SniperCape_Up2")) ? 0f : 0.5f);
		return (cat < 0 || cat >= list.Count) ? 0f : list[cat];
	}

	public static float GetChanceToIgnoreHeadshot(int categoryNabor, string currentCape, string currentMask)
	{
		float num = 0f;
		switch ((ShopNGUIController.CategoryNames)categoryNabor)
		{
		case ShopNGUIController.CategoryNames.SpecilCategory:
			if (currentCape.Equals("cape_BloodyDemon"))
			{
				num += 0.25f;
			}
			else if (currentCape.Equals("BerserkCape_Up1"))
			{
				num += 0.35f;
			}
			else if (currentCape.Equals("BerserkCape_Up2"))
			{
				num += 0.5f;
			}
			break;
		case ShopNGUIController.CategoryNames.PremiumCategory:
			if (currentMask.Equals("mask_sniper"))
			{
				num += 0.25f;
			}
			else if (currentMask.Equals("mask_sniper_up1"))
			{
				num += 0.35f;
			}
			else if (currentMask.Equals("mask_sniper_up2"))
			{
				num += 0.5f;
			}
			break;
		}
		return num;
	}
}
