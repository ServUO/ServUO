using System;
using System.Collections.Generic;
using Server;

namespace Server.Engines.Plants
{
	[Flags]
	public enum PlantHue
	{
		Plain			= 0x1 | Crossable | Reproduces,

		Red				= 0x2 | Crossable | Reproduces,
		Blue			= 0x4 | Crossable | Reproduces,
		Yellow			= 0x8 | Crossable | Reproduces,

		BrightRed		= Red | Bright,
		BrightBlue		= Blue | Bright,
		BrightYellow	= Yellow | Bright,

		Purple			= Red | Blue,
		Green			= Blue | Yellow,
		Orange			= Red | Yellow,

		BrightPurple	= Purple | Bright,
		BrightGreen		= Green | Bright,
		BrightOrange	= Orange | Bright,

		Black			= 0x10,
		White			= 0x20,
		Pink			= 0x40,
		Magenta			= 0x80,
		Aqua			= 0x100,
		FireRed			= 0x200,

		None			= 0,
		Reproduces		= 0x2000000,
		Crossable		= 0x4000000,
		Bright			= 0x8000000
	}

	public class PlantHueInfo
	{
		private static Dictionary<PlantHue, PlantHueInfo> m_Table;

		static PlantHueInfo()
		{
			m_Table = new Dictionary<PlantHue, PlantHueInfo>();

			m_Table[PlantHue.Plain]			= new PlantHueInfo( 0,		1060813, PlantHue.Plain,		0x835 );
			m_Table[PlantHue.Red]			= new PlantHueInfo( 0x66D,	1060814, PlantHue.Red,			0x24 );
			m_Table[PlantHue.Blue]			= new PlantHueInfo( 0x53D,	1060815, PlantHue.Blue,			0x6 );
			m_Table[PlantHue.Yellow]		= new PlantHueInfo( 0x8A5,	1060818, PlantHue.Yellow,		0x38 );
			m_Table[PlantHue.BrightRed]		= new PlantHueInfo( 0x21,	1060814, PlantHue.BrightRed,	0x21 );
			m_Table[PlantHue.BrightBlue]	= new PlantHueInfo( 0x5,	1060815, PlantHue.BrightBlue,	0x6 );
			m_Table[PlantHue.BrightYellow]	= new PlantHueInfo( 0x38,	1060818, PlantHue.BrightYellow,	0x35 );
			m_Table[PlantHue.Purple]		= new PlantHueInfo( 0xD,	1060816, PlantHue.Purple,		0x10 );
			m_Table[PlantHue.Green]			= new PlantHueInfo( 0x59B,	1060819, PlantHue.Green,		0x42 );
			m_Table[PlantHue.Orange]		= new PlantHueInfo( 0x46F,	1060817, PlantHue.Orange,		0x2E );
			m_Table[PlantHue.BrightPurple]	= new PlantHueInfo( 0x10,	1060816, PlantHue.BrightPurple,	0xD );
			m_Table[PlantHue.BrightGreen]	= new PlantHueInfo( 0x42,	1060819, PlantHue.BrightGreen,	0x3F );
			m_Table[PlantHue.BrightOrange]	= new PlantHueInfo( 0x2B,	1060817, PlantHue.BrightOrange,	0x2B );
			m_Table[PlantHue.Black]			= new PlantHueInfo( 0x455,	1060820, PlantHue.Black,		0 );
			m_Table[PlantHue.White]			= new PlantHueInfo( 0x481,	1060821, PlantHue.White,		0x481 );
			m_Table[PlantHue.Pink]			= new PlantHueInfo( 0x48E,	1061854, PlantHue.Pink );
			m_Table[PlantHue.Magenta]		= new PlantHueInfo( 0x486,	1061852, PlantHue.Magenta );
			m_Table[PlantHue.Aqua]			= new PlantHueInfo( 0x495,	1061853, PlantHue.Aqua );
			m_Table[PlantHue.FireRed]		= new PlantHueInfo( 0x489,	1061855, PlantHue.FireRed );
		}

		public static PlantHueInfo GetInfo( PlantHue plantHue )
		{
			PlantHueInfo info = null;

			if (m_Table.TryGetValue(plantHue, out info))
				return info;
			else
				return m_Table[PlantHue.Plain];
		}

        public static PlantHueInfo GetInfo(int hue)
        {
            foreach (PlantHueInfo info in m_Table.Values)
            {
                if (info.Hue == hue)
                    return info;
            }
            return (PlantHueInfo)m_Table[PlantHue.Plain];
        }

		public static PlantHue RandomFirstGeneration()
		{
			switch ( Utility.Random( 4 ) )
			{
				case 0: return PlantHue.Plain;
				case 1: return PlantHue.Red;
				case 2: return PlantHue.Blue;
				default: return PlantHue.Yellow;
			}
		}

		public static bool CanReproduce( PlantHue plantHue )
		{
			return (plantHue & PlantHue.Reproduces) != PlantHue.None;
		}

		public static bool IsCrossable( PlantHue plantHue )
		{
			return (plantHue & PlantHue.Crossable) != PlantHue.None;
		}

		public static bool IsBright( PlantHue plantHue )
		{
			return (plantHue & PlantHue.Bright) != PlantHue.None;
		}

		public static PlantHue GetNotBright( PlantHue plantHue )
		{
			return plantHue & ~PlantHue.Bright;
		}

		public static bool IsPrimary( PlantHue plantHue )
		{
			return plantHue == PlantHue.Red || plantHue == PlantHue.Blue || plantHue == PlantHue.Yellow;
		}

		public static PlantHue Cross( PlantHue first, PlantHue second )
		{
			if ( !IsCrossable( first ) || !IsCrossable( second ) )
				return PlantHue.None;

			if ( Utility.RandomDouble() < 0.01 )
				return Utility.RandomBool() ? PlantHue.Black : PlantHue.White;

			if ( first == PlantHue.Plain || second == PlantHue.Plain )
				return PlantHue.Plain;

			PlantHue notBrightFirst = GetNotBright( first );
			PlantHue notBrightSecond = GetNotBright( second );

			if ( notBrightFirst == notBrightSecond )
				return first | PlantHue.Bright;

			bool firstPrimary = IsPrimary( notBrightFirst );
			bool secondPrimary = IsPrimary( notBrightSecond );

			if ( firstPrimary && secondPrimary )
				return notBrightFirst | notBrightSecond;

			if ( firstPrimary && !secondPrimary )
				return notBrightFirst;

			if ( !firstPrimary && secondPrimary )
				return notBrightSecond;

			return notBrightFirst & notBrightSecond;
		}

		private int m_Hue;
		private int m_Name;
		private PlantHue m_PlantHue;
		private int m_GumpHue;

		public int Hue { get { return m_Hue; } }
		public int Name { get { return m_Name; } }
		public PlantHue PlantHue { get { return m_PlantHue; } }
		public int GumpHue { get { return m_GumpHue; } }

		private PlantHueInfo( int hue, int name, PlantHue plantHue ) : this( hue, name, plantHue, hue )
		{
		}

		private PlantHueInfo( int hue, int name, PlantHue plantHue, int gumpHue )
		{
			m_Hue = hue;
			m_Name = name;
			m_PlantHue = plantHue;
			m_GumpHue = gumpHue;
		}

		public bool IsCrossable()
		{
			return IsCrossable( m_PlantHue );
		}

		public bool IsBright()
		{
			return IsBright( m_PlantHue );
		}

		public PlantHue GetNotBright()
		{
			return GetNotBright( m_PlantHue );
		}

		public bool IsPrimary()
		{
			return IsPrimary( m_PlantHue );
		}
	}
}