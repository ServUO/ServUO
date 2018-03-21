using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Spells.Chivalry
{
    public class ConsecrateWeaponSpell : PaladinSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Consecrate Weapon", "Consecrus Arma",
            -1,
            9002);

        private static Dictionary<Mobile, ConsecratedWeaponContext> m_Table = new Dictionary<Mobile, ConsecratedWeaponContext>();

        public ConsecrateWeaponSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(0.5);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 15.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 10;
            }
        }
        public override int RequiredTithing
        {
            get
            {
                return 10;
            }
        }
        public override int MantraNumber
        {
            get
            {
                return 1060720;
            }
        }// Consecrus Arma
        public override bool BlocksMovement
        {
            get
            {
                return false;
            }
        }
        public override void OnCast()
        {
            BaseWeapon weapon = this.Caster.Weapon as BaseWeapon;

            if (Caster.Player && (weapon == null || weapon is Fists))
            {
                this.Caster.SendLocalizedMessage(501078); // You must be holding a weapon.
            }
            else if (this.CheckSequence())
            {
                /* Temporarily enchants the weapon the caster is currently wielding.
                * The type of damage the weapon inflicts when hitting a target will
                * be converted to the target's worst Resistance type.
                * Duration of the effect is affected by the caster's Karma and lasts for 3 to 11 seconds.
                */
                int itemID, soundID;

                switch ( weapon.Skill )
                {
                    case SkillName.Macing:
                        itemID = 0xFB4;
                        soundID = 0x232;
                        break;
                    case SkillName.Archery:
                        itemID = 0x13B1;
                        soundID = 0x145;
                        break;
                    default:
                        itemID = 0xF5F;
                        soundID = 0x56;
                        break;
                }

                this.Caster.PlaySound(0x20C);
                this.Caster.PlaySound(soundID);
                this.Caster.FixedParticles(0x3779, 1, 30, 9964, 3, 3, EffectLayer.Waist);

                IEntity from = new Entity(Serial.Zero, new Point3D(this.Caster.X, this.Caster.Y, this.Caster.Z), this.Caster.Map);
                IEntity to = new Entity(Serial.Zero, new Point3D(this.Caster.X, this.Caster.Y, this.Caster.Z + 50), this.Caster.Map);
                Effects.SendMovingParticles(from, to, itemID, 1, 0, false, false, 33, 3, 9501, 1, 0, EffectLayer.Head, 0x100);

                double seconds = this.ComputePowerValue(20);

                // TODO: Should caps be applied?

                int pkarma = this.Caster.Karma;

                if (pkarma > 5000)
                    seconds = 11.0;
                else if (pkarma >= 4999)
                    seconds = 10.0;
                else if (pkarma >= 3999)
                    seconds = 9.00;
                else if (pkarma >= 2999)
                    seconds = 8.0;
                else if (pkarma >= 1999)
                    seconds = 7.0;
                else if (pkarma >= 999)
                    seconds = 6.0;
                else
                    seconds = 5.0;

                TimeSpan duration = TimeSpan.FromSeconds(seconds);
                ConsecratedWeaponContext context;

                if (IsUnderEffects(Caster))
                {
                    context = m_Table[Caster];

                    if (context.Timer != null)
                    {
                        context.Timer.Stop();
                        context.Timer = null;
                    }

                    context.Weapon = weapon;
                }
                else
                {
                    context = new ConsecratedWeaponContext(Caster, weapon);
                }

                weapon.ConsecratedContext = context;
                context.Timer = Timer.DelayCall<Mobile>(duration, RemoveEffects, Caster);

                m_Table[Caster] = context;

                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.ConsecrateWeapon, 1151385, 1151386, duration, Caster, String.Format("{0}\t{1}", context.ConsecrateProcChance, context.ConsecrateDamageBonus)));
            }

            this.FinishSequence();
        }

        public static bool IsUnderEffects(Mobile m)
        {
            return m_Table.ContainsKey(m);
        }

        public static void RemoveEffects(Mobile m)
        {
            if (m_Table.ContainsKey(m))
            {
                var context = m_Table[m];

                context.Expire();

                m_Table.Remove(m);
            }
        }
    }

    public class ConsecratedWeaponContext
    {
        public Mobile Owner { get; private set; }
        public BaseWeapon Weapon { get; set; }

        public Timer Timer { get; set; }

        public int ConsecrateProcChance
        {
            get
            {
                if (!Core.SA || Owner.Skills.Chivalry.Value >= 80)
                {
                    return 100;
                }

                return (int)Owner.Skills.Chivalry.Value;
            }
        }

        public int ConsecrateDamageBonus
        {
            get
            {
                if (Core.SA)
                {
                    double value = Owner.Skills.Chivalry.Value;

                    if (value >= 90)
                    {
                        return (int)Math.Truncate((value - 90) / 2);
                    }
                }

                return 0;
            }
        }

        public ConsecratedWeaponContext(Mobile owner, BaseWeapon weapon)
        {
            Owner = owner;
            Weapon = weapon;
        }

        public void Expire()
        {
            Weapon.ConsecratedContext = null;

            Effects.PlaySound(Weapon.GetWorldLocation(), Weapon.Map, 0x1F8);

            if (Timer != null)
            {
                Timer.Stop();
                Timer = null;
            }
        }
    }
}