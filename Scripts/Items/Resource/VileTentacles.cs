namespace Server.Items
{
    public class VileTentacles : Item, ICommodity
    {
        public override int LabelNumber => 1113333;  // vile tentacles
        public override double DefaultWeight => 0.1;

        [Constructable]
        public VileTentacles()
            : this(1)
        {
        }

        [Constructable]
        public VileTentacles(int amount)
            : base(0x5727)
        {
            Stackable = true;
            Amount = amount;
        }

        public VileTentacles(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;


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
