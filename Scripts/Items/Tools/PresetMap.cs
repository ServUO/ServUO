using System;
using Server;

namespace Server.Items
{
	public class PresetMap : MapItem
	{
		private int m_LabelNumber;

		[Constructable]
		public PresetMap( PresetMapType type )
		{
			int v = (int)type;

			if ( v >= 0 && v < PresetMapEntry.Table.Length )
				InitEntry( PresetMapEntry.Table[v] );
		}

		public PresetMap( PresetMapEntry entry )
		{
			InitEntry( entry );
		}

		public void InitEntry( PresetMapEntry entry )
		{
			m_LabelNumber = entry.Name;

			Width = entry.Width;
			Height = entry.Height;

			Bounds = entry.Bounds;
		}

		public override int LabelNumber{ get{ return (m_LabelNumber == 0 ? base.LabelNumber : m_LabelNumber); } }

		public PresetMap( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );

			writer.Write( (int) m_LabelNumber );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_LabelNumber = reader.ReadInt();
					break;
				}
			}
		}
	}

	public class PresetMapEntry
	{
		private int m_Name;
		private int m_Width, m_Height;
		private Rectangle2D m_Bounds;

		public int Name{ get{ return m_Name; } }
		public int Width{ get{ return m_Width; } }
		public int Height{ get{ return m_Height; } }
		public Rectangle2D Bounds{ get{ return m_Bounds; } }

		public PresetMapEntry( int name, int width, int height, int xLeft, int yTop, int xRight, int yBottom )
		{
			m_Name = name;
			m_Width = width;
			m_Height = height;
			m_Bounds = new Rectangle2D( xLeft, yTop, xRight - xLeft, yBottom - yTop );
		}

		private static PresetMapEntry[] m_Table = new PresetMapEntry[]
			{
				new PresetMapEntry( 1041189, 200, 200, 1092, 1396, 1736, 1924 ), // map of Britain
				new PresetMapEntry( 1041203, 200, 200, 0256, 1792, 1736, 2560 ), // map of Britain to Skara Brae
				new PresetMapEntry( 1041192, 200, 200, 1024, 1280, 2304, 3072 ), // map of Britain to Trinsic
				new PresetMapEntry( 1041183, 200, 200, 2500, 1900, 3000, 2400 ), // map of Buccaneer's Den
				new PresetMapEntry( 1041198, 200, 200, 2560, 1792, 3840, 2560 ), // map of Buccaneer's Den to Magincia
				new PresetMapEntry( 1041194, 200, 200, 2560, 1792, 3840, 3072 ), // map of Buccaneer's Den to Ocllo
				new PresetMapEntry( 1041181, 200, 200, 1088, 3572, 1528, 4056 ), // map of Jhelom
				new PresetMapEntry( 1041186, 200, 200, 3530, 2022, 3818, 2298 ), // map of Magincia
				new PresetMapEntry( 1041199, 200, 200, 3328, 1792, 3840, 2304 ), // map of Magincia to Ocllo
				new PresetMapEntry( 1041182, 200, 200, 2360, 0356, 2706, 0702 ), // map of Minoc
				new PresetMapEntry( 1041190, 200, 200, 0000, 0256, 2304, 3072 ), // map of Minoc to Yew
				new PresetMapEntry( 1041191, 200, 200, 2467, 0572, 2878, 0746 ), // map of Minoc to Vesper
				new PresetMapEntry( 1041188, 200, 200, 4156, 0808, 4732, 1528 ), // map of Moonglow
				new PresetMapEntry( 1041201, 200, 200, 3328, 0768, 4864, 1536 ), // map of Moonglow to Nujelm
				new PresetMapEntry( 1041185, 200, 200, 3446, 1030, 3832, 1424 ), // map of Nujelm
				new PresetMapEntry( 1041197, 200, 200, 3328, 1024, 3840, 2304 ), // map of Nujelm to Magincia
				new PresetMapEntry( 1041187, 200, 200, 3582, 2456, 3770, 2742 ), // map of Ocllo
				new PresetMapEntry( 1041184, 200, 200, 2714, 3329, 3100, 3639 ), // map of Serpent's Hold
				new PresetMapEntry( 1041200, 200, 200, 2560, 2560, 3840, 3840 ), // map of Serpent's Hold to Ocllo
				new PresetMapEntry( 1041180, 200, 200, 0524, 2064, 0960, 2452 ), // map of Skara Brae
				new PresetMapEntry( 1041204, 200, 200, 0000, 0000, 5199, 4095 ), // map of The World
				new PresetMapEntry( 1041177, 200, 200, 1792, 2630, 2118, 2952 ), // map of Trinsic
				new PresetMapEntry( 1041193, 200, 200, 1792, 1792, 3072, 3072 ), // map of Trinsic to Buccaneer's Den
				new PresetMapEntry( 1041195, 200, 200, 0256, 1792, 2304, 4095 ), // map of Trinsic to Jhelom
				new PresetMapEntry( 1041178, 200, 200, 2636, 0592, 3064, 1012 ), // map of Vesper
				new PresetMapEntry( 1041196, 200, 200, 2636, 0592, 3840, 1536 ), // map of Vesper to Nujelm
				new PresetMapEntry( 1041179, 200, 200, 0236, 0741, 0766, 1269 ), // map of Yew
				new PresetMapEntry( 1041202, 200, 200, 0000, 0512, 1792, 2048 ), // map of Yew to Britain
                new PresetMapEntry( 0,       200, 200, 600,  3280, 950,  3650 ), // Royal City
                new PresetMapEntry( 0,       200, 200, 900,  3780, 1160, 4060 ), // Holy City
                new PresetMapEntry( 0,       200, 200, 890,  430,  1090, 600  ), // Luna
                new PresetMapEntry( 0,       200, 200, 1920, 1240, 2130, 1430 ), // Umbra
                new PresetMapEntry( 0,       200, 200, 5630, 3080, 5860, 3330 ), // Papua
                new PresetMapEntry( 0,       200, 200, 5140, 3900, 5330, 4094 ), // Deliucia
			};

		public static PresetMapEntry[] Table{ get{ return m_Table; } }
	}

	public enum PresetMapType
	{
		Britain,
		BritainToSkaraBrae,
		BritainToTrinsic,
		BucsDen,
		BucsDenToMagincia,
		BucsDenToOcllo,
		Jhelom,
		Magincia,
		MaginciaToOcllo,
		Minoc,
		MinocToYew,
		MinocToVesper,
		Moonglow,
		MoonglowToNujelm,
		Nujelm,
		NujelmToMagincia,
		Ocllo,
		SerpentsHold,
		SerpentsHoldToOcllo,
		SkaraBrae,
		TheWorld,
		Trinsic,
		TrinsicToBucsDen,
		TrinsicToJhelom,
		Vesper,
		VesperToNujelm,
		Yew,
		YewToBritain
	}
}