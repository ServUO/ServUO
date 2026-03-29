using System;

namespace Server.Items
{
    public class FeudalGhostwalkers : Sandals
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public FeudalGhostwalkers()
        {
            Name = "Feudal Ghostwalkers";
            Hue = 1150;

            SkillBonuses.SetValues(0, SkillName.Stealth, 10.0);
            SkillBonuses.SetValues(1, SkillName.Hiding, 10.0);
            SkillBonuses.SetValues(2, SkillName.Ninjitsu, 10.0);
            Attributes.DefendChance = 10;
            Attributes.BonusDex = 5;
            Attributes.NightSight = 1;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public FeudalGhostwalkers(Serial serial)
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

    public class GargishFeudalGhostwalkers : LeatherTalons
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishFeudalGhostwalkers()
        {
            Name = "Gargish Feudal Ghostwalkers";
            Hue = 1150;

            SkillBonuses.SetValues(0, SkillName.Stealth, 10.0);
            SkillBonuses.SetValues(1, SkillName.Hiding, 10.0);
            SkillBonuses.SetValues(2, SkillName.Ninjitsu, 10.0);
            Attributes.DefendChance = 10;
            Attributes.BonusDex = 5;
            Attributes.NightSight = 1;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public GargishFeudalGhostwalkers(Serial serial)
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
