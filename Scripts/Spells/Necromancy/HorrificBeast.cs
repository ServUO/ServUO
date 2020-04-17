using System;

namespace Server.Spells.Necromancy
{
    public class HorrificBeastSpell : TransformationSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Horrific Beast", "Rel Xen Vas Bal",
            203,
            9031,
            Reagent.BatWing,
            Reagent.DaemonBlood);
        public HorrificBeastSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(2.25);
        public override double RequiredSkill => 40.0;
        public override int RequiredMana => 11;
        public override int Body => 746;
        public override void DoEffect(Mobile m)
        {
            m.PlaySound(0x165);
            m.FixedParticles(0x3728, 1, 13, 9918, 92, 3, EffectLayer.Head);

            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.HorrificBeast, 1060514, 1153763, "20\t25"));

            m.Delta(MobileDelta.WeaponDamage);

            m.ResetStatTimers();
        }

        public override void RemoveEffect(Mobile m)
        {
            m.Delta(MobileDelta.WeaponDamage);
            BuffInfo.RemoveBuff(m, BuffIcon.HorrificBeast);
        }
    }
}
