using System;

namespace Server.Items
{
    public class AcidVine : Item
    {
		public override int LabelNumber { get { return 1111655; } } // magic vines
		public override bool ForceShowProperties { get { return ObjectPropertyList.Enabled; } }
		
        [Constructable]
        public AcidVine()
            : base(3313)
        {
            Weight = 1.0;
            Movable = false;
        }

        public AcidVine(Serial serial)
            : base(serial)
        {
        }
        
        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 1))
            {
                from.SendLocalizedMessage(1111658); // The vines tighten their grip, stopping you from opening the door.
            }
            else if (from.InRange(this.GetWorldLocation(), 4))
            {
				from.SendLocalizedMessage(1111665); // You notice something odd about the vines covering the wall.
            }
            else if (!from.InRange(this.GetWorldLocation(), 4))
            {
				from.SendLocalizedMessage(1019045); // I can't reach that.
            }

            base.OnDoubleClick(from);
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