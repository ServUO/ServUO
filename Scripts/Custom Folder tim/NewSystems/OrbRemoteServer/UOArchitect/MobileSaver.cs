// Created by Peoharen
using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Server
{
	public class MobileSaver
	{
		public static List<Type> ConversionTable = new List<Type>();
		public const int Offset = 1000;

		public static void Configure()
		{
			#region NPCS
			// Vendors NPC
			ConversionTable.Add( typeof( Alchemist ) );
			ConversionTable.Add( typeof( AnimalTrainer ) );
			ConversionTable.Add( typeof( Architect ) );
			ConversionTable.Add( typeof( Armorer ) );
			ConversionTable.Add( typeof( Baker ) );
			ConversionTable.Add( typeof( Bard ) );
			ConversionTable.Add( typeof( Barkeeper ) );
			ConversionTable.Add( typeof( Beekeeper ) );
			ConversionTable.Add( typeof( Blacksmith ) );
			ConversionTable.Add( typeof( Bowyer ) );
			ConversionTable.Add( typeof( Butcher ) );
			ConversionTable.Add( typeof( Carpenter ) );
			ConversionTable.Add( typeof( Cobbler ) );
			ConversionTable.Add( typeof( Cook ) );
			ConversionTable.Add( typeof( CustomHairstylist ) );
			ConversionTable.Add( typeof( Farmer ) );
			ConversionTable.Add( typeof( Fisherman ) );
			ConversionTable.Add( typeof( Furtrader ) );
			ConversionTable.Add( typeof( Glassblower ) );
			ConversionTable.Add( typeof( GolemCrafter ) );
			ConversionTable.Add( typeof( GypsyAnimalTrainer ) );
			ConversionTable.Add( typeof( GypsyBanker ) );
			ConversionTable.Add( typeof( GypsyMaiden ) );
			ConversionTable.Add( typeof( HairStylist ) );
			ConversionTable.Add( typeof( Herbalist ) );
			ConversionTable.Add( typeof( HolyMage ) );
			ConversionTable.Add( typeof( InnKeeper ) );
			ConversionTable.Add( typeof( IronWorker ) );
			ConversionTable.Add( typeof( Jeweler ) );
			ConversionTable.Add( typeof( KeeperOfChivalry ) );
			ConversionTable.Add( typeof( LeatherWorker ) );
			ConversionTable.Add( typeof( Mage ) );
			ConversionTable.Add( typeof( Mapmaker ) );
			ConversionTable.Add( typeof( Miller ) );
			ConversionTable.Add( typeof( Miner ) );
			ConversionTable.Add( typeof( Monk ) );
			ConversionTable.Add( typeof( Provisioner ) );
			ConversionTable.Add( typeof( Rancher ) );
			ConversionTable.Add( typeof( Ranger ) );
			ConversionTable.Add( typeof( RealEstateBroker ) );
			ConversionTable.Add( typeof( Scribe ) );
			ConversionTable.Add( typeof( Shipwright ) );
			ConversionTable.Add( typeof( StoneCrafter ) );
			ConversionTable.Add( typeof( Tailor ) );
			ConversionTable.Add( typeof( Tanner ) );
			ConversionTable.Add( typeof( TavernKeeper ) );
			ConversionTable.Add( typeof( Thief ) );
			ConversionTable.Add( typeof( Tinker ) );
			ConversionTable.Add( typeof( Vagabond ) );
			ConversionTable.Add( typeof( VarietyDealer ) );
			ConversionTable.Add( typeof( Veterinarian ) );
			ConversionTable.Add( typeof( Waiter ) );
			ConversionTable.Add( typeof( Weaponsmith ) );
			ConversionTable.Add( typeof( Weaver ) );
			// Vendors Guildmasters
			ConversionTable.Add( typeof( BardGuildmaster ) );
			ConversionTable.Add( typeof( BlacksmithGuildmaster ) );
			ConversionTable.Add( typeof( FisherGuildmaster ) );
			ConversionTable.Add( typeof( HealerGuildmaster ) );
			ConversionTable.Add( typeof( MageGuildmaster ) );
			ConversionTable.Add( typeof( MerchantGuildmaster ) );
			ConversionTable.Add( typeof( MinerGuildmaster ) );
			ConversionTable.Add( typeof( RangerGuildmaster ) );
			ConversionTable.Add( typeof( TailorGuildmaster ) );
			ConversionTable.Add( typeof( ThiefGuildmaster ) );
			ConversionTable.Add( typeof( TinkerGuildmaster ) );
			ConversionTable.Add( typeof( WarriorGuildmaster ) );
			// Townfolk
			ConversionTable.Add( typeof( Actor ) );
			ConversionTable.Add( typeof( Artist ) );
			ConversionTable.Add( typeof( Banker ) );
			ConversionTable.Add( typeof( BrideGroom ) );
			ConversionTable.Add( typeof( EscortableMage ) );
			ConversionTable.Add( typeof( Gypsy ) );
			ConversionTable.Add( typeof( HarborMaster ) );
			ConversionTable.Add( typeof( Merchant ) );
			ConversionTable.Add( typeof( Messenger ) );
			ConversionTable.Add( typeof( Minter ) );
			ConversionTable.Add( typeof( Ninja ) );
			ConversionTable.Add( typeof( Noble ) );
			ConversionTable.Add( typeof( Peasant ) );
			ConversionTable.Add( typeof( Samurai ) );
			ConversionTable.Add( typeof( Sculptor ) );
			ConversionTable.Add( typeof( SeekerOfAdventure ) );
			ConversionTable.Add( typeof( TownCrier ) );
			// Guards
			ConversionTable.Add( typeof( ArcherGuard ) );
			ConversionTable.Add( typeof( WarriorGuard ) );
			// Healers
			ConversionTable.Add( typeof( EvilHealer ) );
			ConversionTable.Add( typeof( EvilWanderingHealer ) );
			//ConversionTable.Add( typeof( FortuneTeller ) );
			ConversionTable.Add( typeof( Healer ) );
			ConversionTable.Add( typeof( PricedHealer ) );
			ConversionTable.Add( typeof( WanderingHealer ) );
			#endregion

			#region Animals
			// Animals Bears
			ConversionTable.Add( typeof( BlackBear ) );
			ConversionTable.Add( typeof( BrownBear ) );
			ConversionTable.Add( typeof( GrizzlyBear ) );
			ConversionTable.Add( typeof( PolarBear ) );
			// Animals Birds
			ConversionTable.Add( typeof( Chicken ) );
			ConversionTable.Add( typeof( Crane ) );
			ConversionTable.Add( typeof( Eagle ) );
			ConversionTable.Add( typeof( Phoenix ) );
			// Animals Canines
			ConversionTable.Add( typeof( DireWolf ) );
			ConversionTable.Add( typeof( GreyWolf ) );
			ConversionTable.Add( typeof( TimberWolf ) );
			ConversionTable.Add( typeof( WhiteWolf ) );
			// Animals Cows
			ConversionTable.Add( typeof( Bull ) );
			ConversionTable.Add( typeof( Cow ) );
			// Animals Felines
			ConversionTable.Add( typeof( Cougar ) );
			ConversionTable.Add( typeof( HellCat ) );
			ConversionTable.Add( typeof( Panther ) );
			ConversionTable.Add( typeof( PredatorHellCat ) );
			ConversionTable.Add( typeof( SnowLeopard ) );
			// Animals Misc
			ConversionTable.Add( typeof( Boar ) );
			ConversionTable.Add( typeof( BullFrog ) );
			ConversionTable.Add( typeof( Dolphin ) );
			ConversionTable.Add( typeof( Gaman ) );
			ConversionTable.Add( typeof( GiantToad ) );
			ConversionTable.Add( typeof( Goat ) );
			ConversionTable.Add( typeof( Gorilla ) );
			ConversionTable.Add( typeof( GreatHart ) );
			ConversionTable.Add( typeof( Hind ) );
			ConversionTable.Add( typeof( Llama ) );
			ConversionTable.Add( typeof( MountainGoat ) );
			ConversionTable.Add( typeof( PackHorse ) );
			ConversionTable.Add( typeof( PackLlama ) );
			ConversionTable.Add( typeof( Pig ) );
			ConversionTable.Add( typeof( Sheep ) );
			ConversionTable.Add( typeof( Walrus ) );
			// Animals Mounts
			ConversionTable.Add( typeof( Beetle ) );
			ConversionTable.Add( typeof( DesertOstard ) );
			ConversionTable.Add( typeof( FireSteed ) );
			ConversionTable.Add( typeof( ForestOstard ) );
			ConversionTable.Add( typeof( HellSteed ) );
			ConversionTable.Add( typeof( Hiryu ) );
			ConversionTable.Add( typeof( Horse ) );
			ConversionTable.Add( typeof( Kirin ) );
			ConversionTable.Add( typeof( LesserHiryu ) );
			ConversionTable.Add( typeof( RidableLlama ) );
			ConversionTable.Add( typeof( Ridgeback ) );
			ConversionTable.Add( typeof( SavageRidgeback ) );
			ConversionTable.Add( typeof( ScaledSwampDragon ) );
			ConversionTable.Add( typeof( SeaHorse ) );
			ConversionTable.Add( typeof( SilverSteed ) );
			ConversionTable.Add( typeof( SkeletalMount ) );
			ConversionTable.Add( typeof( SwampDragon ) );
			ConversionTable.Add( typeof( Unicorn ) );
			// Animals Mounts War Horses
			ConversionTable.Add( typeof( CoMWarHorse ) );
			ConversionTable.Add( typeof( MinaxWarHorse ) );
			ConversionTable.Add( typeof( SLWarHorse ) );
			ConversionTable.Add( typeof( TBWarHorse ) );
			// Animals Reptiles
			ConversionTable.Add( typeof( Alligator ) );
			ConversionTable.Add( typeof( GiantSerpent ) );
			ConversionTable.Add( typeof( IceSerpent ) );
			ConversionTable.Add( typeof( IceSnake ) );
			ConversionTable.Add( typeof( LavaLizard ) );
			ConversionTable.Add( typeof( LavaSerpent ) );
			ConversionTable.Add( typeof( LavaSnake ) );
			ConversionTable.Add( typeof( SilverSerpent ) );
			ConversionTable.Add( typeof( Snake ) );
			// Animals Rodents
			ConversionTable.Add( typeof( GiantRat ) );
			ConversionTable.Add( typeof( JackRabbit ) );
			ConversionTable.Add( typeof( Rabbit ) );
			ConversionTable.Add( typeof( Sewerrat ) );
			// Animals Slimes
			ConversionTable.Add( typeof( Jwilson ) );
			// Animals Town Critters
			ConversionTable.Add( typeof( Bird ) );
			ConversionTable.Add( typeof( Cat ) );
			ConversionTable.Add( typeof( Dog ) );
			ConversionTable.Add( typeof( Rat ) );
			#endregion
			
			#region Special
			ConversionTable.Add( typeof( Barracoon ) );
			ConversionTable.Add( typeof( ChaosGuard ) );
			ConversionTable.Add( typeof( Harrower ) );
			ConversionTable.Add( typeof( HarrowerTentacles ) );
			ConversionTable.Add( typeof( LordOaks ) );
			ConversionTable.Add( typeof( Mephitis ) );
			ConversionTable.Add( typeof( Neira ) );
			ConversionTable.Add( typeof( OrderGuard ) );
			ConversionTable.Add( typeof( Rikktor ) );
			ConversionTable.Add( typeof( Semidar ) );
			ConversionTable.Add( typeof( Serado ) );
			ConversionTable.Add( typeof( ServantOfSemidar ) );
			ConversionTable.Add( typeof( Silvani ) );
			#endregion

			#region Monsters
			// Monsters Ants
			ConversionTable.Add( typeof( AntLion ) );
			ConversionTable.Add( typeof( BlackSolenInfiltratorQueen ) );
			ConversionTable.Add( typeof( BlackSolenInfiltratorWarrior ) );
			ConversionTable.Add( typeof( BlackSolenQueen ) );
			ConversionTable.Add( typeof( BlackSolenWarrior ) );
			ConversionTable.Add( typeof( BlackSolenWorker ) );
			ConversionTable.Add( typeof( RedSolenInfiltratorQueen ) );
			ConversionTable.Add( typeof( RedSolenInfiltratorWarrior ) );
			ConversionTable.Add( typeof( RedSolenQueen ) );
			ConversionTable.Add( typeof( RedSolenWarrior ) );
			ConversionTable.Add( typeof( RedSolenWorker ) );
			// Monsters AOS
			ConversionTable.Add( typeof( AbysmalHorror ) );
			ConversionTable.Add( typeof( BoneDemon ) );
			ConversionTable.Add( typeof( CrystalElemental ) );
			ConversionTable.Add( typeof( DarknightCreeper ) );
			ConversionTable.Add( typeof( DemonKnight ) );
			ConversionTable.Add( typeof( Devourer ) );
			ConversionTable.Add( typeof( FleshGolem ) );
			ConversionTable.Add( typeof( FleshRenderer ) );
			ConversionTable.Add( typeof( Gibberling ) );
			ConversionTable.Add( typeof( GoreFiend ) );
			ConversionTable.Add( typeof( Impaler ) );
			ConversionTable.Add( typeof( MoundOfMaggots ) );
			ConversionTable.Add( typeof( PatchworkSkeleton ) );
			ConversionTable.Add( typeof( Ravager ) );
			ConversionTable.Add( typeof( Revenant ) );
			ConversionTable.Add( typeof( ShadowKnight ) );
			ConversionTable.Add( typeof( SkitteringHopper ) );
			ConversionTable.Add( typeof( Treefellow ) );
			ConversionTable.Add( typeof( VampireBat ) );
			ConversionTable.Add( typeof( WailingBanshee ) );
			ConversionTable.Add( typeof( WandererOfTheVoid ) );
			// Monsters Arachnid Magic
			ConversionTable.Add( typeof( DreadSpider ) );
			ConversionTable.Add( typeof( TerathanAvenger ) );
			ConversionTable.Add( typeof( TerathanMatriarch ) );
			// Monsters Arachnid Melee
			ConversionTable.Add( typeof( FrostSpider ) );
			ConversionTable.Add( typeof( GiantBlackWidow ) );
			ConversionTable.Add( typeof( GiantSpider ) );
			ConversionTable.Add( typeof( TerathanDrone ) );
			ConversionTable.Add( typeof( TerathanWarrior ) );
			// Monsters Elemental Magic
			ConversionTable.Add( typeof( AirElemental ) );
			ConversionTable.Add( typeof( BloodElemental ) );
			ConversionTable.Add( typeof( Efreet ) );
			ConversionTable.Add( typeof( FireElemental ) );
			ConversionTable.Add( typeof( IceElemental ) );
			ConversionTable.Add( typeof( PoisonElemental ) );
			ConversionTable.Add( typeof( ToxicElemental ) );
			ConversionTable.Add( typeof( WaterElemental ) );
			// Monsters Elemental Melee
			ConversionTable.Add( typeof( EarthElemental ) );
			ConversionTable.Add( typeof( SnowElemental ) );
			// Monsters Humanoid Magic
			ConversionTable.Add( typeof( AncientLich ) );
			ConversionTable.Add( typeof( ArcaneDaemon ) );
			ConversionTable.Add( typeof( Balron ) );
			ConversionTable.Add( typeof( Betrayer ) );
			ConversionTable.Add( typeof( Bogle ) );
			ConversionTable.Add( typeof( BoneMagi ) );
			ConversionTable.Add( typeof( Daemon ) );
			ConversionTable.Add( typeof( ElderGazer ) );
			ConversionTable.Add( typeof( EvilMage ) );
			ConversionTable.Add( typeof( EvilMageLord ) );
			ConversionTable.Add( typeof( FireGargoyle ) );
			ConversionTable.Add( typeof( Gargoyle ) );
			ConversionTable.Add( typeof( GargoyleDestroyer ) );
			ConversionTable.Add( typeof( GargoyleEnforcer ) );
			ConversionTable.Add( typeof( Gazer ) );
			ConversionTable.Add( typeof( GolemController ) );
			ConversionTable.Add( typeof( IceFiend ) );
			ConversionTable.Add( typeof( Imp ) );
			ConversionTable.Add( typeof( Lich ) );
			ConversionTable.Add( typeof( LichLord ) );
			ConversionTable.Add( typeof( OrcishMage ) );
			ConversionTable.Add( typeof( RatmanMage ) );
			ConversionTable.Add( typeof( SavageShaman ) );
			ConversionTable.Add( typeof( Shade ) );
			ConversionTable.Add( typeof( SkeletalMage ) );
			ConversionTable.Add( typeof( Spectre ) );
			ConversionTable.Add( typeof( Spellbinder ) );
			ConversionTable.Add( typeof( Succubus ) );
			ConversionTable.Add( typeof( Titan ) );
			ConversionTable.Add( typeof( Wraith ) );
			// Monsters Humanoid Melee
			// LBR Exodus
			// LBR Jukas
			// LBR Meers
			// Mammel Melee
			// Misc Magic
			// Misc Melee
			// ML Animal
			// ML Humanoid Magic
			// ML Humanoid Melee
			// ML Misc Magic
			// ML Misc Melee
			// ML Special
			// Ore Elementals
			// Plant Magic
			// Plant Melee
			// Reptile Magic
			// Reptile Melee
			// SE
			// Summons
			#endregion


			//ConversionTable.Add( typeof(  ) );

		}

		public static Spawner LoadMobile( Item item )
		{
			Type t = GetType( item.Z - Offset );

			if ( t == null )
				return null;

			Spawner spawner = new Spawner();
			//spawner.SpawnNames.Add( t.ToString() );
			spawner.Running = true;
			spawner.HomeRange = 0;
			spawner.WalkingRange = 4;
			//spawner.Respawn();
			return spawner;
		}

		public static int GetSaveFlag( Mobile m )
		{
			return ConversionTable.IndexOf( m.GetType() ) + Offset;
		}

		public static Type GetType( int saveflag )
		{
			if ( saveflag > 0 && saveflag < ConversionTable.Count )
				return ConversionTable[saveflag];
			else
				return null;
		}
	}
}