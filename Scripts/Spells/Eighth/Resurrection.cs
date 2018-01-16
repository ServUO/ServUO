using System;
using Server.Gumps;
using Server.Targeting;

namespace Server.Spells.Eighth
{
    public class ResurrectionSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Resurrection", "An Corp",
            245,
            9062,
            Reagent.Bloodmoss,
            Reagent.Garlic,
            Reagent.Ginseng);
        public ResurrectionSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Eighth;
            }
        }
        
        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile m)
        {
            if (!this.Caster.CanSee(m))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (m == this.Caster)
            {
                this.Caster.SendLocalizedMessage(501039); // Thou can not resurrect thyself.
            }
            else if (!this.Caster.Alive)
            {
                this.Caster.SendLocalizedMessage(501040); // The resurrecter must be alive.
            }
            else if (m.Alive)
            {
                this.Caster.SendLocalizedMessage(501041); // Target is not dead.
            }
            else if (!this.Caster.InRange(m, 1))
            {
                this.Caster.SendLocalizedMessage(501042); // Target is not close enough.
            }
            else if (!m.Player)
            {
                this.Caster.SendLocalizedMessage(501043); // Target is not a being.
            }
            else if (m.Map == null || !m.Map.CanFit(m.Location, 16, false, false))
            {
                this.Caster.SendLocalizedMessage(501042); // Target can not be resurrected at that location.
                m.SendLocalizedMessage(502391); // Thou can not be resurrected there!
            }
            else if (m.Region != null && m.Region.IsPartOf("Khaldun"))
            {
                this.Caster.SendLocalizedMessage(1010395); // The veil of death in this area is too strong and resists thy efforts to restore life.
            }
            else if (this.CheckBSequence(m, true))
            {
                SpellHelper.Turn(this.Caster, m);

                m.PlaySound(0x214);
                m.FixedEffect(0x376A, 10, 16);

                m.CloseGump(typeof(ResurrectGump));
                m.SendGump(new ResurrectGump(m, this.Caster));
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly ResurrectionSpell m_Owner;
            public InternalTarget(ResurrectionSpell owner)
                : base(1, false, TargetFlags.Beneficial)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                {
                    this.m_Owner.Target((Mobile)o);
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}
