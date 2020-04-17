namespace Server.Items
{
    public class CaptainBlackheartsFishingPole : FishingPole
    {
        [Constructable]
        public CaptainBlackheartsFishingPole()
            : base()
        {
        }

        public CaptainBlackheartsFishingPole(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074571;// Captain Blackheart's Fishing Pole
        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add(1073634); // An aquarium decoration
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