using System;
using System.Collections;

namespace Server.Spells.Chivalry
{
    public class DivineFurySpell : PaladinSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Divine Fury", "Divinum Furis",
            -1,
            9002);
        private static readonly Hashtable m_Table = new Hashtable();
        public DivineFurySpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(1.0);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 25.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 15;
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
                return 1060722;
            }
        }// Divinum Furis
        public override bool BlocksMovement
        {
            get
            {
                return false;
            }
        }
        public static bool UnderEffect(Mobile m)
        {
            return m_Table.Contains(m);
        }

        public override void OnCast()
        {
            if (this.CheckSequence())
            {
                this.Caster.PlaySound(0x20F);
                this.Caster.PlaySound(this.Caster.Female ? 0x338 : 0x44A);
                this.Caster.FixedParticles(0x376A, 1, 31, 9961, 1160, 0, EffectLayer.Waist);
                this.Caster.FixedParticles(0x37C4, 1, 31, 9502, 43, 2, EffectLayer.Waist);

                this.Caster.Stam = this.Caster.StamMax;

                Timer t = (Timer)m_Table[this.Caster];

                if (t != null)
                    t.Stop();

                int delay = this.ComputePowerValue(10);

                // TODO: Should caps be applied?
                if (delay < 7)
                    delay = 7;
                else if (delay > 24)
                    delay = 24;

                m_Table[this.Caster] = t = Timer.DelayCall(TimeSpan.FromSeconds(delay), new TimerStateCallback(Expire_Callback), this.Caster);
                this.Caster.Delta(MobileDelta.WeaponDamage);

                BuffInfo.AddBuff(this.Caster, new BuffInfo(BuffIcon.DivineFury, 1060589, 1075634, TimeSpan.FromSeconds(delay), this.Caster));
            }

            this.FinishSequence();
        }

        private static void Expire_Callback(object state)
        {
            Mobile m = (Mobile)state;

            m_Table.Remove(m);

            m.Delta(MobileDelta.WeaponDamage);
            m.PlaySound(0xF8);
        }
    }
}