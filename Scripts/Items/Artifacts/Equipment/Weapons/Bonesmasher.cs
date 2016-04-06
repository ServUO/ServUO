using System;

namespace Server.Items
{
    public class Bonesmasher : DiamondMace
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public Bonesmasher()
        {
            this.ItemID = 0x2D30;
            this.Hue = 0x482;

            this.SkillBonuses.SetValues(0, SkillName.Macing, 10.0);

            this.WeaponAttributes.HitLeechMana = 40;
            this.WeaponAttributes.SelfRepair = 2;
        }

        public Bonesmasher(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075030;
            }
        }// Bonesmasher
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

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}