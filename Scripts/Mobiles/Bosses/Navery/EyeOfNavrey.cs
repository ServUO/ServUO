using System;

namespace Server.Items
{
    public class EyeOfNavrey : Item
    {
        [Constructable]
        public EyeOfNavrey()
            : base(0x318D)
        {
            this.Weight = 1;
            this.Hue = 68;
            this.LootType = LootType.Blessed;
        }

        public EyeOfNavrey(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1095154;
            }
        }// Eye of Navrey Night-Eyes
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