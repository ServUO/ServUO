using System;

namespace Server.Items
{
    public class MalekisHonor : MetalKiteShield
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public MalekisHonor()
            : base()
        {
            this.SetHue = 0x76D;
			
            this.SetSelfRepair = 3;			
            this.SetAttributes.DefendChance = 10;
            this.SetAttributes.BonusStr = 10;
            this.SetAttributes.WeaponSpeed = 35;
        }

        public MalekisHonor(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074312;
            }
        }// Maleki's Honor (Juggernaut Set)
        public override SetItem SetID
        {
            get
            {
                return SetItem.Juggernaut;
            }
        }
        public override int Pieces
        {
            get
            {
                return 2;
            }
        }
        public override int BasePhysicalResistance
        {
            get
            {
                return 3;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 3;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 3;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 3;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 3;
            }
        }
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);//version
        }
    }
}