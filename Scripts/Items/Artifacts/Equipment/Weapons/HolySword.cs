using System;

namespace Server.Items
{
    public class HolySword : Longsword
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public HolySword()
        {
            Hue = 0x482;
            LootType = LootType.Blessed;
            Slayer = SlayerName.Silver;
            Attributes.WeaponDamage = 40;
            WeaponAttributes.SelfRepair = 10;
            WeaponAttributes.LowerStatReq = 100;
            WeaponAttributes.UseBestSkill = 1;
        }

        public HolySword(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1062921;
            }
        }// The Holy Sword
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