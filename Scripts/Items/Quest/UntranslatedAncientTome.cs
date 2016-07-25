using System;

namespace Server.Items
{
    public class UntransTome : Item
    {
        public override int LabelNumber { get { return 1112992; } }// Untranslated Ancient Tome

        [Constructable]
        public UntransTome()
            : base(0xFF2)
        {
            this.Weight = 1.0;
            this.Hue = 1109;
        }

        public UntransTome(Serial serial)
            : base(serial)
        {
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