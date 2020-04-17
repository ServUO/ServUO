using Server.Mobiles;
using Server.Targeting;
using System;

namespace Server.Spells.Necromancy
{
    public class VengefulSpiritSpell : NecromancerSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Vengeful Spirit", "Kal Xen Bal Beh",
            203,
            9031,
            Reagent.BatWing,
            Reagent.GraveDust,
            Reagent.PigIron);
        public VengefulSpiritSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(2.25);
        public override double RequiredSkill => 80.0;
        public override int RequiredMana => 41;
        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if ((Caster.Followers + 3) > Caster.FollowersMax)
            {
                Caster.SendLocalizedMessage(1049645); // You have too many followers to summon that creature.
                return false;
            }

            return true;
        }

        public void Target(Mobile m)
        {
            if (Caster == m)
            {
                Caster.SendLocalizedMessage(1061832); // You cannot exact vengeance on yourself.
            }
            else if (CheckHSequence(m))
            {
                SpellHelper.Turn(Caster, m);

                /* Summons a Revenant which haunts the target until either the target or the Revenant is dead.
                * Revenants have the ability to track down their targets wherever they may travel.
                * A Revenant's strength is determined by the Necromancy and Spirit Speak skills of the Caster.
                * The effect lasts for ((Spirit Speak skill level * 80) / 120) + 10 seconds.
                */

                TimeSpan duration = TimeSpan.FromSeconds(((GetDamageSkill(Caster) * 80) / 120) + 10);

                Revenant rev = new Revenant(Caster, m, duration);

                if (BaseCreature.Summon(rev, false, Caster, m.Location, 0x81, TimeSpan.FromSeconds(duration.TotalSeconds + 2.0)))
                    rev.FixedParticles(0x373A, 1, 15, 9909, EffectLayer.Waist);
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly VengefulSpiritSpell m_Owner;
            public InternalTarget(VengefulSpiritSpell owner)
                : base(10, false, TargetFlags.Harmful)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                    m_Owner.Target((Mobile)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
