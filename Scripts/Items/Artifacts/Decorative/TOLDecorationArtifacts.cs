using System;
using Server;

namespace Server.Items
{
    public class StretchedDinosaurHide : BaseDecorationArtifact
	{
		public override int ArtifactRarity { get { return 11; } }
		public override bool ShowArtifactRarity { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

		[Constructable]
		public StretchedDinosaurHide() : base(4202)
		{
            Name = "Stretched Dinosaur Hide";
            Hue = 2523;
		}
		
		public StretchedDinosaurHide(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
 
			writer.WriteEncodedInt( 0 ); // version
		}
 
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
 
			int version = reader.ReadEncodedInt();
		}
	}
	
	public class CarvedMyrmydexGlyph : BaseDecorationArtifact // Barrab
	{
		public override int ArtifactRarity { get { return 11; } }
		public override bool ShowArtifactRarity { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

		[Constructable]
		public CarvedMyrmydexGlyph() : base(4676)
		{
            Name = "Carved Myrmydex Glyph";
            Hue = 2952;
		}
		
		public CarvedMyrmydexGlyph(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
 
			writer.WriteEncodedInt( 0 ); // version
		}
 
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
 
			int version = reader.ReadEncodedInt();
		}
	}
	
	public class WakuOnASpit : BaseDecorationArtifact // Barako
	{
		public override int ArtifactRarity { get { return 11; } }
		public override bool ShowArtifactRarity { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

		[Constructable]
		public WakuOnASpit() : base(7832)
		{
            Name = "Waku on a Spit";
		}
		
		public WakuOnASpit(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
 
			writer.WriteEncodedInt( 0 ); // version
		}
 
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
 
			int version = reader.ReadEncodedInt();
		}
	}
	
	public class SacredLavaRock : BaseDecorationArtifact // Jukari
	{
		public override int ArtifactRarity { get { return 11; } }
		public override bool ShowArtifactRarity { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

		[Constructable]
		public SacredLavaRock() : base(4962)
		{
            Name = "Sacred Lava Rock";
            Hue = 1964;
		}
		
		public SacredLavaRock(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
 
			writer.WriteEncodedInt( 0 ); // version
		}
 
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
 
			int version = reader.ReadEncodedInt();
		}
	}
	
	public class WhiteTigerFigurine : BaseDecorationArtifact // Kurak
	{
		public override int ArtifactRarity { get { return 11; } }
		public override bool ShowArtifactRarity { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

		[Constructable]
		public WhiteTigerFigurine() : base(38980)
		{
            Name = "Hand Carved White Tiger Figurine";
            Hue = 2500;
		}
		
		public WhiteTigerFigurine(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
 
			writer.WriteEncodedInt( 0 ); // version
		}
 
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
 
			int version = reader.ReadEncodedInt();
		}
	}
	
	public class DragonTurtleHatchlingNet : BaseDecorationArtifact // Urali
	{
		public override int ArtifactRarity { get { return 11; } }
		public override bool ShowArtifactRarity { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

		[Constructable]
		public DragonTurtleHatchlingNet() : base(3574)
		{
            Name = "Dragon Turtle Hatchling Net";
		}
		
		public DragonTurtleHatchlingNet(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
 
			writer.WriteEncodedInt( 0 ); // version
		}
 
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
 
			int version = reader.ReadEncodedInt();
		}
	}
}