using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;
using Server.Items;

/*When activated the shield user will execute a shield bash on successfully hitting or parrying their opponent 
  causing physical damage and paralyzing their opponent based on parry skill, best weapon skill, and mastery level.*/

namespace Server.Spells.SkillMasteries
{
    public class ShieldBashSpell : SkillMasterySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Shield Bash", "",
                -1,
                9002
            );

        public override int RequiredMana { get { return 40; } }
        public override bool BlocksMovement { get { return false; } }
        public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds(1.0); } }

        public override SkillName CastSkill { get { return SkillName.Parry; } }

        public ShieldBashSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!HasShield())
                return false;

            if (HasSpell(Caster, this.GetType()))
                return false;

            return base.CheckCast();
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                Caster.FixedParticles(0x376A, 9, 32, 5030, 1168, 0, EffectLayer.Waist, 0);
                Caster.PlaySound(0x51A);

                TimeSpan duration = TimeSpan.FromSeconds(5);

                Caster.SendLocalizedMessage(1156022); // You ready your shield.

                Expires = DateTime.UtcNow + duration;
                BeginTimer();

                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.ShieldBash, 1155923, 1156093, duration, Caster));
                //Places you in an offensive stance which allows you to strike your target with your shield on your next successful attack or parry.
            }

            FinishSequence();
        }

        public override bool OnTick()
        {
            if (!HasShield())
            {
                Expire();
                return false;
            }

            return base.OnTick();
        }

        private bool HasShield()
        {
            if (!Caster.Player)
                return true;

            BaseShield shield = Caster.FindItemOnLayer(Layer.TwoHanded) as BaseShield;

            if (shield == null)
            {
                Caster.SendLocalizedMessage(1156096); // You must be wielding a shield to use this ability!
                return false;
            }

            return true;
        }

        public override void EndEffects()
        {
            BuffInfo.RemoveBuff(Caster, BuffIcon.ShieldBash);
        }

        public override void OnHit(Mobile defender, ref int damage)
        {
            if (!HasShield())
            {
                Expire();
                return;
            }

            Caster.SendLocalizedMessage(1156027); // You bash you target with your shield!
            bool pvp = defender is PlayerMobile;

            damage = (pvp ? damage * 2 : damage * 5) + Utility.RandomMinMax(-4, 4);

            if (pvp && damage > 35)
                damage = 35;

            Server.Timer.DelayCall(TimeSpan.FromMilliseconds(100), () =>
                {
                    CheckParalyze(defender, TimeSpan.FromSeconds(pvp ? 3 : 6));
                });

            Expire();
        }

        private void CheckParalyze(Mobile defender, TimeSpan duration)
        {
            if (ParalyzingBlow.IsImmune(defender))
            {
                Caster.SendLocalizedMessage(1070804); // Your target resists paralysis.
                defender.SendLocalizedMessage(1070813); // You resist paralysis.
                return;
            }

            defender.FixedEffect(0x376A, 9, 32);
            defender.PlaySound(0x204);

            Caster.SendLocalizedMessage(1060163); // You deliver a paralyzing blow!
            defender.SendLocalizedMessage(1060164); // The attack has temporarily paralyzed you!

            // Treat it as paralyze not as freeze, effect must be removed when damaged.
            defender.Paralyze(duration);

            ParalyzingBlow.BeginImmunity(defender, duration);
        }

        public override void OnParried(Mobile attacker)
        {
            BaseWeapon wep = Caster.Weapon as BaseWeapon;

            if (wep != null)
                wep.OnHit(Caster, attacker, 1.0);
        }
    }
}