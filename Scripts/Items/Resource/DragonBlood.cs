namespace Server.Items
{
    public class DragonBlood : BaseReagent, ICommodity
    {
        [Constructable]
        public DragonBlood()
            : this(1)
        {
        }

        [Constructable]
        public DragonBlood(int amount)
            : base(0x4077, amount)
        {
        }

        public DragonBlood(Serial serial)
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
