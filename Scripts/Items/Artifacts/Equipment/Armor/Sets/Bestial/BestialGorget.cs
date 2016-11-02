using System;
using Server.Items;

namespace Server.Items
{
	public class BestialGorget : LeatherGorget
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber{ get{ return 1151200; } } // Bestial Gorget

        public override SetItem SetID{ get{ return SetItem.Bestial; } }
		public override int Pieces{ get{ return 4; } }
        public override int Berserk { get { return 1; } }

        public override int BasePhysicalResistance{ get{ return 6; } }
		public override int BaseFireResistance{ get{ return 20; } }
		public override int BaseColdResistance{ get{ return 7; } }
		public override int BasePoisonResistance{ get{ return 7; } }
		public override int BaseEnergyResistance{ get{ return 8; } }
		public override int InitMinHits{ get{ return 125; } }
		public override int InitMaxHits{ get{ return 125; } }

		[Constructable]
		public BestialGorget() : base()
		{
            this.Hue = 2010;
            this.Weight = 1;
            this.StrRequirement = 25;
            this.SetHue = 2010;
        }

		public BestialGorget( Serial serial ) : base( serial )
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