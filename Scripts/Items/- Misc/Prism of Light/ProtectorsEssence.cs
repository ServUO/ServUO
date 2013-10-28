using System;

namespace Server.Items
{
    public class ProtectorsEssence : Item
    {
        [Constructable]
        public ProtectorsEssence()
            : base(0x23F)
        {
        }

        public ProtectorsEssence(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073159;
            }
        }// Protector's Essence
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