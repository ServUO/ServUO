using System;

namespace Server.Items
{
    public class ShaminoCrossbow : RepeatingCrossbow
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ShaminoCrossbow()
        {
            Hue = 0x504;
            LootType = LootType.Blessed;
            Attributes.AttackChance = 15;
            Attributes.WeaponDamage = 40;
            WeaponAttributes.SelfRepair = 10;
            WeaponAttributes.LowerStatReq = 100;
        }

        public ShaminoCrossbow(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1062915;
            }
        }// Shamino�s Best Crossbow
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}