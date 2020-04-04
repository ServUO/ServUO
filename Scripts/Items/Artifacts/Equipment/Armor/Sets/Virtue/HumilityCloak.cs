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
            LootType = LootType.Blessed;
            Weight = 6.0;
            SetHue = 0;
            Hue = 0x226;
			
            SetSelfRepair = 5;			
            SetPhysicalBonus = 5;
            SetFireBonus = 5;
            SetColdBonus = 5;
            SetPoisonBonus = 5;
            SetEnergyBonus = 5;
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