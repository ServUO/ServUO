using System;
using Server.Engines.Craft;

namespace Server.Items
{
    public class SpikedWhip : BaseSword, Server.Engines.Craft.IRepairable
    {
		public Server.Engines.Craft.CraftSystem RepairSystem { get { return Server.Engines.Craft.DefTinkering.CraftSystem; } }
        public override int LabelNumber { get { return 1125634; } } // Spiked Whip

        [Constructable]
        public SpikedWhip()
            : base(0xA292)
        {
            Weight = 5.0;
        }

        public SpikedWhip(Serial serial)
            : base(serial)
        {
        }

		public override bool CanBeWornByGargoyles { get { return true; } }
        public override WeaponAbility PrimaryAbility { get { return WeaponAbility.ArmorPierce; } }
        public override WeaponAbility SecondaryAbility { get { return WeaponAbility.WhirlwindAttack; } }
        public override int AosStrengthReq { get { return 20; } }
        public override int AosMinDamage { get { return 13; } }
        public override int AosMaxDamage { get { return 17; } }
        public override float MlSpeed { get { return 3.25f; } }
        public override int DefHitSound { get { return 0x23B; } }
        public override int DefMissSound { get { return 0x23A; } }
        public override int InitMinHits { get { return 30; } }
        public override int InitMaxHits { get { return 60; } }
        public override SkillName DefSkill { get { return SkillName.Fencing; } }
        public override WeaponType DefType { get { return WeaponType.Piercing; } }
        public override WeaponAnimation DefAnimation { get { return WeaponAnimation.Pierce1H; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
