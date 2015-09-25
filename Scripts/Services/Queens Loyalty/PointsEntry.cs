using System;
using Server;
using Server.Mobiles;
using Server.Items;
using System.Collections.Generic;
using Server.Engines.Quests;

namespace Server.Engines.QueensLoyalty
{
	public class PointsEntry
	{
		public static void Initialize()
		{
			m_Entries[typeof(FireAnt)] 				= new PointsEntry(typeof(FireAnt), 				2, .2);
			m_Entries[typeof(LavaLizard)] 			= new PointsEntry(typeof(LavaLizard), 			2, .2);
			m_Entries[typeof(LavaSnake)] 			= new PointsEntry(typeof(LavaSnake), 			2, .2);
			m_Entries[typeof(CoralSnake)] 			= new PointsEntry(typeof(CoralSnake), 			3, .3);
			m_Entries[typeof(Korpre)] 				= new PointsEntry(typeof(Korpre), 				3, .3);
			m_Entries[typeof(Ortanord)] 			= new PointsEntry(typeof(Ortanord), 			3, .3);
			m_Entries[typeof(ClanRC)] 	            = new PointsEntry(typeof(ClanRC), 	            5, .5);
			m_Entries[typeof(Daemon)] 				= new PointsEntry(typeof(Daemon), 				5, .5);
			m_Entries[typeof(TrapdoorSpider)] 		= new PointsEntry(typeof(TrapdoorSpider), 		5, .5);
			m_Entries[typeof(Gremlin)] 				= new PointsEntry(typeof(Gremlin), 				5, .5);
			m_Entries[typeof(WolfSpider)] 			= new PointsEntry(typeof(WolfSpider), 			5, .5);
			m_Entries[typeof(GrayGoblin)] 			= new PointsEntry(typeof(GrayGoblin), 			8, .8);
			m_Entries[typeof(GreenGoblin)] 			= new PointsEntry(typeof(GreenGoblin), 			8, .8);
			m_Entries[typeof(Anlorzen)] 			= new PointsEntry(typeof(Anlorzen), 			10, 1);
			m_Entries[typeof(Anzuanord)] 			= new PointsEntry(typeof(Anzuanord), 			10, 1);
            m_Entries[typeof(Ballem)]               = new PointsEntry(typeof(Ballem),               10, 1);
			m_Entries[typeof(LowlandBoura)] 		= new PointsEntry(typeof(LowlandBoura), 		10, 1);
			m_Entries[typeof(Kepetch)] 				= new PointsEntry(typeof(Kepetch), 				10, 1);
			m_Entries[typeof(GrayGoblinKeeper)] 	= new PointsEntry(typeof(GrayGoblinKeeper), 	10, 1);
			m_Entries[typeof(GrayGoblinMage)] 		= new PointsEntry(typeof(GrayGoblinMage), 		10, 1);
			m_Entries[typeof(GreenGoblinAlchemist)] = new PointsEntry(typeof(GreenGoblinAlchemist), 10, 1);
			m_Entries[typeof(GreenGoblinScout)] 	= new PointsEntry(typeof(GreenGoblinScout), 	10, 1);
			m_Entries[typeof(Rotworm)] 				= new PointsEntry(typeof(Rotworm), 				10, 1);
			m_Entries[typeof(Skree)] 				= new PointsEntry(typeof(Skree), 				15, 1.5);
			m_Entries[typeof(Slith)] 				= new PointsEntry(typeof(Slith), 				15, 1.5);
			m_Entries[typeof(BloodWorm)] 			= new PointsEntry(typeof(BloodWorm), 			15, 1.5);
			m_Entries[typeof(HighPlainsBoura)] 		= new PointsEntry(typeof(HighPlainsBoura), 		15, 1.5);
			m_Entries[typeof(PutridUndeadGargoyle)] = new PointsEntry(typeof(PutridUndeadGargoyle), 15, 1.5);
			m_Entries[typeof(Anlorlem)] 			= new PointsEntry(typeof(Anlorlem), 			20, 2);
			m_Entries[typeof(Ballem)] 				= new PointsEntry(typeof(Ballem), 				20, 2);
			m_Entries[typeof(Raptor)] 				= new PointsEntry(typeof(Raptor), 				20, 2);
			m_Entries[typeof(Relanord)] 			= new PointsEntry(typeof(Relanord), 			20, 2);
			m_Entries[typeof(StoneSlith)] 			= new PointsEntry(typeof(StoneSlith), 			20, 2);
			m_Entries[typeof(LavaElemental)] 		= new PointsEntry(typeof(LavaElemental), 		20, 2);
			m_Entries[typeof(IronBeetle)] 			= new PointsEntry(typeof(IronBeetle), 			20, 2);
			m_Entries[typeof(KepetchAmbusher)] 		= new PointsEntry(typeof(KepetchAmbusher), 		25, 2.5);
			m_Entries[typeof(ToxicSlith)] 			= new PointsEntry(typeof(ToxicSlith), 			30, 3);
			m_Entries[typeof(Anlorvaglem)] 			= new PointsEntry(typeof(Anlorvaglem), 			50, 5);
			m_Entries[typeof(VitaviRenowned)] 		= new PointsEntry(typeof(VitaviRenowned), 		50, 5);
			m_Entries[typeof(WyvernRenowned)] 		= new PointsEntry(typeof(WyvernRenowned), 		50, 5);
            m_Entries[typeof(MinionOfScelestus)]    = new PointsEntry(typeof(MinionOfScelestus),    35, 3.5);
            /*m_Entries[typeof(GargishRouser)]        = new PointsEntry(typeof(GargishRouser),        50, 5.0);
            m_Entries[typeof(GargishOutcast)]       = new PointsEntry(typeof(GargishOutcast),       25, 2.5);
            m_Entries[typeof(VoidManifestation)]    = new PointsEntry(typeof(VoidManifestation),    50, 5);*/
			m_Entries[typeof(Navrey)] 				= new PointsEntry(typeof(Navrey), 				75, 7.5);
			m_Entries[typeof(Niporailem)] 			= new PointsEntry(typeof(Niporailem), 			75, 7.5);
			
            //Quests
			m_Entries[typeof(ABrokenVaseQuest)]     			= new PointsEntry(typeof(ABrokenVaseQuest),      				5, 0.5);
			m_Entries[typeof(PuttingThePiecesTogetherQuest)]    = new PointsEntry(typeof(PuttingThePiecesTogetherQuest),      	15, 1.5);
			m_Entries[typeof(TheExchangeQuest)]			        = new PointsEntry(typeof(TheExchangeQuest),      		        35, 3.5);
			m_Entries[typeof(YeOldeGargishQuest)]     			= new PointsEntry(typeof(YeOldeGargishQuest),      				50, 5.0);
			m_Entries[typeof(AWorthyPropositionQuest)]			= new PointsEntry(typeof(AWorthyPropositionQuest),     			50, 5.0);
			m_Entries[typeof(UnusualGoods)]     				= new PointsEntry(typeof(UnusualGoods),      					75, 7.5);
		}
		
		private static Dictionary<Type, PointsEntry> m_Entries = new Dictionary<Type, PointsEntry>();
		public static Dictionary<Type, PointsEntry> Entries { get { return m_Entries; } }
		
		private Type m_Type;
		private double m_TopAttackerPoints;
		private double m_RightsPoints;
		
		public Type KillType { get { return m_Type; } }
		public double TopAttackerPoints { get { return m_TopAttackerPoints; } }
		public double RightsPoints { get { return m_RightsPoints; } }
		
		public PointsEntry(Type type, double topAttacker, double rights)
		{
			m_Type = type;
			m_TopAttackerPoints = topAttacker;
			m_RightsPoints = rights;
		}
	}
}