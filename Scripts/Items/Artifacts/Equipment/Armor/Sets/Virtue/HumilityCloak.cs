using System;

namespace Server.Items
{
    [FlipableAttribute(0x2B04, 0x2B05)]
    public class HumilityCloak : BaseClothing
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public HumilityCloak()
            : base(0x2B04, Layer.Cloak)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 6.0;
            this.SetHue = 0;
            this.Hue = 0x226;
			
            this.SetSelfRepair = 5;			
            this.SetPhysicalBonus = 5;
            this.SetFireBonus = 5;
            this.SetColdBonus = 5;
            this.SetPoisonBonus = 5;
            this.SetEnergyBonus = 5;
        }

        public HumilityCloak(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075195;
            }
        }// Cloak of Humility (Virtue Armor Set)
        public override SetItem SetID
        {
            get
            {
                return SetItem.Virtue;
            }
        }
        public override int Pieces
        {
            get
            {
                return 8;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 0;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 0;
            }
        }
        public override int AosStrReq
        {
            get
            {
                return 10;
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