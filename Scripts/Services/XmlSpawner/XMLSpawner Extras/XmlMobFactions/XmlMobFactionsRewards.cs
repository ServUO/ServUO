using System;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections;

/*
** XmlMobFactionsRewards
** ArteGordon
** updated 11/09/04
**
** this class lets you specify rewards that can be purchased for XmlMobFactions kill Credits.
** The items will be displayed in the MobFactionsRewardGump that is opened by the MobFactionsRewardStone
*/

namespace Server.Engines.XmlSpawner2
{
    public class XmlMobFactionsRewards
    {
        public int Cost;       // cost of the reward in credits
        public XmlMobFactions.GroupTypes RequiredFaction;       //  faction requirement
        public int MinFaction;                                  // min faction level
        public Type  RewardType;   // this will be used to create an instance of the reward
        public string Name;         // used to describe the reward in the gump
        public int ItemID;     // used for display purposes
        public object [] RewardArgs; // arguments passed to the reward constructor

        private static ArrayList    MobFactionsRewardList = new ArrayList();

        public static ArrayList RewardsList { get { return MobFactionsRewardList; } }
        
        public XmlMobFactionsRewards(string faction, int minfaction, Type reward, string name, int cost, int id, object[] args)
        {
            RewardType = reward;
            Cost = cost;
            ItemID = id;
            Name = name;
            RewardArgs = args;
            RequiredFaction = XmlMobFactions.GroupTypes.End_Unused;
            if(faction != null)
                try{
                    RequiredFaction = (XmlMobFactions.GroupTypes)Enum.Parse(typeof(XmlMobFactions.GroupTypes), faction);
                } catch{};
            MinFaction = minfaction;
        }
        
        public static void Initialize()
        {
            // these are items as reward's. Note that the args list must match a constructor for the reward type specified.
            MobFactionsRewardList.Add( new XmlMobFactionsRewards( "Player", 1000, typeof(LeatherGlovesOfMining), "+20 Leather Gloves Of Mining", 20000, 0x13c6, new object[] { 20 }));
            MobFactionsRewardList.Add( new XmlMobFactionsRewards( "Player", 1000, typeof(ColoredAnvil), "Colored Anvil", 40000, 0xFAF, null ));
            MobFactionsRewardList.Add( new XmlMobFactionsRewards( "Player", 1000, typeof(PowderOfTemperament), "Powder Of Temperament, 10 uses", 30000, 4102, new object[] { 10 }));
            //MobFactionsRewardList.Add( new XmlMobFactionsRewards( "Player", 1000, typeof(SocketHammer), "Socket Hammer, 50 uses", 10000, 0x13E4, null ));
            //MobFactionsRewardList.Add( new XmlMobFactionsRewards( "Player", 1000, typeof(BagOfHolding), "Bag of Holding, 10 items", 20000, 0xE76, new object[] { 10 } ));
            //MobFactionsRewardList.Add( new XmlMobFactionsRewards( "Player", 1000, typeof(BagOfHolding), "Bag of Holding, 20 items", 50000, 0xE76, new object[] { 20 } ));
            MobFactionsRewardList.Add( new XmlMobFactionsRewards( "Player", 2500, typeof(AncientSmithyHammer), "+20 Ancient Smithy Hammer, 50 uses", 50000, 0x13E4, new object[] { 20, 50 }));
            MobFactionsRewardList.Add( new XmlMobFactionsRewards( "Player", 5000, typeof(PowerScroll), "105 Smithing powerscroll", 100000, 0x14F0, new object[] { SkillName.Blacksmith, 105 }));
            MobFactionsRewardList.Add( new XmlMobFactionsRewards( "Player", 8000, typeof(PowerScroll), "110 Smithing powerscroll", 200000, 0x14F0, new object[] { SkillName.Blacksmith, 110 }));
            MobFactionsRewardList.Add( new XmlMobFactionsRewards( "Player", 12000, typeof(PowerScroll), "115 Smithing powerscroll", 350000, 0x14F0, new object[] { SkillName.Blacksmith, 115 }));
            MobFactionsRewardList.Add( new XmlMobFactionsRewards( "Player", 20000, typeof(PowerScroll), "120 Smithing powerscroll", 600000, 0x14F0, new object[] { SkillName.Blacksmith, 120 }));

            // this is an example of adding a mobile as a reward
            //MobFactionsRewardList.Add( new XmlMobFactionsRewards( null, 0, typeof(RidableLlama),"Ridable Llama", 1000, 0x20f6, null));
            
            // this is an example of adding an attachment as a reward
            //MobFactionsRewardList.Add( new XmlMobFactionsRewards( "Elemental", 1000, typeof(XmlEnemyMastery), "+200% Balron Mastery for 1 day", 3000, 0, new object[] { "Balron", 50, 200, 1440.0 }));
            //MobFactionsRewardList.Add( new XmlMobFactionsRewards( "Abyss", 2000, typeof(XmlFactionMastery), "+200% Elemental Mastery for 1 day", 5000, 0, new object[] { "Elemental", 50, 200, 1440.0 }));
            //MobFactionsRewardList.Add( new XmlMobFactionsRewards( null, 0, typeof(XmlStr), "+20 Strength for 1 day", 10000, 0, new object[] { 20, 86400.0 }));
        }

    }
}
