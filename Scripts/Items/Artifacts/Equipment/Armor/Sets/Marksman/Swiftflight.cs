using System;

namespace Server.Items
{
    public class Swiftflight : Bow
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public Swiftflight()
            : base()
        {
            this.SetHue = 0x594;
			
            this.Attributes.WeaponDamage = 40;
			
            this.SetSelfRepair = 3;			
            this.SetAttributes.AttackChance = 15;
            this.SetAttributes.BonusDex = 8;
            this.SetAttributes.WeaponSpeed = 30;
            this.SetAttributes.WeaponDamage = 20;
        }

        public Swiftflight(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074308;
            }
        }// Swiftflight (Marksman Set)
        public override SetItem SetID
        {
            get
            {
                return SetItem.Marksman;
            }
        }
        public override int Pieces
        {
            get
            {
                return 2;
            }
        }
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