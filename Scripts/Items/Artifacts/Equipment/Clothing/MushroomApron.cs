using System;

namespace Server.Items
{
    public class MushroomApron : HalfApron
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public MushroomApron()
        {
            Name = "Mushroom Apron";
            Hue = 1167;

            SkillBonuses.SetValues(0, SkillName.Alchemy, 10.0);
            Attributes.BonusHits = 5;
            Attributes.RegenHits = 2;
            Attributes.EnhancePotions = 15;
        }

        public MushroomApron(Serial serial)
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
            reader.ReadInt();
        }
    }

    public class GargishMushroomApron : GargoyleHalfApron
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishMushroomApron()
        {
            Name = "Gargish Mushroom Apron";
            Hue = 1167;

            SkillBonuses.SetValues(0, SkillName.Alchemy, 10.0);
            Attributes.BonusHits = 5;
            Attributes.RegenHits = 2;
            Attributes.EnhancePotions = 15;
        }

        public GargishMushroomApron(Serial serial)
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
            reader.ReadInt();
        }
    }
}
