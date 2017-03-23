using System;

namespace Server.Items
{
    public class CanvassRobe : Robe
    {
        public override int LabelNumber { get { return 1154238; } } // A Canvass Robe
        public override bool CanBeWornByGargoyles { get { return true; } }

        [Constructable]
        public CanvassRobe()
            : base()
        {
            this.Hue = 1153;
            this.LootType = LootType.Blessed;
            this.StrRequirement = 10;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public CanvassRobe(Serial serial)
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
