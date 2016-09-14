using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Engines.Quests;
using System.Collections.Generic;

namespace Server.Engines.Points
{
	public class QueensLoyalty : PointsSystem
	{
		public enum QueensLoyaltyRating
		{
			Enemy,
			Friend,
			Citizen,
			Noble
		}
	
		public override PointsType Loyalty { get { return PointsType.QueensLoyalty; } }
		public override TextDefinition Name { get { return m_Name; } }
		public override bool AutoAdd { get { return true; } }
		public override double MaxPoints { get { return 15000; } }

        private TextDefinition m_Name = new TextDefinition(1095163);

        public QueensLoyalty()
		{
			InitializeEntries();
		}
		
		public override void ProcessKill(BaseCreature victim, Mobile damager, int index)
		{
			if(victim.Map != Map.TerMur || damager.Map != Map.TerMur)
				return;
				
			Type type = victim.GetType();

            if (damager is BaseCreature && (((BaseCreature)damager).Controlled || ((BaseCreature)damager).Summoned))
                damager = ((BaseCreature)damager).GetMaster();

            if (damager == null)
                return;

			if(index == 0)
			{
				if(Entries.ContainsKey(type))
					AwardPoints(damager, Entries[type].Item1, false);
			}
			else
			{
				if(Entries.ContainsKey(type))
					AwardPoints(damager, Entries[type].Item2, false);
			}
		}
		
		public override void ProcessQuest(Mobile from, BaseQuest quest)
		{
			if(from == null || quest == null)
				return;
				
			Type type = quest.GetType();
				
			if(Entries.ContainsKey(type))
				AwardPoints(from, Entries[type].Item1, true);
		}
		
		public override void OnPlayerAdded(PlayerMobile pm)
		{
            if (pm.Race == Race.Gargoyle)
            {
                AwardPoints(pm, 2000, false, false);
            }
		}
		
		public bool IsNoble(Mobile from)
		{
			return GetLoyalty(from as PlayerMobile) >= QueensLoyaltyRating.Noble;
		}
		
		public QueensLoyaltyRating GetLoyalty(PlayerMobile from)
		{
            if (from == null)
                return QueensLoyaltyRating.Enemy;

			double points = GetPoints(from);
			
			if(points <= 0)
                return QueensLoyaltyRating.Enemy;
            if(points <= 1999)
                return QueensLoyaltyRating.Friend;
            if(points <= 9999)
                return QueensLoyaltyRating.Citizen;
                   
            return QueensLoyaltyRating.Noble;
		}
		
		public override TextDefinition GetTitle(PlayerMobile from)
		{
			switch(GetLoyalty(from))
			{
				default:
                case QueensLoyaltyRating.Friend: return new TextDefinition(1095166);
                case QueensLoyaltyRating.Noble: return new TextDefinition(1095167);
                case QueensLoyaltyRating.Enemy: return new TextDefinition(1095164);
                case QueensLoyaltyRating.Citizen: return new TextDefinition(1095165);
			}
		}
		
		public Dictionary<Type, Tuple<double, double>> Entries;
		
		public void InitializeEntries()
		{
			Entries = new Dictionary<Type, Tuple<double, double>>();
			
			Entries[typeof(FireAnt)] 				= new Tuple<double, double>(2, .2);
			Entries[typeof(LavaLizard)] 			= new Tuple<double, double>(2, .2);
			Entries[typeof(LavaSnake)] 				= new Tuple<double, double>(2, .2);
			Entries[typeof(CoralSnake)] 			= new Tuple<double, double>(3, .3);
			Entries[typeof(Korpre)] 				= new Tuple<double, double>(3, .3);
			Entries[typeof(Ortanord)] 				= new Tuple<double, double>(3, .3);
            //Entries[typeof(ClanRibbonCourtier)]     = new Tuple<double, double>(5, .5);
			Entries[typeof(Daemon)] 				= new Tuple<double, double>(5, .5);
			Entries[typeof(TrapdoorSpider)] 		= new Tuple<double, double>(5, .5);
			Entries[typeof(Gremlin)] 				= new Tuple<double, double>(5, .5);
			Entries[typeof(WolfSpider)] 			= new Tuple<double, double>(5, .5);
			Entries[typeof(GrayGoblin)] 			= new Tuple<double, double>(8, .8);
			Entries[typeof(GreenGoblin)] 			= new Tuple<double, double>(8, .8);
			Entries[typeof(Anlorzen)] 				= new Tuple<double, double>(10, 1);
			Entries[typeof(Anzuanord)] 				= new Tuple<double, double>(10, 1);
            Entries[typeof(Ballem)]               	= new Tuple<double, double>(10, 1);
			Entries[typeof(LowlandBoura)] 			= new Tuple<double, double>(10, 1);
			Entries[typeof(Kepetch)] 				= new Tuple<double, double>(10, 1);
			Entries[typeof(GrayGoblinKeeper)] 		= new Tuple<double, double>(10, 1);
			Entries[typeof(GrayGoblinMage)] 		= new Tuple<double, double>(10, 1);
			Entries[typeof(GreenGoblinAlchemist)] 	= new Tuple<double, double>(10, 1);
			Entries[typeof(GreenGoblinScout)] 		= new Tuple<double, double>(10, 1);
			Entries[typeof(Rotworm)] 				= new Tuple<double, double>(10, 1);
			Entries[typeof(Skree)] 					= new Tuple<double, double>(15, 1.5);
			Entries[typeof(Slith)] 					= new Tuple<double, double>(15, 1.5);
			Entries[typeof(BloodWorm)] 				= new Tuple<double, double>(15, 1.5);
			Entries[typeof(HighPlainsBoura)] 		= new Tuple<double, double>(15, 1.5);
			Entries[typeof(PutridUndeadGargoyle)] 	= new Tuple<double, double>(15, 1.5);
			Entries[typeof(Anlorlem)] 				= new Tuple<double, double>(20, 2);
			Entries[typeof(Ballem)] 				= new Tuple<double, double>(20, 2);
			Entries[typeof(Raptor)] 				= new Tuple<double, double>(20, 2);
			Entries[typeof(Relanord)] 				= new Tuple<double, double>(20, 2);
			Entries[typeof(StoneSlith)] 			= new Tuple<double, double>(20, 2);
			Entries[typeof(LavaElemental)] 			= new Tuple<double, double>(20, 2);
			Entries[typeof(IronBeetle)] 			= new Tuple<double, double>(20, 2);
			Entries[typeof(KepetchAmbusher)] 		= new Tuple<double, double>(25, 2.5);
			Entries[typeof(ToxicSlith)] 			= new Tuple<double, double>(30, 3);
			Entries[typeof(Anlorvaglem)] 			= new Tuple<double, double>(50, 5);
			Entries[typeof(VitaviRenowned)] 		= new Tuple<double, double>(50, 5);
			Entries[typeof(WyvernRenowned)] 		= new Tuple<double, double>(50, 5);
            Entries[typeof(MinionOfScelestus)]    	= new Tuple<double, double>(35, 3.5);
            Entries[typeof(GargishRouser)]      	= new Tuple<double, double>(50, 5.0);
            Entries[typeof(GargishOutcast)]       	= new Tuple<double, double>(25, 2.5);
            Entries[typeof(VoidManifestation)]    	= new Tuple<double, double>(50, 5);
			Entries[typeof(Navrey)] 				= new Tuple<double, double>(75, 7.5);
			Entries[typeof(Niporailem)] 			= new Tuple<double, double>(75, 7.5);
            Entries[typeof(Navrey)]                 = new Tuple<double, double>(75, 7.5);
            Entries[typeof(StygianDragon)]          = new Tuple<double, double>(150, 15.0);
            Entries[typeof(PrimevalLich)]           = new Tuple<double, double>(150, 15.0);
            Entries[typeof(Medusa)]                 = new Tuple<double, double>(150, 15.0);
            Entries[typeof(AbyssalInfernal)]        = new Tuple<double, double>(150, 15.0);
			
            //Quests
			Entries[typeof(ABrokenVaseQuest)]     				= new Tuple<double, double>(5, 0.5);
			Entries[typeof(PuttingThePiecesTogetherQuest)]  	= new Tuple<double, double>(15, 1.5);
            Entries[typeof(ALittleSomething)]  	                = new Tuple<double, double>(25, 2.5);
			Entries[typeof(TheExchangeQuest)]			        = new Tuple<double, double>(35, 3.5);
			Entries[typeof(YeOldeGargishQuest)]     			= new Tuple<double, double>(50, 5.0);
			Entries[typeof(AWorthyPropositionQuest)]			= new Tuple<double, double>(50, 5.0);
			Entries[typeof(UnusualGoods)]     					= new Tuple<double, double>(75, 7.5);
		}
	}
}