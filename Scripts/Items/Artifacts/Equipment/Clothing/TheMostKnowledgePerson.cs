using Server.Engines.Craft;

namespace Server.Items
{
    public class TheMostKnowledgePerson : BaseOuterTorso, IRepairable
    {
        public CraftSystem RepairSystem => DefTailoring.CraftSystem;

        public override bool IsArtifact => true;
        [Constructable]
        public TheMostKnowledgePerson()
            : base(0x2684)
        {
            Hue = 0x117;
            Attributes.BonusHits = 3 + Utility.RandomMinMax(0, 2);
        }

        public TheMostKnowledgePerson(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1094893;// The Most Knowledge Person [Replica]
        public override int InitMinHits => 150;
        public override int InitMaxHits => 150;
        public override bool CanFortify => false;
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