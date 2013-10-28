using System;
using System.Collections;
using Server.Items;

namespace Server.Spells.Chivalry
{
    public class ConsecrateWeaponSpell : PaladinSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Consecrate Weapon", "Consecrus Arma",
            -1,
            9002);
        private static readonly Hashtable m_Table = new Hashtable();
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

            if (weapon == null || weapon is Fists)
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
                if (seconds < 3.0)
                    seconds = 3.0;
                else if (seconds > 11.0)
                    seconds = 11.0;

                TimeSpan duration = TimeSpan.FromSeconds(seconds);

                Timer t = (Timer)m_Table[weapon];

                if (t != null)
                    t.Stop();

                weapon.Consecrated = true;

                m_Table[weapon] = t = new ExpireTimer(weapon, duration);

                t.Start();
            }

            this.FinishSequence();
        }

        private class ExpireTimer : Timer
        {
            private readonly BaseWeapon m_Weapon;
            public ExpireTimer(BaseWeapon weapon, TimeSpan delay)
                : base(delay)
            {
                this.m_Weapon = weapon;
                this.Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                this.m_Weapon.Consecrated = false;
                Effects.PlaySound(this.m_Weapon.GetWorldLocation(), this.m_Weapon.Map, 0x1F8);
                m_Table.Remove(this);
            }
        }
    }
}