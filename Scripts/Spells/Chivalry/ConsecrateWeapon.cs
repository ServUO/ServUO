using System;
using System.Collections;
using Server.Items;

namespace Server.Spells.Chivalry
{
    /// <summary>
    /// * 020416 Crome696 - Proc Damage based on http://www.uoguide.com/Consecrate_Weapon
    /// </summary>
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

                Timer t = (Timer)m_Table[weapon];

                if (t != null)
                {
                    BuffInfo.RemoveBuff(Caster, BuffIcon.ConsecrateWeapon);
                    t.Stop();
                }


                weapon.ConsecrateDamageBonus = 0;
                weapon.ConsecrateProcChance = 0;

                if (Caster.Skills.Chivalry.Value >= 90)
                {
                    double calc = Caster.Skills.Chivalry.Value - 90;
                    weapon.ConsecrateDamageBonus = ((int)Math.Truncate(calc / 2));
                }

                if (Caster.Skills.Chivalry.Value >= 80)
                    weapon.ConsecrateProcChance = 100;
                else
                    weapon.ConsecrateProcChance = Caster.Skills.Chivalry.Value;

                weapon.Consecrated = true;

                m_Table[weapon] = t = new ExpireTimer(weapon, duration);
                
                
                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.ConsecrateWeapon, 1151385, 1151386, duration, Caster, String.Format("{0}\t{1}", weapon.ConsecrateProcChance, weapon.ConsecrateDamageBonus)));

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