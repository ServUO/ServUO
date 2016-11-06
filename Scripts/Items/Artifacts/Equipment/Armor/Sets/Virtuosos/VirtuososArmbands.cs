using System;
using Server.Items;

namespace Server.Items
{
	public class VirtuososArmbands : GargishPlateArms
    {
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber{ get{ return 1151558; } } // Virtuoso's Armbands

        public override SetItem SetID{ get{ return SetItem.Virtuoso; } }
		public override int Pieces{ get{ return 4; } }
        public override bool BardMasteryBonus { get { return true; } }

        public override int BasePhysicalResistance{ get{ return 24; } }
		public override int BaseFireResistance{ get{ return 10; } }
		public override int BaseColdResistance{ get{ return 9; } }
		public override int BasePoisonResistance{ get{ return 10; } }
		public override int BaseEnergyResistance{ get{ return 9; } }
		public override int InitMinHits{ get{ return 125; } }
		public override int InitMaxHits{ get{ return 125; } }

		[Constructable]
		public VirtuososArmbands() : base()
		{
            this.Hue = 1374;
            this.Weight = 5;
            this.StrRequirement = 80;
            this.SetHue = 1374;
        }

		public VirtuososArmbands( Serial serial ) : base( serial )
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