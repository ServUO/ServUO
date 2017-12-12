using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Spells.Necromancy
{
    public class WraithFormSpell : TransformationSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Wraith Form", "Rel Xen Um",
            203,
            9031,
            Reagent.NoxCrystal,
            Reagent.PigIron);
        public WraithFormSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(2.0);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 20.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 17;
            }
        }
        public override int Body
        {
            get
            {
                return this.Caster.Female ? 747 : 748;
            }
        }
        public override int Hue
        {
            get
            {
                return this.Caster.Female ? 0 : 0x4001;
            }
        }
        public override int PhysResistOffset
        {
            get
            {
                return +15;
            }
        }
        public override int FireResistOffset
        {
            get
            {
                return -5;
            }
        }
        public override int ColdResistOffset
        {
            get
            {
                return 0;
            }
        }
        public override int PoisResistOffset
        {
            get
            {
                return 0;
            }
        }
        public override int NrgyResistOffset
        {
            get
            {
                return -5;
            }
        }
        public override void DoEffect(Mobile m)
        {
            if (m is PlayerMobile)
                ((PlayerMobile)m).IgnoreMobiles = true;

            m.PlaySound(0x17F);
            m.FixedParticles(0x374A, 1, 15, 9902, 1108, 4, EffectLayer.Waist);

            int manadrain = (int)(5 + ((15 * m.Skills.SpiritSpeak.Value) / 100));

            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.WraithForm, 1060524, 1153829, String.Format("15\t5\t5\t{0}", manadrain)));
        }

        public override void RemoveEffect(Mobile m)
        {
            if (m is PlayerMobile && m.IsPlayer())
                ((PlayerMobile)m).IgnoreMobiles = false;

            BuffInfo.RemoveBuff(m, BuffIcon.WraithForm);
        }
    }
}
