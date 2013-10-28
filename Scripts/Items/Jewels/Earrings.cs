using System;

namespace Server.Items
{
    public abstract class BaseEarrings : BaseJewel
    {
        public BaseEarrings(int itemID)
            : base(itemID, Layer.Earrings)
        {
        }

        public BaseEarrings(Serial serial)
            : base(serial)
        {
        }

        public override int BaseGemTypeNumber
        {
            get
            {
                return 1044203;
            }
        }// star sapphire earrings
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

    public class GoldEarrings : BaseEarrings
    {
        [Constructable]
        public GoldEarrings()
            : base(0x1087)
        {
            this.Weight = 0.1;
        }

        public GoldEarrings(Serial serial)
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

    public class SilverEarrings : BaseEarrings
    {
        [Constructable]
        public SilverEarrings()
            : base(0x1F07)
        {
            this.Weight = 0.1;
        }

        public SilverEarrings(Serial serial)
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