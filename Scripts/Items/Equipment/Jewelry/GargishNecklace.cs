namespace Server.Items
{
    public class GargishNecklace : BaseArmor
    {
        public override ArmorMaterialType MaterialType => ArmorMaterialType.Chainmail;
        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;

        public override int BasePhysicalResistance => 1;
        public override int BaseFireResistance => 2;
        public override int BaseColdResistance => 2;
        public override int BasePoisonResistance => 2;
        public override int BaseEnergyResistance => 3;

        public override int InitMinHits => 30;
        public override int InitMaxHits => 40;

        [Constructable]
        public GargishNecklace()
            : base(0x4210)
        {
            Layer = Layer.Neck;
        }

        public override int GetDurabilityBonus()
        {
            int bonus = Quality == ItemQuality.Exceptional ? 20 : 0;

            return bonus + ArmorAttributes.DurabilityBonus;
        }

        protected override void ApplyResourceResistances(CraftResource oldResource)
        {
        }

        public GargishNecklace(int itemID)
            : base(itemID)
        {
        }

        public GargishNecklace(Serial serial)
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

    public class GargishAmulet : GargishNecklace
    {
        [Constructable]
        public GargishAmulet()
            : base(0x4D0B)
        {
        }

        public GargishAmulet(Serial serial)
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

    public class GargishStoneAmulet : GargishNecklace
    {
        public override ArmorMaterialType MaterialType => ArmorMaterialType.Stone;
        public override int StrReq => 40;

        [Constructable]
        public GargishStoneAmulet()
            : base(0x4D0A)
        {
            Hue = 2500;
        }

        public GargishStoneAmulet(Serial serial)
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
