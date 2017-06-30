using System;
using Server;
using Server.Mobiles;

namespace Server.Engines.CannedEvil
{
	public enum MiniChampType
	{
		CrimsonVeins,
		FairyDragonLair,
		AbyssalLair,
		DiscardedCavernClanRibbon,
		DiscardedCavernClanScratch,
		DiscardedCavernClanChitter,
		PassageofTears,
		LandsoftheLich,
		SecretGarden,
        FireTemple,
		EnslavedGoblins,
        SkeletalDragon,
        LavaCaldera, 
		MeraktusTheTormented
	}

	public class MiniChampInfo
	{
		private string m_Name;
		private Type m_Champion;
		private Type m_Champion2;
		private Type[][] m_SpawnTypes;

		public string Name { get { return m_Name; } }
		public Type Champion { get { return m_Champion; } }
		public Type Champion2 { get { return m_Champion2; } }
		public Type[][] SpawnTypes { get { return m_SpawnTypes; } }

		public MiniChampInfo( string name, Type champion, Type[][] spawnTypes )
		{
			m_Name = name;
			m_Champion = champion;
			m_SpawnTypes = spawnTypes;
		}
		
		public MiniChampInfo( string name, Type champion, Type champion2, Type[][] spawnTypes )
		{
			m_Name = name;
			m_Champion = champion;
			m_Champion2 = champion2;
			m_SpawnTypes = spawnTypes;
		}

		public static MiniChampInfo[] Table{ get { return m_Table; } }

		private static readonly MiniChampInfo[] m_Table = new MiniChampInfo[]
			{
				new MiniChampInfo( "Crimson Veins", typeof( FireElementalRenowned ), new Type[][]
				{																										
					new Type[]{ typeof( FireAnt ), typeof( LavaSnake ), typeof( LavaLizard ) },															// Level 1										
					new Type[]{ typeof( Efreet ), typeof( FireGargoyle ) },																				// Level 2							
					new Type[]{ typeof( LavaElemental ), typeof( FireDaemon ) }																			// Level 3						
				} ),
				new MiniChampInfo( "Fairy Dragon Lair", typeof( WyvernRenowned ), new Type[][]
				{																										
					new Type[]{ typeof( FairyDragon ) },																								// Level 1								
					new Type[]{ typeof( Wyvern ) },																										// Level 2
					new Type[]{ typeof( ForgottenServant ) }																							// Level 3
				} ),
				new MiniChampInfo( "Abyssal Lair", typeof( DevourerRenowned ), new Type[][]
				{																											
					new Type[]{ typeof( GreaterMongbat ), typeof( Imp ) },																				// Level 1												
					new Type[]{ typeof( Daemon ) },																										// Level 2
					new Type[]{ typeof( PitFiend ) }																									// Level 3
				} ),
				new MiniChampInfo( "Discarded Cavern Clan Ribbon", typeof( VitaviRenowned ), new Type[][]
				{																									
					new Type[]{ typeof( ClanRibbonPlagueRat ), typeof( ClanRS ) },														                // Level 1
					new Type[]{ typeof( ClanRibbonPlagueRat ), typeof( ClanRC ) }															            // Level 2														
				} ),
				new MiniChampInfo( "Discarded Cavern Clan Scratch", typeof( TikitaviRenowned ), new Type[][]
				{																									
					new Type[]{ typeof( ClanSSW ), typeof( ClanSS ) },														                            // Level 1
					new Type[]{ typeof( ClanSSW ), typeof( ClanSH ) }														                            // Level 2														
				} ),
				new MiniChampInfo( "Discarded Cavern Clan Chitter", typeof( RakktaviRenowned ), new Type[][]
				{																									
					new Type[]{ typeof( ClockworkScorpion ), typeof( ClanCA ) },															            // Level 1
					new Type[]{ typeof( ClockworkScorpion ), typeof( ClanCT ) }															                // Level 2													
				} ),
				new MiniChampInfo( "Passage of Tears", typeof( AcidElementalRenowned ), new Type[][]	
				{																											
					new Type[]{ typeof( AcidSlug ), typeof( CorrosiveSlime ) },																			// Level 1
					new Type[]{ typeof( AcidElemental ) },																								// Level 2
					new Type[]{ typeof( InterredGrizzle )}																								// Level 3												
				} ),
				new MiniChampInfo( "Lands of the Lich", typeof( AncientLichRenowned ), new Type[][]
				{																											
					new Type[]{ typeof( Wraith ), typeof( Spectre ), typeof( Shade ), typeof( Skeleton ), typeof( Zombie ) },							// Level 1
					new Type[]{ typeof( BoneMagi ), typeof( SkeletalMage ), typeof( BoneKnight ), typeof( SkeletalKnight ), typeof( WailingBanshee )},	// Level 2
					new Type[]{ typeof( SkeletalLich ), typeof( RottingCorpse ) }																		// Level 3
				} ),
				new MiniChampInfo( "Secret Garden", typeof( PixieRenowned ), new Type[][]	
				{																										
					new Type[]{ typeof( Pixie )},																										// Level 1
					new Type[]{ typeof( Wisp )},																										// Level 2
					new Type[]{ typeof( DarkWisp )}																										// Level 3						
				} ),
                new MiniChampInfo( "Fire Temple Ruins", typeof( FireDaemonRenowned ), new Type[][]
				{																										
					new Type[]{ typeof( LavaSnake ), typeof( LavaLizard ), typeof( FireAnt ) },															// Level 1
					new Type[]{ typeof( LavaSerpent ), typeof( HellCat ), typeof( HellHound ) },														// Level 2
					new Type[]{ typeof( FireDaemon ), typeof( LavaElemental ) }																			// Level 3
				} ),
				new MiniChampInfo( "Enslaved Goblins", typeof( GrayGoblinMageRenowned ), typeof( SkeletalDragonRenowned ), new Type[][]
				{																											
					new Type[]{ typeof( EnslavedGrayGoblin ), typeof( EnslavedGreenGoblin ) },															// Level 1
					new Type[]{ typeof( EnslavedGoblinScout ), typeof( EnslavedGoblinKeeper ) },														// Level 2
					new Type[]{ typeof( EnslavedGoblinMage ), typeof( EnslavedGreenGoblinAlchemist ) }													// Level 3
				} ),
				new MiniChampInfo( "Skeletal Dragon", typeof( SkeletalDragonRenowned ), new Type[][]
				{																											
					new Type[]{ typeof( PatchworkSkeleton ), typeof( Skeleton ) },																		// Level 1
					new Type[]{ typeof( BoneKnight ), typeof( SkeletalKnight ) },																		// Level 2
					new Type[]{ typeof( BoneMagi ), typeof( SkeletalMage ) },																			// Level 3
					new Type[]{ typeof( SkeletalLich ) }																								// Level 4
				} ),
                new MiniChampInfo( "Lava Caldera", typeof( FireDaemonRenowned ), new Type[][]
				{																										
					new Type[]{ typeof( LavaSnake ), typeof( LavaLizard ), typeof( FireAnt ) },															// Level 1
					new Type[]{ typeof( LavaSerpent ), typeof( HellCat ), typeof( HellHound ) },														// Level 2
					new Type[]{ typeof( FireDaemon ), typeof( LavaElemental ) }																			// Level 3
				} ),
				new MiniChampInfo( "Meraktus the Tormented", typeof( Meraktus ), new Type[][]
				{																										
					new Type[]{ typeof( Minotaur ) },																									// Level 1
					new Type[]{ typeof( MinotaurScout ) },																								// Level 2
					new Type[]{ typeof( MinotaurCaptain ) }																								// Level 3
				} )
			};

		public static MiniChampInfo GetInfo( MiniChampType type )
		{
			int v = (int)type;

			if( v < 0 || v >= m_Table.Length )
				v = 0;

			return m_Table[v];
		}
	}
}