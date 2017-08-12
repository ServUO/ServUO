using System;

namespace Server.Items
{
    public class PowerCore : BaseDecayingItem
    {
        public override int LabelNumber { get { return 1154429; } } // Power Core

        [Constructable]
        public PowerCore()
            : base(0x47E6)
        {
            this.Weight = 1.0;
            this.LootType = LootType.Blessed;
            this.Hue = 1967;
        }
		
		public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan { get { return 18000; } }

        public PowerCore(Serial serial)
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
