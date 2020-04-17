namespace Server.Items
{
    public class ToxicVenomSac : Item, ICommodity
    {
        [Constructable]
        public ToxicVenomSac()
            : this(1)
        {
        }

        [Constructable]
        public ToxicVenomSac(int amount)
            : base(0x4005)
        {
            Stackable = true;
            Amount = amount;
        }

        public ToxicVenomSac(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override int LabelNumber => 1112291;// toxic venom sac
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
