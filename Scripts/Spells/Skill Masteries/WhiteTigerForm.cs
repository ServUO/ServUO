using System;
using System.Collections.Generic;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;
using Server.Spells.Ninjitsu;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Fifth;
using Server.Items;

namespace Server.Spells.SkillMasteries
{
    public class WhiteTigerFormSpell : SkillMasterySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "White Tiger Form", "",
                -1,
                9002
            );

        public override int RequiredMana { get { return 10; } }

        public override SkillName CastSkill { get { return SkillName.Ninjitsu; } }
        public override SkillName DamageSkill { get { return SkillName.Stealth; } }

        public override bool BlockedByAnimalForm { get { return false; } }
        public override bool BlocksMovement { get { return false; } }
        public override int CastRecoveryBase { get { return (Core.ML ? 10 : base.CastRecoveryBase); } }

        public WhiteTigerFormSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!Caster.CanBeginAction(typeof(PolymorphSpell)))
            {
                Caster.SendLocalizedMessage(1061628); // You can't do that while polymorphed.
                return false;
            }
            else if (TransformationSpellHelper.UnderTransformation(Caster))
            {
                Caster.SendLocalizedMessage(1063219); // You cannot mimic an animal while in that form.
                return false;
            }
            else if (DisguiseTimers.IsDisguised(Caster))
            {
                Caster.SendLocalizedMessage(1061631); // You can't do that while disguised.
                return false;
            }

            return base.CheckCast();
        }

        public override bool CheckFizzle()
        {
            // Spell is initially always successful, and with no skill gain.
            return true;
        }

        public override void OnCast()
        {
            if (!Caster.CanBeginAction(typeof(PolymorphSpell)))
			{
				Caster.SendLocalizedMessage(1061628); // You can't do that while polymorphed.
			}
			else if (TransformationSpellHelper.UnderTransformation(Caster))
			{
				Caster.SendLocalizedMessage(1063219); // You cannot mimic an animal while in that form.
			}
			else if (!Caster.CanBeginAction(typeof(IncognitoSpell)) || (Caster.IsBodyMod && AnimalForm.GetContext(Caster) == null))
			{
				DoFizzle();
			}
            else if (CheckSequence())
            {
                AnimalFormContext context = AnimalForm.GetContext(Caster);
                int mana = ScaleMana(RequiredMana);

                if (mana > Caster.Mana)
                {
                    Caster.SendLocalizedMessage(1060174, mana.ToString()); // You must have at least ~1_MANA_REQUIREMENT~ Mana to use this ability.
                }
                else if (context != null)
                {
                    AnimalForm.RemoveContext(Caster, context, true);
                    Caster.Mana -= mana;

                    BuffInfo.RemoveBuff(Caster, BuffIcon.WhiteTigerForm);
                    return;
                }
                else
                {
                    double ninjitsu = Caster.Skills.Ninjitsu.Value;

                    if (ninjitsu < RequiredSkill + 37.5)
                    {
                        double chance = (ninjitsu - RequiredSkill) / 37.5;

                        if (chance < Utility.RandomDouble())
                        {
                            DoFizzle();
                            return;
                        }
                    }
                }

                Caster.FixedParticles(0x3728, 10, 13, 2023, EffectLayer.Waist);
                Caster.Mana -= mana;

                Caster.CheckSkill(SkillName.Ninjitsu, 0.0, 90.0);

                BaseMount.Dismount(Caster);

                int bodyMod = Caster.Female ? 1255 : 1254;
                int hueMod = 2500;

                Caster.BodyMod = bodyMod;
                Caster.HueMod = hueMod;

                Caster.Send(SpeedControl.MountSpeed);

                Timer timer = new AnimalFormTimer(Caster, bodyMod, hueMod);
                timer.Start();

                int skills = (int)((Caster.Skills[CastSkill].Value + Caster.Skills[DamageSkill].Value + (GetMasteryLevel() * 40)) / 3);

                AnimalForm.AddContext(Caster, new AnimalFormContext(timer, null, true, typeof(WildWhiteTiger), null));
                Caster.CheckStatTimers();

                int bleedMod = (int)(((Caster.Skills[SkillName.Ninjitsu].Value + Caster.Skills[SkillName.Stealth].Value + (GetMasteryLevel() * 40)) / 3) / 10);
                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.WhiteTigerForm, 1155911, 1156060, String.Format("{0}\t{1}\t{2}\t{3}", "20", "5", "", bleedMod.ToString()))); 
                // +~1_ARG~ Defense Chance Increase.<br>+~2_ARG~ Max Defense Chance Increase Cap.<br> Chance to evade attacks.<br>Applies bleed to victim with a max damage of ~4_ARG~.

                Caster.Delta(MobileDelta.WeaponDamage);
            }

            FinishSequence();
        }

        public static bool CheckEvasion(Mobile m)
        {
            if (IsActive(m))
                return MasteryInfo.GetMasteryLevel(m, SkillName.Ninjitsu) + 2 > Utility.Random(100);

            return false;
        }

        public static int GetDamageBonus(Mobile m)
        {
            AnimalFormContext context = AnimalForm.GetContext(m);

            if (context != null && context.Type == typeof(WildWhiteTiger))
                return (int)(((m.Skills[SkillName.Ninjitsu].Value + m.Skills[SkillName.Stealth].Value + (MasteryInfo.GetMasteryLevel(m, SkillName.Ninjitsu) * 40)) / 3) / 10) / 2;

            return 0;
        }

        public static bool IsActive(Mobile m)
        {
            AnimalFormContext context = AnimalForm.GetContext(m);

            return context != null && context.Type == typeof(WildWhiteTiger);
        }

        public static int GetDefenseCap(Mobile m)
        {
            AnimalFormContext context = AnimalForm.GetContext(m);

            if (context != null && context.Type == typeof(WildWhiteTiger))
                return 5;

            return 0;
        }

        public static void OnHit(Mobile attacker, Mobile defender)
        {
            CheckTable();

            int damage;
            if (!HasBleedMod(attacker, out damage) || (_Table != null && _Table.ContainsKey(attacker)))
                return;

            double bleedchance = (double)((attacker.Skills.Ninjitsu.Value + attacker.Skills.Stealth.Value + (MasteryInfo.GetMasteryLevel(attacker, SkillName.Ninjitsu) * 40)) / 3.0) / 15.0;

            if (bleedchance > Utility.RandomDouble())
            {
                BleedAttack.BeginBleed(defender, attacker, false);

                if (_Table == null)
                    _Table = new Dictionary<Mobile, DateTime>();

                _Table[attacker] = DateTime.UtcNow + TimeSpan.FromMinutes(1);
            }
        }

        public static bool HasBleedMod(Mobile m, out int damage)
        {
            CheckTable();

            damage = GetDamageBonus(m);

            return damage > 0;
        }

        private static Dictionary<Mobile, DateTime> _Table;

        private static void CheckTable()
        {
            if (_Table == null)
                return;

            ColUtility.ForEach(_Table, (mob, expires) =>
                {
                    if (expires < DateTime.UtcNow)
                        _Table.Remove(mob);
                });
        }
    }
}