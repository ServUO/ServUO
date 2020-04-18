namespace Server.Items
{
    public class CreepingVine : Item
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1112401;

        [Constructable]
        public CreepingVine()
            : base(Utility.Random(18322, 4))
        {
        }

        public CreepingVine(Serial serial)
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
            int version = reader.ReadInt();
        }
    }
}