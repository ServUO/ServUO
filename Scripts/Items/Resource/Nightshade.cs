namespace Server.Items
{
    public class Nightshade : BaseReagent, ICommodity
    {
        [Constructable]
        public Nightshade()
            : this(1)
        {
        }

        [Constructable]
        public Nightshade(int amount)
            : base(0xF88, amount)
        {
        }

        public Nightshade(Serial serial)
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