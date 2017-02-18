using Server;
using System;
using Server.Mobiles;

namespace Server.Items
{
	public class AloronsLongSkirt : TigerPeltLongSkirt
	{
		public override int LabelNumber { get { return 1156243; } } // Aloron's Armor
		
		public override SetItem SetID{ get{ return SetItem.Aloron; } }
		public override int Pieces{ get{ return 4; } }
		
		public override int BasePhysicalResistance{ get{ return 7; } }
        public override int BaseFireResistance{ get{ return 7; } }
        public override int BaseColdResistance{ get{ return 6; } }
        public override int BasePoisonResistance{ get{ return 7; } }
        public override int BaseEnergyResistance{ get{ return 7; } }
		
		[Constructable]
		public AloronsLongSkirt()
		{
            AbsorptionAttributes.EaterCold = 2;
            Attributes.BonusDex = 4;
            Attributes.BonusStam = 4;
            Attributes.RegenStam = 3;

            SetAttributes.BonusMana = 15;
            SetAttributes.LowerManaCost = 20;
            SetSelfRepair = 3;

            SetPhysicalBonus = 8;
            SetFireBonus = 8;
            SetColdBonus = 9;
            SetPoisonBonus = 8;
            SetEnergyBonus = 8;
		}

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1156345); // Dinosaur Slayer
        }
		
		public AloronsLongSkirt( Serial serial ) : base( serial )
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