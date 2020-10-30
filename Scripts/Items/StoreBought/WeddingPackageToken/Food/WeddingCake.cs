using Server.Network;

namespace Server.Items
{
    public class WeddingCake : Item
    {
        public override int LabelNumber => 1124648; // Cake

        [Constructable]
        public WeddingCake(int id)
            : base(id)
        {
            Weight = 10;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.LocalOverheadMessage(MessageType.Regular, 0x47E, 1157341); // *You cut a slice from the cake.*
            from.AddToBackpack(new WeddingCakeSlice());
        }

        public WeddingCake(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class WeddingCakeSlice : Food
    {
        public override int LabelNumber => 1124648; // Cake

        [Constructable]
        public WeddingCakeSlice()
            : base(Utility.RandomList(0x9EF5, 0x9EF6))
        {
            Stackable = false;
            Weight = 1.0;
            FillFactor = 5;
        }

        public WeddingCakeSlice(Serial serial)
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
            reader.ReadInt();
        }
    }
}
