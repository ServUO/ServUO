using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefTinkering), typeof(GargishGlasses), true)]
    public class ElvenGlasses : BaseArmor, IRepairable, ICanBeElfOrHuman
    {
        public override int LabelNumber => 1032216;  // elven glasses
        public CraftSystem RepairSystem => DefTinkering.CraftSystem;

        private bool _ElvesOnly;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ElfOnly { get { return _ElvesOnly; } set { _ElvesOnly = value; } }

        [Constructable]
        public ElvenGlasses()
            : base(0x2FB8)
        {
            Weight = 2;
        }

        public ElvenGlasses(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 2;
        public override int BaseFireResistance => 4;
        public override int BaseColdResistance => 4;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 2;
        public override int InitMinHits => 36;
        public override int InitMaxHits => 48;
        public override int StrReq => 45;
        public override ArmorMaterialType MaterialType => ArmorMaterialType.Leather;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;
        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(2); // version

            writer.Write(_ElvesOnly);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version > 1)
            {
                _ElvesOnly = reader.ReadBool();
            }
        }
    }
}
