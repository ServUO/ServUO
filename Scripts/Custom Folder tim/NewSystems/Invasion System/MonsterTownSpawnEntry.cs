using System;
using Server.Mobiles;

namespace Server.Customs.Invasion_System
{
	public class MonsterTownSpawnEntry
	{
		#region MonsterSpawnEntries
		
		public static MonsterTownSpawnEntry[] Undead = new MonsterTownSpawnEntry[]
		{
			//Monster													//Amount
			new MonsterTownSpawnEntry( typeof( Zombie ),						165 ),
			new MonsterTownSpawnEntry( typeof( Skeleton ),						65 ),
			new MonsterTownSpawnEntry( typeof( SkeletalMage ),					40 ),
			new MonsterTownSpawnEntry( typeof( BoneKnight ),					45 ),
			new MonsterTownSpawnEntry( typeof( SkeletalKnight ),				45 ),
			new MonsterTownSpawnEntry( typeof( Lich ),							45 ),
			new MonsterTownSpawnEntry( typeof( Ghoul ),							40 ),
			new MonsterTownSpawnEntry( typeof( BoneMagi ),						40 ),
			new MonsterTownSpawnEntry( typeof( Wraith ),						35 ),
			new MonsterTownSpawnEntry( typeof( RottingCorpse ),					35 ),
			new MonsterTownSpawnEntry( typeof( LichLord ),						55 ),
			new MonsterTownSpawnEntry( typeof( Spectre ),						30 ),
			new MonsterTownSpawnEntry( typeof( Shade ),							30 ),
			new MonsterTownSpawnEntry( typeof( AncientLich ),					50 )
		};

		public static MonsterTownSpawnEntry[] Humanoid = new MonsterTownSpawnEntry[]
		{
			//Monster														//Amount
			new MonsterTownSpawnEntry( typeof( Brigand ),						60 ),
			new MonsterTownSpawnEntry( typeof( Executioner ),					30 ),
			new MonsterTownSpawnEntry( typeof( EvilMage ),						70 ),
			new MonsterTownSpawnEntry( typeof( EvilMageLord ),					40 ),
			new MonsterTownSpawnEntry( typeof( Ettin ),							45 ),
			new MonsterTownSpawnEntry( typeof( Ogre ),							45 ),
			new MonsterTownSpawnEntry( typeof( OgreLord ),						40 ),
			new MonsterTownSpawnEntry( typeof( ArcticOgreLord ),				40 ),
			new MonsterTownSpawnEntry( typeof( Troll ),							55 ),
			new MonsterTownSpawnEntry( typeof( Cyclops ),						55 ),
			new MonsterTownSpawnEntry( typeof( Titan ),							40 )
		};

		public static MonsterTownSpawnEntry[] OrcsandRatmen = new MonsterTownSpawnEntry[]
		{
			//Monster														//Amount
			new MonsterTownSpawnEntry( typeof( Orc ),							80 ),
			new MonsterTownSpawnEntry( typeof( OrcishMage ),					45 ),
			new MonsterTownSpawnEntry( typeof( OrcishLord ),					55 ),
			new MonsterTownSpawnEntry( typeof( OrcCaptain ),					50 ),
			new MonsterTownSpawnEntry( typeof( OrcBomber ),						55 ),
			new MonsterTownSpawnEntry( typeof( OrcBrute ),						40 ),
			new MonsterTownSpawnEntry( typeof( Ratman ),						80 ),
			new MonsterTownSpawnEntry( typeof( RatmanArcher ),					50 ),
			new MonsterTownSpawnEntry( typeof( RatmanMage ),					45 )
		};

		public static MonsterTownSpawnEntry[] Elementals = new MonsterTownSpawnEntry[]
		{
			//Monster														//Amount
			new MonsterTownSpawnEntry( typeof( EarthElemental ),				95 ),
			new MonsterTownSpawnEntry( typeof( AirElemental ),					70 ),
			new MonsterTownSpawnEntry( typeof( FireElemental ),					60 ),
			new MonsterTownSpawnEntry( typeof( WaterElemental ),				60 ),
			new MonsterTownSpawnEntry( typeof( SnowElemental ),					40 ),
			new MonsterTownSpawnEntry( typeof( IceElemental ),					40 ),
			new MonsterTownSpawnEntry( typeof( Efreet ),						45 ),
			new MonsterTownSpawnEntry( typeof( PoisonElemental ),				35 ),
			new MonsterTownSpawnEntry( typeof( BloodElemental ),				35 )
		};

		public static MonsterTownSpawnEntry[] OreElementals = new MonsterTownSpawnEntry[]
		{
			//Monster														//Amount
			new MonsterTownSpawnEntry( typeof( DullCopperElemental ),			90 ),
			new MonsterTownSpawnEntry( typeof( CopperElemental ),				80 ),
			new MonsterTownSpawnEntry( typeof( BronzeElemental ),				50 ),
			new MonsterTownSpawnEntry( typeof( ShadowIronElemental ),			60 ),
			new MonsterTownSpawnEntry( typeof( GoldenElemental ),				55 ),
			new MonsterTownSpawnEntry( typeof( AgapiteElemental ),				45 ),
			new MonsterTownSpawnEntry( typeof( VeriteElemental ),				40 ),
			new MonsterTownSpawnEntry( typeof( ValoriteElemental ),				40 )
		};

		public static MonsterTownSpawnEntry[] Ophidian = new MonsterTownSpawnEntry[]
		{
			//Monster														//Amount
			new MonsterTownSpawnEntry( typeof( OphidianWarrior ),				100 ),
			new MonsterTownSpawnEntry( typeof( OphidianMage ),					70 ),
			new MonsterTownSpawnEntry( typeof( OphidianArchmage ),				30 ),
			new MonsterTownSpawnEntry( typeof( OphidianKnight ),				35 ),
			new MonsterTownSpawnEntry( typeof( OphidianMatriarch ),				35 )
		};

		public static MonsterTownSpawnEntry[] Arachnid = new MonsterTownSpawnEntry[]
		{
			//Monster														//Amount
			new MonsterTownSpawnEntry( typeof( Scorpion ),						75 ),
			new MonsterTownSpawnEntry( typeof( GiantSpider ),					75 ),
			new MonsterTownSpawnEntry( typeof( TerathanDrone ),					45 ),
			new MonsterTownSpawnEntry( typeof( TerathanWarrior ),				30 ),
			new MonsterTownSpawnEntry( typeof( TerathanMatriarch ),				45 ),
			new MonsterTownSpawnEntry( typeof( TerathanAvenger ),				45 ),
			new MonsterTownSpawnEntry( typeof( DreadSpider ),					40 ),
			new MonsterTownSpawnEntry( typeof( FrostSpider ),					35 )
		};

		public static MonsterTownSpawnEntry[] Snakes = new MonsterTownSpawnEntry[]
		{
			//Monster														//Amount
			new MonsterTownSpawnEntry( typeof( Snake ),							95 ),
			new MonsterTownSpawnEntry( typeof( GiantSerpent ),					95 ),
			new MonsterTownSpawnEntry( typeof( LavaSnake ),						50 ),
			new MonsterTownSpawnEntry( typeof( LavaSerpent ),					55 ),
			new MonsterTownSpawnEntry( typeof( IceSnake ),						50 ),
			new MonsterTownSpawnEntry( typeof( IceSerpent ),					55 ),
			new MonsterTownSpawnEntry( typeof( SilverSerpent ),					40 )
		};

		public static MonsterTownSpawnEntry[] Abyss = new MonsterTownSpawnEntry[]
		{
			//Monster														//Amount
			new MonsterTownSpawnEntry( typeof( Gargoyle ),						100 ),
			new MonsterTownSpawnEntry( typeof( StoneGargoyle ),					60 ),
			new MonsterTownSpawnEntry( typeof( FireGargoyle ),					60 ),
			new MonsterTownSpawnEntry( typeof( Daemon ),						60 ),
			new MonsterTownSpawnEntry( typeof( IceFiend ),						50 ),
			new MonsterTownSpawnEntry( typeof( Balron ),						30 )
		};

		public static MonsterTownSpawnEntry[] DragonKind = new MonsterTownSpawnEntry[]
		{
			//Monster														//Amount
			new MonsterTownSpawnEntry( typeof( Wyvern ),						100 ),
			new MonsterTownSpawnEntry( typeof( Drake ),							60 ),
			new MonsterTownSpawnEntry( typeof( Dragon ),						60 ),
			new MonsterTownSpawnEntry( typeof( WhiteWyrm ),						60 ),
			new MonsterTownSpawnEntry( typeof( ShadowWyrm ),					10 ),
			new MonsterTownSpawnEntry( typeof( AncientWyrm ),					30 )
		};

		#endregion

		private Type m_Monster;
		private int m_Amount;

		public Type Monster { get { return m_Monster; } set { m_Monster = value; } }
		public int Amount { get { return m_Amount; } set { m_Amount = value; } }

		public MonsterTownSpawnEntry( Type monster, int amount )
		{
			m_Monster = monster;
			m_Amount = amount;
		}
	}
}