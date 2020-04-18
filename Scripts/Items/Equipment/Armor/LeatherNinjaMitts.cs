namespace Server.Items
{
    public class LeatherNinjaMitts : BaseArmor
    {
        [Constructable]
        public LeatherNinjaMitts()
            : base(0x2792)
        {
            Weight = 2.0;
        }

        public LeatherNinjaMitts(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 2;
        public override int BaseFireResistance => 4;
        public override int BaseColdResistance => 3;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 3;
        public override int InitMinHits => 25;
        public override int InitMaxHits => 25;
        public override int StrReq => 10;
        public override ArmorMaterialType MaterialType => ArmorMaterialType.Leather;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;
        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(2);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        if (reader.ReadBool())
                        {
                            reader.ReadInt();
                            reader.ReadInt();
                        }

                        break;
                    }
            }
        }
    }
}
