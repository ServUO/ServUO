using System;

namespace Server.Items
{
    public class ArielHavenWritofMembership : Item
    {
        [Constructable]
        public ArielHavenWritofMembership()
            : base(0x2831)
        {
        }

        public ArielHavenWritofMembership(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1094998;
            }
        }//Ariel Haven Writ of Membership
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}