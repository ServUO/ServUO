using System;

namespace Server.Items
{
    public class AquaGem : BaseDecayingItem
    {
        public override int LabelNumber { get { return 1154244; } } // Aqua Gem

        [Constructable]
        public AquaGem() : base(0x4B48)
        {
            Stackable = false;
            Weight = 1.0;
            Hue = 1916;
            LootType = LootType.Blessed;
        }

        public override int Lifespan { get { return 3600; } }

        public AquaGem(Serial serial) : base(serial)
        { }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
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
