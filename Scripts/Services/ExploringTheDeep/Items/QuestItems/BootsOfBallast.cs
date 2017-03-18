using System;

namespace Server.Items
{
    public class BootsOfBallast : Boots
    {
        public override int LabelNumber { get { return 1154242; } } // Boots of Ballast
        public override bool CanBeWornByGargoyles { get { return true; } }

        [Constructable]
        public BootsOfBallast()
            : base()
        {
            this.Hue = 2072;
            this.LootType = LootType.Blessed;
            this.StrRequirement = 10;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public BootsOfBallast(Serial serial)
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
