using System; 
using Server;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
	public class MonsterContractType
	{
		public static MonsterContractType[] Get = new MonsterContractType[]
		{
			new MonsterContractType (typeof(Scorpion), 				"Scorpions", 				10),
			new MonsterContractType (typeof(Shade), 				"Shades", 					10),
			new MonsterContractType (typeof(GiantSpider), 			"Giant Spiders",		10),
			new MonsterContractType (typeof(Harpy), 				"Harpies",					10),
			new MonsterContractType (typeof(Lizardman), 			"Lizard Men",			10),
			new MonsterContractType (typeof(AirElemental), 			"Air Elementals",		30),
			new MonsterContractType (typeof(FireElemental), 		"Fire Elementals",		30),
			new MonsterContractType (typeof(WaterElemental), 		"Water Elementals",		30),
			new MonsterContractType (typeof(BloodElemental), 		"Blood Elementals",		50),
			new MonsterContractType (typeof(EarthElemental), 		"Earth Elementals",	20),
			new MonsterContractType (typeof(Imp), 					"Imps",						30),
			new MonsterContractType (typeof(Spectre), 				"Spectres",					30),
			new MonsterContractType (typeof(Gazer), 				"Beholders",				30),
			new MonsterContractType (typeof(Reaper), 				"Reapers",						50),
			new MonsterContractType (typeof(Wraith), 				"Wraiths",				50),
			new MonsterContractType (typeof(OgreLord), 				"OgreLords",			60),
			new MonsterContractType (typeof(Lich), 					"Liches",					60),
			new MonsterContractType (typeof(FrostTroll), 			"Frost Trolls",		70),
			new MonsterContractType (typeof(BoneKnight), 			"Bone Knights",			60),
			new MonsterContractType (typeof(SkeletalDragon), 		"Skeletal Dragons",	80),
			new MonsterContractType (typeof(LichLord), 				"Lich Lords",			80),
			new MonsterContractType (typeof(AncientLich), 			"Ancient Liches",			80),
			new MonsterContractType (typeof(Dragon), 				"Dragons",					80),
			new MonsterContractType (typeof(Daemon), 				"Daemons",					80)  //Attention pas de virgule à la dernière ligne
		};
		
		public static int Random()
		{
			double rarety = Utility.RandomDouble();
			int result = 0;
			for(int i=0;i<10;i++)
			{
				result = Utility.Random(MonsterContractType.Get.Length);
				if(rarety > MonsterContractType.Get[result].Rarety)
					break;
			}
			
			return result;
		}
		
		private Type m_Type;
		public Type Type
		{
			get { return m_Type;}
			set { m_Type = value;}
		}
		
		private string m_Name;
		public string Name
		{
			get { return m_Name;}
			set { m_Name = value;}
		}
		
		private int m_Rarety;
		public int Rarety
		{
			get { return m_Rarety;}
			set { m_Rarety = value;}
		}
		
		public MonsterContractType(Type type, string name, int rarety)
		{
			Type = type;
			Name = name;
			Rarety = rarety;
		}
	}
}
