using System;
using Server;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
	public enum CompassionPigmentType
	{
		PhoenixRed = 0,
		AuraOfAmber,
		DeepViolet,
		PolishedBronze,
		VibrantCrimson,
		Lavender,
		GleamingFuchsia,
		DeepBlue,
		GlossyFuchsia,
		DarkVoid,
		MurkySeagreen,
		ReflectiveShadow,
		LiquidSunshine,
		ShadowyBlue,
		BlackAndGreen,
		GlossyBlue,
		HunterGreen,
		SlateBlue,
		MotherOfPearl,
		StarBlue,
		MurkyAmber,
		VibranSeagreen,
        VibrantOcher,
        OliveGreen,
        MossyGreen,
        MottledSunsetBlue,
        TyrianPurple,
        IntenseTeal
	}

	public class CompassionPigmentInfo
	{
		public CompassionPigmentType Type { get; private set; }
		public int Hue { get; private set; }
		public int LabelNumber { get; private set; }

		public CompassionPigmentInfo( CompassionPigmentType type, int hue, int labelNumber )
		{
			Type = type;
			Hue = hue;
			LabelNumber = labelNumber;
		}

		public static CompassionPigmentInfo GetInfo( CompassionPigmentType type )
		{
			int v = (int) type;

			if ( v < 0 || v >= m_Table.Length )
				v = 0;

			return m_Table[v];
		}

		private static CompassionPigmentInfo[] m_Table = new CompassionPigmentInfo[]
		{
			new CompassionPigmentInfo( CompassionPigmentType.PhoenixRed, 1964, 1151651 ), // Phoenix Red Pigment
			new CompassionPigmentInfo( CompassionPigmentType.AuraOfAmber, 1967, 1152308 ), // Aura of Amber Pigment
			new CompassionPigmentInfo( CompassionPigmentType.DeepViolet, 1929, 1151912 ), // Deep Violet Pigment
			new CompassionPigmentInfo( CompassionPigmentType.PolishedBronze, 1944, 1151909 ), // Polished Bronze Pigment
			new CompassionPigmentInfo( CompassionPigmentType.VibrantCrimson, 1964, 1153386 ), // Vibrant Crimson Pigment
			new CompassionPigmentInfo( CompassionPigmentType.Lavender, 1951, 1151650 ), // Lavender Pigment
			new CompassionPigmentInfo( CompassionPigmentType.GleamingFuchsia, 1930, 1152311 ), // Gleaming Fuchsia Pigment
			new CompassionPigmentInfo( CompassionPigmentType.DeepBlue, 1939, 1152348 ), // Deep Blue Pigment
			new CompassionPigmentInfo( CompassionPigmentType.GlossyFuchsia, 1919, 1152347 ), // Glossy Fuchsia Pigment
			new CompassionPigmentInfo( CompassionPigmentType.DarkVoid, 2068, 1154214 ), // Dark Void Pigment
			new CompassionPigmentInfo( CompassionPigmentType.MurkySeagreen, 1992, 1152309 ), // Murky Seagreen Pigment
			new CompassionPigmentInfo( CompassionPigmentType.ReflectiveShadow, 1910, 1153387 ), // Reflective Shadow Pigment
			new CompassionPigmentInfo( CompassionPigmentType.LiquidSunshine, 1923, 1154213 ), // Liquid Sunshine Pigment
			new CompassionPigmentInfo( CompassionPigmentType.ShadowyBlue, 1960, 1152310 ), // Shadowy Blue Pigment
			new CompassionPigmentInfo( CompassionPigmentType.BlackAndGreen, 1979, 1151911 ), // Black And Green Pigment
			new CompassionPigmentInfo( CompassionPigmentType.GlossyBlue, 1916, 1151910 ), // Glossy Blue Pigment
			new CompassionPigmentInfo( CompassionPigmentType.HunterGreen, 1936, 1151649 ), // Hunter Green Pigment
			new CompassionPigmentInfo( CompassionPigmentType.SlateBlue, 1983, 1151653 ), // Slate Blue Pigment
			new CompassionPigmentInfo( CompassionPigmentType.MotherOfPearl, 2720, 1154120 ), // Mother Of Pearl Pigment
			new CompassionPigmentInfo( CompassionPigmentType.StarBlue, 2723, 1154121 ), // Star Blue Pigment
			new CompassionPigmentInfo( CompassionPigmentType.MurkyAmber, 1989, 1152350 ), // Murky Amber Pigment
			new CompassionPigmentInfo( CompassionPigmentType.VibranSeagreen, 1970, 1152349 ), // Vibran Seagreen Pigment
            new CompassionPigmentInfo( CompassionPigmentType.VibrantOcher, 2725, 1154736 ), // Vibrant Ocher Pigment
            new CompassionPigmentInfo( CompassionPigmentType.MossyGreen, 2684, 1154731 ), // Mossy Green Pigment
            new CompassionPigmentInfo( CompassionPigmentType.OliveGreen, 2709, 1154733 ), // Olive Green Pigment
            new CompassionPigmentInfo( CompassionPigmentType.MottledSunsetBlue, 2714, 1154734 ), // Mottled Sunset Blue Pigment
            new CompassionPigmentInfo( CompassionPigmentType.TyrianPurple, 2716, 1154735 ), // Tyrian Purple Pigment
            new CompassionPigmentInfo( CompassionPigmentType.IntenseTeal, 2691, 1154732 ), // Intense Teal Pigment
        };
	}

	public class CompassionPigment : Item, IUsesRemaining
	{
		public override int LabelNumber
		{
			get
			{
				return CompassionPigmentInfo.GetInfo( m_Type ).LabelNumber;
			}
		}

		private CompassionPigmentType m_Type;
		private int m_UsesRemaining;

		[CommandProperty( AccessLevel.GameMaster )]
		public CompassionPigmentType Type
		{
			get { return m_Type; }
			set
			{
				m_Type = value;
				InvalidateHue();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int UsesRemaining
		{
			get { return m_UsesRemaining; }
			set
			{
				m_UsesRemaining = value;
				InvalidateProperties();
			}
		}

		public bool ShowUsesRemaining
		{
			get { return true; }
			set
			{
			}
		}

		private void InvalidateHue()
		{
			Hue = CompassionPigmentInfo.GetInfo( m_Type ).Hue;
		}

		private static CompassionPigmentType GetRandomType()
		{
			var values = Enum.GetValues( typeof( CompassionPigmentType ) );
			return (CompassionPigmentType) values.GetValue( Utility.Random( values.Length ) );
		}

		[Constructable]
		public CompassionPigment()
			: this( GetRandomType() )
		{
		}

		[Constructable]
		public CompassionPigment( CompassionPigmentType type )
			: this( type, 5 )
		{
		}

		[Constructable]
		public CompassionPigment( CompassionPigmentType type, int uses )
			: base( 0xEFF )
		{
			m_Type = type;
			m_UsesRemaining = uses;

			Weight = 1.0;

			InvalidateHue();
		}

		public override void OnDoubleClick( Mobile from )
		{
			from.SendLocalizedMessage( 1070929 ); // Select the artifact or enhanced magic item to dye.

			from.Target = new DyeTarget( this );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~
		}

		public CompassionPigment( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );

			writer.Write( (int) m_Type );
			writer.Write( (int) m_UsesRemaining );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_Type = (CompassionPigmentType) reader.ReadInt();
			m_UsesRemaining = reader.ReadInt();
		}

		private class DyeTarget : Target
		{
			private CompassionPigment m_Pigment;

			public DyeTarget( CompassionPigment pigment )
				: base( 8, false, TargetFlags.None )
			{
				m_Pigment = pigment;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				Item item = targeted as Item;

				if ( item == null )
					return;

				if ( !item.IsChildOf( from.Backpack ) )
				{
					from.SendLocalizedMessage( 1062334 ); // This item must be in your backpack to be used.
				}
                else if (item is MetalPigmentsOfTokuno || item is LesserPigmentsOfTokuno || item is PigmentsOfTokuno || item is CompassionPigment)
                {
					from.SendLocalizedMessage( 1042083 ); // You cannot dye that.
				}
				else if ( item.IsLockedDown )
				{
					from.SendLocalizedMessage( 1070932 ); // You may not dye artifacts and enhanced magic items which are locked down.
				}
				else if (BasePigmentsOfTokuno.IsValidItem(item))
				{
					item.Hue = m_Pigment.Hue;

					m_Pigment.UsesRemaining--;

					if ( m_Pigment.UsesRemaining <= 0 )
						m_Pigment.Delete();
				}
				else
				{
					from.SendLocalizedMessage( 1070931 ); // You can only dye artifacts and enhanced magic items with this tub.
				}
			}
		}
	}
}
