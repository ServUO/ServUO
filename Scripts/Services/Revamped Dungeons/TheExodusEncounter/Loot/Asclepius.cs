namespace Server.Items
{
    public class Asclepius : GnarledStaff
    {
        public override bool IsArtifact => true;

        [Constructable]
        public Asclepius()
        {
        }

        public override bool CanFortify => false;

        public Asclepius(Serial serial) : base(serial)
        {
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public override int LabelNumber => 1153526;  // Asclepius [Replica]

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add(1153525); // 15% Bandage Healing Bonus 
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
