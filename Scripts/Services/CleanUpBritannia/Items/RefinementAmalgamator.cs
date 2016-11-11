using Server.Targeting;
using System;

namespace Server.Items
{
    public class RefinementAmalgamator : Item
    {
        [Constructable]
        public RefinementAmalgamator()
            : base(0x9966)
        {
            this.Hue = 1152;
            this.Weight = 1;
        }

        public RefinementAmalgamator(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1154340;
            }
        }// Refinement Amalgamator

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                // The item must be in your backpack to use it.
                from.SendLocalizedMessage(1060640);
            }
            else
            {
                from.SendLocalizedMessage(1060640); // Target the refinement you wish to combine.
                from.Target = new InternalTarget(from);
            }
        }

        private class InternalTarget : Target
        {
            private readonly Mobile m_Mobile;

            public InternalTarget(Mobile m)
                : base(-1, true, TargetFlags.None)
            {
                this.m_Mobile = m;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                //from.SendLocalizedMessage(1154456); // You must first select a refinement to combine. 
                //from.SendLocalizedMessage(1154457); // This is not a refinement.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}