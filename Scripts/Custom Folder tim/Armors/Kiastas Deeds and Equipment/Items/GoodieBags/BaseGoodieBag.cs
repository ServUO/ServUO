using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
    public class BaseGoodieBag : Bag
    {
        [Constructable]
        public BaseGoodieBag() : this(1)
        {
        }

        [Constructable]
        public BaseGoodieBag(int amount)
        {
            Weight = 0.0;
            LootType = Settings.Misc.BagLootType;
        }

        public BaseGoodieBag(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
