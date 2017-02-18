using System;
using Server.Items;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class HumilityQuestMobile : BaseVendor
    {
        public virtual int Greeting { get { return 0; } }

        public override bool IsActiveVendor { get { return false; } }
        public override bool IsInvulnerable { get { return true; } }
        public override bool CanTeach { get { return false; } }
        public override bool PlayerRangeSensitive { get { return false; } }

        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        public override void InitSBInfo()
        {
        }

        public HumilityQuestMobile(string name) : base(null)
        {
            Name = name;
            m_NextGreet = DateTime.UtcNow;
        }

        public HumilityQuestMobile(string name, string title) : base(title)
        {
            Name = name;
            m_NextGreet = DateTime.UtcNow;
        }

        public static List<HumilityQuestMobile> Instance { get { return m_Instances; } }
        private static List<HumilityQuestMobile> m_Instances = new List<HumilityQuestMobile>();

        public HumilityQuestMobile(Serial serial) : base(serial)
        {
        }

        private DateTime m_NextGreet;

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            PlayerMobile pm = m as PlayerMobile;

            if (pm == null || !pm.InRange(this.Location, 3))
                return;

            WhosMostHumbleQuest quest = QuestHelper.GetQuest(pm, typeof(WhosMostHumbleQuest)) as WhosMostHumbleQuest;

            if (quest != null)
            {
                if (m_NextGreet < DateTime.UtcNow && pm is PlayerMobile)
                {
                    Item item = pm.FindItemOnLayer(Layer.Cloak);

                    if (item is GreyCloak && ((GreyCloak)item).Owner == null && Greeting > 0)
                    {
                        SayTo(pm, Greeting);

                        m_NextGreet = DateTime.UtcNow + TimeSpan.FromSeconds(5);
                    }
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (pm == null || !InRange(from.Location, 3))
                return;

            WhosMostHumbleQuest quest = QuestHelper.GetQuest(pm, typeof(WhosMostHumbleQuest)) as WhosMostHumbleQuest;

            if (quest != null && pm.Backpack != null && !quest.HasGivenTo(this))
            {
                Item item = from.FindItemOnLayer(Layer.Cloak);

                if (item is GreyCloak && ((GreyCloak)item).Owner == null)
                {
                    int idx = HumilityQuestMobileInfo.GetNPCIndex(this.GetType());

                    if (idx > -1 && quest.Infos.ContainsKey(idx) && idx < quest.Infos.Count)
                    {
                        Type needs = quest.Infos[idx].Needs;

                        Item need = from.Backpack.FindItemByType(needs);

                        // Found needed item
                        if (need != null)
                        {
                            need.Delete();
                            quest.RemoveQuestItem(need);

                            Item nextItem = Loot.Construct(quest.Infos[idx].Gives);

                            if (nextItem != null)
                            {
                                from.Backpack.DropItem(nextItem);
                                quest.AddQuestItem(nextItem, this);

                                if (this is Sean)
                                    SayTo(from, Greeting + 3, String.Format("#{0}", quest.Infos[idx].NeedsLoc));
                                else
                                    SayTo(from, Greeting + 4, String.Format("#{0}\t#{1}", quest.Infos[idx].NeedsLoc, quest.Infos[idx].GivesLoc));
                            }
                        }
                        else //Didn't find needed item
                        {
                            from.SendGump(new HumilityItemQuestGump(this, quest, idx));
                        }
                    }
                    else
                        Console.WriteLine("Error finding index for {0}", this);
                }
                else
                    base.OnDoubleClick(from);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_NextGreet = DateTime.UtcNow;
        }
    }

    public class HumilityQuestMobileInfo
    {
        // Greeting(say), Greeting2(gump), Desire(say), Gift(say), OnExchange(say)

        private Type m_Needs;
        private Type m_Gives;
        private int m_NeedsLoc;
        private int m_GivesLoc;

        public Type Needs { get { return m_Needs; } }
        public Type Gives { get { return m_Gives; } }
        public int NeedsLoc { get { return m_NeedsLoc; } }
        public int GivesLoc { get { return m_GivesLoc; } }

        public HumilityQuestMobileInfo(Type needs, Type gives, int needsLoc, int givesLoc)
        {
            m_Needs = needs;
            m_Gives = gives;
            m_NeedsLoc = needsLoc;
            m_GivesLoc = givesLoc;
        }

        public HumilityQuestMobileInfo(GenericReader reader)
        {
            int version = reader.ReadInt();

            int needs = reader.ReadInt();
            int gives = reader.ReadInt();

            m_Needs = m_ItemTypes[needs];

            if (gives == -1)
                m_Gives = typeof(IronChain);
            else
                m_Gives = m_ItemTypes[gives];

            m_NeedsLoc = reader.ReadInt();
            m_GivesLoc = reader.ReadInt();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)0);

            int needs = Array.IndexOf(m_ItemTypes, m_Needs);
            writer.Write(needs);

            int gives = Array.IndexOf(m_ItemTypes, m_Gives);
            writer.Write(gives);

            writer.Write(m_NeedsLoc);
            writer.Write(m_GivesLoc);
        }

        public static int GetNPCIndex(Type type)
        {
            for (int i = 0; i < m_MobileTypes.Length; i++)
            {
                if (m_MobileTypes[i] == type)
                    return i;
            }

            return -1;
        }

        public static int GetLoc(Type type)
        {
            for (int i = 0; i < m_ItemTypes.Length; i++)
            {
                if (type == m_ItemTypes[i])
                    return m_ItemLocs[i];
            }

            return -1;
        }

        public static Type[] ItemTypes { get { return m_ItemTypes; } }
        private static Type[] m_ItemTypes = new Type[]
        {
            typeof(BrassRing),
            typeof(SeasonedSkillet), 
            typeof(VillageCauldron),
            typeof(ShortStool),
            typeof(FriendshipMug),
            typeof(WornHammer),
            typeof(PairOfWorkGloves),
            typeof(IronChain)
        };

        public static int[] ItemLocs { get { return m_ItemLocs; } }
        private static int[] m_ItemLocs = new int[]
        {
            1075778,
            1075774, 
            1075775,
            1075776,
            1075777,
            1075779,
            1075780,
            1075788
        };

        public static Type[] MobileTypes { get { return m_MobileTypes; } }
        private static Type[] m_MobileTypes = new Type[]
        {
            typeof(Maribel), 
            typeof(Dierdre),
            typeof(Kevin),
            typeof(Jason),
            typeof(Walton),
            typeof(Nelson),
            typeof(Sean)
        };
    }
}