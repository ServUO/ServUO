using System;

namespace Server.Items
{
    public class SerpentSkinQuiver : BaseQuiver
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public SerpentSkinQuiver() : base(0x2B02)
        {
            Name = "Serpent Skin Quiver";
            Hue = 1267;

            DamageIncrease = 10;
            Capacity = 1000;
            WeightReduction = 50;
            LowerAmmoCost = 50;

            SkillBonuses.SetValues(0, SkillName.Anatomy, 10.0);
            Attributes.Luck = 125;
            Attributes.WeaponSpeed = 10;
            Attributes.WeaponDamage = 20;
            Attributes.BonusStam = 8;
            Attributes.RegenHits = 2;
        }

        public SerpentSkinQuiver(Serial serial)
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

    public class GargishSerpentSkinWingArmor : GargishLeatherWingArmor
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishSerpentSkinWingArmor()
        {
            Name = "Gargish Serpent Skin Wing Armor";
            Hue = 1267;

            SkillBonuses.SetValues(0, SkillName.Anatomy, 10.0);
            Attributes.Luck = 125;
            Attributes.WeaponSpeed = 10;
            Attributes.WeaponDamage = 20;
            Attributes.BonusStam = 8;
            Attributes.RegenHits = 2;
        }

        public GargishSerpentSkinWingArmor(Serial serial)
            : base(serial)
        {
        }

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
