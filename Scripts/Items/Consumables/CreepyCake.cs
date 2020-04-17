namespace Server.Items
{
    public class CreepyCake : Food
    {
        public override int LabelNumber => 1153776;  // Creepy Cake

        [Constructable]
        public CreepyCake()
            : base(0x9e9)
        {
            Hue = 0x3E4;
        }

        public CreepyCake(Serial serial)
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