using System;

namespace Server.Items
{
    public class ChurchillsWarMace : WarMace
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ChurchillsWarMace()
        {
            this.LootType = LootType.Blessed;

            this.Attributes.AttackChance = 5;
            this.Attributes.WeaponSpeed = 10;
            this.Attributes.WeaponDamage = 25;
            this.WeaponAttributes.LowerStatReq = 70;
        }

        public ChurchillsWarMace(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1078062;
            }
        }// Churchill's War Mace
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}