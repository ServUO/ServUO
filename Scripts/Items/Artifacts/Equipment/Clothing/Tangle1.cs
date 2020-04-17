using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefTailoring), typeof(GargishTangle1))]
    public class Tangle1 : HalfApron
    {
        public override int LabelNumber => 1114784;  // Tangle
        public override bool IsArtifact => true;

        [Constructable]
        public Tangle1()
            : base()
        {
            Hue = 506;
            Attributes.BonusInt = 10;
            Attributes.DefendChance = 5;
            Attributes.RegenMana = 2;
        }

        public Tangle1(Serial serial)
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

    public class GargishTangle1 : GargoyleHalfApron
    {
        public override int LabelNumber => 1114784;  // Tangle
        public override bool IsArtifact => true;

        [Constructable]
        public GargishTangle1()
            : base()
        {
            Hue = 506;
            Attributes.BonusInt = 10;
            Attributes.DefendChance = 5;
            Attributes.RegenMana = 2;
        }

        public GargishTangle1(Serial serial)
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