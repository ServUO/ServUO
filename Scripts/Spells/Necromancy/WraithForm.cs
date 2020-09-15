using Server.Mobiles;
using System;

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

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(2.25);
        public override double RequiredSkill => 20.0;
        public override int RequiredMana => 17;
        public override int Body => Caster.Female ? 747 : 748;
        public override int Hue => Caster.Female ? 0 : 0x4001;
        public override int PhysResistOffset => +15;
        public override int FireResistOffset => -5;
        public override int ColdResistOffset => 0;
        public override int PoisResistOffset => 0;
        public override int NrgyResistOffset => -5;
        public override void DoEffect(Mobile m)
        {
            if (m is PlayerMobile)
                ((PlayerMobile)m).IgnoreMobiles = true;

            m.PlaySound(0x17F);
            m.FixedParticles(0x374A, 1, 15, 9902, 1108, 4, EffectLayer.Waist);

            int manadrain = Math.Max(8, 5 + (int)(0.16 * m.Skills.SpiritSpeak.Value));

            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.WraithForm, 1060524, 1153829, string.Format("15\t5\t5\t{0}", manadrain)));
        }

        public override void RemoveEffect(Mobile m)
        {
            if (m is PlayerMobile && m.IsPlayer())
                ((PlayerMobile)m).IgnoreMobiles = false;

            BuffInfo.RemoveBuff(m, BuffIcon.WraithForm);
        }
    }
}
