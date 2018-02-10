using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using System.Collections.Generic;

namespace Server.Spells.SkillMasteries
{
    public class InjectedStrikeSpell : SkillMasterySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Injected Strike", "",
                -1,
                9002
            );
 
        public override int RequiredMana { get { return 30; } }
		
        public override SkillName CastSkill { get { return SkillName.Poisoning; } }
		public override SkillName DamageSkill { get { return SkillName.Anatomy; } }

        public override bool ClearOnSpecialAbility { get { return true; } }
        public override bool CancelsWeaponAbility { get { return true; } }

        public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds(1.0); } }

        public override void GetCastSkills(out double min, out double max)
        {
            min = RequiredSkill;
            max = RequiredSkill + 10.0;
        }

        public InjectedStrikeSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override void SendCastEffect()
        {
            Caster.FixedParticles(0x3728, 0xA, 0x7, 0x13CB, 0x66C, 3, (EffectLayer)2, 0);
        }

        public override void OnCast()
        {
            BaseWeapon weapon = GetWeapon();

			if(CheckWeapon())
			{
                if (weapon.Poison == null || weapon.PoisonCharges == 0)
                {
                    var poison = GetLastPotion(Caster);

                    Caster.SendLocalizedMessage(502137); // Select the poison you wish to use.
                    Caster.Target = new MasteryTarget(this, autoEnd: false);

                    return;
                }
                else if (!HasSpell(Caster, this.GetType()))
                {
                    if (CheckSequence())
                    {
                        BeginTimer();
                        Caster.SendLocalizedMessage(1156138); // You ready your weapon to unleash an injected strike!

                        int bonus = 30;

                        // Your next successful attack will poison your target and reduce its poison resist by:<br>~1_VAL~% PvM<br>~2_VAL~% PvP
                        BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.InjectedStrike, 1155927, 1156163, String.Format("{0}\t{1}", bonus.ToString(), (bonus / 2).ToString())));
                        Caster.FixedParticles(0x3728, 0x1, 0xA, 0x251E, 0x4F7, 7, (EffectLayer)2, 0);

                        weapon.InvalidateProperties();
                    }
                }
                else
                    Caster.SendLocalizedMessage(501775); // This spell is already in effect.
			}
			else
				Caster.SendLocalizedMessage(1060179); //You must be wielding a weapon to use this ability!
			
			FinishSequence();
        }

        protected override void OnTarget(object o)
        {
            BaseWeapon weapon = GetWeapon();
            BasePoisonPotion potion = GetLastPotion(Caster);

            if (o is BasePoisonPotion)
            {
                potion = o as BasePoisonPotion;

                if (!potion.IsChildOf(Caster.Backpack))
                {
                    Caster.SendLocalizedMessage(1080058); // This must be in your backpack to use it.
                }
                else if (CheckSequence())
                {
                    if (Caster.CheckTargetSkill(CastSkill, potion, potion.MinPoisoningSkill, potion.MaxPoisoningSkill))
                    {
                        ApplyPoison(weapon, potion);
                        return;
                    }
                    else
                    {
                        Caster.SendLocalizedMessage(1010518); // You fail to apply a sufficient dose of poison
                    }
                }
            }
            else if (o is BaseWeapon && weapon != null && (BaseWeapon)o == weapon && potion != null)
            {
                if (CheckSequence())
                {
                    if (potion != null)
                    {
                        if (Caster.CheckTargetSkill(CastSkill, potion, potion.MinPoisoningSkill, potion.MaxPoisoningSkill))
                        {
                            ApplyPoison(weapon, potion);
                            return;
                        }
                        else
                        {
                            Caster.SendLocalizedMessage(1010518); // You fail to apply a sufficient dose of poison
                        }
                    }
                }
            }
            else if (potion == null)
                Caster.SendLocalizedMessage(502143); // The poison vial not usable.
            else
                Caster.SendLocalizedMessage(1060179); //You must be wielding a weapon to use this ability!

        }

        private void ApplyPoison(BaseWeapon weapon, BasePoisonPotion potion)
        {
            if (potion == null || !Caster.InRange(potion.GetWorldLocation(), 2) || !Caster.InLOS(potion))
            {
                Caster.SendLocalizedMessage(502138); // That is too far away for you to use.
                return;
            }

            weapon.Poison = potion.Poison;
            weapon.PoisonCharges = 18 - (potion.Poison.RealLevel * 2);

            Caster.SendLocalizedMessage(1010517); // You apply the poison
            Caster.PlaySound(0x246);

            potion.Consume();
            Caster.Backpack.DropItem(new Bottle());

            OnCast();

            if (potion.Deleted)
            {
                if (_LastPotion != null && _LastPotion.ContainsKey(Caster) && _LastPotion[Caster] == potion)
                {
                    _LastPotion.Remove(Caster);

                    if (_LastPotion.Count == 0)
                        _LastPotion = null;
                }
            }
            else
            {
                if (_LastPotion == null)
                    _LastPotion = new Dictionary<Mobile, BasePoisonPotion>();

                if (!_LastPotion.ContainsKey(Caster) || _LastPotion[Caster] != potion)
                    _LastPotion[Caster] = potion;
            }
        }

        public override void EndEffects()
        {
            BuffInfo.RemoveBuff(Caster, BuffIcon.InjectedStrike);
        }

        public override void OnHit(Mobile defender, ref int damage)
        {
            BaseWeapon weapon = GetWeapon();

            if (!CheckWeapon())
                return;

            Poison p = weapon.Poison;

            if (p == null || weapon.PoisonCharges <= 0)
            {
                Caster.SendLocalizedMessage(1061141); // Your weapon must have a dose of poison to perform an infectious strike!
                return;
            }

            // Skill Masteries
            int noChargeChance = MasteryInfo.NonPoisonConsumeChance(Caster);

            if (noChargeChance == 0 || noChargeChance < Utility.Random(100))
                --weapon.PoisonCharges;
            else
                Caster.SendLocalizedMessage(1156095); // Your mastery of poisoning allows you to use your poison charge without consuming it.

            int maxLevel = Caster.Skills[SkillName.Poisoning].Fixed / 200;
            if (maxLevel < 0) maxLevel = 0;

            #region Mondain's Legacy
            if (p == Poison.DarkGlow)
                p = Poison.GetPoison(10 + Math.Min(maxLevel, 2));
            else if (p == Poison.Parasitic)
                p = Poison.GetPoison(14 + Math.Min(maxLevel, 3));
            else if (p.Level > maxLevel)
                p = Poison.GetPoison(maxLevel);
            #endregion

            if ((Caster.Skills[SkillName.Poisoning].Value / 100.0) > Utility.RandomDouble() && p.Level < 3)
            {
                int level = p.Level + 1;
                Poison newPoison = Poison.GetPoison(level);

                if (newPoison != null)
                {
                    p = newPoison;

                    Caster.SendLocalizedMessage(1060080); // Your precise strike has increased the level of the poison by 1
                    defender.SendLocalizedMessage(1060081); // The poison seems extra effective!
                }
            }

            defender.PlaySound(0xDD);
            defender.FixedParticles(0x3728, 244, 25, 9941, 1266, 0, EffectLayer.Waist);

            if (defender.ApplyPoison(Caster, p) != ApplyPoisonResult.Immune)
            {
                Caster.SendLocalizedMessage(1008096, true, defender.Name); // You have poisoned your target : 
                defender.SendLocalizedMessage(1008097, false, Caster.Name); //  : poisoned you!
            }

            int malus = 30;

            if (defender is PlayerMobile)
                malus /= 2;

            if (weapon is BaseRanged)
                malus /= 2;

            ResistanceMod mod = new ResistanceMod(ResistanceType.Poison, -malus);
            defender.AddResistanceMod(mod);

            // ~2_NAME~ reduces your poison resistance by ~1_VAL~.
            BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.InjectedStrikeDebuff, 1155927, 1156133, TimeSpan.FromSeconds(7), defender, String.Format("{0}\t{1}", malus, Caster.Name)));

            Server.Timer.DelayCall(TimeSpan.FromSeconds(7), () =>
                {
                    defender.RemoveResistanceMod(mod);
                });

            Expire();
        }

        public override void OnWeaponRemoved(BaseWeapon weapon)
        {
            Expire();
        }

        private static Dictionary<Mobile, BasePoisonPotion> _LastPotion;

        public static BasePoisonPotion GetLastPotion(Mobile m)
        {
            if (_LastPotion == null || !_LastPotion.ContainsKey(m))
                return null;

            if (_LastPotion[m] == null || _LastPotion[m].Deleted)
                return null;

            return _LastPotion[m];
        }
    }
}