using System;

namespace Server.Items
{
    public class EvilIdolSkull : Item
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public EvilIdolSkull()
            : base(0x1F18)
        {
        }

        public EvilIdolSkull(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1095237;
            }
        }// Evil Idol
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