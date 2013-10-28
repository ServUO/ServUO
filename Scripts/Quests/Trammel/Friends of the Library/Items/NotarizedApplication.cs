using System;

namespace Server.Items
{
    public class NotarizedApplication : Item
    {
        [Constructable]
        public NotarizedApplication()
            : base(0x14EF)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 1.0;
        }

        public NotarizedApplication(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073135;
            }
        }// Notarized Application
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