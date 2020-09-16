using Server.ContextMenus;
using Server.Engines.Quests;
using Server.Items;
using Server.SkillHandlers;

using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [CorpseName("a wisp corpse")]
    public class MysteriousWisp : BaseCreature
    {
        public static readonly int MinBudget = 300;
        public static readonly int MaxBudget = 600;
        public static readonly int ItemCount = 10;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ForceRestock { get { return false; } set { if (value) DoRestock(true); } }

        private readonly Dictionary<Mobile, int> m_Conversation = new Dictionary<Mobile, int>();

        [Constructable]
        public MysteriousWisp()
            : base(AIType.AI_Mage, FightMode.None, 10, 1, 0.2, 0.4)
        {
            CantWalk = true;
            Blessed = true;
            SpeechHue = 52;

            Name = "a mysterious wisp";
            Body = 58;
            BaseSoundID = 466;

            SetStr(196, 225);
            SetDex(196, 225);
            SetInt(196, 225);

            SetHits(118, 135);

            SetDamage(17, 18);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 20, 40);
            SetResistance(ResistanceType.Cold, 10, 30);
            SetResistance(ResistanceType.Poison, 5, 10);
            SetResistance(ResistanceType.Energy, 50, 70);

            SetSkill(SkillName.EvalInt, 80.0);
            SetSkill(SkillName.Magery, 80.0);
            SetSkill(SkillName.MagicResist, 80.0);
            SetSkill(SkillName.Tactics, 80.0);
            SetSkill(SkillName.Wrestling, 80.0);

            if (Backpack != null)
                Backpack.Delete();

            DoRestock();
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            return m_Conversation.ContainsKey(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(Location, 4))
            {
                if (!m_Conversation.ContainsKey(from))
                {
                    SayTo(from, m_Responses[0]);
                    m_Conversation[from] = 1;
                }

                if (Backpack != null)
                {
                    Backpack.DisplayTo(from);
                }
            }

            if (from is PlayerMobile pm && QuestHelper.CheckDoneOnce(pm, typeof(WishesOfTheWispQuest), null, false))
            {
                WhisperingWithWispsQuest q = QuestHelper.GetQuest<WhisperingWithWispsQuest>(pm);

                if (q == null)
                {
                    BaseQuest quest = QuestHelper.RandomQuest(pm, new Type[] { typeof(WhisperingWithWispsQuest) }, this);

                    if (quest != null)
                    {
                        pm.CloseGump(typeof(MondainQuestGump));
                        pm.SendGump(new MondainQuestGump(quest));
                    }
                }
                else if (q.Completed)
                {
                    q.CompleteQuest();
                }           
            }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m is PlayerMobile && InRange(m.Location, 5) && !InRange(oldLocation, 5))
            {
                WishesOfTheWispQuest quest = QuestHelper.GetQuest<WishesOfTheWispQuest>((PlayerMobile)m);

                if (quest != null)
                {
                    quest.CompleteQuest();
                }
            }
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (!m_Conversation.ContainsKey(e.Mobile))
                return;

            string speech = e.Speech.ToLower();
            int idx = m_Conversation[e.Mobile];

            if (idx < 0 || idx >= m_Keywords.Length)
            {
                m_Conversation.Remove(e.Mobile);
                return;
            }

            foreach (string str in m_Keywords[idx])
            {
                if (speech.Contains(str))
                {
                    SayTo(e.Mobile, m_Responses[idx]);

                    if (idx + 1 >= m_Keywords.Length)
                        m_Conversation.Remove(e.Mobile);
                    else
                        m_Conversation[e.Mobile]++;

                    break;
                }
            }
        }

        private readonly string[][] m_Keywords = new string[][]
        {
            new string[] { },
            new string[] { "corporeal" },
            new string[] { "sentient" },
            new string[] { "deal" },
            new string[] { "learn", "teach" },
            new string[] { "good and evil", "good", "evil" },
            new string[] { "guide" },
            new string[] { "follow" },
            new string[] { "fight" },
            new string[] { "resonate" },
            new string[] { "join" },
            new string[] { "command" },
            new string[] { "trade" },
        };

        private readonly int[] m_Responses = new int[]
        {
            1153441,
            1153443,
            1153445,
            1153447,
            1153449,
            1153451,
            1153453,
            1153455,
            1153457,
            1153459,
            1153461,
            1153463,
            1153467
        };

        private void DoRestock()
        {
            DoRestock(false);
        }

        private void DoRestock(bool wipe)
        {
            int count;

            if (Backpack == null)
            {
                var pack = new BuyBackpack();

                AddItem(pack);
            }

            if (wipe)
            {
                count = ItemCount;
                ColUtility.SafeDelete(Backpack.Items);
            }
            else
            {
                count = ItemCount - Backpack.Items.Count;
            }

            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    Item item;

                    switch (Utility.Random(3))
                    {
                        default:
                        case 0: item = Loot.RandomArmorOrShieldOrWeaponOrJewelry(false, false, true); break;
                        case 1: item = Loot.RandomArmorOrShieldOrWeaponOrJewelry(false, true, false); break;
                        case 2: item = Loot.RandomArmorOrShieldOrWeaponOrJewelry(true, false, false); break;
                    }

                    var failSafe = 0;
                    bool success = true;

                    do
                    {
                        RunicReforging.GenerateRandomItem(item, null, Utility.RandomMinMax(MinBudget, MaxBudget), 0, ReforgedPrefix.None, ReforgedSuffix.None);

                        if (++failSafe == 25 && Imbuing.GetTotalWeight(item, -1, false, true) == 0)
                        {
                            item.Delete();
                            success = false;
                            i--;
                            break;
                        }
                    }
                    while (Imbuing.GetTotalWeight(item, -1, false, true) == 0);

                    if (success)
                    {
                        item.Movable = false;
                        PackItem(item);
                    }
                }
            }
        }

        public int GetCostFor(Item item)
        {
            return (int)((double)Imbuing.GetTotalWeight(item, -1, false, true) * 2.18);
        }

        public void TryBuyItem(Mobile from, Item item)
        {
            if (item.Deleted || !Backpack.Items.Contains(item))
            {
                from.SendLocalizedMessage(1150244); // Transaction no longer available
                return;
            }

            int points = (int)Engines.Points.PointsSystem.DespiseCrystals.GetPoints(from);
            int cost = GetCostFor(item);

            if (points >= cost)
            {
                Engines.Points.PointsSystem.DespiseCrystals.DeductPoints(from, cost);
                item.Movable = true;

                if (from.Backpack == null || !from.Backpack.TryDropItem(from, item, false))
                {
                    item.MoveToWorld(from.Location, from.Map);
                }

                from.SendLocalizedMessage(1153431, cost.ToString()); // You purchase the item for ~1_AMT~ Despise Crystals.

                if (Backpack.Items.Count == 0)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(Utility.RandomMinMax(30, 180)), DoRestock);
                }
            }
            else
            {
                from.SendLocalizedMessage(1153430); // You cannot afford that
            }
        }

        public MysteriousWisp(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                reader.ReadInt();
                reader.ReadInt();
                reader.ReadInt();
                reader.ReadInt();
                reader.ReadDouble();
            }

            Timer.DelayCall(() => DoRestock(true));
        }

        public class BuyBackpack : Backpack
        {
            public BuyBackpack()
            {
                Movable = false;
                Layer = Layer.Backpack;
                Weight = 1.0;
            }

            public override int DefaultMaxWeight => 0;

            public override bool IsAccessibleTo(Mobile m)
            {
                return true;
            }

            public override void GetChildProperties(ObjectPropertyList list, Item item)
            {
                base.GetChildProperties(list, item);

                MysteriousWisp wisp = RootParent as MysteriousWisp;

                if (wisp == null)
                {
                    return;
                }

                list.Add(1153425, wisp.GetCostFor(item).ToString()); // Price: ~1_AMT~ Despise Crystals
            }

            public override void GetChildContextMenuEntries(Mobile from, List<ContextMenuEntry> list, Item item)
            {
                base.GetChildContextMenuEntries(from, list, item);

                list.Add(new SimpleContextMenuEntry<Item>(from, 1062219, (m, i) => // Buy
                {
                    var wisp = RootParent as MysteriousWisp;

                    if (wisp != null && i.IsChildOf(this))
                    {
                        wisp.TryBuyItem(m, i);
                    }
                },
                item, nonlocalUse: true));

                list.Add(new SimpleContextMenuEntry<Item>(from, 1153469, (m, i) => // Move
                {
                    if (i.IsChildOf(this))
                    {
                        DropItem(i);
                    }
                },
                item, nonlocalUse: true));
            }

            public BuyBackpack(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write(0); // version
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();
            }
        }
    }
}
