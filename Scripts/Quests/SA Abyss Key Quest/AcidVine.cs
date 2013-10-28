using System;

namespace Server.Items
{
    public class AcidVine : Item
    {
        [Constructable]
        public AcidVine()
            : base(3313)
        {
            this.Name = "vines";
            this.Weight = 1.0;
            this.Movable = false;
        }

        public AcidVine(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 1))
            {
                from.SendMessage("The vines tighten their grip, stopping you from opening the secret door!");
            }
            else if (from.InRange(this.GetWorldLocation(), 4))
            {
                from.SendMessage("You notice something odd about the vines covering the wall.");
            }
            else if (!from.InRange(this.GetWorldLocation(), 4))
            {
                from.SendMessage("I can't reach that!");
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