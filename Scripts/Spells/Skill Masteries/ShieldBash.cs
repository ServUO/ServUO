using Server.Items;
using Server.Mobiles;
using Server.Network;
using System;

/*When activated the shield user will execute a shield bash on successfully hitting or parrying their opponent 
  causing physical damage and paralyzing their opponent based on parry skill, best weapon skill, and mastery level.*/

namespace Server.Spells.SkillMasteries
{
    public class ShieldBashSpell : SkillMasterySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
                "Shield Bash", "",
                -1,
                9002
            );

        public override int RequiredMana => 40;
        public override bool BlocksMovement => false;
        public override bool CancelsWeaponAbility => true;
        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(1.0);

        public override SkillName CastSkill => SkillName.Parry;

        public override int ExpireMessage => 1063119; // You return to your normal stance.

        public ShieldBashSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!HasShield())
                return false;

            if (HasSpell(Caster, GetType()))
                return false;

            return base.CheckCast();
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                Effects.SendPacket(Caster, Caster.Map, new HuedEffect(EffectType.FixedFrom, Caster.Serial, Serial.Zero, 0x37C4, Caster, Caster, 10, 7, false, false, 4, 0, 3));

                Server.Timer.DelayCall(TimeSpan.FromMilliseconds(250), () =>
                {
                    Effects.SendPacket(Caster.Location, Caster.Map, new ParticleEffect(EffectType.FixedFrom, Caster.Serial, Serial.Zero, 0x375A, Caster.Location, Caster.Location, 1, 17, false, false, 45, 0, 3, 9502, 1, Caster.Serial, 209, 0));
                });

                Caster.PlaySound(0x51A);

                TimeSpan duration = TimeSpan.FromSeconds(3);

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

            bool pvp = Caster is PlayerMobile && defender is PlayerMobile;

            int dmg = GetDamage(pvp, GetMasteryLevel());

            if (pvp)
            {
                AOS.Damage(defender, Caster, dmg, 0, 0, 0, 0, 0, 0, 100);

                damage /= 10;
            }
            else
            {
                damage = dmg;
            }

            Server.Timer.DelayCall(TimeSpan.FromMilliseconds(100), () =>
                {
                    if (defender.Alive)
                    {
                        CheckParalyze(defender, TimeSpan.FromSeconds(3));
                    }
                });

            Expire();
        }

        private int GetDamage(bool pvp, int level)
        {
            int damage;

            if (pvp)
            {
                damage = Math.Min(35, Utility.RandomMinMax(27, 35) * level);
            }
            else
            {
                damage = Utility.RandomMinMax(45, 65) * level;
            }

            return damage;
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
