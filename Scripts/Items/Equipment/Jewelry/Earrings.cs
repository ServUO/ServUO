namespace Server.Items
{
    public abstract class BaseEarrings : BaseJewel
    {
        public override int BaseGemTypeNumber => 1044203; // star sapphire earrings

        public BaseEarrings(int itemID)
            : base(itemID, Layer.Earrings)
        {
        }

        public BaseEarrings(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
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
            Weight = 0.1;
        }

        public GoldEarrings(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
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
            Weight = 0.1;
        }

        public SilverEarrings(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}