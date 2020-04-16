namespace Server.Items
{
    public class DeathShroud : BaseSuit
    {
        [Constructable]
        public DeathShroud()
            : base(AccessLevel.GameMaster, 0x0, 0x204E)
        {
        }

        public DeathShroud(Serial serial)
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