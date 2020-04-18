using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefTinkering), typeof(GargishGlasses), true)]
    public class Glasses : BaseArmor, IRepairable
    {
        public CraftSystem RepairSystem => DefTinkering.CraftSystem;

        [Constructable]
        public Glasses()
            : base(0x2FB8)
        {
            Weight = 2.0;
        }

        public Glasses(Serial serial)
            : base(serial)
        {
        }

        public override int StrReq => 45;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Leather;

        public override CraftResource DefaultResource => CraftResource.RegularLeather;

        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
