using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefTailoring), typeof(GargishLieutenantOfTheBritannianRoyalGuard))]
    public class LieutenantOfTheBritannianRoyalGuard : BodySash
    {
        public override bool IsArtifact => true;
        [Constructable]
        public LieutenantOfTheBritannianRoyalGuard()
        {
            Hue = 0xe8;
            Attributes.BonusInt = 5;
            Attributes.RegenMana = 2;
            Attributes.LowerRegCost = 10;
        }

        public LieutenantOfTheBritannianRoyalGuard(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1094910;// Lieutenant of the Britannian Royal Guard [Replica]
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

    public class GargishLieutenantOfTheBritannianRoyalGuard : GargishSash
    {
        public override bool IsArtifact => true;

        [Constructable]
        public GargishLieutenantOfTheBritannianRoyalGuard()
        {
            Hue = 0xe8;
            Attributes.BonusInt = 5;
            Attributes.RegenMana = 2;
            Attributes.LowerRegCost = 10;
        }

        public GargishLieutenantOfTheBritannianRoyalGuard(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1094910;// Lieutenant of the Britannian Royal Guard [Replica]
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