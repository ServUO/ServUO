namespace Server.Items
{
    [Furniture]
    public class DecorativeMagesCrystalBall : Item
    {
        public override int LabelNumber => 1126487;  // crystal ball

        [Constructable]
        public DecorativeMagesCrystalBall()
            : base(0xA5DB)
        {
            Weight = 1;
            LootType = LootType.Blessed;
        }

        public DecorativeMagesCrystalBall(Serial serial)
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
}
