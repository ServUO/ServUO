using System;

namespace Server.Items
{
    public class ShadowMastersTalisman : BaseTalisman
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public ShadowMastersTalisman()
            : base(0x2F58)
        {
            Name = "Shadow Master's Talisman";
            Hue = 1109;
            Weight = 1.0;

            SkillBonuses.SetValues(0, SkillName.Stealth, 10.0);
            SkillBonuses.SetValues(1, SkillName.Ninjitsu, 10.0);

            Attributes.RegenHits = 3;
            Attributes.RegenMana = 3;
            Attributes.DefendChance = 10;
            Attributes.BonusMana = 8;
            Attributes.BonusStam = 8;
            Attributes.AttackChance = 10;
            SAAbsorptionAttributes.EaterFire = 10;

            LootType = LootType.Blessed;
        }

        public ShadowMastersTalisman(Serial serial)
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
