using System;

namespace Server.Items
{
    public class SolariasSecretPoisons : GoldEarrings
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public SolariasSecretPoisons()
        {
            Name = "Solaria's Secret Poisons";
            Hue = 1267;
            Weight = 1.0;

            SkillBonuses.SetValues(0, SkillName.Ninjitsu, 10.0);
            Attributes.AttackChance = 10;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public SolariasSecretPoisons(Serial serial)
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

    public class GargishSolariasSecretPoisons : GargishEarrings
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishSolariasSecretPoisons()
        {
            Name = "Gargish Solaria's Secret Poisons";
            Hue = 1267;
            Weight = 1.0;

            SkillBonuses.SetValues(0, SkillName.Ninjitsu, 10.0);
            Attributes.AttackChance = 10;
        }

        public GargishSolariasSecretPoisons(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance { get { return 15; } }
        public override int BaseFireResistance { get { return 18; } }
        public override int BaseColdResistance { get { return 15; } }
        public override int BasePoisonResistance { get { return 18; } }
        public override int BaseEnergyResistance { get { return 15; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

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
