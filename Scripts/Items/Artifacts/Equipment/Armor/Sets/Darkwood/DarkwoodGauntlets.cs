using System;
using Server.Engines.Craft;

namespace Server.Items
{
    public class DarkwoodGloves : WoodlandGloves
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DarkwoodGloves()
            : base()
        {
            this.Hue = 0x455;								
            this.SetHue = 0x494;
						
            this.Attributes.BonusHits = 2;		
            this.Attributes.DefendChance = 5;
			
            this.SetAttributes.ReflectPhysical = 25;
            this.SetAttributes.BonusStr = 10;		
            this.SetAttributes.NightSight = 1;		
			
            this.SetSelfRepair = 3;
			
            this.SetPhysicalBonus = 2;
            this.SetFireBonus = 5;
            this.SetColdBonus = 5;
            this.SetPoisonBonus = 3;
            this.SetEnergyBonus = 5;
        }

        public DarkwoodGloves(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073486;
            }
        }// Darkwood Gauntlets
        public override SetItem SetID
        {
            get
            {
                return SetItem.Darkwood;
            }
        }
        public override int Pieces
        {
            get
            {
                return 6;
            }
        }
        public override int BasePhysicalResistance
        {
            get
            {
                return 8;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 7;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 5;
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

        public override int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            if (resHue > 0)
                this.Hue = resHue;
				
            Type resourceType = typeRes;

            if (resourceType == null)
                resourceType = craftItem.Resources.GetAt(0).ItemType;

            this.Resource = CraftResources.GetFromType(resourceType);
			
            switch ( this.Resource )
            {
                case CraftResource.Bloodwood:
                    this.Attributes.RegenHits = 2;
                    break;
                case CraftResource.Heartwood:
                    this.Attributes.Luck = 40;
                    break;
                case CraftResource.YewWood:
                    this.Attributes.RegenHits = 1;
                    break;
            }
			
            return 0;
        }
    }
}