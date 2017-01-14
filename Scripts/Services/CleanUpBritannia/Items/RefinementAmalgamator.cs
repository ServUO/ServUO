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
                from.SendLocalizedMessage(1154351); // Target the refinement you wish to combine.
                from.Target = new InternalTarget(from);
            }
        }

        private class InternalTarget : Target
        {
            private Mobile m_Mobile;
            private RefinementComponent m_First;

            public InternalTarget(Mobile m, RefinementComponent first = null)
                : base(-1, true, TargetFlags.None)
            {
                m_Mobile = m;
                m_First = first;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                Item item = targeted as Item;

                if (item == null)
                    return;

                if(!item.IsChildOf(from.Backpack))
                    from.SendLocalizedMessage(1060640); // The item must be in your backpack to use it.
                else if (targeted is RefinementComponent)
                {
                    RefinementComponent comp = (RefinementComponent)targeted;

                    if (m_First == null)
                    {
                        if (comp.ModType == ModType.Invulnerability)
                        {
                            from.SendLocalizedMessage(1154353); // You can't upgrade this refinement.
                        }
                        else
                        {
                            from.SendLocalizedMessage(1154351); // Target the refinement you wish to combine.
                            from.Target = new InternalTarget(from, comp);
                        }
                    }
                    else
                    {
                        if (m_First.RefinementType != comp.RefinementType 
                            || m_First.CraftType != comp.CraftType 
                            || m_First.SubCraftType != comp.SubCraftType
                            || m_First.ModType != comp.ModType)
                        {
                            from.SendLocalizedMessage(1154354); // This refinement does not match the type currently being combined.
                        }
                        else
                        {
                            comp.Delete();
                            m_First.ModType++;

                            from.SendLocalizedMessage(1154352); // You've completed the amalgamation and received an upgraded version of your refinement.
                        }
                    }
                }
                else
                    from.SendLocalizedMessage(1154457); // This is not a refinement.
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