using System;
using Server.Items;

namespace Server.Items
{
	public class VirtuososCap : JesterHat
    {
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber{ get{ return 1151320; } } // Virtuoso's Cap

        public override SetItem SetID{ get{ return SetItem.Virtuoso; } }
		public override int Pieces{ get{ return 4; } }
        public override bool BardMasteryBonus { get { return true; } }

        public override int BasePhysicalResistance{ get{ return 3; } }
		public override int BaseFireResistance{ get{ return 8; } }
		public override int BaseColdResistance{ get{ return 23; } }
		public override int BasePoisonResistance{ get{ return 8; } }
		public override int BaseEnergyResistance{ get{ return 8; } }
		public override int InitMinHits{ get{ return 125; } }
		public override int InitMaxHits{ get{ return 125; } }

        [Constructable]
		public VirtuososCap() : base()
		{
            this.Hue = 1374;
            this.Weight = 5;
            this.StrRequirement = 10;
            this.SetHue = 1374;
        }

		public VirtuososCap( Serial serial ) : base( serial )
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