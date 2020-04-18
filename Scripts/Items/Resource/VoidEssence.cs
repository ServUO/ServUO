namespace Server.Items
{
    public class VoidEssence : Item, ICommodity
    {
        [Constructable]
        public VoidEssence()
            : this(1)
        {
        }

        [Constructable]
        public VoidEssence(int amount)
            : base(0x4007)
        {
            Stackable = true;
            Amount = amount;
        }

        public VoidEssence(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override int LabelNumber => 1112327;// void essence
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
