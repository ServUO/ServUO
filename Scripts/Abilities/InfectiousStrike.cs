using System;

namespace Server.Items
{
    /// <summary>
    /// This special move represents a significant change to the use of poisons in Age of Shadows.
    /// Now, only certain weapon types — those that have Infectious Strike as an available special move — will be able to be poisoned.
    /// Targets will no longer be poisoned at random when hit by poisoned weapons.
    /// Instead, the wielder must use this ability to deliver the venom.
    /// While no skill in Poisoning is directly required to use this ability, being knowledgeable in the application and use of toxins
    /// will allow a character to use Infectious Strike at reduced mana cost and with a chance to inflict more deadly poison on his victim.
    /// With this change, weapons will no longer be corroded by poison.
    /// Level 5 poison will be possible when using this special move.
    /// </summary>
    public class InfectiousStrike : WeaponAbility
    {
        public InfectiousStrike()
        {
        }

        public override int BaseMana
        {
            get
            {
                return 20;
            }
        }
        
        public override bool RequiresSecondarySkill(Mobile from)
        {
            return false;
        }
        
        public override SkillName GetSecondarySkill(Mobile from)
        {
            return SkillName.Poisoning;
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!this.Validate(attacker))
                return;

            ClearCurrentAbility(attacker);

            BaseWeapon weapon = attacker.Weapon as BaseWeapon;

            if (weapon == null)
                return;

            Poison p = weapon.Poison;

            if (p == null || weapon.PoisonCharges <= 0)
            {
                attacker.SendLocalizedMessage(1061141); // Your weapon must have a dose of poison to perform an infectious strike!
                return;
            }

            if (!this.CheckMana(attacker, true))
                return;

            // Skill Masteries
            int noChargeChance = Server.Spells.SkillMasteries.MasteryInfo.NonPoisonConsumeChance(attacker);

            if (noChargeChance == 0 || noChargeChance < Utility.Random(100))
                --weapon.PoisonCharges;
            else
                attacker.SendLocalizedMessage(1156095); // Your mastery of poisoning allows you to use your poison charge without consuming it.

            // Infectious strike special move now uses poisoning skill to help determine potency 
            int maxLevel = 0;
            if (p == Poison.DarkGlow)
            {
            	maxLevel = 10 + (attacker.Skills[SkillName.Poisoning].Fixed / 333);
            	if (maxLevel > 13)
            		maxLevel = 13;
            }
            else if (p == Poison.Parasitic)
            {
            	maxLevel = 14 + (attacker.Skills[SkillName.Poisoning].Fixed / 250);
            	if (maxLevel > 18)
            		maxLevel = 18;
            }
			else            
			{
				maxLevel = attacker.Skills[SkillName.Poisoning].Fixed / 200;
				if (maxLevel > 5)
					maxLevel = 5;
			}
			
            if (maxLevel < 0)
                maxLevel = 0;
            if (p.Level > maxLevel) // If they don't have enough Poisoning Skill for the potion strength, lower it.
                p = Poison.GetPoison(maxLevel);

            if ((attacker.Skills[SkillName.Poisoning].Value / 100.0) > Utility.RandomDouble())
            {
            	if (p !=null && p.Level + 1 <= maxLevel)
            	{
            		int level = p.Level + 1;
                	Poison newPoison = Poison.GetPoison(level);
           	
	                if (newPoison != null)
	                {
                 	   p = newPoison;

 	                   attacker.SendLocalizedMessage(1060080); // Your precise strike has increased the level of the poison by 1
 	                   defender.SendLocalizedMessage(1060081); // The poison seems extra effective!
	                }
            	}
            }

            defender.PlaySound(0xDD);
            defender.FixedParticles(0x3728, 244, 25, 9941, 1266, 0, EffectLayer.Waist);

            if (defender.ApplyPoison(attacker, p) != ApplyPoisonResult.Immune)
            {
                attacker.SendLocalizedMessage(1008096, true, defender.Name); // You have poisoned your target : 
                defender.SendLocalizedMessage(1008097, false, attacker.Name); //  : poisoned you!
            }
        }
    }
}
