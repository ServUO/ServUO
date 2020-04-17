namespace Server.Items
{
    public class GraveDust : BaseReagent, ICommodity
    {
        [Constructable]
        public GraveDust()
            : this(1)
        {
        }

        [Constructable]
        public GraveDust(int amount)
            : base(0xF8F, amount)
        {
        }

        public GraveDust(Serial serial)
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