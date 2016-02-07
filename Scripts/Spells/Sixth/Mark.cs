using System;
using Server.Items;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.Sixth
{
    public class MarkSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Mark", "Kal Por Ylem",
            218,
            9002,
            Reagent.BlackPearl,
            Reagent.Bloodmoss,
            Reagent.MandrakeRoot);
        public MarkSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Sixth;
            }
        }
        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            return SpellHelper.CheckTravel(this.Caster, TravelCheckType.Mark);
        }

        public void Target(RecallRune rune)
        {
            if (!this.Caster.CanSee(rune))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (!SpellHelper.CheckTravel(this.Caster, TravelCheckType.Mark))
            {
            }
            else if (SpellHelper.CheckMulti(this.Caster.Location, this.Caster.Map, !Core.AOS))
            {
                this.Caster.SendLocalizedMessage(501942); // That location is blocked.
            }
            else if (!rune.IsChildOf(this.Caster.Backpack))
            {
                this.Caster.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1062422); // You must have this rune in your backpack in order to mark it.
            }
            else if (this.CheckSequence())
            {
                rune.Mark(this.Caster);

                this.Caster.PlaySound(0x1FA);
                Effects.SendLocationEffect(this.Caster, this.Caster.Map, 14201, 16);
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly MarkSpell m_Owner;
            public InternalTarget(MarkSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.None)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is RecallRune)
                {
                    this.m_Owner.Target((RecallRune)o);
                }
                else
                {
                    from.Send(new MessageLocalized(from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 501797, from.Name, "")); // I cannot mark that object.
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}