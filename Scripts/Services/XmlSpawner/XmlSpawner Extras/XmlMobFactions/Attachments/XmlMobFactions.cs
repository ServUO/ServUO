using System;
using System.Data;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Spells;
using System.Collections.Generic;
using Server.Commands.Generic;
using Server.Commands;
using Server.Gumps;
using System.Text;

/*
** XmlMobFactions
** 10/27/04
** ArteGordon
**
** This attachment will allow you to create a system for creating mob factions that can be maintained on a player or a piece of equipment
** Mob factions can be set up to add/subtract faction based upon mobs that are killed within the specified group or in opponent groups
** Mob factions can also be created to be used in quests or other systems that can modify faction through means other than killing mobs
*/
namespace Server.Engines.XmlSpawner2
{
	public class XmlMobFactions : XmlAttachment
	{
		private static Group [] m_KillGroups;       // this is the list of groups that gain/lose faction through killing mobs

		public static Group[] KillGroups { get{ return m_KillGroups; } }
		
		private bool verboseMobFactions = true;   // determines whether faction gain/lost messages are displayed on each kill. True by default

		private int m_Credits = 0;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int Credits { get{ return m_Credits; } set { m_Credits = value; } }
		
		// this scaling to determines credits gained per mob kill.
		// default is set to 0.1% of fame.  Note, regardless of the scaling at least 1 credit will be gained per kill
		private static double m_CreditScale = 0.001;

		// these are the mapping between faction levels and titles
		public static string GetTitle(int level)
		{
			if(level < -20000)
				return "AttackOnSight";
			if(level >= -20000 && level < -15000)
				return "Hated";
			if(level >= -15000 && level < -10000)
				return "Despised";
			if(level >= -10000 && level < -5000)
				return "Disliked";
			if(level >= -5000 && level < -500)
				return "Distrusted";
			if(level >= -500 && level <= 500)
				return "Neutral";
			if(level <= 5000 && level > 500)
				return "Trusted";
			if(level <= 10000 && level > 5000)
				return "Liked";
			if(level <= 15000 && level > 10000)
				return "Admired";
			if(level <= 20000 && level > 15000)
				return "Revered";
			if(level <= 25000 && level > 20000)
				return "Worshipped";
			if(level > 25000)
				return "Untouchable";
			
			// should never get here
			return null;
		}

		// ------------------------------------------------------------------------------
		// BEGINNING of user-defined faction information
		// ------------------------------------------------------------------------------

		// these are the Members that define group members
		private static Type[] PlayerMembers =
			new Type[]
				{
					typeof( PlayerMobile )
				};

		private static Type[] HumanoidMembers =
			new Type[]
				{
					typeof( Ogre ), typeof( OgreLord ), typeof( ArcticOgreLord ),typeof( Orc ), typeof( OrcishMage ),
					typeof( OrcishLord ), typeof( OrcBrute ), typeof( OrcBomber ), typeof( OrcCaptain ), typeof( Troll ),
					typeof( Cyclops ), typeof( Titan ), typeof( Juggernaut ), typeof( JukaMage ), typeof( JukaLord ),
					typeof( JukaWarrior ), typeof( Betrayer ), typeof( Ettin ), typeof( Ratman ),
					typeof( RatmanMage ), typeof( RatmanArcher )
				};

		private static Type[] UndeadMembers =
			new Type[]
				{
					typeof( AncientLich ), typeof( Bogle ), typeof( BoneMagi ), typeof( Lich ), typeof( LichLord ),
					typeof( Shade ), typeof( Spectre ), typeof( Wraith ), typeof( BoneKnight ), typeof( Ghoul ),
					typeof( Mummy ), typeof( SkeletalKnight ), typeof( Skeleton ), typeof( Zombie ), typeof(Succubus),
					typeof( SkeletalMage), typeof( RottingCorpse )
				};

		private static Type[] ReptilianMembers =
			new Type[]
				{
					typeof( AncientWyrm ), typeof( Dragon ), typeof( Drake ), typeof( SerpentineDragon ), typeof( ShadowWyrm ),
					typeof( SkeletalDragon ), typeof( SwampDragon ), typeof( WhiteWyrm ), typeof( Wyvern ) , typeof( Lizardman ),
					typeof( OphidianArchmage ), typeof( OphidianKnight ), typeof( OphidianMage ), typeof( OphidianMatriarch ), typeof( OphidianWarrior ),
					typeof( IceSerpent ), typeof( GiantIceWorm ), typeof( GiantSerpent ), typeof( IceSnake ), typeof( LavaSerpent ), typeof( LavaSnake ),
					typeof( SilverSerpent ), typeof( Snake ), typeof( SeaSerpent ), typeof( Kraken), typeof( DeepSeaSerpent)
				};

		private static Type[] ArachnidMembers =
			new Type[]
				{
					typeof( DreadSpider ), typeof( FrostSpider ), typeof( GiantBlackWidow ), typeof( GiantSpider ),
					typeof( TerathanAvenger ), typeof( TerathanDrone ), typeof( TerathanMatriarch ), typeof( TerathanWarrior ),
					typeof( Scorpion ), typeof( Mephitis ), typeof(SolenHelper), typeof( RedSolenWorker), typeof( RedSolenWarrior),
					typeof( RedSolenQueen), typeof( RedSolenInfiltratorWarrior), typeof( RedSolenInfiltratorQueen), typeof( AntLion),
					typeof( Beetle)
				};

		private static Type[] ElementalMembers =
			new Type[]
				{
					typeof( BloodElemental ), typeof( EarthElemental ), typeof( SummonedEarthElemental ), typeof( AgapiteElemental ),
					typeof( BronzeElemental ), typeof( CopperElemental ), typeof( DullCopperElemental ), typeof( GoldenElemental ),
					typeof( ShadowIronElemental ), typeof( ValoriteElemental ), typeof( VeriteElemental ), typeof( PoisonElemental ),
					typeof( FireElemental ), typeof( SummonedFireElemental ), typeof( SnowElemental ), typeof( AirElemental ),
					typeof( SummonedAirElemental ), typeof( WaterElemental ), typeof( SummonedWaterElemental ), typeof(CrystalElemental),
					typeof(AcidElemental), typeof( Efreet )
				};
		private static Type[] AbyssMembers =
			new Type[]
				{
					typeof( AbysmalHorror ), typeof( Balron ), typeof( BoneDemon ), typeof( ChaosDaemon ), typeof( Daemon ),
					typeof( SummonedDaemon ), typeof( DemonKnight ), typeof( Gargoyle ), typeof( FireGargoyle ),
					typeof( HordeMinion ), typeof( IceFiend ), typeof( Imp ),
					typeof( StoneGargoyle ), typeof(HellHound), typeof(EnslavedGargoyle), typeof(Nightmare)
				};

		private static Type[] FairieMembers =
			new Type[]
				{
					typeof( Pixie ), typeof( Wisp ), typeof( ShadowWisp ), typeof( EtherealWarrior )
				};
		private static Type[] PlantMembers =
			new Type[]
				{
					typeof( SwampTentacle ), typeof( Quagmire ), typeof( Corpser ), typeof( BogThing ), typeof( Bogling ), typeof( WhippingVine ),
					typeof( Reaper )
				};
		private static Type[] UnderworldMembers =
			new Type[]
				{
					typeof( Gazer), typeof(ElderGazer), typeof(WandererOfTheVoid), typeof(WailingBanshee), typeof(VampireBat), typeof(Revenant),
					typeof(Ravager), typeof(Impaler), typeof(GoreFiend), typeof(Gibberling), typeof(FleshRenderer), typeof(PatchworkSkeleton),
					typeof(FleshGolem), typeof(FleshRenderer), typeof(Devourer), typeof(DarknightCreeper), typeof(AbysmalHorror),
					typeof(SkitteringHopper), typeof(MoundOfMaggots)
				};


		// this is the list that contains ALL of the group types that can have faction.
		// Note that just because they are listed here does not mean they will actually have a faction associated with them.   Faction groups have to be
		// set up in the Initialize method.
		// You can add as many as you like but End_Unused MUST be the last grouptype
		public enum GroupTypes
		{
			Player,
			Humanoid,
			Undead,
			Reptilian,
			Arachnid,
			Elemental,
			Abyss,
			DragonLords,
			NecroMasters,
			Fairie,
			Plant,
			Underworld,
			End_Unused         // End_Unused MUST remain at the end of the list
		};



		// all mob factions are set up here
		public static new void Initialize()
		{
			// stress testing
			//Timer.DelayCall( TimeSpan.FromSeconds(15),TimeSpan.FromSeconds(15), new TimerCallback( XmlSpawner.XmlTrace_OnCommand ) );

			CommandSystem.Register("VerboseMobFactions", AccessLevel.Player, new CommandEventHandler(VerboseMobFactions_OnCommand));
			CommandSystem.Register("AddAllMobFactions", AccessLevel.Administrator, new CommandEventHandler(AddAllMobFactions_OnCommand));
			CommandSystem.Register("RemoveAllMobFactions", AccessLevel.Administrator, new CommandEventHandler(RemoveAllMobFactions_OnCommand));
			CommandSystem.Register("CheckMobFactions", AccessLevel.Player, new CommandEventHandler(CheckMobFactions_OnCommand));

			// set up all of the mob factions

			Group PlayerGroup = new Group(GroupTypes.Player);
			Group UndeadGroup = new Group(GroupTypes.Undead);
			Group HumanoidGroup = new Group(GroupTypes.Humanoid);
			Group ArachnidGroup = new Group(GroupTypes.Arachnid);
			Group ReptilianGroup = new Group(GroupTypes.Reptilian);
			Group ElementalGroup = new Group(GroupTypes.Elemental);
			Group AbyssGroup = new Group(GroupTypes.Abyss);
			Group DragonLordsGroup = new Group();
			Group NecroMastersGroup = new Group(GroupTypes.NecroMasters);
			Group FairieGroup = new Group(GroupTypes.Fairie);
			Group PlantGroup = new Group(GroupTypes.Plant);
			Group UnderworldGroup = new Group(GroupTypes.Underworld);

			// these groups have Members and opponents lists that determine which mobs give faction as well as the multipliers for the
			// amount of faction gained and lost by killing
			PlayerGroup.Opponents = new Group[] {ArachnidGroup, HumanoidGroup, UndeadGroup, ReptilianGroup, ElementalGroup, AbyssGroup};
			PlayerGroup.OpponentGain = new double [] { 2,        1,              2,          2,              1,              4 };    // scale factor for faction gained by opponent groups
			PlayerGroup.Allies = new Group[] { PlayerGroup, FairieGroup };
			PlayerGroup.AllyLoss = new double [] { 30.0,    10 };          // scale factor for faction lost for killing within group
			PlayerGroup.Members = new List<Type>(PlayerMembers);

			UndeadGroup.Opponents = new Group[] {HumanoidGroup, PlayerGroup, FairieGroup};
			UndeadGroup.OpponentGain = new double [] {  1,       0.1,        0.5 };
			UndeadGroup.Allies = new Group[] { UndeadGroup, AbyssGroup };
			UndeadGroup.AllyLoss = new double [] {1.2,      0.4};
			UndeadGroup.Members = new List<Type>(UndeadMembers);
			UndeadGroup.DynamicFaction = "Undead";

			HumanoidGroup.Opponents = new Group[] {UndeadGroup, PlayerGroup, PlantGroup};
			HumanoidGroup.OpponentGain = new double [] {  1,       0.05,     0.5 };
			HumanoidGroup.Allies = new Group[] { HumanoidGroup };
			HumanoidGroup.AllyLoss = new double [] {1.2};
			HumanoidGroup.Members = new List<Type>(HumanoidMembers);
			HumanoidGroup.DynamicFaction = "Humanoid";

			ArachnidGroup.Opponents = new Group[] {ReptilianGroup, PlayerGroup, FairieGroup};
			ArachnidGroup.OpponentGain = new double [] {  1,       0.15,        0.3 };
			ArachnidGroup.Allies = new Group[] { ArachnidGroup, PlantGroup };
			ArachnidGroup.AllyLoss = new double [] {1.2,        0.5 };
			ArachnidGroup.Members = new List<Type>(ArachnidMembers);
			ArachnidGroup.DynamicFaction = "Arachnid";

			ReptilianGroup.Opponents = new Group[] {ArachnidGroup, PlayerGroup};
			ReptilianGroup.OpponentGain = new double [] {  1,       0.1 };
			ReptilianGroup.Allies = new Group[] { ReptilianGroup };
			ReptilianGroup.AllyLoss = new double [] {1.2};
			ReptilianGroup.Members = new List<Type>(ReptilianMembers);
			ReptilianGroup.DynamicFaction = "Reptilian";

			ElementalGroup.Opponents = new Group[] {AbyssGroup, PlayerGroup, PlantGroup};
			ElementalGroup.OpponentGain = new double [] {  1,       0.05,    0.5 };
			ElementalGroup.Allies = new Group[] { ElementalGroup };
			ElementalGroup.AllyLoss = new double [] {1.2};
			ElementalGroup.Members = new List<Type>(ElementalMembers);
			ElementalGroup.DynamicFaction = "Elemental";

			AbyssGroup.Opponents = new Group[] {ElementalGroup, PlayerGroup, FairieGroup};
			AbyssGroup.OpponentGain = new double [] {  1,       0.2,           1 };
			AbyssGroup.Allies = new Group[] { AbyssGroup, UndeadGroup };
			AbyssGroup.AllyLoss = new double [] {1.2,      0.4};
			AbyssGroup.Members = new List<Type>(AbyssMembers);
			AbyssGroup.DynamicFaction = "Abyss";

			FairieGroup.Opponents = new Group[] {ArachnidGroup, UndeadGroup, AbyssGroup};
			FairieGroup.OpponentGain = new double [] {  0.5,       0.7,       1 };
			FairieGroup.Allies = new Group[] { FairieGroup, PlayerGroup };
			FairieGroup.AllyLoss = new double [] { 1.2,     0.5};
			FairieGroup.Members = new List<Type>(FairieMembers);
			FairieGroup.DynamicFaction = "Fairie";

			PlantGroup.Opponents = new Group[] {ElementalGroup, HumanoidGroup};
			PlantGroup.OpponentGain = new double [] {  0.5,       0.7 };
			PlantGroup.Allies = new Group[] { PlantGroup };
			PlantGroup.AllyLoss = new double [] { 1.2 };
			PlantGroup.Members = new List<Type>(PlantMembers);
			PlantGroup.DynamicFaction = "Plant";
			
			UnderworldGroup.Opponents = new Group[] { AbyssGroup, HumanoidGroup};
			UnderworldGroup.OpponentGain = new double [] {  0.5,       0.7 };
			UnderworldGroup.Allies = new Group[] { UnderworldGroup, UndeadGroup, ElementalGroup };
			UnderworldGroup.AllyLoss = new double [] { 1.2,          1.0,        1.0};
			UnderworldGroup.Members = new List<Type>(UnderworldMembers);
			UnderworldGroup.DynamicFaction = "Underworld";

			// this group does not have a predefined set of members but instead uses only the dynamic faction system to determine whether a mob
			// is a member of the group
			NecroMastersGroup.Members = new List<Type>();
			NecroMastersGroup.DynamicFaction = "NecroMasters";
			NecroMastersGroup.Allies = new Group[] { NecroMastersGroup };
			NecroMastersGroup.AllyLoss = new double [] { 1 };

			// Note that the groups do not have to have opponents or members and therefore do not have to gain/lose faction by killing mobs
			// these could be used to maintain factions that are set by quests or other events
			DragonLordsGroup.GroupType = GroupTypes.DragonLords;

			// only have to list groups here that gain/lose faction through killing mobs and/or should be actively checked for AI related functions
			// such as target acquisition, taming, mob control etc.  
			// You can leave out groups that are intended to be passive placeholders for faction, such as the DragonLordsGroup example.

			// stress testing
			//m_KillGroups = new Group[1000];
			//for(int i=0;i<1000;i++) m_KillGroups[i] = AbyssGroup;

			m_KillGroups = new Group[]
			{
				PlayerGroup,
				UndeadGroup,
				HumanoidGroup,
				ArachnidGroup,
				ReptilianGroup,
				ElementalGroup,
				AbyssGroup,
				FairieGroup,
				PlantGroup,
				UnderworldGroup,
				NecroMastersGroup
			};

		}

		// define these properties to allow public access to faction levels for the various groups
		// if you add new faction groups, you should add a faction property for them

		[CommandProperty( AccessLevel.GameMaster )]
		public int Player { get { return GetFactionLevel(GroupTypes.Player); } set { SetFactionLevel(GroupTypes.Player,value); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Humanoid { get { return GetFactionLevel(GroupTypes.Humanoid); } set { SetFactionLevel(GroupTypes.Humanoid,value); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Undead { get { return GetFactionLevel(GroupTypes.Undead); } set { SetFactionLevel(GroupTypes.Undead,value); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Reptilian{ get { return GetFactionLevel(GroupTypes.Reptilian); } set { SetFactionLevel(GroupTypes.Reptilian,value); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Arachnid { get { return GetFactionLevel(GroupTypes.Arachnid); } set { SetFactionLevel(GroupTypes.Arachnid,value); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Elemental { get { return GetFactionLevel(GroupTypes.Elemental); } set { SetFactionLevel(GroupTypes.Elemental,value); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Abyss { get { return GetFactionLevel(GroupTypes.Abyss); } set { SetFactionLevel(GroupTypes.Abyss,value); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int DragonLords { get { return GetFactionLevel(GroupTypes.DragonLords); } set { SetFactionLevel(GroupTypes.DragonLords,value); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int NecroMasters { get { return GetFactionLevel(GroupTypes.NecroMasters); } set { SetFactionLevel(GroupTypes.NecroMasters,value); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Fairie { get { return GetFactionLevel(GroupTypes.Fairie); } set { SetFactionLevel(GroupTypes.Fairie,value); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Plant { get { return GetFactionLevel(GroupTypes.Plant); } set { SetFactionLevel(GroupTypes.Plant,value); } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int Underworld { get { return GetFactionLevel(GroupTypes.Underworld); } set { SetFactionLevel(GroupTypes.Underworld,value); } }

		// ------------------------------------------------------------------------------
		// END of user-defined faction information
		// ------------------------------------------------------------------------------


		// these variables control the way in which faction level affects mob target acquisition if the BaseAI AquireFocusMob modification has been made.
		// These settings work best when combined with a reduction in the BaseCreature ReaquireDelay setting from the default of 10 secs to 1-2 secs.
		static int baseValue = 180000;      // increasing this increases the faction effect on delayed acquisition. Higher=longer average delay
		static int maxFaction = 25000;      // this sets the faction level above which mobs will no longer acquire the target

		// This method will calculate an target acquisition probability based upon the targets faction level with the mob
		// This was intended to be called from BaseAI in the AquireFocusMob method
		// Higher faction will lead to longer acquisition time
		// Lower or negative faction will lead to faster acquisition time
		// mob will be the creature and target will be the player
		public static bool CheckAcquire(Mobile mob, Mobile target)
		{
			// only acquisition of players is affected by faction
			if(!target.Player)
			{
				return true;
			}

			// by default, mobs that arent in a faction group will have the target acquisition probability calculated as though
			// they had a faction level of 0

			int facvalue = (int)GetScaledFaction(target, mob, -baseValue, maxFaction, 1);

			// return true if the target can be acquired by the mob.
			// higher mob faction on the target means lower probability of acquisition
			bool cantarget = (baseValue + facvalue) < Utility.Random(baseValue + maxFaction);
			
			return cantarget;

		}

		public static double GetScaledFaction(Mobile from, Mobile mob, double min, double max, double scale)
		{
			if(from == null || mob == null || !from.Player) return 0;

			double facvalue = 0;

			//XmlMobFactions x = (XmlMobFactions)XmlAttach.FindAttachment(XmlAttach.MobileAttachments, from, typeof(XmlMobFactions), "Standard");

			XmlMobFactions x = XmlAttach.FindAttachment(from, typeof(XmlMobFactions), "Standard") as XmlMobFactions;
			if(x != null)
			{
				//if(x != null)
				//{

				int count = 0;
				// find any static groups that this mob belongs to.  Note, if it belongs to more than one group, calculate the average.
				List<GroupTypes> glist = FindGroups(mob);

				if(glist != null && glist.Count > 0)
				{
					foreach( GroupTypes g in glist)
					{

						if(g != GroupTypes.End_Unused)
						{
							facvalue += x.GetFactionLevel(g)*scale;
							count++;
						}
					}
				}

				// does this mob have dynamic faction assignments?
				List<XmlAttachment> dlist = XmlAttach.FindAttachments(mob, typeof(XmlDynamicFaction));
				if(dlist != null && dlist.Count > 0)
				{

					//if(XmlAttach.FindAttachment(XmlAttach.MobileAttachments, mob, typeof(XmlDynamicFaction)) != null)
					//{
					// do this for dynamic factions as well
					List<GroupTypes> dglist = DynamicFindGroups(mob);

					if(dglist != null && dglist.Count > 0)
					{
						foreach( GroupTypes g in dglist)
						{
	
							if(g != GroupTypes.End_Unused)
							{
								facvalue += x.GetFactionLevel(g)*scale;
								count++;
							}
						}
					}
				}
				
				// compute the average faction value
				if(count > 0)
				{
					facvalue /= count;
				}

			}
			if(facvalue > max) facvalue = max;
			if(facvalue < min) facvalue = min;

			return facvalue;
		}

		public static int GetCredits(Mobile m)
		{
			int val = 0;

			XmlMobFactions x = XmlAttach.FindAttachment(m, typeof(XmlMobFactions), "Standard") as  XmlMobFactions;
			if(x != null)
			{
				val = x.Credits;
			}

			return val;
		}
		
		public static bool HasCredits(Mobile m, int credits)
		{
			if(m == null || m.Deleted) return false;

			XmlMobFactions x = XmlAttach.FindAttachment(m, typeof(XmlMobFactions), "Standard") as XmlMobFactions;
			if(x != null)
			{
				if(x.Credits >= credits)
				{
					return true;
				}
			}

			return false;
		}

		public static bool TakeCredits(Mobile m, int credits)
		{
			if(m == null || m.Deleted) return false;

			XmlMobFactions x = XmlAttach.FindAttachment(m, typeof(XmlMobFactions), "Standard") as XmlMobFactions;
			if(x != null)
			{
				if(x.Credits >= credits)
				{
					x.Credits -= credits;
					return true;
				}
			}

			return false;
		}

		public static int GetFactionLevel(Mobile m, GroupTypes grouptype)
		{
			if(m == null || grouptype == GroupTypes.End_Unused) return 0;

			XmlMobFactions x = XmlAttach.FindAttachment(m, typeof(XmlMobFactions), "Standard") as XmlMobFactions;
			if(x != null)
			{
				if(x.FactionList == null ) return 0;

				foreach(GroupStatus g in x.FactionList)
				{
					if(g.GroupType == grouptype) return g.FactionLevel;
				}
			}
			return 0;
		}

		private static Dictionary<Type, List<XmlMobFactions.GroupTypes>> GroupTypeHash = new Dictionary<Type, List<XmlMobFactions.GroupTypes>>();

		// this method returns a list of the group types that the mobile belongs to using the KillGroups master list
		public static List<GroupTypes> FindGroups(Mobile m)
		{
			if(m == null) return null;

			List<GroupTypes> list;
			// see whether this mobile type is already in the dictionary
			if(GroupTypeHash.TryGetValue(m.GetType(), out list) && list!=null)
			{
				// then get the list from there
				return GroupTypeHash[m.GetType()];
			}

			list = new List<XmlMobFactions.GroupTypes>();

			foreach(Group g in KillGroups)
			{
				if(MatchType(g.Members, m))
				{
					list.Add(g.GroupType);
				}
			}
			GroupTypeHash[m.GetType()]=list;

			return list;
		}
		
		// this method returns a list of the group types that the mobile belongs based upon dynamic factions
		public static List<GroupTypes> DynamicFindGroups(Mobile m)
		{
			if(m == null) return null;

			List<GroupTypes> list = new List<XmlMobFactions.GroupTypes>();

			foreach(Group g in KillGroups)
			{
				if(XmlDynamicFaction.MatchFaction(m, g.DynamicFaction))
				{
					list.Add(g.GroupType);
				}
			}

			return list;
		}

		public static Group FindGroup(GroupTypes gtype)
		{
			foreach(Group g in KillGroups)
			{
				if(g.GroupType == gtype)
				{
					return g;
				}
			}
			return null;
		}

		private static bool MatchType(List<Type> list, Mobile m)
		{
			if(list == null || m == null) return false;

			foreach(Type o in list)
			{
				if(o == m.GetType() || o.IsSubclassOf(m.GetType())) return true;
			}
			return false;
		}

		public class Group
		{
			public string DynamicFaction;
			public GroupTypes GroupType;//an attachment added to this list? this allows what? we can't add manually attachment in this way!
			public List<Type> Members;
			public Group []    Opponents;
			public Group []    Allies;
			public double [] AllyLoss;        // scale factor for faction lost by killing this group
			public double [] OpponentGain;        // scale factor for faction gained by opponent groups for killing this group
			
			public Group(GroupTypes type)
			{
				GroupType = type;
			}
			public Group()
			{
				GroupType = GroupTypes.End_Unused;
			}
		}

		private class GroupStatus
		{
			public GroupTypes GroupType;
			public int     FactionLevel;
			
			public GroupStatus(GroupTypes type)
			{
				GroupType = type;
			}
			public GroupStatus()
			{
				GroupType = GroupTypes.End_Unused;
			}
		}

		private GroupStatus [] FactionList;           // this keeps track of all of the faction information in the attachment
		private TimeSpan m_Refractory = TimeSpan.Zero;    // 0 seconds default time between activations
		private DateTime m_EndTime;


		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan Refractory { get { return m_Refractory; } set { m_Refractory  = value; } }


		public int GetFactionLevel(GroupTypes grouptype)
		{
			if(FactionList == null ) return 0;

			foreach(GroupStatus g in FactionList)
			{
				if(g.GroupType == grouptype) return g.FactionLevel;
			}
			return 0;
		}

		public void SetFactionLevel(GroupTypes grouptype, int value)
		{
			if(FactionList == null ) return;

			foreach(GroupStatus g in FactionList)
			{
				if(g.GroupType == grouptype)
				{
					g.FactionLevel = value;
					return;
				}
			}
		}

		[Usage( "VerboseMobFactions [true/false]" )]
		[Description( "Turns on/off display of faction gain/loss on mob kills" )]
		public static void VerboseMobFactions_OnCommand( CommandEventArgs e )
		{
			// get the mob factions attachment
			XmlMobFactions x = XmlAttach.FindAttachment(e.Mobile, typeof(XmlMobFactions), "Standard") as XmlMobFactions;
			if(x != null)
			{
				if(e.Arguments.Length > 0)
				{
					try
					{
						x.verboseMobFactions = bool.Parse(e.Arguments[0]);
					} 
					catch{}

				}

				e.Mobile.SendMessage("VerboseMobFactions is set to {0}",x.verboseMobFactions);

			} 
			else
			{
				e.Mobile.SendMessage("Standard XmlMobFactions attachment not found");
			}
		}

		[Usage( "CheckMobFactions" )]
		[Description( "Reports faction levels" )]
		public static void CheckMobFactions_OnCommand( CommandEventArgs e )
		{
			// get the mob factions attachment
			XmlMobFactions x = XmlAttach.FindAttachment(e.Mobile, typeof(XmlMobFactions), "Standard") as XmlMobFactions;
			if(x != null)
			{
				e.Mobile.SendMessage("{0}", x.OnIdentify(e.Mobile));
			}
		}

		[Usage( "AddAllMobFactions" )]
		[Description( "Adds the standard XmlMobFaction attachment to all players" )]
		public static void AddAllMobFactions_OnCommand( CommandEventArgs e )
		{
			int count = 0;
			foreach(Mobile m in World.Mobiles.Values)
			{
				if(m.Player)
				{
					// does this player already have a points attachment?
					XmlMobFactions x = XmlAttach.FindAttachment(m, typeof(XmlMobFactions), "Standard") as XmlMobFactions;
					if(x == null)
					{
						x = new XmlMobFactions();
						XmlAttach.AttachTo(e.Mobile, m, x);
						count++;
					}
				}
			}
			e.Mobile.SendMessage("Added XmlMobFaction attachments to {0} players",count);
		}

		[Usage( "RemoveAllMobFactions" )]
		[Description( "Removes the standard XmlMobFaction attachment from all players" )]
		public static void RemoveAllMobFactions_OnCommand( CommandEventArgs e )
		{
			int count = 0;
			int total = 0;
			foreach(Mobile m in World.Mobiles.Values)
			{
				if(m.Player)
				{
					List<XmlAttachment> list = XmlAttach.FindAttachments(XmlAttach.MobileAttachments,  m, typeof(XmlMobFactions), "Standard");
					if(list != null && list.Count > 0)
					{
						foreach(XmlAttachment x in list)
						{
							x.Delete();
						}
						count++;
					}
					total++;
				}
			}
			e.Mobile.SendMessage("Removed XmlMobFaction attachments from {0} players ({1} total players)", count, total);
		}

		// These are the various ways in which the message attachment can be constructed.
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments

		// a serial constructor is REQUIRED
		public XmlMobFactions(ASerial serial) : base(serial)
		{
		}

		[Attachable]
		public XmlMobFactions(string name, double refractory)
		{
			Name = name;
			Refractory = TimeSpan.FromSeconds(refractory);
			FactionList = new GroupStatus[(int)GroupTypes.End_Unused];
			for(int i = 0;i<FactionList.Length;i++)
			{
				FactionList[i] = new GroupStatus();
				FactionList[i].GroupType = (GroupTypes)i;
			}
		}
		
		[Attachable]
		public XmlMobFactions() : this("Standard", 0)
		{
		}

		[Attachable]
		public XmlMobFactions(double refractory) : this("Standard", refractory)
		{
		}

		private bool SameGuild(Mobile killed, Mobile killer)
		{
			return ( killer.Guild == killed.Guild && killer.Guild != null && killed.Guild != null);
		}

		public override bool HandlesOnKill { get { return true; } }

		private static Dictionary<Type, List<XmlMobFactions.Group>> GroupHash = new Dictionary<Type, List<XmlMobFactions.Group>>();

		private bool m_ChallengeStatus = false;

		public override void OnBeforeKill(Mobile killed, Mobile killer )
		{
			// if you have XmlPoints installed and wish to prevent challenge games and duels from affecting
			// faction points then uncomment the following line
			//m_ChallengeStatus = XmlPoints.AreChallengers(killer, killed);
		}

		public override void OnKill(Mobile killed, Mobile killer )
		{
			base.OnKill(killed, killer);

			// supports ignoring XmlPoints challenges
			if(m_ChallengeStatus)
			{
				m_ChallengeStatus = false;
				return;
			}

			if(killed == null || killer == null || killer == killed) return;
			
			// check for within guild kills and ignore them
			if(SameGuild(killed,killer)) return;

			// this calculates the base faction level that will be gained/lost based upon the fame of the killed mob
			double value = (double)(killed.Fame/1000.0);
			if(value <= 0) value = 1;

			// calculates credits gained in a similar way
			int cval = (int)(killed.Fame*m_CreditScale);
			if(cval <= 0) cval = 1;

			Credits += cval;

			// prepare the group lists that will be checked for faction
			List<XmlMobFactions.Group> glist = null;
			List<XmlMobFactions.Group> dglist = null;

			// check to see whether this mob type has already been hashed into a group list
			if(!GroupHash.TryGetValue(killed.GetType(), out glist) || glist==null)
			{
				// otherwise look it up the slow way and prepare a hash entry for it at the same time
				// unless it is using dynamic faction
				glist = new List<XmlMobFactions.Group>();
				foreach(Group g in KillGroups)
				{
					if(MatchType(g.Members, killed))
					{
						glist.Add(g);
					}
				}
				GroupHash[killed.GetType()]=glist;
			}

			// have to look up dynamic factions the exhaustive way
			// does this mob have dynamic faction assignments?
			XmlAttachment dynam = XmlAttach.FindAttachment(killed, typeof(XmlDynamicFaction));
			if(dynam != null)
			{
				//if(XmlAttach.FindAttachment(XmlAttach.MobileAttachments, killed, typeof(XmlDynamicFaction)) != null)
				//{
				dglist = new List<XmlMobFactions.Group>();
				foreach(Group g in KillGroups)
				{
					if(XmlDynamicFaction.MatchFaction(killed, g.DynamicFaction))
					{
						dglist.Add(g);
					}
				}
			}

			List<List<XmlMobFactions.Group>> alist = new List<List<XmlMobFactions.Group>>();
			if(glist != null && glist.Count > 0)
				alist.Add(glist);
			if(dglist != null && dglist.Count > 0)
				alist.Add(dglist);

			//  go through this with static and dynamic factions
			foreach(List<XmlMobFactions.Group> al in alist)
			{
				foreach(Group g in al)
				{
					// tabulate the faction loss from target group allies
					if(g.Allies != null && g.Allies.Length > 0)
					{
						for(int i = 0; i< g.Allies.Length;i++)
						{
							Group ally = g.Allies[i];

							int facloss = 0;
							try
							{
								facloss = (int)(value*g.AllyLoss[i]);
							} 
							catch{}
							if(facloss <= 0) facloss = 1;
		
							int p = GetFactionLevel(ally.GroupType) - facloss;
							SetFactionLevel(ally.GroupType, p);
							if(verboseMobFactions)
								killer.SendMessage("lost {0} faction {1}", ally.GroupType,facloss);
						}
					}

					// tabulate the faction gain from target group opponents
					if(g.Opponents != null && g.Opponents.Length > 0)
					{
						for(int i = 0; i< g.Opponents.Length;i++)
						{
							Group opp = g.Opponents[i];

							int facgain = 0;
							try
							{
								facgain = (int)(value*g.OpponentGain[i]);
							} 
							catch {}
							if(facgain <= 0) facgain = 1;

							int p = GetFactionLevel(opp.GroupType) + facgain;
							SetFactionLevel(opp.GroupType, p);
							if(verboseMobFactions)
								killer.SendMessage("gained {0} faction {1}",opp.GroupType, facgain);
						}
					}
				}
			}

			m_EndTime = DateTime.UtcNow + Refractory;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);

			writer.Write( (int) 1 );

			// version 2
			// version 1
			writer.Write( m_Credits );
			// version 0
			writer.Write( verboseMobFactions );
			if(FactionList != null)
			{
				writer.Write(FactionList.Length);
				for(int i = 0;i<FactionList.Length;i++)
				{
					GroupStatus g = FactionList[i];
					// by saving the group type as a string, it allows proper deserialization even if the list of faction group types change
					writer.Write(g.GroupType.ToString());
					writer.Write(g.FactionLevel);
				}
			} 
			else
			{
				writer.Write((int)0);
			}
			writer.Write(m_Refractory);
			writer.Write(m_EndTime - DateTime.UtcNow);

		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			switch(version)
			{
				case 1:
					m_Credits = reader.ReadInt();
					goto case 0;
				case 0:
					// version 0
					verboseMobFactions = reader.ReadBool();
					int count = reader.ReadInt();
					int newcount = (int)GroupTypes.End_Unused;;

					// prepare the new faction status list
					FactionList = new GroupStatus[newcount];

					// initialize the faction status list with the default grouptypes
					for(int j = 0;j<newcount;j++)
					{
						FactionList[j] = new GroupStatus((GroupTypes)j);
					}

					// now read in the serialized FactionList entries and cross reference to the current GroupTypes
					for(int i = 0;i<count;i++)
					{
						string gname = reader.ReadString();
						int gfac = reader.ReadInt();

						// look up the enum by name
						GroupTypes gtype = GroupTypes.End_Unused;
						if(!BaseXmlSpawner.TryParse(gname, out gtype))
							gtype=GroupTypes.End_Unused;

						// try to find the matching entry in the recently constructed faction status list
						if(gtype != GroupTypes.End_Unused)
						{
							for(int j = 0;j<newcount;j++)
							{
								GroupStatus g = FactionList[j];
								if(g.GroupType == gtype)
								{
									g.FactionLevel = gfac;
									break;
								}
							}
						}
					}

					Refractory = reader.ReadTimeSpan();
					TimeSpan remaining = reader.ReadTimeSpan();
					m_EndTime = DateTime.UtcNow + remaining;
					break;
			}
		}
		
		private class MobFactionGump : Gump
		{
			private XmlMobFactions m_attachment;
			private string m_text;

			public MobFactionGump( Mobile from, XmlMobFactions a, string text) : base( 0,0)
			{
				if(a == null) return;

				m_attachment = a;
				m_text = text;

				// prepare the page
				AddPage( 0 );
	
				AddBackground( 0, 0, 400, 330, 5054 );
				AddAlphaRegion( 0, 0, 400, 330 );
				AddLabel( 20, 2, 55, "Mob Faction Standings" );

				if(a != null)
				{
					AddLabel( 200, 2, 30, String.Format("Available Credits: {0}", a.Credits) );
				}

				AddHtml( 20,20, 360, 260, text, true , true );
				
				// add the verbose factions checkbox
				AddLabel( 50, 290, 55, "Verbose Factions" );
				AddButton( 20, 290, (a.verboseMobFactions ? 0xD3 :0xD2), (a.verboseMobFactions ? 0xD2 :0xD3), 100, GumpButtonType.Reply, 0);
			}
			
			public override void OnResponse( NetState state, RelayInfo info )
			{

				if(m_attachment == null || state == null || state.Mobile == null || info == null) return;
				
				switch(info.ButtonID)
				{
					case 100:
						m_attachment.verboseMobFactions = !m_attachment.verboseMobFactions;
					
						state.Mobile.SendGump( new MobFactionGump(state.Mobile, m_attachment, m_text));
						break;
				}

			}
		}

		public override string OnIdentify(Mobile from)
		{
			// dont let other people identify your faction standings
			if(AttachedTo is Mobile && (from != (Mobile)AttachedTo && from != null && from.AccessLevel == AccessLevel.Player)) return null;

			// display the faction status in a gump
			StringBuilder gumpmsg = new StringBuilder();

			gumpmsg.AppendFormat("\n{0,-15}{1,-15}{2,-10}\n",
				"Mob Faction","Standing","Level");
			if(FactionList != null)
				foreach(GroupStatus g in FactionList)
				{
					gumpmsg.AppendFormat("{0,-15}{1,-15}{2,-10}\n",
						g.GroupType, GetTitle(g.FactionLevel), g.FactionLevel);
				}


			if(from != null)
			{
				from.CloseGump(typeof(MobFactionGump));
				from.SendGump(new MobFactionGump(from, this, gumpmsg.ToString()));
			}
			string msg = null;

			if(Expiration > TimeSpan.Zero)
			{
				msg = String.Format("{0}expires in {0} mins ", msg, Expiration.TotalMinutes);
			}

			if(Refractory > TimeSpan.Zero)
			{
				return String.Format("{0}with a minimum of {1} secs between gains",msg, Refractory.TotalSeconds);
			} 
			else
				return msg;
		}
	}
}
