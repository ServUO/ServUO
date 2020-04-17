using Server.Engines.Craft;

namespace Server.Items
{
    public class MagicWand : BaseBashing, IRepairable
    {
        public CraftSystem RepairSystem => DefCarpentry.CraftSystem;

        [Constructable]
        public MagicWand()
            : base(0xDF2)
        {
            Weight = 1.0;
        }

        public MagicWand(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.Dismount;
        public override WeaponAbility SecondaryAbility => WeaponAbility.Disarm;
        public override int StrengthReq => 5;
        public override int MinDamage => 9;
        public override int MaxDamage => 11;
        public override float Speed => 2.75f;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 110;
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