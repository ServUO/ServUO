using System;

namespace Server.Spells.Necromancy
{
    public class LichFormSpell : TransformationSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Lich Form", "Rel Xen Corp Ort",
            203,
            9031,
            Reagent.GraveDust,
            Reagent.DaemonBlood,
            Reagent.NoxCrystal);
        public LichFormSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(2.25);
        public override double RequiredSkill => 70.0;
        public override int RequiredMana => 23;
        public override int Body => 749;
        public override int FireResistOffset => -10;
        public override int ColdResistOffset => +10;
        public override int PoisResistOffset => +10;
        public override double TickRate => 2;
        public override void DoEffect(Mobile m)
        {
            m.PlaySound(0x19C);
            m.FixedParticles(0x3709, 1, 30, 9904, 1108, 6, EffectLayer.RightFoot);
            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.LichForm, 1060515, 1153767, "5\t13\t10\t10\t10"));

            m.ResetStatTimers();
        }
        public override void OnTick(Mobile m)
        {
            --m.Hits;
        }

        public override void RemoveEffect(Mobile m)
        {
            BuffInfo.RemoveBuff(m, BuffIcon.LichForm);
        }
    }
}
