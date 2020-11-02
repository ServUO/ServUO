namespace Server.Items
{
    [Flipable(0x9EC1, 0x9EC2)]
    public class MaleTopper : Item
    {
        public override int LabelNumber => 1124665; // Cake Topper

        [Constructable]
        public MaleTopper()
            : base(0x9EC1)
        {
			LootType = LootType.Blessed;
        }

        public MaleTopper(Serial serial)
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
	
	[Flipable(0x9EC3, 0x9EC4)]
    public class FemaleTopper : Item
    {
        public override int LabelNumber => 1124665; // Cake Topper

        [Constructable]
        public FemaleTopper()
            : base(0x9EC3)
        {
			LootType = LootType.Blessed;
        }

        public FemaleTopper(Serial serial)
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
