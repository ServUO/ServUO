using Server;
using System;
using Server.Mobiles;

namespace Server.Items
{
	public class DardensSleeves : DragonTurtleHideArms
	{
		public override int LabelNumber { get { return 1156242; } } // Darden's Armor
		
		public override SetItem SetID{ get{ return SetItem.Darden; } }
		public override int Pieces{ get{ return 4; } }
		
		public override int BasePhysicalResistance{ get{ return 6; } }
        public override int BaseFireResistance{ get{ return 7; } }
        public override int BaseColdResistance{ get{ return 7; } }
        public override int BasePoisonResistance{ get{ return 7; } }
        public override int BaseEnergyResistance{ get{ return 7; } }
		
		[Constructable]
		public DardensSleeves()
		{
            AbsorptionAttributes.EaterKinetic = 2;
            Attributes.BonusStr = 4;
            Attributes.BonusHits = 4;
            Attributes.LowerRegCost = 15;

            SetAttributes.BonusMana = 15;
            SetAttributes.LowerManaCost = 20;
            SetSelfRepair = 3;

            SetPhysicalBonus = 9;
            SetFireBonus = 8;
            SetColdBonus = 8;
            SetPoisonBonus = 8;
            SetEnergyBonus = 8;
		}

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1156346); // Myrmidex Slayer
        }
		
		public DardensSleeves( Serial serial ) : base( serial )
        {
        }
       
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
           
            writer.Write( (int) 0 ); // version
        }
       
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize( reader );
           
            int version = reader.ReadInt(); 
        }
	}
}