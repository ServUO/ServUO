using System;

namespace Server.Items
{
    public class FlyWheel : BaseDecayingItem
    {
        public override int LabelNumber { get { return 1154427; } } // Flywheel

        [Constructable]
        public FlyWheel()
            : base(0x46FE)
        {
            this.Weight = 1.0;
            this.LootType = LootType.Blessed;
            this.Hue = 1901;
        }
		
		public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan { get { return 18000; } }

        public FlyWheel(Serial serial)
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
